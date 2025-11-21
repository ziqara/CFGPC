-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Ноя 15 2025 г., 06:32
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
  `componentId` int(11) NOT NULL,
  `formFactor` varchar(20) DEFAULT NULL,
  `size` enum('full_tower','mid_tower','compact') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `cases`
--

INSERT INTO `cases` (`componentId`, `formFactor`, `size`) VALUES
(7, 'mATX', 'full_tower');

-- --------------------------------------------------------

--
-- Структура таблицы `components`
--

CREATE TABLE `components` (
  `componentId` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `brand` varchar(100) DEFAULT NULL,
  `model` varchar(100) DEFAULT NULL,
  `componentType` enum('cpu','motherboard','ram','gpu','storage','psu','case','cooling') NOT NULL,
  `price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `stockQuantity` int(11) NOT NULL DEFAULT '0',
  `description` text,
  `isAvailable` tinyint(1) NOT NULL DEFAULT '1',
  `photoUrl` varchar(500) DEFAULT NULL,
  `supplierInn` int(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `components`
--

INSERT INTO `components` (`componentId`, `name`, `brand`, `model`, `componentType`, `price`, `stockQuantity`, `description`, `isAvailable`, `photoUrl`, `supplierInn`) VALUES
(1, 'Intel Core i5-13400F', 'Intel', 'i5-13400F', 'cpu', '15000.00', 10, '10 ядер, 4.6 ГГц', 1, '/Resources/cpu.jpg', 1),
(2, 'MSI B550M PRO-VDH WiFi', 'MSI', 'B550M PRO-VDH WiFi', 'motherboard', '8000.00', 10, 'Socket AM4, mATX', 1, '/Resources/mobo.jpg', 1),
(3, 'Corsair Vengeance LPX 16GB (2x8GB) DDR4-3200', 'Corsair', 'CMK16GX4M2B3200C16', 'ram', '5000.00', 10, 'DDR4, CL16', 1, '/Resources/ram.jpg', 1),
(4, 'NVIDIA GeForce RTX 4060 8GB', 'NVIDIA', 'RTX 4060', 'gpu', '35000.00', 10, 'PCIe 4.0, 1830 МГц', 1, '/Resources/gpu.jpg', 1),
(5, 'Samsung 980 PRO 1TB', 'Samsung', 'MZ-V8P1T0B/AM', 'storage', '8000.00', 10, 'NVMe M.2 2280', 1, '/Resources/storage.jpg', 1),
(6, 'Corsair RM750x (2021)', 'Corsair', 'CP-9020186-NA', 'psu', '8000.00', 10, '750W, 80+ Gold', 1, '/Resources/psu.jpg', 1),
(7, 'Fractal Design Core 1000', 'Fractal Design', 'FD-C-CORE-1000-BK', 'case', '3000.00', 10, 'mATX, без БП', 1, '/Resources/case.jpg', 1),
(8, 'Noctua NH-U12S Redux', 'Noctua', 'NH-U12S REDUX-1700', 'cooling', '4000.00', 10, 'Воздушное, 1580 RPM', 1, '/Resources/cooling.jpg', 1);

-- --------------------------------------------------------

--
-- Структура таблицы `configurations`
--

CREATE TABLE `configurations` (
  `configId` int(11) NOT NULL,
  `configName` varchar(255) NOT NULL,
  `description` text,
  `totalPrice` decimal(10,2) DEFAULT '0.00',
  `targetUse` enum('gaming','professional','office','student') DEFAULT NULL,
  `status` enum('draft','validated','in_cart','ordered') NOT NULL DEFAULT 'draft',
  `isPreset` tinyint(1) NOT NULL DEFAULT '0',
  `createdDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `userEmail` varchar(191) DEFAULT NULL,
  `rgb` tinyint(1) DEFAULT '0',
  `otherOptions` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `config_components`
--

CREATE TABLE `config_components` (
  `configComponentId` int(11) NOT NULL,
  `configId` int(11) NOT NULL,
  `componentId` int(11) NOT NULL,
  `quantity` int(11) NOT NULL DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `coolings`
--

CREATE TABLE `coolings` (
  `componentId` int(11) NOT NULL,
  `coolerType` enum('air','liquid') NOT NULL,
  `tdpSupport` int(11) DEFAULT NULL,
  `fanRpm` int(11) DEFAULT NULL,
  `size` enum('full_tower','mid_tower','compact') DEFAULT NULL,
  `isRgb` tinyint(1) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `coolings`
--

INSERT INTO `coolings` (`componentId`, `coolerType`, `tdpSupport`, `fanRpm`, `size`, `isRgb`) VALUES
(8, 'air', 150, 1580, 'full_tower', 0);

-- --------------------------------------------------------

--
-- Структура таблицы `cpus`
--

CREATE TABLE `cpus` (
  `componentId` int(11) NOT NULL,
  `socket` varchar(50) DEFAULT NULL,
  `cores` int(11) DEFAULT NULL,
  `tdp` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `cpus`
--

INSERT INTO `cpus` (`componentId`, `socket`, `cores`, `tdp`) VALUES
(1, 'LGA1700', 10, 159);

-- --------------------------------------------------------

--
-- Структура таблицы `gpus`
--

CREATE TABLE `gpus` (
  `componentId` int(11) NOT NULL,
  `pcieVersion` enum('3.0','4.0') DEFAULT NULL,
  `tdp` int(11) DEFAULT NULL,
  `vramGb` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `gpus`
--

INSERT INTO `gpus` (`componentId`, `pcieVersion`, `tdp`, `vramGb`) VALUES
(4, '4.0', 115, 8);

-- --------------------------------------------------------

--
-- Структура таблицы `motherboards`
--

CREATE TABLE `motherboards` (
  `componentId` int(11) NOT NULL,
  `socket` varchar(50) DEFAULT NULL,
  `chipset` varchar(50) DEFAULT NULL,
  `ramType` enum('DDR4','DDR5') DEFAULT NULL,
  `pcieVersion` enum('3.0','4.0','5.0') DEFAULT NULL,
  `formFactor` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `motherboards`
--

INSERT INTO `motherboards` (`componentId`, `socket`, `chipset`, `ramType`, `pcieVersion`, `formFactor`) VALUES
(2, 'AM4', 'B550', 'DDR4', '4.0', 'mATX');

-- --------------------------------------------------------

--
-- Структура таблицы `orders`
--

CREATE TABLE `orders` (
  `orderId` int(11) NOT NULL,
  `configId` int(11) NOT NULL,
  `userEmail` varchar(191) NOT NULL,
  `orderDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` enum('pending','processing','assembled','shipped','delivered','cancelled') NOT NULL DEFAULT 'pending',
  `totalPrice` decimal(10,2) DEFAULT '0.00',
  `deliveryAddress` text,
  `deliveryMethod` enum('courier','pickup','self') DEFAULT 'courier',
  `paymentMethod` enum('card','cash_on_delivery','bank_transfer') DEFAULT 'card',
  `isPaid` tinyint(1) DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `psus`
--

CREATE TABLE `psus` (
  `componentId` int(11) NOT NULL,
  `wattage` int(11) DEFAULT NULL,
  `efficiencyRating` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `psus`
--

INSERT INTO `psus` (`componentId`, `wattage`, `efficiencyRating`) VALUES
(6, 750, '80+ Gold');

-- --------------------------------------------------------

--
-- Структура таблицы `rams`
--

CREATE TABLE `rams` (
  `componentId` int(11) NOT NULL,
  `ramType` enum('DDR4','DDR5') DEFAULT NULL,
  `capacityGb` int(11) DEFAULT NULL,
  `speedMhz` int(11) DEFAULT NULL,
  `slotsNeeded` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `rams`
--

INSERT INTO `rams` (`componentId`, `ramType`, `capacityGb`, `speedMhz`, `slotsNeeded`) VALUES
(3, 'DDR4', 16, 3200, 2);

-- --------------------------------------------------------

--
-- Структура таблицы `reviews`
--

CREATE TABLE `reviews` (
  `orderId` int(11) NOT NULL,
  `userEmail` varchar(191) DEFAULT NULL,
  `rating` int(11) NOT NULL,
  `text` text,
  `photoUrl` varchar(500) DEFAULT NULL,
  `createdDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `storages`
--

CREATE TABLE `storages` (
  `componentId` int(11) NOT NULL,
  `interface` enum('SATA','NVMe') DEFAULT NULL,
  `capacityGb` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `storages`
--

INSERT INTO `storages` (`componentId`, `interface`, `capacityGb`) VALUES
(5, 'NVMe', 1000);

-- --------------------------------------------------------

--
-- Структура таблицы `suppliers`
--

CREATE TABLE `suppliers` (
  `inn` int(9) NOT NULL,
  `name` varchar(191) NOT NULL,
  `contactEmail` varchar(255) NOT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `address` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Дамп данных таблицы `suppliers`
--

INSERT INTO `suppliers` (`inn`, `name`, `contactEmail`, `phone`, `address`) VALUES
(1, 's', 'ss@gmail.com', '1212', 'sdsddssddssdsdsddsdsdssddssddssd');

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `email` varchar(191) NOT NULL,
  `passwordHash` varchar(255) NOT NULL,
  `fullName` varchar(255) DEFAULT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `address` text,
  `registrationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Структура таблицы `warranties`
--

CREATE TABLE `warranties` (
  `warrantyId` int(11) NOT NULL,
  `orderId` int(11) NOT NULL,
  `durationMonths` int(11) DEFAULT NULL,
  `issueDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `downloadUrl` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `cases`
--
ALTER TABLE `cases`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `components`
--
ALTER TABLE `components`
  ADD PRIMARY KEY (`componentId`),
  ADD KEY `ix_components_type` (`componentType`),
  ADD KEY `fk_components_supplier` (`supplierInn`);

--
-- Индексы таблицы `configurations`
--
ALTER TABLE `configurations`
  ADD PRIMARY KEY (`configId`),
  ADD KEY `ix_config_user` (`userEmail`);

--
-- Индексы таблицы `config_components`
--
ALTER TABLE `config_components`
  ADD PRIMARY KEY (`configComponentId`),
  ADD UNIQUE KEY `ux_cc_config_component` (`configId`,`componentId`),
  ADD KEY `fk_cc_component` (`componentId`);

--
-- Индексы таблицы `coolings`
--
ALTER TABLE `coolings`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `cpus`
--
ALTER TABLE `cpus`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `gpus`
--
ALTER TABLE `gpus`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `motherboards`
--
ALTER TABLE `motherboards`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `orders`
--
ALTER TABLE `orders`
  ADD PRIMARY KEY (`orderId`),
  ADD KEY `fk_orders_config` (`configId`),
  ADD KEY `fk_orders_user` (`userEmail`);

--
-- Индексы таблицы `psus`
--
ALTER TABLE `psus`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `rams`
--
ALTER TABLE `rams`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `reviews`
--
ALTER TABLE `reviews`
  ADD PRIMARY KEY (`orderId`),
  ADD KEY `fk_reviews_user` (`userEmail`);

--
-- Индексы таблицы `storages`
--
ALTER TABLE `storages`
  ADD PRIMARY KEY (`componentId`);

--
-- Индексы таблицы `suppliers`
--
ALTER TABLE `suppliers`
  ADD PRIMARY KEY (`inn`),
  ADD UNIQUE KEY `ux_suppliers_email` (`contactEmail`),
  ADD UNIQUE KEY `ux_suppliers_name` (`name`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`email`);

--
-- Индексы таблицы `warranties`
--
ALTER TABLE `warranties`
  ADD PRIMARY KEY (`warrantyId`),
  ADD UNIQUE KEY `ux_warranty_order` (`orderId`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `components`
--
ALTER TABLE `components`
  MODIFY `componentId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT для таблицы `configurations`
--
ALTER TABLE `configurations`
  MODIFY `configId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `config_components`
--
ALTER TABLE `config_components`
  MODIFY `configComponentId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `orders`
--
ALTER TABLE `orders`
  MODIFY `orderId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `warranties`
--
ALTER TABLE `warranties`
  MODIFY `warrantyId` int(11) NOT NULL AUTO_INCREMENT;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `cases`
--
ALTER TABLE `cases`
  ADD CONSTRAINT `fk_cases_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `components`
--
ALTER TABLE `components`
  ADD CONSTRAINT `fk_components_supplier` FOREIGN KEY (`supplierInn`) REFERENCES `suppliers` (`inn`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `configurations`
--
ALTER TABLE `configurations`
  ADD CONSTRAINT `fk_config_user` FOREIGN KEY (`userEmail`) REFERENCES `users` (`email`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `config_components`
--
ALTER TABLE `config_components`
  ADD CONSTRAINT `fk_cc_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_cc_config` FOREIGN KEY (`configId`) REFERENCES `configurations` (`configId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `coolings`
--
ALTER TABLE `coolings`
  ADD CONSTRAINT `fk_coolings_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `cpus`
--
ALTER TABLE `cpus`
  ADD CONSTRAINT `fk_cpus_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `gpus`
--
ALTER TABLE `gpus`
  ADD CONSTRAINT `fk_gpus_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `motherboards`
--
ALTER TABLE `motherboards`
  ADD CONSTRAINT `fk_motherboards_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `orders`
--
ALTER TABLE `orders`
  ADD CONSTRAINT `fk_orders_config` FOREIGN KEY (`configId`) REFERENCES `configurations` (`configId`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_orders_user` FOREIGN KEY (`userEmail`) REFERENCES `users` (`email`) ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `psus`
--
ALTER TABLE `psus`
  ADD CONSTRAINT `fk_psus_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `rams`
--
ALTER TABLE `rams`
  ADD CONSTRAINT `fk_rams_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `fk_reviews_order` FOREIGN KEY (`orderId`) REFERENCES `orders` (`orderId`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_reviews_user` FOREIGN KEY (`userEmail`) REFERENCES `users` (`email`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `storages`
--
ALTER TABLE `storages`
  ADD CONSTRAINT `fk_storages_component` FOREIGN KEY (`componentId`) REFERENCES `components` (`componentId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `warranties`
--
ALTER TABLE `warranties`
  ADD CONSTRAINT `fk_warranty_order` FOREIGN KEY (`orderId`) REFERENCES `orders` (`orderId`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
