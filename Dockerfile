# Use the official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FantasyChas-Backend.csproj", "./"]
RUN dotnet restore "FantasyChas-Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet publish "FantasyChas-Backend.csproj" -c Release -o /app/publish -r linux-x64 --self-contained true

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./FantasyChas-Backend"]
