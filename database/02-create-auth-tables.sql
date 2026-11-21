USE `CarRentalDb`;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `app_users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `username` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `normalized_username` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `email` varchar(254) CHARACTER SET utf8mb4 NOT NULL,
    `password_hash` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `role` varchar(30) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'User',
    CONSTRAINT `PK_app_users` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `IX_app_users_email` ON `app_users` (`email`);

CREATE UNIQUE INDEX `IX_app_users_normalized_username` ON `app_users` (`normalized_username`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261112004732_InitialAuthSchema', '8.0.34');

COMMIT;

