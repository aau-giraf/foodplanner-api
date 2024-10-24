# Foodplanner-api

## Setting up the development environment

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
    "ClientId": "14a28b29-5849-4d64-9549-6d3d0163e107",
    "ClientSecret": "54437c9a662eaa5bb32832486b8040df7c7855c7f71d116db5767f88cc187ec6",
    "Workspace": "f687b673-33f6-49df-9d7e-e5ee1717c14e"
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