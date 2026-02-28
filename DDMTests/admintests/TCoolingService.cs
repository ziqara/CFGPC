using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TCoolingService
    {
        [TestMethod]
        public void GetAllCoolings_ReturnsListOfCoolings()
        {
            // Arrange
            var expected = new List<Cooling>
            {
                new Cooling { Name = "Cooling1", Brand = "Brand1", Model = "Model1" },
                new Cooling { Name = "Cooling2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<ICoolingRepository>();
            mockRepo.Setup(r => r.ReadAllCoolings()).Returns(expected);

            var service = new CoolingService(mockRepo.Object, new CoolingValidator());

            // Act
            var result = service.GetAllCoolings();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateCooling_ValidCooling_ReturnsEmptyString()
        {
            // Arrange
            var cooling = new Cooling { Name = "Cooling1", Brand = "Brand1", Model = "Model1", Price = 100, StockQuantity = 10, CoolerType = "air", TdpSupport = 150, FanRpm = 1200, Size = "mid_tower" };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddCooling(It.IsAny<Cooling>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCooling(cooling);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddCooling(It.IsAny<Cooling>()), Times.Once);
        }

        [TestMethod]
        public void CreateCooling_DuplicateCooling_ReturnsErrorMessage()
        {
            // Arrange
            var cooling = new Cooling { Name = "Cooling1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCooling(cooling);

            // Assert
            Assert.AreEqual("Такое охлаждение уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddCooling(It.IsAny<Cooling>()), Times.Never);
        }

        [TestMethod]
        public void CreateCooling_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var cooling = new Cooling { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCooling(cooling);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddCooling(It.IsAny<Cooling>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCooling_ValidCooling_ReturnsEmptyString()
        {
            // Arrange
            var cooling = new Cooling { Name = "Cooling1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 100, StockQuantity = 10, CoolerType = "air", TdpSupport = 150, FanRpm = 1200, Size = "mid_tower" };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateCooling(It.IsAny<Cooling>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCooling(cooling);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateCooling(It.IsAny<Cooling>()), Times.Once);
        }

        [TestMethod]
        public void UpdateCooling_DuplicateCooling_ReturnsErrorMessage()
        {
            // Arrange
            var cooling = new Cooling { Name = "Cooling1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCooling(cooling);

            // Assert
            Assert.AreEqual("Такое охлаждение уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateCooling(It.IsAny<Cooling>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCooling_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var cooling = new Cooling { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICoolingRepository>();
            var mockValidator = new Mock<CoolingValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cooling>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameCooling(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new CoolingService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCooling(cooling);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateCooling(It.IsAny<Cooling>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCooling_Success_ReturnsEmptyString()
        {
            // Arrange
            var cooling = new Cooling { ComponentId = 1, Name = "Cooling1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICoolingRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, new CoolingValidator());

            // Act
            var result = service.DeleteCooling(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteCooling_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICoolingRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new CoolingService(mockRepo.Object, new CoolingValidator());

            // Act
            var result = service.DeleteCooling(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: охлаждение участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCooling_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var cooling = new Cooling { ComponentId = 1, Name = "Cooling1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICoolingRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new CoolingService(mockRepo.Object, new CoolingValidator());

            // Act
            var result = service.DeleteCooling(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
