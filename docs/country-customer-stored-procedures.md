# Base MySQL: países ISO y clientes

Los repositorios `CountryRepository` y `CustomerRepository` usan `IMySqlDataService` y la conexión `ConnectionStrings:MysqlConnection`, que apunta al esquema `carrental`. El esquema, las tablas y los procedimientos quedan definidos en [03-create-catalog-stored-procedures.sql](../database/03-create-catalog-stored-procedures.sql); también se puede ejecutar desde MySQL Workbench.

| Entidad | Procedimiento | Parámetros | Resultado |
|---|---|---|---|
| País ISO | `GetCountries` | ninguno | `CountryId`, `Iso2Code`, `Iso3Code`, `CountryName` |
| País ISO | `GetCountryById` | `pCountryId` | Las mismas columnas que `GetCountries` |
| País ISO | `InsertCountry` | `pIso2Code`, `pIso3Code`, `pCountryName` | `newCountryId` |
| País ISO | `UpdateCountry` | `pCountryId` y los campos de inserción | `affectedRows` |
| Cliente | `GetCustomers` | ninguno | `IdCustomer`, `FirstName`, `LastName`, `SSN`, `Email`, `Country`, `CountryId`, `Mobile`, `StateName` |
| Cliente | `GetCustomerById` | `pCustomerId` | Las mismas columnas que `GetCustomers` |
| Cliente | `InsertCustomer` | `pFirstName`, `pLastName`, `pSSN`, `pEmail`, `pCountryId`, `pMobile`, `pStateName` | `newCustomerId` |
| Cliente | `UpdateCustomer` | `pCustomerId` y los campos de inserción | `affectedRows` |
| Cliente | `DeleteCustomer` | `pCustomerId` | `affectedRows` |

El esquema de autenticación está separado en `CarRentalDb` y usa EF Core; su instalación está descrita por `database/01-create-database.sql` y `database/02-create-auth-tables.sql`. Las credenciales locales se guardan en User Secrets, no en el repositorio.

La API publica los cuatro métodos bajo `/IsoCountries` y `/Customers`. Las lecturas requieren JWT; las altas y actualizaciones requieren rol `Admin`. Los ejemplos de solicitudes y aserciones de estado están en la colección de Postman de la carpeta [postman](../postman/README.md).
