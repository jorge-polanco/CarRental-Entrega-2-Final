USE `carrental`;

-- Datos ficticios para la segunda entrega; el script se puede volver a ejecutar.
INSERT INTO `isoCountries` (`Iso2Code`, `Iso3Code`, `CountryName`)
VALUES
  ('US', 'USA', 'Estados Unidos'),
  ('CA', 'CAN', 'Canada')
ON DUPLICATE KEY UPDATE `CountryName` = VALUES(`CountryName`);

INSERT INTO `customers`
  (`FirstName`, `LastName`, `SSN`, `Email`, `CountryId`, `Mobile`, `StateName`)
SELECT 'Carlos', 'Prueba', 'TEST-CUST-002', 'carlos.prueba@example.test', c.`CountryId`, '000-000-0002', 'Dato de prueba'
FROM `isoCountries` AS c
WHERE c.`Iso2Code` = 'US'
ON DUPLICATE KEY UPDATE
  `FirstName` = VALUES(`FirstName`),
  `LastName` = VALUES(`LastName`),
  `CountryId` = VALUES(`CountryId`),
  `Mobile` = VALUES(`Mobile`),
  `StateName` = VALUES(`StateName`);

INSERT INTO `customers`
  (`FirstName`, `LastName`, `SSN`, `Email`, `CountryId`, `Mobile`, `StateName`)
SELECT 'Maria', 'Demo', 'TEST-CUST-003', 'maria.demo@example.test', c.`CountryId`, '000-000-0003', 'Dato de prueba'
FROM `isoCountries` AS c
WHERE c.`Iso2Code` = 'CA'
ON DUPLICATE KEY UPDATE
  `FirstName` = VALUES(`FirstName`),
  `LastName` = VALUES(`LastName`),
  `CountryId` = VALUES(`CountryId`),
  `Mobile` = VALUES(`Mobile`),
  `StateName` = VALUES(`StateName`);
