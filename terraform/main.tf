terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>2.0"
    }
  }
  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "MasterChass"

    workspaces {
      name = "MasterChass-Backend"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "MasterChass-Backend"
  location = "West Europe"
}

resource "azurerm_virtual_network" "vnet" {
  name                = "MasterChass-Backend-vnet"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_subnet" "subnet" {
  name                 = "MasterChass-Backend-subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

resource "azurerm_public_ip" "pip" {
  name                = "MasterChass-Backend-pip"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  allocation_method   = "Static"  
}

resource "azurerm_network_interface" "nic" {
  name                = "MasterChass-Backend-nic"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  ip_configuration {
    name                          = "MasterChass-Backend-primary"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.pip.id
  }
}

resource "azurerm_network_security_group" "nsg" {
  name                = "MasterChass-Backend-nsg"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  security_rule {
    name                       = "SSH"
    priority                   = 1001
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "22"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

resource "azurerm_network_interface" "nic2" {
  name                = "MasterChass-Backend-nic2"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  ip_configuration {
    name                          = "MasterChass-Backend-internal"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
  }
}

resource "azurerm_linux_virtual_machine" "vm" {
  name                = "MasterChass-Backend-vm"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  size                = "Standard_B1s"
  admin_username      = "masterchassadmin"
    disable_password_authentication = true 
  admin_ssh_key {
    username   = "masterchassadmin"
    public_key = file("~/.ssh/masterchass.pub")
  }
  network_interface_ids = [
    azurerm_network_interface.nic.id,
    azurerm_network_interface.nic2.id,
  ]

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts-gen2"
    version   = "latest"
  }

  custom_data = base64encode(<<-EOF
#cloud-config
package_upgrade: true
packages:
  - docker.io
runcmd:
  - systemctl start docker
  - systemctl enable docker
  - docker pull ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker pull ghcr.io/f-eighty7/fantasy-chas-backend/app:latest
  - docker run -d -p 80:80 ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker run -d -p 80:80 ghcr.io/f-eighty7/fantasy-chas--backend/app:latest
EOF
  )
}