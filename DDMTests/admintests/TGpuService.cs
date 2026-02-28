using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TGpuService
    {
        [TestMethod]
        public void GetAllGpus_ReturnsListOfGpus()
        {
            // Arrange
            var expected = new List<Gpu>
            {
                new Gpu { Name = "GPU1", Brand = "Brand1", Model = "Model1" },
                new Gpu { Name = "GPU2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<IGpuRepository>();
            mockRepo.Setup(r => r.ReadAllGpus()).Returns(expected);

            var service = new GpuService(mockRepo.Object, new GpuValidator());

            // Act
            var result = service.GetAllGpus();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateGpu_ValidGpu_ReturnsEmptyString()
        {
            // Arrange
            var gpu = new Gpu { Name = "GPU1", Brand = "Brand1", Model = "Model1", Price = 100, StockQuantity = 10, PcieVersion = "4.0", Tdp = 250, VramGb = 8 };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddGpu(It.IsAny<Gpu>())).Returns(true);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateGpu(gpu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddGpu(It.IsAny<Gpu>()), Times.Once);
        }

        [TestMethod]
        public void CreateGpu_DuplicateGpu_ReturnsErrorMessage()
        {
            // Arrange
            var gpu = new Gpu { Name = "GPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateGpu(gpu);

            // Assert
            Assert.AreEqual("Такая видеокарта уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddGpu(It.IsAny<Gpu>()), Times.Never);
        }

        [TestMethod]
        public void CreateGpu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var gpu = new Gpu { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateGpu(gpu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddGpu(It.IsAny<Gpu>()), Times.Never);
        }

        [TestMethod]
        public void UpdateGpu_ValidGpu_ReturnsEmptyString()
        {
            // Arrange
            var gpu = new Gpu { Name = "GPU1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 100, StockQuantity = 10, PcieVersion = "4.0", Tdp = 250, VramGb = 8 };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateGpu(It.IsAny<Gpu>())).Returns(true);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateGpu(gpu);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateGpu(It.IsAny<Gpu>()), Times.Once);
        }

        [TestMethod]
        public void UpdateGpu_DuplicateGpu_ReturnsErrorMessage()
        {
            // Arrange
            var gpu = new Gpu { Name = "GPU1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateGpu(gpu);

            // Assert
            Assert.AreEqual("Такая видеокарта уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateGpu(It.IsAny<Gpu>()), Times.Never);
        }

        [TestMethod]
        public void UpdateGpu_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var gpu = new Gpu { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IGpuRepository>();
            var mockValidator = new Mock<GpuValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Gpu>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameGpu(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new GpuService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateGpu(gpu);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateGpu(It.IsAny<Gpu>()), Times.Never);
        }

        [TestMethod]
        public void DeleteGpu_Success_ReturnsEmptyString()
        {
            // Arrange
            var gpu = new Gpu { ComponentId = 1, Name = "GPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IGpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new GpuService(mockRepo.Object, new GpuValidator());

            // Act
            var result = service.DeleteGpu(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteGpu_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IGpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new GpuService(mockRepo.Object, new GpuValidator());

            // Act
            var result = service.DeleteGpu(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: видеокарта участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteGpu_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var gpu = new Gpu { ComponentId = 1, Name = "GPU1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IGpuRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new GpuService(mockRepo.Object, new GpuValidator());

            // Act
            var result = service.DeleteGpu(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
