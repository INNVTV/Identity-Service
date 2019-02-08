# Identity Service
Identity-As-A-Service written from scratch in .Net Core using a CQRS architecture with a CosmosDB/Redis persistence layer and RSA signed JWT Tokens with public key distribution for authentication and claims.

# Architecture
![Architecture](https://github.com/INNVTV/Identity-Service/blob/master/_docs/imgs/architecture.png)

## Features
 * Microservices Ready
 * JWT token authentication w/ RSA Public/Private key pairs
 * CQRS Architecture using MediatR, FluentValidation and Custom Middleware
 * CosmosDB/Redis Persistence Layer
 * User/Role/Invitations Management
 * Private gRPC Endpoints
 * OpenAPI Endpoints w/ SwaggerUI
 * Well known endpoints for public key distribution
 * Redis caching
 * Serilog for structured logging
 * Max Attempt Lockouts, Invitations, Erc...
 * Tracking of login attempts, last logins, etc...
 * UserName or Email logins
 * Easy to refactor to your needs


## Users, Roles and Authorization API Endpoints
The UsersController, RolesController and AuthoriationControllers contain endpoints that should only be used for debugging and development purposes. A production instance of Identty Services shoul comment out these endpoints and rely on the secure gRPC endpoints to accept commands and queries from associated microservices within the same application space.

The only public endpoints should be the Authentication endpoint.


# Architecture Notes
Project architecture is based off of my [.Net Core Clean Architecture](https://github.com/INNVTV/NetCore-Clean-Architecture) project. This means there is a strong CQRS pattern in place using MediatR.

## OpenAPI/Swagger
All OpenAPI endpoints are secured by ApiKey. Swagger UI will allow you to authorize your calls for debugging purposes.

## NSwag Generated Client Code
Be sure that have **Inject HttpClient via constructor** set to true so that X-API-KEY header can be passed into your client calls.

Here is an example of calling the API:

    var httpClient = new System.Net.Http.HttpClient();
    httpClient.DefaultRequestHeaders.Add("X-API-KEY", "X-API-KEY");

    var usersClient = new Services.IdentityService.UsersClient("http://localhost:53227", httpClient);
    var results = await usersClient.ListAsync(40, Services.IdentityService.OrderBy.CreatedDate, Services.IdentityService.OrderDirection.ASC, "");

### Shared Client Library
Shared client library for OpenAPI/Swagger/gRPC services are found in the "Utilities" folder.

## gRPC
gRPC services are partially built out for those that wish to use remote pocedure calls.

# RSA Key Generation
Utilities/Cryptography/RSAKeyGeneration

## Public Keys API
    /api/public/keys

## Public Keys Uri
    /public/keys

### JSON Web Tokens (JWT)
For more on [JSON Web Tokens](https://jwt.io/) visit the project site.


