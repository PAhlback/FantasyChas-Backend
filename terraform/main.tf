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
  location = "East US"
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
runcmd:
  - systemctl start docker
  - systemctl enable docker
  - echo "OPENAI_KEY=your_openai_key" > /etc/environment
  - echo "CONNECTION_STRING=Server=10.0.1.6,1433;Database=FantasyChasDB;User Id=sqladmin;Password=;" >> /etc/environment
  - echo "EMAIL=your_email" >> /etc/environment
  - echo "PASSWORD=your_password" >> /etc/environment
  - docker pull ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker pull ghcr.io/f-eighty7/fantasychas-backend/app:latest
  - docker run -d -p 8080:80 --env-file /etc/environment ghcr.io/f-eighty7/chaschallenger/app:latest
  - docker run -d -p 8080:81 --env-file /etc/environment ghcr.io/f-eighty7/fantasychas-backend/app:latest
EOF
  )
}

resource "azurerm_private_endpoint" "sql_server_endpoint" {
  name                = "sql-server-endpoint"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  subnet_id           = azurerm_subnet.subnet.id

  private_service_connection {
    name                           = "sql-server-connection"
    private_connection_resource_id = azurerm_sql_server.fantasy.id
    subresource_names              = ["sqlServer"]
    is_manual_connection           = false
  }
}

resource "azurerm_sql_server" "fantasy" {
  name                         = "fantasy-sqlserver"
  resource_group_name          = azurerm_resource_group.fantasy.name
  location                     = azurerm_resource_group.fantasy.location
  version                      = "12.0"
  administrator_login          = "fantasy-admin"
  administrator_login_password = "YourStrong@Passw0rd"
  
  dynamic "server_version" {
    for_each = [azurerm_sql_server.fantasy]
    content {
      public_network_access_enabled = false
      private_endpoint_connections {
        name                              = azurerm_private_endpoint.sql_server_endpoint.name
        private_endpoint_id               = azurerm_private_endpoint.sql_server_endpoint.id
        private_link_service_connection {
          name                           = "sql-server-connection"
          private_link_service_id        = azurerm_sql_server.fantasy.private_endpoint_connection.0.private_link_service_id
          group_ids                      = ["sqlServer"]
        }
      }
    }
  }

  private_endpoint_ip_address   = "10.0.1.6"
}
