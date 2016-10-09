# The Firefly Experiment

JSON REST API made with .NET Core 1.0 + ASP.NET Core 1.0 + Entity Framework Core (EF7)

## Features

 - Oauth 2.0 password and refresh_token flow
 - Working ASP.NET Core ACL
 - Simple generic model for data queries with metadata & hateoas
 - CORS
 - MVC ready for application logic
 - Debugging headers in development mode


## Sample routes

 - Oauth: `/oauth/token`
 - Current user `/api/users/current`
 - Sample *Articles* resource `/api/articles`
 - Sample *Authors* resource `/api/authors`

## Requirements & Installation

 - Since .NET Core is multiplatform you can run it on *Windows*, *OSX* or *Linux*.
 - PostgreSQL DB server

### Basic installation

 1. Install .NET Core on your system. Follow official docs here: [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
 2. Check-out this
 3. Run `dotnet restore` in project root folder in order to install packages.
 4. Configure application and database connection string in `appsettings.json`
 5. Run migrations by `dotnet ef database update`. Migrations will pour in postgres schema `firefly` (which will be created).
 6. Run application by `dotnet run`. The web server will start on localhost:5000

### Creating an Oauth user

There is a UserSeeder class in `Firefly.Models` namespace. Fill values you like, uncomment the `return;` statement within `Initialize()` and run application. There is no sophisticated user seeder so far.

### Creating self-signed certificate for token signing

The OAuth server uses a x509 certificate for signing tokens. 
If you want to scale this application, you have to use a custom cerificate distributed for every instance of this application.

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



