# Seguridad JWT y MySQL

La API `CarRental` agrega registro e inicio de sesión con JWT y almacena las cuentas en la base `CarRentalDb`. Las contraseñas se guardan con el hasher de ASP.NET Core; la API nunca acepta el rol durante el registro. Las consultas de las entidades del negocio conservan los repositorios existentes.

## Crear la base en MySQL Workbench

1. Abre una conexión a tu servidor MySQL local.
2. En Workbench, abre y ejecuta `database/01-create-database.sql`.
3. En la misma conexión de Workbench, abre y ejecuta `database/02-create-auth-tables.sql` para crear `app_users` y `__EFMigrationsHistory`. Como alternativa, aplica la migración desde Visual Studio con los comandos más abajo.
4. Configura `ConnectionStrings:AuthDatabase` para el usuario de tu servidor. Para mantener credenciales fuera del repositorio, puedes guardarla con User Secrets desde la carpeta de la solución:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:AuthDatabase" "Server=127.0.0.1;Port=3306;Database=CarRentalDb;User ID=TU_USUARIO;Password=TU_PASSWORD;" --project .\CarRental\CarRental.csproj
   dotnet user-secrets set "ConnectionStrings:MysqlConnection" "Server=127.0.0.1;Port=3306;Database=carrental;User ID=TU_USUARIO;Password=TU_PASSWORD;" --project .\CarRental\CarRental.csproj
   dotnet user-secrets set "Jwt:Key" "SUSTITUYE_POR_UN_SECRETO_ALEATORIO_DE_AL_MENOS_32_BYTES" --project .\CarRental\CarRental.csproj
   ```

   `Issuer`, `Audience` y duración ya tienen valores de desarrollo en `appsettings.json`. No reutilices la clave JWT de ejemplo en producción.

## Crear las tablas con EF Core

La migración `InitialAuthSchema` ya está incluida. En la Consola del Administrador de paquetes de Visual Studio, selecciona `CarRental` como proyecto predeterminado y ejecuta:

```powershell
Update-Database -Project CarRental -StartupProject CarRental
```

La migración crea `app_users` y sus índices únicos. Workbench conserva el esquema `CarRentalDb` para autenticación; los repositorios SQL Server/MySQL existentes mantienen sus conexiones actuales.

## Países ISO y clientes

Después de crear `CarRentalDb` y las tablas de autenticación, ejecuta `database/03-create-catalog-stored-procedures.sql` en MySQL Workbench para crear el esquema `carrental`, las tablas `isoCountries` y `customers`, y sus procedimientos almacenados. Ejecuta después `database/04-seed-assignment-samples.sql` para insertar datos ficticios de demostración de forma repetible. El conjunto deja al menos tres registros por tabla y permite comprobar las consultas desde la API.

Los proyectos API y CarRentalServices usan .NET 8. El proyecto de pruebas también apunta a .NET 8 para que coincida con los proyectos que prueba.

## Endpoints

- `POST /api/auth/register`: recibe `username`, `email` y `password`; el rol inicial siempre es `User`.
- `POST /api/auth/login`: recibe `username` y `password`; devuelve el JWT y su vencimiento UTC.
- Los endpoints de catálogo y clientes requieren JWT. Las operaciones de escritura requieren rol `Admin`.
- `GET /api/vehiculos`: ejemplo público de la guía.
- `POST /api/vehiculos`: requiere cualquier JWT y devuelve `201 Created`.
- `DELETE /api/vehiculos/{id}`: requiere JWT con rol `Admin`.

El controlador `VehiculosController` es un ejemplo en memoria para practicar JWT; no modifica las tablas del catálogo. Los controladores reales de autos, categorías, marcas, colores, clientes y ubicaciones siguen conectados a sus repositorios existentes.

## Práctica con Postman

Ejecuta `CarRental` con el perfil `http` de Visual Studio (`http://localhost:5231`) y usa estas solicitudes:

1. **Registrar usuario** — `POST http://localhost:5231/api/auth/register`, Body → raw → JSON:

   ```json
   {
     "username": "alumno1",
     "email": "alumno1@example.com",
     "password": "Password123!"
   }
   ```

   Debe responder `201 Created`. El rol siempre será `User`; enviar una propiedad `role` no concede privilegios.

2. **Iniciar sesión** — `POST http://localhost:5231/api/auth/login`:

   ```json
   {
     "username": "alumno1",
     "password": "Password123!"
   }
   ```

   Copia el valor `token` de la respuesta.

3. **Comprobar que la ruta está protegida** — `POST http://localhost:5231/api/vehiculos`, Body raw JSON: `"Tesla Model 3"`. Sin Authorization debe responder `401 Unauthorized`.

4. **Probar acceso con JWT** — repite la solicitud anterior con Authorization → Bearer Token y pega el token. Debe responder `201 Created`.

5. **Probar autorización por rol** — `DELETE http://localhost:5231/api/vehiculos/1` con el token del usuario `alumno1`. Debe responder `403 Forbidden`, porque el registro público asigna rol `User`.

6. **Probar como Admin** — después de registrar una cuenta de práctica, actualiza el usuario en `database/02-promote-admin.sql`, ejecuta ese script en Workbench, inicia sesión de nuevo para obtener un JWT con el rol actualizado y repite el DELETE. Debe responder `200 OK`.

`GET /api/vehiculos` puede usarse para comprobar el endpoint público; no requiere token.

Para habilitar una cuenta administradora, registra la cuenta, cambia `CAMBIA_ESTE_USUARIO` en `database/02-promote-admin.sql` y ejecuta el script desde Workbench.

En Swagger, usa **Authorize** y pega el token que devolvió el login.
