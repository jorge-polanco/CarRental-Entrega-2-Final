# Countries and Customers API Implementation Plan

**Goal:** Complete the Core API and service layers for Country/isoCountries and Customer, with four stored-procedure-backed endpoints and unit coverage per endpoint.

**Architecture:** Use the existing MySQL `IMySqlDataService` boundary for Country and Customer, create their catalog schema and stored procedures, wire both repositories into the Core API, and use thin authorized controllers. Unit tests mock repositories at the API boundary; Postman integration checks exercise the running API and local MySQL database.

**Tech Stack:** .NET 8 Web API and service library, MySqlConnector, xUnit, Moq, Postman.

**Spec:** User requirements in the current conversation (sections 3-5).

## Global Constraints

- Each controller exposes GET all, GET by id, POST, and PUT.
- Each repository method delegates to a MySQL stored procedure via `IMySqlDataService.ExecSP`.
- Add unit tests under `CarRental.Tests/Countries` and `CarRental.Tests/Customers`, with a test for each action.
- Preserve existing JWT and Admin-only write access conventions.

## Review Focus

- SQL result sets may be empty or contain `DBNull`; map safely and return null/empty collections.
- POST must return 201; GET-by-id must return 404 for missing entities; PUT must return 404 when the repository reports no update.
- The Customers repository must be injected through the same interface registration used by the controller.
- The tests must not require a live database.

---

### Task 1: API unit-test setup and controller tests

**Files:**
- Modify `CarRental.Tests/CarRental.Tests.csproj` to reference `CarRental`.
- Create `CarRental.Tests/Countries/CountriesControllerTests.cs`.
- Create `CarRental.Tests/Customers/CustomersControllerTests.cs`.

Write and run the tests first; each class covers GET all, GET by id, POST, and PUT using mocked repository contracts.

### Task 2: Service contracts and entities

**Files:**
- Create `CarRentalServices/Entities/Country.cs`.
- Create `CarRentalServices/Contracts/ICountryRepository.cs`.
- Update `ICustomerRepository.cs` only if signatures need alignment.

### Task 3: Stored-procedure repositories

**Files:**
- Create `CarRentalServices/Repositories/CountryRepository.cs`.
- Update `CarRentalServices/Repositories/CustomerRepository.cs` to implement GET by id and safe row mapping.

Use `GetCountries`, `GetCountryById`, `InsertCountry`, `UpdateCountry`; for Customer use `GetCustomers`, `GetCustomerById`, `InsertCustomer`, `UpdateCustomer`, and `DeleteCustomer`. Define these in `database/03-create-catalog-stored-procedures.sql`.

### Task 4: Controllers and dependency injection

**Files:**
- Create `CarRental/Controllers/CountriesController.cs`.
- Update `CarRental/Controllers/CustomersController.cs` to expose Get/Get(id)/Post/Put through `ICustomerRepository`.
- Update `CarRental/Program.cs` registrations.

### Task 5: Verification

Run the Countries and Customers unit tests, build the solution, and run the imported Postman collection against the local MySQL instance.
