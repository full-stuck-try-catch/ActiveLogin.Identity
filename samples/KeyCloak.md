# Keycloak + BankID sample setup

These samples demonstrate how to front BankID authentication with Keycloak and consume the resulting OpenID Connect tokens from a .NET 8 portal.

## Projects

- **KeyCloak.ServerExample** – Hosts the ActiveLogin BankID handlers and is registered as an OpenID Connect identity provider in Keycloak. The API exposes the `/profile` endpoint that requires a BankID session and can be protected with Keycloak bearer tokens.
- **KeyCloak.ClientExample** – Minimal Razor Pages portal that relies on Keycloak for user sign-in. It initiates sign-in with Keycloak, which in turn offers BankID as a login option in the realm.

## Running locally

1. Start Keycloak with a realm configured for external Entra ID and BankID (for instance, by mapping the KeyCloak.ServerExample endpoints as an identity provider). Update `appsettings.json` in both projects with your realm URL, client id/secret, and SSL settings.
2. Run the server project:
   ```bash
   dotnet run --project samples/KeyCloak.ServerExample
   ```
3. Run the client project in a separate terminal:
   ```bash
   dotnet run --project samples/KeyCloak.ClientExample
   ```
4. Navigate to the client (https://localhost:5001 by default). Choose **Login with BankID via Keycloak** to complete the flow using the QR code from the BankID simulator.

> These projects intentionally mirror the flow from the historic IdentityServer samples: the server hosts the BankID logic and the client consumes the OpenID Connect metadata produced by the identity provider.
