terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0.0"
    }
  }
  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "FantasyChass"

    workspaces {
      name = "FantasyChass-Backend"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "FantasyChas-Backend"
  location = "East Us"
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

  security_rule {
    name                       = "HTTP"
    priority                   = 1002
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "HTTPS"
    priority                   = 1003
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "SQL"
    priority                   = 1004
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "1433"
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
  admin_username      = "fantasychasadmin"
  disable_password_authentication = true
  admin_ssh_key {
    username   = "fantasychasadmin"
    public_key = file("~/.ssh/fantasychas.pub")
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
  - nginx
write_files:
  - path: /etc/nginx/sites-available/my-reverse-proxy
    content: |
      server {
          listen 80;
          server_name 52.149.227.5;

          location / {
              proxy_pass http://localhost:8080;
              proxy_set_header Host $host;
              proxy_set_header X-Real-IP $remote_addr;
              proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
              proxy_set_header X-Forwarded-Proto $scheme;
          }

          location /api {
              proxy_pass http://localhost:8081;
              proxy_set_header Host $host;
              proxy_set_header X-Real-IP $remote_addr;
              proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
              proxy_set_header X-Forwarded-Proto $scheme;
          }
      }
runcmd:
  - systemctl start nginx
  - systemctl enable nginx
  - systemctl start docker
  - systemctl enable docker
  - docker pull ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker pull ghcr.io/f-eighty7/fantasychas-backend/app:latest
  - docker run -d --name frontend-container -p 8080:80 ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker run -d --name backend-container -p 8081:8080 -e "CONNECTION_STRING=${var.connection_string}" -e "OPENAI_KEY=${var.openai_key}" ghcr.io/f-eighty7/fantasychas-backend/app:latest
EOF
)


variable "connection_string" {
  description = "Connection string for the database"
}

variable "openai_key" {
  description = "API key for OpenAI"
}