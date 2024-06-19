# Stage 1: Build the application with .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/build

# List the contents of /app/build for debugging
RUN ls -al /app/build

# Stage 2: Publish the application using .NET SDK 8.0
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# List the contents of /app/publish for debugging
RUN ls -al /app/publish

# Stage 3: Final stage, create runtime image with .NET Runtime 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Expose port 80 for web traffic
EXPOSE 80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "TheHighInnovation.POS.Web.dll"]
