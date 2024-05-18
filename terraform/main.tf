variable "ghcr_token" {
  description = "GitHub Container Registry token"
  type        = string
}

variable "ghcr_username" {
  description = "GitHub Container Registry username"
  type        = string
}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"

  backend "remote" {
    organization = "FantasyChas-Backend"
    workspaces {
      name = "FantasyChas-Backend2"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "fantasychas_rg" {
  name     = "FantasyChasResourceGroup"
  location = "Sweden Central"
}

resource "azurerm_virtual_network" "main" {
  name                = "fantasychas-network"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.fantasychas_rg.location
  resource_group_name = azurerm_resource_group.fantasychas_rg.name
}

resource "azurerm_subnet" "internal" {
  name                 = "internal"
  resource_group_name  = azurerm_resource_group.fantasychas_rg.name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = ["10.0.2.0/24"]
}

resource "azurerm_public_ip" "pip" {
  name                = "fantasychas-terraform-pip"
  resource_group_name = azurerm_resource_group.fantasychas_rg.name
  location            = azurerm_resource_group.fantasychas_rg.location
  allocation_method   = "Dynamic"
}

resource "azurerm_network_interface" "main" {
  name                = "fantasychas-terraform-nic1"
  resource_group_name = azurerm_resource_group.fantasychas_rg.name
  location            = azurerm_resource_group.fantasychas_rg.location

  ip_configuration {
    name                          = "primary"
    subnet_id                     = azurerm_subnet.internal.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.pip.id
  }
}

resource "azurerm_network_interface" "internal" {
  name                = "fantasychas-terraform-nic2"
  resource_group_name = azurerm_resource_group.fantasychas_rg.name
  location            = azurerm_resource_group.fantasychas_rg.location

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.internal.id
    private_ip_address_allocation = "Dynamic"
  }
}

resource "azurerm_linux_virtual_machine" "main" {
  name                = "fantasychas-terraform-vm"
  resource_group_name = azurerm_resource_group.fantasychas_rg.name
  location            = azurerm_resource_group.fantasychas_rg.location
  size                = "Standard_B1s"
  admin_username      = "adminuser"
  admin_password      = "Varförfunkarintelösernordet127!"
  network_interface_ids = [
    azurerm_network_interface.main.id,
    azurerm_network_interface.internal.id,
  ]

  tags = {
    environment = "dev"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts"
    version   = "latest"
  }

  os_disk {
    storage_account_type = "Standard_LRS"
    caching              = "ReadWrite"
  }
}
