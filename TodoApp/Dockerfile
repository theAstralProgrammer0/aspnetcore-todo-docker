# State starting build and context
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .

# Restore and Install dotnet-ef globally in build
RUN dotnet restore     
RUN dotnet tool install --global dotnet-ef

# Add .NET tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Publish build
RUN dotnet publish -c Release -o /out

# Reuse Published build to run app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "TodoApp.dll"]
