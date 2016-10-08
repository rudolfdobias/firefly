# The Firefly Experiment

JSON REST API made with .NET Core 1.0 + ASP.NET Core 1.0 + Entity Framework Core (EF7)

## Requirements & Installation

 - Since .NET Core is multiplatform you can run it on *Windows*, *OSX* or *Linux*.
 - PostgreSQL DB server

 1. Install .NET Core on your system. Follow official docs here: [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
 2. Check-out this
 3. Run `dotnet restore` in project root folder in order to install packages.
 4. Configure application and database connection string in `appsettings.json`
 5. Run migrations by `dotnet ef database update`. Migrations will pour in postgres schema `firefly` (which will be created).
 6. Run application by `dotnet run`. The web server will start on localhost:5000

### Creating an Oauth user

There is a UserSeeder class in `Firefly.Models` namespace. Fill values you like, uncomment the `return;` statement within `Initialize()` and run application. There is no sophisticated user seeder so far.

## Implementations

 - Oauth 2.0 password and refresh_token flow
 - Simple generic model for data queries
 - CORS
 - MVC ready for application logic

## Sample routes

 - Oauth: `/oauth/token`
 - Sample *Articles* `/api/articles`
 - Sample *Authors* `/api/authors`



