CREATE DATABASE IF NOT EXISTS `carrental`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `carrental`;

CREATE TABLE IF NOT EXISTS `isoCountries` (
  `CountryId` int NOT NULL AUTO_INCREMENT,
  `Iso2Code` char(2) NOT NULL,
  `Iso3Code` char(3) NOT NULL,
  `CountryName` varchar(100) NOT NULL,
  PRIMARY KEY (`CountryId`),
  UNIQUE KEY `UX_isoCountries_Iso2Code` (`Iso2Code`),
  UNIQUE KEY `UX_isoCountries_Iso3Code` (`Iso3Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `customers` (
  `IdCustomer` int NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `SSN` varchar(50) NOT NULL,
  `Email` varchar(254) NOT NULL,
  `CountryId` int NOT NULL,
  `Mobile` varchar(30) NOT NULL,
  `StateName` varchar(100) NOT NULL,
  PRIMARY KEY (`IdCustomer`),
  UNIQUE KEY `UX_customers_SSN` (`SSN`),
  UNIQUE KEY `UX_customers_Email` (`Email`),
  KEY `IX_customers_CountryId` (`CountryId`),
  CONSTRAINT `FK_customers_isoCountries_CountryId`
    FOREIGN KEY (`CountryId`) REFERENCES `isoCountries` (`CountryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `isoCountries` (`Iso2Code`, `Iso3Code`, `CountryName`)
VALUES ('MX', 'MEX', CONVERT(0x4DC3A97869636F USING utf8mb4))
ON DUPLICATE KEY UPDATE `CountryName` = VALUES(`CountryName`);

DROP PROCEDURE IF EXISTS `GetCountries`;
DROP PROCEDURE IF EXISTS `GetCountryById`;
DROP PROCEDURE IF EXISTS `InsertCountry`;
DROP PROCEDURE IF EXISTS `UpdateCountry`;
DROP PROCEDURE IF EXISTS `GetCustomers`;
DROP PROCEDURE IF EXISTS `GetCustomerById`;
DROP PROCEDURE IF EXISTS `InsertCustomer`;
DROP PROCEDURE IF EXISTS `UpdateCustomer`;
DROP PROCEDURE IF EXISTS `DeleteCustomer`;

DELIMITER $$

CREATE PROCEDURE `GetCountries`()
BEGIN
  SELECT `CountryId`, `Iso2Code`, `Iso3Code`, `CountryName`
  FROM `isoCountries`
  ORDER BY `CountryName`;
END$$

CREATE PROCEDURE `GetCountryById`(IN pCountryId int)
BEGIN
  SELECT `CountryId`, `Iso2Code`, `Iso3Code`, `CountryName`
  FROM `isoCountries`
  WHERE `CountryId` = pCountryId;
END$$

CREATE PROCEDURE `InsertCountry`(
  IN pIso2Code varchar(2),
  IN pIso3Code varchar(3),
  IN pCountryName varchar(100)
)
BEGIN
  INSERT INTO `isoCountries` (`Iso2Code`, `Iso3Code`, `CountryName`)
  VALUES (pIso2Code, pIso3Code, pCountryName);
  SELECT LAST_INSERT_ID() AS `newCountryId`;
END$$

CREATE PROCEDURE `UpdateCountry`(
  IN pCountryId int,
  IN pIso2Code varchar(2),
  IN pIso3Code varchar(3),
  IN pCountryName varchar(100)
)
BEGIN
  UPDATE `isoCountries`
  SET `Iso2Code` = pIso2Code,
      `Iso3Code` = pIso3Code,
      `CountryName` = pCountryName
  WHERE `CountryId` = pCountryId;
  SELECT ROW_COUNT() AS `affectedRows`;
END$$

CREATE PROCEDURE `GetCustomers`()
BEGIN
  SELECT c.`IdCustomer`, c.`FirstName`, c.`LastName`, c.`SSN`, c.`Email`,
         country.`CountryName` AS `Country`, c.`CountryId`, c.`Mobile`, c.`StateName`
  FROM `customers` AS c
  INNER JOIN `isoCountries` AS country ON country.`CountryId` = c.`CountryId`
  ORDER BY c.`IdCustomer`;
END$$

CREATE PROCEDURE `GetCustomerById`(IN pCustomerId int)
BEGIN
  SELECT c.`IdCustomer`, c.`FirstName`, c.`LastName`, c.`SSN`, c.`Email`,
         country.`CountryName` AS `Country`, c.`CountryId`, c.`Mobile`, c.`StateName`
  FROM `customers` AS c
  INNER JOIN `isoCountries` AS country ON country.`CountryId` = c.`CountryId`
  WHERE c.`IdCustomer` = pCustomerId;
END$$

CREATE PROCEDURE `InsertCustomer`(
  IN pFirstName varchar(50),
  IN pLastName varchar(50),
  IN pSSN varchar(50),
  IN pEmail varchar(254),
  IN pCountryId int,
  IN pMobile varchar(30),
  IN pStateName varchar(100)
)
BEGIN
  INSERT INTO `customers` (`FirstName`, `LastName`, `SSN`, `Email`, `CountryId`, `Mobile`, `StateName`)
  VALUES (pFirstName, pLastName, pSSN, pEmail, pCountryId, pMobile, pStateName);
  SELECT LAST_INSERT_ID() AS `newCustomerId`;
END$$

CREATE PROCEDURE `UpdateCustomer`(
  IN pCustomerId int,
  IN pFirstName varchar(50),
  IN pLastName varchar(50),
  IN pSSN varchar(50),
  IN pEmail varchar(254),
  IN pCountryId int,
  IN pMobile varchar(30),
  IN pStateName varchar(100)
)
BEGIN
  UPDATE `customers`
  SET `FirstName` = pFirstName,
      `LastName` = pLastName,
      `SSN` = pSSN,
      `Email` = pEmail,
      `CountryId` = pCountryId,
      `Mobile` = pMobile,
      `StateName` = pStateName
  WHERE `IdCustomer` = pCustomerId;
  SELECT ROW_COUNT() AS `affectedRows`;
END$$

CREATE PROCEDURE `DeleteCustomer`(IN pCustomerId int)
BEGIN
  DELETE FROM `customers` WHERE `IdCustomer` = pCustomerId;
  SELECT ROW_COUNT() AS `affectedRows`;
END$$

DELIMITER ;
