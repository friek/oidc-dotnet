# OpenID playground application

A test application to test OpenID authentication in .NET core.
The purpose of this application was to verify:

* That the OpenID authentication is valid
* That the used cookie size is sufficiently small

Regardless of if OpenID connect is used, .NET will store the entire 
authentication token of a user in a chunked cookie. This may lead
to ginormous cookies and may lead to the request size being too large
on some server configurations. With this application you can verify
that this is actually working correctly.

What you need:
* An OpenID auth service ([Keycloak](https://www.keycloak.org) for example)
* A configured client on the auth service
* `OIDC:Authority` configured in appsettings.json
* `OIDC:ClientId` and `OIDC:ClientSecret` in app secrets
