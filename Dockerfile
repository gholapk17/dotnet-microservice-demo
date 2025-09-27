# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj . 
RUN dotnet restore

# Copy the rest of the source code and publish the app
COPY . . 
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published app from build stage
COPY --from=build /app/publish .

# Set environment variable to listen on port 3000
ENV ASPNETCORE_URLS=http://+:3000

# Expose port 3000
EXPOSE 3000

# Run the app
ENTRYPOINT ["dotnet", "MicroserviceDemo.dll"]
