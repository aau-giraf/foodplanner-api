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

