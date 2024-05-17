terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.0"
    }
  }
  required_version = ">= 0.14.9"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-fantasychas"
  location = "West Europe"
}

resource "azurerm_app_service_plan" "asp" {
  name                = "asp-fantasychas"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "Linux"
  reserved            = true
   os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_app_service" "app" {
  name                = "app-fantasychas"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.asp.id
  https_only          = true

  site_config {
    linux_fx_version = "DOCKER|${var.docker_image}"
    always_on        = true
    minimum_tls_version = "1.2"
  }

  app_settings = {
    "DOCKER_REGISTRY_SERVER_URL"      = "https://${var.docker_registry_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME" = var.docker_registry_username
    "DOCKER_REGISTRY_SERVER_PASSWORD" = var.docker_registry_password
  }
}
