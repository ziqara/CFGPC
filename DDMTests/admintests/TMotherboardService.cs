using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TMotherboardService
    {
        [TestMethod]
        public void GetAllMotherboards_ReturnsListOfMotherboards()
        {
            // Arrange
            var expected = new List<Motherboard>
            {
                new Motherboard { Name = "MB1", Brand = "Brand1", Model = "Model1" },
                new Motherboard { Name = "MB2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<IMotherboardRepository>();
            mockRepo.Setup(r => r.ReadAllMotherboards()).Returns(expected);

            var service = new MotherboardService(mockRepo.Object, new MotherboardValidator());

            // Act
            var result = service.GetAllMotherboards();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateMotherboard_ValidMotherboard_ReturnsEmptyString()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "MB1", Brand = "Brand1", Model = "Model1", Price = 150, StockQuantity = 5, RamType = "DDR4", PcieVersion = "4.0" };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddMotherboard(It.IsAny<Motherboard>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateMotherboard(motherboard);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddMotherboard(It.IsAny<Motherboard>()), Times.Once);
        }

        [TestMethod]
        public void CreateMotherboard_DuplicateMotherboard_ReturnsErrorMessage()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "MB1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateMotherboard(motherboard);

            // Assert
            Assert.AreEqual("Такая материнская плата уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddMotherboard(It.IsAny<Motherboard>()), Times.Never);
        }

        [TestMethod]
        public void CreateMotherboard_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateMotherboard(motherboard);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddMotherboard(It.IsAny<Motherboard>()), Times.Never);
        }

        [TestMethod]
        public void UpdateMotherboard_ValidMotherboard_ReturnsEmptyString()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "MB1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 150, StockQuantity = 5, RamType = "DDR4", PcieVersion = "4.0" };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateMotherboard(It.IsAny<Motherboard>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateMotherboard(motherboard);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateMotherboard(It.IsAny<Motherboard>()), Times.Once);
        }

        [TestMethod]
        public void UpdateMotherboard_DuplicateMotherboard_ReturnsErrorMessage()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "MB1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateMotherboard(motherboard);

            // Assert
            Assert.AreEqual("Такая материнская плата уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateMotherboard(It.IsAny<Motherboard>()), Times.Never);
        }

        [TestMethod]
        public void UpdateMotherboard_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var motherboard = new Motherboard { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IMotherboardRepository>();
            var mockValidator = new Mock<MotherboardValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Motherboard>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameMotherboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new MotherboardService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateMotherboard(motherboard);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateMotherboard(It.IsAny<Motherboard>()), Times.Never);
        }

        [TestMethod]
        public void DeleteMotherboard_Success_ReturnsEmptyString()
        {
            // Arrange
            var motherboard = new Motherboard { ComponentId = 1, Name = "MB1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IMotherboardRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, new MotherboardValidator());

            // Act
            var result = service.DeleteMotherboard(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteMotherboard_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IMotherboardRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new MotherboardService(mockRepo.Object, new MotherboardValidator());

            // Act
            var result = service.DeleteMotherboard(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: материнская плата участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteMotherboard_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var motherboard = new Motherboard { ComponentId = 1, Name = "MB1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IMotherboardRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new MotherboardService(mockRepo.Object, new MotherboardValidator());

            // Act
            var result = service.DeleteMotherboard(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
