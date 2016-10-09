# The Firefly Experiment


JSON REST API made with .NET Core 1.0 + ASP.NET Core 1.0 + Entity Framework Core (EF7)


---


[![build status](https://gitlab.com/rudolfdobias/firefly/badges/master/build.svg)](https://gitlab.com/rudolfdobias/firefly/commits/master)


---


## Features

 - Oauth 2.0 password and refresh_token flow
 - Distributed X509 certificate token signing
 - Working ASP.NET Core ACL
 - Simple generic model for data queries with metadata & hateoas
 - CORS
 - MVC ready for application logic
 - Debugging headers in development mode
 - Docker-ready
 - Async request handling by ASP.NET Core design

## Sample routes

 - Oauth: `/oauth/token`
 - Current user `/api/users/current`
 - Sample *Articles* resource `/api/articles`
 - Sample *Authors* resource `/api/authors`


---

## Requirements & Installation

 - Since .NET Core is multiplatform you can run it on *Windows*, *OSX* or *Linux*.
 - PostgreSQL DB server

### Basic installation

 1. Install .NET Core on your system. Follow official docs here: [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
 2. Check-out this
 3. Run `dotnet restore` in project root folder in order to install packages.
 4. Configure application and database connection string in `appsettings.json` by provided `appsettings.Template.json` template.
 5. Run migrations by `dotnet ef database update`. Migrations will pour in postgres schema `firefly` (which will be created).
 6. Run application by `dotnet run`. The web server will start on localhost:8000


## Configuration & Environment

You can set specific environment by setting ENV variable `ASPNETCORE_ENVIRONMENT` to values `Development|Staging|Production`. Furthermore, you can create env-specific configuration files like `appsettings.Production.json`.
The details are described [here](https://docs.asp.net/en/latest/fundamentals/environments.html#development-staging-production).

### Configuration through CLI or ENV

The application accepts command line arguments or ENV variables in specific format and parses them into the configuration. 

Examples:

    dotnet run --environment=Staging
    dotnet run --Keys:OwnCertificate=true
    dotnet run --MySettingsKey:MySubKey:SomeProperty="My Value"

    or

    export MYSETTINGSKEY__MYSUBKEY__SOMEPROPERTY="My Value"
    dotnet run

*(note that in env variables you must use '__' instead of ':')*


### Host Address:Port binding

The server address:port binging is configurable in `hosting.json` or by CLI arg: 

`dotnet run --server.urls=localhost:666`

---

## Creating an Oauth user

There is a UserSeeder class in `Firefly.Models` namespace. Fill values you like, uncomment the `return;` statement within `Initialize()` and run application. 
Or create it in the DB manually.

There is no other sophisticated user seeder. Yet.

---

## Creating self-signed certificate for token signing

The OAuth server uses a x509 certificate for signing tokens. 
If you want to scale this application, you have to use a custom cerificate distributed for every instance of this application.
Otherwise the tokens issued by one application will not match with any other.

For enabling custom certificate:

 1. Set `Keys:OwnCertificate` in your appsetings to `true`
 2. If you do not have self signed x509 certificate with embedded private key, generate it:
    
    `openssl genrsa -out private.key 1024`

    `openssl req -new -x509 -key private.key -out publickey.cer -days 365`

    `openssl pkcs12 -export -out certificate.pfx -inkey private.key -in publickey.cer`

    And copy it to the `keys/` folder
 3. Provide the path and the password to the certificate in settings in `Keys` section of your `appconig.json` version.
 4. For detailed informations you can see [this StackOverflow thread](http://stackoverflow.com/a/39938602/3330597)

With distributed custom certificate is application ready to be scaled on multiple docker instances.

---

## Docker usage

 1. Prepare configuration files for use in Docker. All `appconfig*.json` files will be included in docker image. 
 2. Run `docker publish` 
 3. Run `docker build -t firefly-api .` Docker image will be built.
 4. Run `docker run -p 8000:8000 -e ASPNETCORE_ENVIRONMENT=<environment, optional> -it firefly-api` and enjoy.

---

## GitLab CI

Docker-in-Docker custom image is used for building this image via custom GitLab Runner. 

Source: [rudolfdobias/dind-dotnet-core-runner](https://github.com/rudolfdobias/dind-dotnet-core-runner)
