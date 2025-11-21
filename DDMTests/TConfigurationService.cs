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
        }
    }
}
