using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests
{
    [TestClass]
    public class TStorageService
    {
        [TestMethod]
        public void GetAllStorages_ReturnsListOfStorages()
        {
            // Arrange
            var expected = new List<Storage>
            {
                new Storage { Name = "Storage1", Brand = "Brand1", Model = "Model1" },
                new Storage { Name = "Storage2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<IStorageRepository>();
            mockRepo.Setup(r => r.ReadAllStorages()).Returns(expected);

            var service = new StorageService(mockRepo.Object, new StorageValidator());

            // Act
            var result = service.GetAllStorages();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateStorage_ValidStorage_ReturnsEmptyString()
        {
            // Arrange
            var storage = new Storage { Name = "Storage1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IStorageRepository>();
            var mockValidator = new Mock<StorageValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Storage>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddStorage(It.IsAny<Storage>())).Returns(true);

            var service = new StorageService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateStorage(storage);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddStorage(It.IsAny<Storage>()), Times.Once);
        }

        [TestMethod]
        public void CreateStorage_DuplicateStorage_ReturnsErrorMessage()
        {
            // Arrange
            var storage = new Storage { Name = "Storage1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IStorageRepository>();
            var mockValidator = new Mock<StorageValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Storage>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new StorageService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateStorage(storage);

            // Assert
            Assert.AreEqual("Такой накопитель уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddStorage(It.IsAny<Storage>()), Times.Never);
        }

        [TestMethod]
        public void CreateStorage_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var storage = new Storage { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IStorageRepository>();
            var mockValidator = new Mock<StorageValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Storage>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new StorageService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateStorage(storage);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddStorage(It.IsAny<Storage>()), Times.Never);
        }

        [TestMethod]
        public void UpdateStorage_ValidStorage_ReturnsEmptyString()
        {
            // Arrange
            var storage = new Storage { Name = "Storage1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IStorageRepository>();
            var mockValidator = new Mock<StorageValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Storage>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateStorage(It.IsAny<Storage>())).Returns(true);

            var service = new StorageService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateStorage(storage);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateStorage(It.IsAny<Storage>()), Times.Once);
        }

        [TestMethod]
        public void UpdateStorage_DuplicateStorage_ReturnsErrorMessage()
        {
            // Arrange
            var storage = new Storage { Name = "Storage1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<IStorageRepository>();
            var mockValidator = new Mock<StorageValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Storage>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new StorageService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateStorage(storage);

            // Assert
            Assert.AreEqual("Такой накопитель уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateStorage(It.IsAny<Storage>()), Times.Never);
        }

        [TestMethod]
        public void DeleteStorage_Success_ReturnsEmptyString()
        {
            // Arrange
            var storage = new Storage { ComponentId = 1, Name = "Storage1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IStorageRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new StorageService(mockRepo.Object, new StorageValidator());

            // Act
            var result = service.DeleteStorage(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteStorage_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IStorageRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new StorageService(mockRepo.Object, new StorageValidator());

            // Act
            var result = service.DeleteStorage(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: накопитель участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteStorage_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var storage = new Storage { ComponentId = 1, Name = "Storage1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<IStorageRepository>();

            // Замокируем метод, чтобы он выбрасывал исключение типа MySqlException
            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new StorageService(mockRepo.Object, new StorageValidator());

            // Act
            var result = service.DeleteStorage(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
