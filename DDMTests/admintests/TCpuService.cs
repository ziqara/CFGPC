using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TCpuService
    {
        [TestMethod]
        public void GetAllCpus_ReturnsListOfCpus()
        {
            // Arrange
            var expected = new List<Cpu>
            {
                new Cpu { Name = "CPU1", Brand = "Brand1", Model = "Model1" },
                new Cpu { Name = "CPU2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<ICpuRepository>();
            mockRepo.Setup(r => r.ReadAllCpus()).Returns(expected);

            var service = new CpuService(mockRepo.Object, new CpuValidator());

            // Act
            var result = service.GetAllCpus();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateCpu_ValidCpu_ReturnsEmptyString()
        {
            // Arrange
            var cpu = new Cpu { Name = "CPU1", Brand = "Brand1", Model = "Model1", Price = 300, StockQuantity = 10, Cores = 8, Tdp = 95 };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddCpu(It.IsAny<Cpu>())).Returns(true);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCpu(cpu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddCpu(It.IsAny<Cpu>()), Times.Once);
        }

        [TestMethod]
        public void CreateCpu_DuplicateCpu_ReturnsErrorMessage()
        {
            // Arrange
            var cpu = new Cpu { Name = "CPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCpu(cpu);

            // Assert
            Assert.AreEqual("Такой процессор уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddCpu(It.IsAny<Cpu>()), Times.Never);
        }

        [TestMethod]
        public void CreateCpu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var cpu = new Cpu { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCpu(cpu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddCpu(It.IsAny<Cpu>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCpu_ValidCpu_ReturnsEmptyString()
        {
            // Arrange
            var cpu = new Cpu { Name = "CPU1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 300, StockQuantity = 10, Cores = 8, Tdp = 95 };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateCpu(It.IsAny<Cpu>())).Returns(true);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCpu(cpu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateCpu(It.IsAny<Cpu>()), Times.Once);
        }

        [TestMethod]
        public void UpdateCpu_DuplicateCpu_ReturnsErrorMessage()
        {
            // Arrange
            var cpu = new Cpu { Name = "CPU1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCpu(cpu);

            // Assert
            Assert.AreEqual("Такой процессор уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateCpu(It.IsAny<Cpu>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCpu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var cpu = new Cpu { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICpuRepository>();
            var mockValidator = new Mock<CpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Cpu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameCpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new CpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCpu(cpu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateCpu(It.IsAny<Cpu>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCpu_Success_ReturnsEmptyString()
        {
            // Arrange
            var cpu = new Cpu { ComponentId = 1, Name = "CPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new CpuService(mockRepo.Object, new CpuValidator());

            // Act
            var result = service.DeleteCpu(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteCpu_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new CpuService(mockRepo.Object, new CpuValidator());

            // Act
            var result = service.DeleteCpu(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: процессор участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCpu_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var cpu = new Cpu { ComponentId = 1, Name = "CPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new CpuService(mockRepo.Object, new CpuValidator());

            // Act
            var result = service.DeleteCpu(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
