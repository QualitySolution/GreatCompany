/*M!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.19-12.3.2-MariaDB, for Linux (x86_64)
--
-- Host: atlas.srv.qsolution.ru    Database: great_company
-- ------------------------------------------------------
-- Server version	10.11.15-MariaDB-deb12-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*M!100616 SET @OLD_NOTE_VERBOSITY=@@NOTE_VERBOSITY, NOTE_VERBOSITY=0 */;

--
-- Table structure for table `base_parameters`
--

DROP TABLE IF EXISTS `base_parameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `base_parameters` (
  `name` varchar(20) NOT NULL,
  `str_value` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `login` varchar(45) NOT NULL,
  `deactivated` tinyint(1) NOT NULL DEFAULT 0,
  `email` varchar(60) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `admin` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `divisions`
--

DROP TABLE IF EXISTS `divisions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `divisions` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `name` varchar(255) NOT NULL COMMENT 'Название',
  `parent_division_id` int(10) unsigned DEFAULT NULL COMMENT 'Головной дивизион',
  PRIMARY KEY (`id`),
  KEY `fk_divisions_parent_division_idx` (`parent_division_id`),
  CONSTRAINT `fk_divisions_parent_division` FOREIGN KEY (`parent_division_id`) REFERENCES `divisions` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Дивизион';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `name` varchar(255) NOT NULL COMMENT 'Название',
  `tax_regime` enum('vat','entrepreneur','cash') NOT NULL COMMENT 'Налоговый режим: НДС, ИП, наличка',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Счет';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `projects`
--

DROP TABLE IF EXISTS `projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `projects` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `name` varchar(255) NOT NULL COMMENT 'Название',
  `division_id` int(10) unsigned NOT NULL COMMENT 'Дивизион',
  PRIMARY KEY (`id`),
  KEY `fk_projects_division_idx` (`division_id`),
  CONSTRAINT `fk_projects_division` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Проект';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `income_articles`
--

DROP TABLE IF EXISTS `income_articles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `income_articles` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `name` varchar(255) NOT NULL COMMENT 'Название',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Статья дохода';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `expense_articles`
--

DROP TABLE IF EXISTS `expense_articles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `expense_articles` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `name` varchar(255) NOT NULL COMMENT 'Название',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Статья расхода';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `planned_incomes`
--

DROP TABLE IF EXISTS `planned_incomes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `planned_incomes` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `date` date NOT NULL COMMENT 'Дата (важен месяц)',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `project_id` int(10) unsigned NOT NULL COMMENT 'Проект',
  `income_article_id` int(10) unsigned NOT NULL COMMENT 'Статья дохода',
  PRIMARY KEY (`id`),
  KEY `fk_planned_incomes_account_idx` (`account_id`),
  KEY `fk_planned_incomes_project_idx` (`project_id`),
  KEY `fk_planned_incomes_income_article_idx` (`income_article_id`),
  CONSTRAINT `fk_planned_incomes_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_planned_incomes_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_planned_incomes_income_article` FOREIGN KEY (`income_article_id`) REFERENCES `income_articles` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='План - приход';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `actual_incomes`
--

