terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>2.0"
    }
  }
  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "FantasyChas-Backend"

    workspaces {
      name = "FantasyChas-Backend2"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "FantasyChas-Backend"
  location = "West Europe"
}

resource "azurerm_virtual_network" "vnet" {
  name                = "FantasyChas-Backend-vnet"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_subnet" "subnet" {
  name                 = "FantasyChas-Backend-subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

resource "azurerm_public_ip" "pip" {
  name                = "FantasyChas-Backend-pip"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  allocation_method   = "Static"  
}

resource "azurerm_network_interface" "nic" {
  name                = "FantasyChas-Backend-nic"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  ip_configuration {
    name                          = "FantasyChas-Backend-primary"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.pip.id
  }
}

resource "azurerm_network_security_group" "nsg" {
  name                = "FantasyChas-Backend-nsg"
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
  name                = "FantasyChas-Backend-nic2"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  ip_configuration {
    name                          = "FantasyChas-Backend-internal"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
  }
}

resource "azurerm_linux_virtual_machine" "vm" {
  name                = "FantasyChas-Backend-vm"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  size                = "Standard_B1s"
  admin_username      = "fantasyadmin"
  admin_password      = "Varförfunkarkodeninte123?"
  disable_password_authentication = false
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
  - docker pull ${var.backend_docker_image}
  - docker run -d -p 80:80 ${var.backend_docker_image}

write_files:
  - path: /home/fantasyadmin/.ssh/authorized_keys
    content: |
      ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQCu2tpC7EGCHD34wxHloZiXhCvNZtQtXE67/id/6VzaAH6xqukx1u3A8Ki7heuNqq2rDhIK38BEhxDM7KHEd0BEO+6as/TttzVIVz8LCoDBx1awTSdjBlpgopkQTP2nNzfDZMzMtysU/8b6CDASfpFBLomyQacN28pbte0RRAB5k5MTWizjT7FWpz/eBYUAtlWmsp3Qhh/FBxj7V2lYukxb/oMBV62ECuxzi0OUaGIryRrLQh/AOWo7dKkRXlSGec72YIGrb05nWqjzfSVWRMgYXDmCMbihAcA/MQZtWm14hRQjWJpkqBzEKKF4hM9+Ml0+W6y/J6wcAZwHkz8Nhnvikn3ttsFJujPt7QN9jBt7suA/gw/wnfa3S8gj0Xo8Q7YyIbysGJ4ixsMzregaYl3+Fr9m4sFr4tmEjza3ENh7Oo+lw75SsD25v7bZ3WkZ0ux2QDJM35I/TkC0hqgVVWpqkB/wPr7zt18PM0aoaW269HYol98iBMvn1CVnRrSbGwjhDRDGlTcTyjFBmayLj4+ls1031+0MCq1AjpRMaR8eCBgouMpJPcB7n+fwGRo6T1LhgVLu08L7xOd5p5agdA92QVkyLcm1D38q+fdPDN7vOCbFllNDmvNQFFDXaEexPAUa22/Sf65OrVJ7W6g2qh5hKNEGyHXVZMCnDFKnHDomsQ== ahink@AK-ROGStrix
    permissions: 0600
EOF
  )
}

variable backend_docker_image" {
  description = "The Docker image to use"
  type        = string
}

#variable "frontend_docker_image" {
#  description = "The Docker image for the frontend service"
#  type        = string
#}
#- docker pull ${var.frontend_docker_image}
#- docker run -d -p 3000:3000 ${var.frontend_docker_image}