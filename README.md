# Identity Service
An Identity-As-A-Service microservices component written from scratch in .Net Core using a CQRS architecture with a CosmosDB persistence layer.

#Architecture
![Architecture](https://github.com/INNVTV/Identity-Service/blob/master/_docs/imgs/architecture.png)

## Features
 * Microservices Ready
 * JWT token authentication w/ RSA Public/Private key pairs
 * CQRS Architecture using MediatR, FluentValidation and Custom Middleware
 * CosmosDB/Redis Persistence Layer
 * User/Role Management
 * Private gRPC Endpoints
 * OpenAPI Endpoints w/ Swagger
 * Well known endpoints for public key distribution
 * Redis caching to allow for multiple instances
 * Max Attempt Lockouts
 * Tracking of login attempts, last logins, etc...
 * UserName or Email logins
 * Easy to refactor to your needs


 using RS256 signed JWT Tokens for authentication and authorization claims.
Built from scratch for microservice deployments in the cloud.
Designed to work with CosmosDB and can be easiliy refactored to work on any cloud servcie provider and any JSON based document data store.



# Notes

## Users, Roles and Authorization API Endpoints
The UsersController, RolesController and AuthoriationControllers contain endpoints that should only be used for debugging and development purposes. A production instance of Identty Services shoul comment out these endpoints and rely on the secure gRPC endpoints to accept commands and queries from associated microservices within the same application space.

The only public endpoints should be the Authentication endpoint.


# Architecture 
The architecture is based off of the [.Net Core Cleal Architecture](https://github.com/INNVTV/NetCore-Clean-Architecture) project. This means there is a strong CQRS pattern in place using MediatR.

I also borrow heavily from the [Identity Server](https://identityserver.io/) project with the addition of a much more flexible microservices approach using the CQRS pattern and allowing for multi-accounts per identity as well as multi-tenant SaaS capabilities with a JSON based document store.

# Archtecture
Roles, Users

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
xxx

# Authentication
xxx

# Authorization
xxx

# Security
xxx

### JSON Web Tokens (JWT)
[JSON Web Tokens](https://jwt.io/)
xxx

### RSA Lookup
xxx

### Certificate Authority
xxx

# Service Communications
xxx