DROP TABLE IF EXISTS `actual_incomes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `actual_incomes` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `date` date NOT NULL COMMENT 'Дата',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `project_id` int(10) unsigned NOT NULL COMMENT 'Проект',
  `income_article_id` int(10) unsigned NOT NULL COMMENT 'Статья дохода',
  `planned_income_id` int(10) unsigned DEFAULT NULL COMMENT 'Ссылка на план',
  PRIMARY KEY (`id`),
  KEY `fk_actual_incomes_account_idx` (`account_id`),
  KEY `fk_actual_incomes_project_idx` (`project_id`),
  KEY `fk_actual_incomes_income_article_idx` (`income_article_id`),
  KEY `fk_actual_incomes_planned_income_idx` (`planned_income_id`),
  CONSTRAINT `fk_actual_incomes_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_actual_incomes_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_actual_incomes_income_article` FOREIGN KEY (`income_article_id`) REFERENCES `income_articles` (`id`),
  CONSTRAINT `fk_actual_incomes_planned_income` FOREIGN KEY (`planned_income_id`) REFERENCES `planned_incomes` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Факт - приход';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `planned_expenses`
--

DROP TABLE IF EXISTS `planned_expenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `planned_expenses` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `date` date NOT NULL COMMENT 'Дата',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `division_id` int(10) unsigned DEFAULT NULL COMMENT 'Дивизион (обязательно или из проекта)',
  `project_id` int(10) unsigned DEFAULT NULL COMMENT 'Проект',
  `expense_article_id` int(10) unsigned NOT NULL COMMENT 'Статья расхода',
  PRIMARY KEY (`id`),
  KEY `fk_planned_expenses_account_idx` (`account_id`),
  KEY `fk_planned_expenses_division_idx` (`division_id`),
  KEY `fk_planned_expenses_project_idx` (`project_id`),
  KEY `fk_planned_expenses_expense_article_idx` (`expense_article_id`),
  CONSTRAINT `fk_planned_expenses_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_planned_expenses_division` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`),
  CONSTRAINT `fk_planned_expenses_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_planned_expenses_expense_article` FOREIGN KEY (`expense_article_id`) REFERENCES `expense_articles` (`id`),
  CONSTRAINT `chk_planned_expenses_division_or_project` CHECK (`division_id` IS NOT NULL OR `project_id` IS NOT NULL)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='План - расход';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `actual_expenses`
--

DROP TABLE IF EXISTS `actual_expenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `actual_expenses` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `date` date NOT NULL COMMENT 'Дата',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `division_id` int(10) unsigned DEFAULT NULL COMMENT 'Дивизион (обязательно или из проекта)',
  `project_id` int(10) unsigned DEFAULT NULL COMMENT 'Проект',
  `expense_article_id` int(10) unsigned NOT NULL COMMENT 'Статья расхода',
  `planned_expense_id` int(10) unsigned DEFAULT NULL COMMENT 'Ссылка на план',
  PRIMARY KEY (`id`),
  KEY `fk_actual_expenses_account_idx` (`account_id`),
  KEY `fk_actual_expenses_division_idx` (`division_id`),
  KEY `fk_actual_expenses_project_idx` (`project_id`),
  KEY `fk_actual_expenses_expense_article_idx` (`expense_article_id`),
  KEY `fk_actual_expenses_planned_expense_idx` (`planned_expense_id`),
  CONSTRAINT `fk_actual_expenses_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_actual_expenses_division` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`),
  CONSTRAINT `fk_actual_expenses_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_actual_expenses_expense_article` FOREIGN KEY (`expense_article_id`) REFERENCES `expense_articles` (`id`),
  CONSTRAINT `fk_actual_expenses_planned_expense` FOREIGN KEY (`planned_expense_id`) REFERENCES `planned_expenses` (`id`),
  CONSTRAINT `chk_actual_expenses_division_or_project` CHECK (`division_id` IS NOT NULL OR `project_id` IS NOT NULL)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Факт - расход';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `accrual_templates`
--

DROP TABLE IF EXISTS `accrual_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `accrual_templates` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `project_id` int(10) unsigned NOT NULL COMMENT 'Проект',
  `income_article_id` int(10) unsigned NOT NULL COMMENT 'Статья дохода',
  PRIMARY KEY (`id`),
  KEY `fk_accrual_templates_account_idx` (`account_id`),
  KEY `fk_accrual_templates_project_idx` (`project_id`),
  KEY `fk_accrual_templates_income_article_idx` (`income_article_id`),
  CONSTRAINT `fk_accrual_templates_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_accrual_templates_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_accrual_templates_income_article` FOREIGN KEY (`income_article_id`) REFERENCES `income_articles` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Шаблон начисления';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `payment_templates`
--

DROP TABLE IF EXISTS `payment_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `payment_templates` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Идентификатор',
  `purpose` text NOT NULL COMMENT 'Назначение',
  `amount` decimal(19,2) NOT NULL COMMENT 'Сумма',
  `vat_amount` decimal(19,2) NOT NULL DEFAULT 0.00 COMMENT 'Сумма НДС',
  `account_id` int(10) unsigned NOT NULL COMMENT 'Счет',
  `division_id` int(10) unsigned DEFAULT NULL COMMENT 'Дивизион (обязательно или из проекта)',
  `project_id` int(10) unsigned DEFAULT NULL COMMENT 'Проект',
  `expense_article_id` int(10) unsigned NOT NULL COMMENT 'Статья расхода',
  PRIMARY KEY (`id`),
  KEY `fk_payment_templates_account_idx` (`account_id`),
  KEY `fk_payment_templates_division_idx` (`division_id`),
  KEY `fk_payment_templates_project_idx` (`project_id`),
  KEY `fk_payment_templates_expense_article_idx` (`expense_article_id`),
  CONSTRAINT `fk_payment_templates_account` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`),
  CONSTRAINT `fk_payment_templates_division` FOREIGN KEY (`division_id`) REFERENCES `divisions` (`id`),
  CONSTRAINT `fk_payment_templates_project` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`),
  CONSTRAINT `fk_payment_templates_expense_article` FOREIGN KEY (`expense_article_id`) REFERENCES `expense_articles` (`id`),
  CONSTRAINT `chk_payment_templates_division_or_project` CHECK (`division_id` IS NOT NULL OR `project_id` IS NOT NULL)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='Шаблон платежей';
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*M!100616 SET NOTE_VERBOSITY=@OLD_NOTE_VERBOSITY */;

-- Dump completed
