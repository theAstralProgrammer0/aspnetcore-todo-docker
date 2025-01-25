# Base build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Coppy project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Install dotnet-ef for migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Run migrations
RUN dotnet ef migrations add InitialCreate --context TodoContext --output-dir Migrations
RUN dotnet ef database update --context TodoContext

# Publish the application
RUN dotnet publish -c Release -o /out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "TodoApp.dll"]
