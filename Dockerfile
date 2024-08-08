# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and project files
COPY TheHighInnovation.POS.WEB.sln ./
COPY TheHighInnovation.POS.Web/*.csproj ./TheHighInnovation.POS.Web/

# Restore dependencies
RUN dotnet restore

# Copy the remaining source files
COPY . .

# Build the application without optimizations for faster builds
WORKDIR /src/TheHighInnovation.POS.Web
RUN dotnet publish -c Debug -o /app/publish

# List the contents of the publish directory for debugging
RUN ls /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# List the contents of the app directory for debugging
RUN ls /app

# Expose port 80
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "TheHighInnovation.POS.WEB.dll"]
