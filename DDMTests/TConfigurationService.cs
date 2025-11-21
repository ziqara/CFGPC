using DDMLib.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDMTests
{
    [TestClass]
    public class TConfigurationService
    {
        [TestMethod]
        public void TestGetUserConfigurations_EmptyList_ReturnsEmptyList()
        {
            Mock<IConfigurationRepository> mockRepository = new Mock<IConfigurationRepository>();
            string testUserEmail = "test@example.com"; // Исходные данные
            List<ConfigurationDto> expectedConfigurations = new List<ConfigurationDto>(); // Пустой список

            // Настройка поведения мока
            mockRepository
                .Setup(repo => repo.GetUserConfigurations(testUserEmail))
                .Returns(expectedConfigurations);

            // Создание тестируемого сервиса с подставленным моком
            ConfigurationService service = new ConfigurationService(mockRepository.Object);

            List<ConfigurationDto> result = service.GetUserConfigurations(testUserEmail);

            Assert.IsNotNull(result); // Проверка, что результат не null
            Assert.AreEqual(0, result.Count); // Проверка, что Count = 0
            // Проверка, что метод репозитория был вызван
            mockRepository.Verify(repo => repo.GetUserConfigurations(testUserEmail), Times.Once);
        }

        [TestMethod]
        public void TestGetUserConfigurations_WithConfigAndComponents_ReturnsConfigurationWithComponents()
        {
            // 1. Arrange (Подготовка)
            string testUserEmail = "user@example.com";

            // Создание объекта Configuration
            Configuration config = new Configuration
            {
                ConfigId = 1,
                ConfigName = "Игровая ракета",
                Description = "Для 4K игр",
                TotalPrice = 150000,
                TargetUse = "gaming",
                Status = "draft",
                IsPreset = false,
                CreatedDate = new System.DateTime(2025, 11, 18, 10, 0, 0),
                UserEmail = testUserEmail,
                Rgb = true,
                OtherOptions = "Тихая система"
            };

            // Создание объектов Component
            DDMLib.Component.Component gpuComponent = new DDMLib.Component.Component
            {
                ComponentId = 101,
                Name = "RTX 4090",
                Type = "gpu",
            };

            DDMLib.Component.Component cpuComponent = new DDMLib.Component.Component
            {
                ComponentId = 102,
                Name = "Intel i9-14900K",
                Type = "cpu",
            };

            // Создание объекта ConfigurationDto
            ConfigurationDto configDto = new ConfigurationDto
            {
                Configuration = config,
                Components = new List<DDMLib.Component.Component> { gpuComponent, cpuComponent }
            };

            List<ConfigurationDto> expectedConfigurations = new List<ConfigurationDto> { configDto };

            // Настройка мока
            Mock<IConfigurationRepository> mockRepository = new Mock<IConfigurationRepository>();
            mockRepository
                .Setup(repo => repo.GetUserConfigurations(testUserEmail))
                .Returns(expectedConfigurations);

            // Создание тестируемого сервиса с подставленным моком
            ConfigurationService service = new ConfigurationService(mockRepository.Object);

            // 2. Act (Действие)
            List<ConfigurationDto> result = service.GetUserConfigurations(testUserEmail);

            // 3. Assert (Проверка)
            Assert.IsNotNull(result); // Проверка, что результат не null
            Assert.AreEqual(1, result.Count); // Проверка, что количество конфигураций = 1

            // Проверка, что метод репозитория был вызван
            mockRepository.Verify(repo => repo.GetUserConfigurations(testUserEmail), Times.Once);

            // Получаем первую (и единственную) конфигурацию из результата
            ConfigurationDto firstConfigDto = result[0];

            // Проверяем основные поля конфигурации
            Assert.AreEqual("Игровая ракета", firstConfigDto.Configuration.ConfigName);
            Assert.AreEqual("Для 4K игр", firstConfigDto.Configuration.Description);
            Assert.AreEqual(150000, firstConfigDto.Configuration.TotalPrice);
            Assert.AreEqual("gaming", firstConfigDto.Configuration.TargetUse);
            Assert.AreEqual("draft", firstConfigDto.Configuration.Status); // "draft" -> "Черновик"
            Assert.AreEqual(new System.DateTime(2025, 11, 18, 10, 0, 0), firstConfigDto.Configuration.CreatedDate);
            Assert.AreEqual(true, firstConfigDto.Configuration.Rgb); // RGB: Да
            Assert.AreEqual("Тихая система", firstConfigDto.Configuration.OtherOptions);

            // Проверяем список компонентов
            Assert.IsNotNull(firstConfigDto.Components); // Проверка, что список компонентов не null
            Assert.AreEqual(2, firstConfigDto.Components.Count); // Проверка, что количество компонентов = 2

            // Проверяем, что компоненты содержат ожидаемые значения (можно проверить по порядку или по ID)
            // Проверка первого компонента (GPU)
            DDMLib.Component.Component firstComponent = firstConfigDto.Components[0];
            Assert.AreEqual("RTX 4090", firstComponent.Name);
            Assert.AreEqual("gpu", firstComponent.Type);

            // Проверка второго компонента (CPU)
            DDMLib.Component.Component secondComponent = firstConfigDto.Components[1];
            Assert.AreEqual("Intel i9-14900K", secondComponent.Name);
            Assert.AreEqual("cpu", secondComponent.Type);
        }

        [TestMethod]
        public void TestGetUserConfigurations_WithConfigButNoComponents_ReturnsConfigurationWithEmptyComponentsList()
        {
            // 1. Arrange (Подготовка)
            string testUserEmail = "another@example.com";

            // Создание объекта Configuration
            Configuration config = new Configuration
            {
                ConfigId = 2,
                ConfigName = "Черновик 1",
                Description = "", // Пустое описание
                TotalPrice = 0, // Нулевая цена
                TargetUse = "office", // Цель - офис
                Status = "draft", // Статус - черновик
                IsPreset = false,
                CreatedDate = new System.DateTime(2025, 11, 17, 15, 30, 0), // Дата создания
                UserEmail = testUserEmail,
                Rgb = false, // Нет RGB
                OtherOptions = "" // Нет других опций
            };

            // Создание объекта ConfigurationDto с ПУСТЫМ списком компонентов
            ConfigurationDto configDto = new ConfigurationDto
            {
                Configuration = config,
                Components = new List<DDMLib.Component.Component>() // Пустой список
            };

            List<ConfigurationDto> expectedConfigurations = new List<ConfigurationDto> { configDto };

            // Настройка мока
            Mock<IConfigurationRepository> mockRepository = new Mock<IConfigurationRepository>();
            mockRepository
                .Setup(repo => repo.GetUserConfigurations(testUserEmail))
                .Returns(expectedConfigurations);

            // Создание тестируемого сервиса с подставленным моком
            ConfigurationService service = new ConfigurationService(mockRepository.Object);
        }
    }
}
