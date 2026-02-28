using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TPsuService
    {
        [TestMethod]
        public void GetAllPsus_ReturnsListOfPsus()
        {
            // Arrange
            var expected = new List<Psu>
            {
                new Psu { Name = "PSU1", Brand = "Brand1", Model = "Model1" },
                new Psu { Name = "PSU2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<IPsuRepository>();
            mockRepo.Setup(r => r.ReadAllPsus()).Returns(expected);

            var service = new PsuService(mockRepo.Object, new PsuValidator());

            // Act
            var result = service.GetAllPsus();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreatePsu_ValidPsu_ReturnsEmptyString()
        {
            // Arrange
            var psu = new Psu { Name = "PSU1", Brand = "Brand1", Model = "Model1", Price = 100, StockQuantity = 10, Wattage = 650, Efficiency = "80+ Gold" };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddPsu(It.IsAny<Psu>())).Returns(true);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreatePsu(psu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddPsu(It.IsAny<Psu>()), Times.Once);
        }

        [TestMethod]
        public void CreatePsu_DuplicatePsu_ReturnsErrorMessage()
        {
            // Arrange
            var psu = new Psu { Name = "PSU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreatePsu(psu);

            // Assert
            Assert.AreEqual("Такой блок питания уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddPsu(It.IsAny<Psu>()), Times.Never);
        }

        [TestMethod]
        public void CreatePsu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var psu = new Psu { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreatePsu(psu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddPsu(It.IsAny<Psu>()), Times.Never);
        }

        [TestMethod]
        public void UpdatePsu_ValidPsu_ReturnsEmptyString()
        {
            // Arrange
            var psu = new Psu { Name = "PSU1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 100, StockQuantity = 10, Wattage = 650, Efficiency = "80+ Gold" };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdatePsu(It.IsAny<Psu>())).Returns(true);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdatePsu(psu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdatePsu(It.IsAny<Psu>()), Times.Once);
        }

        [TestMethod]
        public void UpdatePsu_DuplicatePsu_ReturnsErrorMessage()
        {
            // Arrange
            var psu = new Psu { Name = "PSU1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdatePsu(psu);

            // Assert
            Assert.AreEqual("Такой блок питания уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdatePsu(It.IsAny<Psu>()), Times.Never);
        }

        [TestMethod]
        public void UpdatePsu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var psu = new Psu { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IPsuRepository>();
            var mockValidator = new Mock<PsuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Psu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSamePsu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new PsuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdatePsu(psu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdatePsu(It.IsAny<Psu>()), Times.Never);
        }

        [TestMethod]
        public void DeletePsu_Success_ReturnsEmptyString()
        {
            // Arrange
            var psu = new Psu { ComponentId = 1, Name = "PSU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IPsuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new PsuService(mockRepo.Object, new PsuValidator());

            // Act
            var result = service.DeletePsu(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeletePsu_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IPsuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new PsuService(mockRepo.Object, new PsuValidator());

            // Act
            var result = service.DeletePsu(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: блок питания участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeletePsu_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var psu = new Psu { ComponentId = 1, Name = "PSU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IPsuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new PsuService(mockRepo.Object, new PsuValidator());

            // Act
            var result = service.DeletePsu(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
