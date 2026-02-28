using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TRamService
    {
        [TestMethod]
        public void GetAllRams_ReturnsListOfRams()
        {
            // Arrange
            var expected = new List<Ram>
            {
                new Ram { Name = "RAM1", Brand = "Brand1", Model = "Model1" },
                new Ram { Name = "RAM2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<IRamRepository>();
            mockRepo.Setup(r => r.ReadAllRams()).Returns(expected);

            var service = new RamService(mockRepo.Object, new RamValidator());

            // Act
            var result = service.GetAllRams();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateRam_ValidRam_ReturnsEmptyString()
        {
            // Arrange
            var ram = new Ram { Name = "RAM1", Brand = "Brand1", Model = "Model1", Price = 100, StockQuantity = 10, RamType = "DDR4", CapacityGb = 8, SpeedMhz = 3200, SlotsNeeded = 2 };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddRam(It.IsAny<Ram>())).Returns(true);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateRam(ram);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddRam(It.IsAny<Ram>()), Times.Once);
        }

        [TestMethod]
        public void CreateRam_DuplicateRam_ReturnsErrorMessage()
        {
            // Arrange
            var ram = new Ram { Name = "RAM1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateRam(ram);

            // Assert
            Assert.AreEqual("Такая оперативная память уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddRam(It.IsAny<Ram>()), Times.Never);
        }

        [TestMethod]
        public void CreateRam_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var ram = new Ram { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateRam(ram);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddRam(It.IsAny<Ram>()), Times.Never);
        }

        [TestMethod]
        public void UpdateRam_ValidRam_ReturnsEmptyString()
        {
            // Arrange
            var ram = new Ram { Name = "RAM1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 100, StockQuantity = 10, RamType = "DDR4", CapacityGb = 8, SpeedMhz = 3200, SlotsNeeded = 2 };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateRam(It.IsAny<Ram>())).Returns(true);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateRam(ram);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateRam(It.IsAny<Ram>()), Times.Once);
        }

        [TestMethod]
        public void UpdateRam_DuplicateRam_ReturnsErrorMessage()
        {
            // Arrange
            var ram = new Ram { Name = "RAM1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateRam(ram);

            // Assert
            Assert.AreEqual("Такая оперативная память уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateRam(It.IsAny<Ram>()), Times.Never);
        }

        [TestMethod]
        public void UpdateRam_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var ram = new Ram { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IRamRepository>();
            var mockValidator = new Mock<RamValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Ram>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameRam(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new RamService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateRam(ram);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateRam(It.IsAny<Ram>()), Times.Never);
        }

        [TestMethod]
        public void DeleteRam_Success_ReturnsEmptyString()
        {
            // Arrange
            var ram = new Ram { ComponentId = 1, Name = "RAM1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IRamRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new RamService(mockRepo.Object, new RamValidator());

            // Act
            var result = service.DeleteRam(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteRam_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IRamRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new RamService(mockRepo.Object, new RamValidator());

            // Act
            var result = service.DeleteRam(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: оперативная память участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteRam_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var ram = new Ram { ComponentId = 1, Name = "RAM1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IRamRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new RamService(mockRepo.Object, new RamValidator());

            // Act
            var result = service.DeleteRam(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
