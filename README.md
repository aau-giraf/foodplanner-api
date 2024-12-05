# Foodplanner API

The Foodplanner API is a backend service for the GIRAF Foodplanner application, providing a robust system to manage meal plans and user roles. Built with **ASP.NET Core**, the API connects to a **PostgreSQL database** and exposes endpoints for seamless integration with the Flutter-based frontend.

## Features

- **User Management**: Role-based access control for teachers and parents.
- **Meal Planning**: Endpoints for creating, updating, and managing meal plans.
- **Database Integration**: Utilizes PostgreSQL for reliable data storage.
- **Database Migration**: Utilizes FlutentMigrator for scalable data storage.
- **RESTful API**: Follows REST principles for easy integration and development.

## Technologies Used

- **Framework**: ASP.NET Core
- **Database**: PostgreSQL
- **Image Database**: Minio
- **Authentication**: JWT (JSON Web Token)
- **Containerization**: Docker (optional)

## Project Structure

```plaintext
src/
├── FoodplannerApi/                        # Contains main functionality
    ├── Controller/                        # API controllers for handling HTTP requests
├── FoodplannerDataAccessSQL/              # Data access layer
    ├── Account/                           # Account repositories
    ├── FeedbackChat/                      # Feedback repositories
    ├── Image/                             # Image repositories
    ├── LunchBox/                          # LunchBox repositories
    ├── Migrations/                        # Database migrations
├── FoodplannerModels/                     # Model layer
    ├── Account/                           # Account models
    ├── FeedbackChat/                      # Feedback models
    ├── Image/                             # Image models
    ├── LunchBox/                          # LunchBox models
├── FoodplannerServices/                   # Service Layer
    ├── Account/                           # Account services
    ├── Auth/                              # Authentication services
    ├── FeedbackChat/                      # Feedback services
    ├── Image/                             # Image services
    ├── LunchBox/                          # LunchBox services
├── Test/                                  # Tests
```

## Migrations

When making changes to the database, such as making tables or making new relations a new migration should be added defining this change. 

Migration files are versioned, which enables rollback to a previous database version. Version names are sequently rising starting at 1, meaning the next migration should have version 2 and so on. Documentation is found on https://fluentmigrator.github.io/articles/intro.html.

New migrations are added by including a new file in the [Migrations folder](https://github.com/aau-giraf/foodplanner-api/tree/staging/FoodplannerDataAccessSql/Migrations) and the class must inherit `Migration`. Important is to implement `up` and `down` methods which must be each others reverse.

## Getting Started

### Prerequisites

Ensure you have the following installed:
- [ASP.NET Core SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com) (optional, for containerized deployment)

### Installation
1. Clone the repository:
```bash
git clone https://github.com/aau-giraf/foodplanner-api.git
```
2. Navigate to the project directory:
```bash
cd foodplanner-api/foodplannerApi
```
3. Install dependencies:
```bash
dotnet restore
```
4. Setup development environment:

Make sure to have a JSON file called `appsettings.Development.json` in the same directory as `appsettings.json`, containing the following properties.
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Infisical": {
    "ClientId": "<ClientId>",
    "ClientSecret": "<ClientSecret>",
    "Workspace": "<Workspace>"
  }
}
```

Overwriting environment variables is possible, this is done by adding to the `"Infisical"` group.
Example
```json
...
"Infisical": {
    "ClientId": "<ClientId>",
    "ClientSecret": "<ClientSecret>",
    "Workspace": "<Workspace>",
    "DB_HOST": "anotherValue"
  }
...
```

### Database Setup
1. Create a PostgreSQL database.
   This can be done using the docker-compose file <link her>

## Running the API
1. Start the API locally:
```bash
dotnet run
```
2. The API will be available at https://localhost:8080

## API Documentation

The API is documented using Swagger, available at:
- https://localhost:8080/swagger/index.html

# Contributing
Contributions are welcome! Follow these steps:
1. Create a branch for your feature or bugfix:
```bash
git checkout -b feature-name
```
2. Commit your changes:
```bash
git commit -m "Add feature name"
```
3. Push to the branch:
```bash
git push origin feature-name
```
5. Open a pull request to the staging branch, test it, and then create a new pull request for main.
