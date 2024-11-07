# Foodplanner-api

## Setting up the development environment

Make sure to have a JSON file called `appsettings.Development.json` in the same directory as `appsettings.json`, containing the following properties. Remember to set the values for `ClientId`, `ClientSecret` and `Workspace`.
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

## Migrations

When making changes to the database, such as making tables or making new relations a new migration should be added defining this change. 

Migration files are versioned, which enables rollback to a previous database version. Version names are sequently rising starting at 1, meaning the next migration should have version 2 and so on. Documentation is found on https://fluentmigrator.github.io/articles/intro.html.

New migrations are added by including a new file in the [Migrations folder](https://github.com/aau-giraf/foodplanner-api/tree/staging/FoodplannerDataAccessSql/Migrations) and the class must inherit `Migration`. Important is to implement `up` and `down` methods which must be each others reverse.
