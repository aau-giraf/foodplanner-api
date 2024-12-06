# Foodplanner API

The Foodplanner API is a backend service for the GIRAF Foodplanner application, providing a robust system to manage meal plans and user roles. Built with **ASP.NET Core**, the API connects to a **PostgreSQL database** and exposes endpoints for seamless integration with the Flutter-based frontend.

## Features

-   **User Management**: Role-based access control for teachers and parents.
-   **Meal Planning**: Endpoints for creating, updating, and managing meal plans.
-   **Database Integration**: Utilizes PostgreSQL for reliable data storage.
-   **Database Migration**: Utilizes FlutentMigrator for scalable data storage.
-   **RESTful API**: Follows REST principles for easy integration and development.

## Technologies Used

-   **Framework**: ASP.NET Core
-   **Database**: PostgreSQL
-   **Image Database**: Minio
-   **Authentication**: JWT (JSON Web Token)
-   **Containerization**: Docker (optional)

## Project Structure

```plaintext
src/
├── FoodplannerApi/                        # Contains main functionality
│   └── Controller/                        # API controllers for handling HTTP requests
├── FoodplannerDataAccessSQL/              # Data access layer
│   ├── Account/                           # Account repositories
│   ├── FeedbackChat/                      # Feedback repositories
│   ├── Image/                             # Image repositories
│   ├── LunchBox/                          # LunchBox repositories
│   └── Migrations/                        # Database migrations
├── FoodplannerModels/                     # Model layer
│   ├── Account/                           # Account models
│   ├── FeedbackChat/                      # Feedback models
│   ├── Image/                             # Image models
│   └── LunchBox/                          # LunchBox models
├── FoodplannerServices/                   # Service Layer
│   ├── Account/                           # Account services
│   ├── Auth/                              # Authentication services
│   ├── FeedbackChat/                      # Feedback services
│   ├── Image/                             # Image services
│   └── LunchBox/                          # LunchBox services
└── Test/                                  # Tests
```

## Migrations

When making changes to the database, such as making tables or making new relations a new migration should be added defining this change.

Migration files are versioned, which enables rollback to a previous database version. Version names are sequently rising starting at 1, meaning the next migration should have version 2 and so on. Documentation is found on https://fluentmigrator.github.io/articles/intro.html.

