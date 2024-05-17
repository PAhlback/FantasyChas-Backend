# Use the official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FantasyChas-Backend.csproj", "./"]
RUN dotnet restore "FantasyChas-Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FantasyChas-Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FantasyChas-Backend.csproj" -c Release -o /app/publish

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FantasyChas-Backend.dll"]
