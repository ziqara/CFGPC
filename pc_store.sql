-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Окт 25 2025 г., 06:21
-- Версия сервера: 5.7.25
-- Версия PHP: 7.1.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `pc_store`
--

-- --------------------------------------------------------

--
-- Структура таблицы `cases`
--

CREATE TABLE `cases` (
  `component_id` int(11) NOT NULL,
  `form_factor` varchar(20) DEFAULT NULL,
  `size` enum('full_tower','mid_tower','compact') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `components`
--

CREATE TABLE `components` (
  `component_id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `brand` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `model` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `type` enum('cpu','motherboard','ram','gpu','storage','psu','case','cooling') COLLATE utf8mb4_unicode_ci NOT NULL,
  `price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `stock_quantity` int(11) NOT NULL DEFAULT '0',
  `description` text COLLATE utf8mb4_unicode_ci,
  `is_available` tinyint(1) NOT NULL DEFAULT '1',
  `photo_url` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `configurations`
--

CREATE TABLE `configurations` (
  `config_id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `total_price` decimal(10,2) DEFAULT '0.00',
  `budget_range` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `target_use` enum('gaming','professional','office','student') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` enum('draft','validated','in_cart','ordered') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'draft',
  `is_preset` tinyint(1) NOT NULL DEFAULT '0',
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `config_components`
--

CREATE TABLE `config_components` (
  `config_component_id` int(11) NOT NULL,
  `config_id` int(11) NOT NULL,
  `component_id` int(11) NOT NULL,
  `quantity` int(11) NOT NULL DEFAULT '1',
  `component_type` enum('cpu','motherboard','ram','gpu','storage','psu','case','cooling') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Триггеры `config_components`
--
DELIMITER $$
CREATE TRIGGER `trg_cc_before_insert` BEFORE INSERT ON `config_components` FOR EACH ROW BEGIN
  DECLARE v_type VARCHAR(20);
  SELECT type INTO v_type FROM components WHERE component_id = NEW.component_id;
  IF v_type IS NULL THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Component not found for config_components';
  END IF;
  SET NEW.component_type = v_type;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_cc_before_update` BEFORE UPDATE ON `config_components` FOR EACH ROW BEGIN
  DECLARE v_type VARCHAR(20);
  SELECT type INTO v_type FROM components WHERE component_id = NEW.component_id;
  IF v_type IS NULL THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Component not found for config_components';
  END IF;
  SET NEW.component_type = v_type;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `config_prefs`
--

CREATE TABLE `config_prefs` (
  `pref_id` int(11) NOT NULL,
  `config_id` int(11) NOT NULL,
  `rgb` tinyint(1) DEFAULT '0',
  `cooling_level` enum('basic','advanced','custom') DEFAULT 'basic',
  `other_options` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `coolings`
--

CREATE TABLE `coolings` (
  `component_id` int(11) NOT NULL,
  `cooler_type` enum('air','liquid') NOT NULL,
  `tdp_support` int(11) DEFAULT NULL,
  `fan_rpm` int(11) DEFAULT NULL,
  `radiator_size` varchar(50) DEFAULT NULL,
  `is_rgb` tinyint(1) DEFAULT '0',
  `compatibility_sockets` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `cpus`
--

CREATE TABLE `cpus` (
  `component_id` int(11) NOT NULL,
  `socket` varchar(50) DEFAULT NULL,
  `cores` int(11) DEFAULT NULL,
  `tdp` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `gpus`
--

CREATE TABLE `gpus` (
  `component_id` int(11) NOT NULL,
  `pcie_version` enum('3.0','4.0') DEFAULT NULL,
  `tdp` int(11) DEFAULT NULL,
  `vram_gb` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `motherboards`
--

CREATE TABLE `motherboards` (
  `component_id` int(11) NOT NULL,
  `socket` varchar(50) DEFAULT NULL,
  `chipset` varchar(50) DEFAULT NULL,
  `ram_type` enum('DDR4','DDR5') DEFAULT NULL,
  `pcie_version` enum('3.0','4.0','5.0') DEFAULT NULL,
  `form_factor` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `orders`
--

CREATE TABLE `orders` (
  `order_id` int(11) NOT NULL,
  `config_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `order_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` enum('pending','processing','assembled','shipped','delivered','cancelled') NOT NULL DEFAULT 'pending',
  `total_price` decimal(10,2) DEFAULT '0.00',
  `delivery_address` text,
  `delivery_method` enum('courier','pickup','self') DEFAULT 'courier',
  `delivery_terms` int(11) DEFAULT NULL,
  `payment_method` enum('card','cash_on_delivery','bank_transfer') DEFAULT 'card',
  `assembly_terms` int(11) DEFAULT NULL,
  `tracking_number` varchar(100) DEFAULT NULL,
  `is_paid` tinyint(1) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Триггеры `orders`
--
DELIMITER $$
CREATE TRIGGER `trg_orders_after_insert` AFTER INSERT ON `orders` FOR EACH ROW BEGIN
  UPDATE components c
  JOIN config_components cc ON c.component_id = cc.component_id
  SET c.stock_quantity = c.stock_quantity - cc.quantity
  WHERE cc.config_id = NEW.config_id;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_orders_after_update` AFTER UPDATE ON `orders` FOR EACH ROW BEGIN
  IF OLD.status <> 'cancelled' AND NEW.status = 'cancelled' THEN
    UPDATE components c
    JOIN config_components cc ON c.component_id = cc.component_id
    SET c.stock_quantity = c.stock_quantity + cc.quantity
    WHERE cc.config_id = NEW.config_id;
  END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_orders_before_insert` BEFORE INSERT ON `orders` FOR EACH ROW BEGIN
  DECLARE v_insufficient INT DEFAULT 0;
  SELECT COUNT(*) INTO v_insufficient
  FROM config_components cc
    JOIN components c ON cc.component_id = c.component_id
  WHERE cc.config_id = NEW.config_id
    AND c.stock_quantity < cc.quantity;
  IF v_insufficient > 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insufficient stock for one or more components in the configuration.';
  END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `psus`
--

CREATE TABLE `psus` (
  `component_id` int(11) NOT NULL,
  `wattage` int(11) DEFAULT NULL,
  `efficiency_rating` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `rams`
--

CREATE TABLE `rams` (
  `component_id` int(11) NOT NULL,
  `type` enum('DDR4','DDR5') DEFAULT NULL,
  `capacity_gb` int(11) DEFAULT NULL,
  `speed_mhz` int(11) DEFAULT NULL,
  `slots_needed` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `reviews`
--

CREATE TABLE `reviews` (
  `review_id` int(11) NOT NULL,
  `order_id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `rating` int(11) NOT NULL,
  `text` text,
  `photo_url` varchar(500) DEFAULT NULL,
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `storages`
--

CREATE TABLE `storages` (
  `component_id` int(11) NOT NULL,
  `interface` enum('SATA','NVMe') DEFAULT NULL,
  `capacity_gb` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `suppliers`
--

CREATE TABLE `suppliers` (
  `supplier_id` int(11) NOT NULL,
  `name` varchar(191) COLLATE utf8mb4_unicode_ci NOT NULL,
  `contact_email` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address` text COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `user_id` int(11) NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `full_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address` text COLLATE utf8mb4_unicode_ci,
  `role` enum('client','admin') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'client',
  `registration_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `warranties`
--

CREATE TABLE `warranties` (
  `warranty_id` int(11) NOT NULL,
  `order_id` int(11) NOT NULL,
  `duration_months` int(11) DEFAULT NULL,
  `issue_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `download_url` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `cases`
--
ALTER TABLE `cases`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `components`
--
ALTER TABLE `components`
  ADD PRIMARY KEY (`component_id`),
  ADD KEY `ix_components_type` (`type`);

--
-- Индексы таблицы `configurations`
--
ALTER TABLE `configurations`
  ADD PRIMARY KEY (`config_id`),
  ADD KEY `ix_config_user` (`user_id`);

--
-- Индексы таблицы `config_components`
--
ALTER TABLE `config_components`
  ADD PRIMARY KEY (`config_component_id`),
  ADD UNIQUE KEY `ux_config_component_type` (`config_id`,`component_type`),
  ADD UNIQUE KEY `ux_config_component` (`config_id`,`component_id`),
  ADD KEY `component_id` (`component_id`),
  ADD KEY `config_id` (`config_id`),
  ADD KEY `ix_cc_type` (`component_type`);

--
-- Индексы таблицы `config_prefs`
--
ALTER TABLE `config_prefs`
  ADD PRIMARY KEY (`pref_id`),
  ADD KEY `config_id` (`config_id`);

--
-- Индексы таблицы `coolings`
--
ALTER TABLE `coolings`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `cpus`
--
ALTER TABLE `cpus`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `gpus`
--
ALTER TABLE `gpus`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `motherboards`
--
ALTER TABLE `motherboards`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `orders`
--
ALTER TABLE `orders`
  ADD PRIMARY KEY (`order_id`),
  ADD UNIQUE KEY `ux_orders_config` (`config_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Индексы таблицы `psus`
--
ALTER TABLE `psus`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `rams`
--
ALTER TABLE `rams`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `reviews`
--
ALTER TABLE `reviews`
  ADD PRIMARY KEY (`review_id`),
  ADD UNIQUE KEY `ux_reviews_order` (`order_id`),
  ADD KEY `fk_reviews_user` (`user_id`);

--
-- Индексы таблицы `storages`
--
ALTER TABLE `storages`
  ADD PRIMARY KEY (`component_id`);

--
-- Индексы таблицы `suppliers`
--
ALTER TABLE `suppliers`
  ADD PRIMARY KEY (`supplier_id`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Индексы таблицы `warranties`
--
ALTER TABLE `warranties`
  ADD PRIMARY KEY (`warranty_id`),
  ADD UNIQUE KEY `ux_warranty_order` (`order_id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `components`
--
ALTER TABLE `components`
  MODIFY `component_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `configurations`
--
ALTER TABLE `configurations`
  MODIFY `config_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `config_components`
--
ALTER TABLE `config_components`
  MODIFY `config_component_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `config_prefs`
--
ALTER TABLE `config_prefs`
  MODIFY `pref_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `orders`
--
ALTER TABLE `orders`
  MODIFY `order_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `reviews`
--
ALTER TABLE `reviews`
  MODIFY `review_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `suppliers`
--
ALTER TABLE `suppliers`
  MODIFY `supplier_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `warranties`
--
ALTER TABLE `warranties`
  MODIFY `warranty_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `cases`
--
ALTER TABLE `cases`
  ADD CONSTRAINT `fk_cases_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `configurations`
--
ALTER TABLE `configurations`
  ADD CONSTRAINT `fk_config_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `config_components`
--
ALTER TABLE `config_components`
  ADD CONSTRAINT `fk_cc_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_cc_config` FOREIGN KEY (`config_id`) REFERENCES `configurations` (`config_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `config_prefs`
--
ALTER TABLE `config_prefs`
  ADD CONSTRAINT `fk_prefs_config` FOREIGN KEY (`config_id`) REFERENCES `configurations` (`config_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `coolings`
--
ALTER TABLE `coolings`
  ADD CONSTRAINT `fk_coolings_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `cpus`
--
ALTER TABLE `cpus`
  ADD CONSTRAINT `fk_cpus_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `gpus`
--
ALTER TABLE `gpus`
  ADD CONSTRAINT `fk_gpus_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `motherboards`
--
ALTER TABLE `motherboards`
  ADD CONSTRAINT `fk_mb_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `orders`
--
ALTER TABLE `orders`
  ADD CONSTRAINT `fk_orders_config` FOREIGN KEY (`config_id`) REFERENCES `configurations` (`config_id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_orders_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `psus`
--
ALTER TABLE `psus`
  ADD CONSTRAINT `fk_psus_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `rams`
--
ALTER TABLE `rams`
  ADD CONSTRAINT `fk_rams_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `fk_reviews_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_reviews_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `storages`
--
ALTER TABLE `storages`
  ADD CONSTRAINT `fk_storages_component` FOREIGN KEY (`component_id`) REFERENCES `components` (`component_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `warranties`
--
ALTER TABLE `warranties`
  ADD CONSTRAINT `fk_warranty_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
