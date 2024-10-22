# Use the .NET SDK image as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["Backend/BackendServer/BackendServer.csproj", "Backend/BackendServer/"]
RUN dotnet restore "Backend/BackendServer/BackendServer.csproj"

# Copy the rest of the application code
COPY . .

# Set the working directory to the project folder and build the project
WORKDIR "/app/Backend/BackendServer"
RUN dotnet build "BackendServer.csproj" -c Debug -o /app/build

# Use the runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the build output from the build stage
COPY --from=build /app/build .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "BackendServer.dll"]
