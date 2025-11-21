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
        }
    }
}
