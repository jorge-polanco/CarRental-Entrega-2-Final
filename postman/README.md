# Postman local setup

1. Start the local MySQL server with `database/Start-LocalMySql.ps1` if it is stopped.
2. The schema script `database/03-create-catalog-stored-procedures.sql` creates the ISO country and customer tables plus the procedures used by the repositories. The authentication schema is in `database/01-create-database.sql` and `database/02-create-auth-tables.sql`.
3. Run the API from Visual Studio using the `http` launch profile, or start it at `http://localhost:5231`.
4. Import `CarRental API - JWT, ISO Countries and Customers.postman_collection.json` and `CarRental Local.postman_environment.json` into Postman. Select the `CarRental Local` environment.
5. Register a local user with `POST {{baseUrl}}/api/auth/register`, then promote that user to `Admin` in the local `CarRentalDb` by updating `database/02-promote-admin.sql` with the registered username and running it in MySQL Workbench.
6. Set `adminUsername` and `adminPassword` as **Current values** in the local Postman environment. Do not put credentials in Initial values or the collection. Run the collection in order: login, Countries, Customers.

The collection saves the returned JWT to the local environment's current value and checks the expected statuses: 200 for reads, 201 for inserts, and 204 for updates. POST creates a QA country/customer record in the local `carrental` database. The country sample uses the reserved test code `QX`/`QXX`; run the suite once against a fresh database, or remove the prior QA country/customer records before repeating it.
