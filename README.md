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
 * Redis caching to allow for multiple instances
 * Max Attempt Lockouts
 * Tracking of login attempts, last logins, etc...
 * UserName or Email logins
 * Easy to refactor to your needs


## Users, Roles and Authorization API Endpoints
The UsersController, RolesController and AuthoriationControllers contain endpoints that should only be used for debugging and development purposes. A production instance of Identty Services shoul comment out these endpoints and rely on the secure gRPC endpoints to accept commands and queries from associated microservices within the same application space.

The only public endpoints should be the Authentication endpoint.


# Architecture Notes
The architecture is based off of the [.Net Core Cleal Architecture](https://github.com/INNVTV/NetCore-Clean-Architecture) project. This means there is a strong CQRS pattern in place using MediatR.


## OpenAPI/Swagger
OpenAPI endpoints are used for authetication, sharing of the RSA public key and data such as list of roles.

The commented out endpoints for users and roles are there for local debugging and should remain comented out (or removed) in a production enviornment. You should use the gRPC services to allow authenticated services use these methods on behalf of logged in users.

## gRPC
gRPC services are used for user and role creation and management.

### Shared Client Library
Shared client library for gRPC services are found in the "XXX" project here: 

#### Building the proto buffers
[Images]


# RSA Key Generation
Utilities/Cryptography/RSAKeyGeneration

# Public Keys URI
/api/rsa/public/keys

# Usernames, Identities and Roles

### Login Lookups
The default scenario uses email/password for login lookups. You can switch to AccountName/password or Username/password scenarios - however this will require only allowing a single email per account/username.

### Roles
TBD

# Authentication
TBD

# Authorization
TBD

# Security
TBD

### JSON Web Tokens (JWT)
For more on [JSON Web Tokens](https://jwt.io/) visit the project site.


