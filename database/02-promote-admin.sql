-- Registra primero una cuenta con POST /api/auth/register y cambia el usuario abajo.
-- Ejecuta este script solo para la cuenta que debe administrar el catálogo.
USE `CarRentalDb`;

UPDATE `app_users`
SET `role` = 'Admin'
WHERE `normalized_username` = UPPER('CAMBIA_ESTE_USUARIO');
