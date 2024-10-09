# Use the official .NET Core SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore any dependencies (use .csproj for the project name)
COPY FoodplannerApi/*.csproj ./FoodplannerApi/
COPY FoodplannerDataAccessSql/*.csproj ./FoodplannerDataAccessSql/
COPY FoodplannerModels/*.csproj ./FoodplannerModels/
COPY FoodplannerServices/*.csproj ./FoodplannerServices/
RUN dotnet restore FoodplannerApi

# Copy the rest of the application code
COPY . .

# Publish the application
RUN dotnet publish FoodplannerApi -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port your application will run on
EXPOSE 80

# Start the application
CMD ["dotnet", "FoodplannerApi.dll"]