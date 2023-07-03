# AuthAPI
AuthAPI is simple JWT-tokens authentication service.

Technology Stack:
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* xUnit
* Moq
* TestContainers
* FluentAssertions

Endpoints:

* [POST] `/auth/register` | Register user.
  + Request body (required)
    - Object
      - string: Login
      - string: Password
      - string: Fingerprint
  + Responses:
      - 200 OK: (body: access token, cookie: refresh token)
      - 400 Bad Request
      - 401 Unauthorized
      - 500 Internal Server Error

* [POST] `/auth/login` | Login user.
  + Request body (required)
    - Object
      - string: Login
      - string: Password
      - string: Fingerprint
  + Responses:
      - 200 OK: (body: access token, cookie: refresh token)
      - 400 Bad Request
      - 401 Unauthorized
      - 500 Internal Server Error

* [POST] `/auth/refresh-tokens` | Refresh access and refresh tokens.
  + Request body (required)
      - string: Fingerprint
  + Responses:
      - 200 OK: (body: access token, cookie: refresh token)
      - 400 Bad Request
      - 401 Unauthorized
      - 500 Internal Server Error
