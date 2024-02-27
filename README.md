# Solidus.Middleware.Authentication

[![Build and Test](https://github.com/solidus-framework/Solidus.Test/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/solidus-framework/Solidus.Test/actions/workflows/build_and_test.yml)
![NuGet Version](https://img.shields.io/nuget/v/Solidus.Middleware.Authentication)

The middleware that handles authentication scenarios.

## NuGet Package

```sh
dotnet add package Solidus.Middleware.Authentication
```

## Provided API

* Account Sign Up : `POST /account/sign-up`

    `Request` body:

    ```json
    {
        "name": "string",
        "password": "string",
        "rememberMe": "boolean",
        "metadata": {
            "key": "value",
            ...
        }
    }
    ```

* Account Sign In : `POST /account/sign-up`

    `Request` body:

    ```json
    {
        "name": "string",
        "password": "string",
        "rememberMe": "boolean"
    }
    ```

* Session Sign Out : `POST /sign-out`

* Session Status : `GET /status`

    `Response` body:

    ```json
    {
        "claims": {
            "type": "value",
            ...
        }
    }
    ```

## Service registrations

> Namespace: `Solidus.Middleware.Authentication`

`AuthenticationBuilder.AddSolidus()` - Registers Solidus authentication

`IMvcBuilder.AddSolidusAuthenticationControllers` - Registers authentication API controllers

## Endpoint registrations

> Namespace: `Solidus.Middleware.Authentication`

`IEndpointRouteBuilder.MapSolidusAuthenticationStatusAction` - Maps `/status` route that provides current session information

`IEndpointRouteBuilder.MapSolidusAuthenticationSignOutAction` - Maps `/sign-out` route that signs out of authenticated session

> Namespace: `Solidus.Middleware.Authentication.Account`

`IEndpointRouteBuilder.MapSolidusAccountSignInAction` - Maps `/account/sign-in` route that is responsible of logging in to account

`IEndpointRouteBuilder.MapSolidusAccountSignUpAction` - Maps `/account/sign-up` route that is responsible of creating a new account

## Authentication Abstractions

> Namespace: `Solidus.Middleware.Authentication`

`IAuthenticationService` - Authentication session management

> Namespace: `Solidus.Middleware.Authentication.Account`

`IAccountClaimsFactory` - Authenticated account claims factory

`IAccountPasswordHasher` - Password hashing and validation

`IAccountProvider` - An account data provider

`IAccountService` - An account management

`IAccountStorage` - An account storage

## Example usage

[See demo instructions here](https://github.com/solidus-framework/Solidus.Middleware.Authentication/blob/main/DEMO.md)