New migrations are added by including a new file in the [Migrations folder](https://github.com/aau-giraf/foodplanner-api/tree/staging/FoodplannerDataAccessSql/Migrations) and the class must inherit `Migration`. Important is to implement `up` and `down` methods which must be each others reverse.

## Getting Started

### Prerequisites

Ensure you have the following installed:

-   [ASP.NET Core SDK](https://dotnet.microsoft.com/en-us/download)
-   [Docker](https://www.docker.com) (optional, for containerized deployment)

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

### Github Actions

Ensure github actions is correctly setup.

TODO: vi skal lige have skrevet den her færdig når vi ved hvad der skal gøres i forhold til docker hub.

### Server Setup

The server will consist of docker containers such as a staging and production API, PostgreSQL database and a Minio database. It will integrate with Github actions to streamline development and automaticly update Staging and Production API to follow newest releases.
To set this up correctly please follow these steps.

1. First step is to figure out where to host the server. A great place is AAU's own hosting platform https://strato-new.claaudia.aau.dk here the most important thing is to pick a server running Ubuntu.

> [!IMPORTANT]
> If Strato is decided then remember to open all the ports that are expected to be used. This can be done in Security Groups under the Network section. These ports could be 5432, 8080, 8081, 9000 and 9001

2. Install the following on the server:

-   [Docker](https://docs.docker.com/engine/install/ubuntu/)
-   [Cron](https://www.digitalocean.com/community/tutorials/how-to-use-cron-to-automate-tasks-ubuntu-1804)

3. Create a new file called `docker-compose.yml` and open it using the following command

    ```bash
    touch docker-compose.yml
    nano docker-compose.yml
    ```

    Paste the following code into the file. **Remember to update the variables with your own information.**

    ```yml
    version: "3.8"

    services:
        minio:
            image: minio/minio:latest
            container_name: minio_giraf
            restart: unless-stopped
            ports:
                - "9000:9000"
                - "9001:9001"
            volumes:
                - ./minio/data:/mnt/data
            environment:
                - MINIO_ROOT_USER=<insert here>
                - MINIO_ROOT_PASSWORD=<insert here>
                - MINIO_VOLUMES=/mnt/data
            command: server /mnt/data --console-address ":9001"

        postgres:
            image: postgres:latest
            container_name: postgres_giraf
            restart: unless-stopped
            ports:
                - "5432:5432"
            volumes:
                - ./postgres/data:/var/lib/postgresql/data
            environment:
                - POSTGRES_PASSWORD=<insert here>
                - POSTGRES_USER=<insert here>
                - POSTGRES_DB=<insert here>
    ```

    Go ahead and run the docker compose file using the following command:

    ```bash
    docker compose -f docker-compose.yml up
    ```

    This will create two containers containing **Minio** for image storage and PostgreSQL for data storage.
    The Minio server can now be accessed and managed on `http://<server-ip>:9001`

4. Connect to the newly created PostgreSQL container using your prefered PostgreSQL database tool ex. [pgAdmin](https://www.pgadmin.org/download/).

> [!TIP]
> **Host name/address**: This is your server-ip<br>
**Port**: 5432 unless changed in docker-compose.yml<br>
**Username**: The one you wrote in docker-compose.yml<br>
**Password**: The one you wrote in docker-compose.yml

    When connected go ahead and create two new databases called `giraf_foodplanner_db_stage` and `giraf_foodplanner_db_prod` You dont have to create any new tables these will be automaticly generated when the dotnet application runs.

6. Create a new file called `docker-auto-deploy.sh` and open it using the following command.

    ```bash
      touch docker-auto-deploy.sh
      nano docker-auto-deploy.sh
    ```

    Paste the following code into the file. **Remember to update the variables with your own information.**

    ```bash
    # Variables

    DOCKER_IMAGE="<docker-hub-name>/foodplanner-api" # Replace with your Docker image name
    CONTAINER_NAME="foodplanner-api" # Name of your running container
    LAST_IMAGE_FILE="/var/tmp/last_image_version_stage.txt" # File to store the last pulled image version
    LAST_IMAGE_FILE_PROD="/var/tmp/last_image_version_prod.txt"

    CLIENT_ID="<client-id>"
    CLIENT_SECRET="<client-secret>"
    WORKSPACE="<workspace>"

    # Function to pull and deploy

    pull_and_deploy() {
    echo "New image found, pulling and deploying..."
    sudo docker pull $DOCKER_IMAGE:staging

        # Stop the existing container
        sudo docker stop $CONTAINER_NAME-stage
        sudo docker rm $CONTAINER_NAME-stage

        # Start a new container with the updated image
        sudo docker run -d --name $CONTAINER_NAME-stage -p 8080:8080 -e CLIENT_ID=$CLIENT_ID -e CLIENT_SECRET=$CLIENT_SECRET -e WORKSPACE=$WORKSPACE -e ASPNETCORE_ENVIRONMENT=Staging  $DOCKER_IMAGE:staging

        # Store the new image digest in the file
        echo $LATEST_DIGEST > $LAST_IMAGE_FILE
        echo "Deployment successful."

    }

    pull_and_deploy_prod() {
    echo "New image found, pulling and deploying..."
    sudo docker pull $DOCKER_IMAGE:prod

        # Stop the existing container
        sudo docker stop $CONTAINER_NAME-prod
        sudo docker rm $CONTAINER_NAME-prod

        # Start a new container with the updated image
        sudo docker run -d --name $CONTAINER_NAME-prod -p 8081:8080 -e CLIENT_ID=$CLIENT_ID -e CLIENT_SECRET=$CLIENT_SECRET -e WORKSPACE=$WORKSPACE -e ASPNETCORE_ENVIRONMENT=Production $DOCKER_IMAGE:prod

        # Store the new image digest in the file
        echo $LATEST_DIGEST_PROD > $LAST_IMAGE_FILE_PROD
        echo "Deployment successful."

    }

    # Get the current latest image digest from Docker Hub

    LATEST_DIGEST=$(curl -s https://hub.docker.com/v2/repositories/$DOCKER_IMAGE/tags/staging/ | jq -r '.images[0].digest')
    LATEST_DIGEST_PROD=$(curl -s https://hub.docker.com/v2/repositories/$DOCKER_IMAGE/tags/prod/ | jq -r '.images[0].digest')

    # For staging

    # Check if the last image version file exists

    if [ ! -f "$LAST_IMAGE_FILE" ]; then
    echo "No previous image found, pulling the latest version..."
    pull_and_deploy
    else # Read the last pulled image digest
    LAST_DIGEST=$(cat $LAST_IMAGE_FILE)

        # Compare the latest digest with the last pulled one
        if [ "$LATEST_DIGEST" != "$LAST_DIGEST" ]; then
            pull_and_deploy
        else
            echo "No new image found."
        fi

        fi

    # For production

    if [ ! -f "$LAST_IMAGE_FILE_PROD" ]; then
    echo "No previous image found, pulling the latest version..."
    pull_and_deploy_prod
    else

    # Read the last pulled image digest

    LAST_DIGEST_PROD=$(cat $LAST_IMAGE_FILE_PROD)

    # Compare the latest digest with the last pulled one

    if [ "$LATEST_DIGEST_PROD" != "$LAST_DIGEST_PROD" ]; then
    pull_and_deploy_prod
    else
    echo "No new image found."
    fi
    fi

    ```

7. Last but not least, we need to set up a cron job to run the `docker-auto-deploy.sh` script periodically.
   Open Cron using the following command

```bash
cronjob -e
```

Then add the following line to the end of the file.

```bash
* * * * * $HOME/docker-auto-deploy.sh >> $HOME/docker-deploy.log 2>&1
```

This will run the bash script once every minute and write the output to `docker-deploy.log`

## Running the API

1. Start the API locally:

```bash
dotnet run
```

2. The API will be available at https://localhost:8080

## API Documentation

The API is documented using Swagger, available at:

-   https://localhost:8080/swagger/index.html

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

4. Open a pull request to the staging branch, test it, and then create a new pull request for main.
