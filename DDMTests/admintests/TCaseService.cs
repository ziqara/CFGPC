using System;
using DDMLib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySql.Data.MySqlClient;

namespace DDMTests.admintests
{
    [TestClass]
    public class TCaseService
    {
        [TestMethod]
        public void GetAllCases_ReturnsListOfCases()
        {
            // Arrange
            var expected = new List<Case>
            {
                new Case { Name = "Case1", Brand = "Brand1", Model = "Model1" },
                new Case { Name = "Case2", Brand = "Brand2", Model = "Model2" }
            };

            var mockRepo = new Mock<ICaseRepository>();
            mockRepo.Setup(r => r.ReadAllCases()).Returns(expected);

            var service = new CaseService(mockRepo.Object, new CaseValidator());

            // Act
            var result = service.GetAllCases();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count, result.Count);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CreateCase_ValidCase_ReturnsEmptyString()
        {
            // Arrange
            var caseItem = new Case { Name = "Case1", Brand = "Brand1", Model = "Model1", Price = 100, StockQuantity = 10, Size = "mid_tower", FormFactor = "ATX" };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockRepo.Setup(r => r.AddCase(It.IsAny<Case>())).Returns(true);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCase(caseItem);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.AddCase(It.IsAny<Case>()), Times.Once);
        }

        [TestMethod]
        public void CreateCase_DuplicateCase_ReturnsErrorMessage()
        {
            // Arrange
            var caseItem = new Case { Name = "Case1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCase(caseItem);

            // Assert
            Assert.AreEqual("Такой корпус уже существует (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.AddCase(It.IsAny<Case>()), Times.Never);
        }

        [TestMethod]
        public void CreateCase_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var caseItem = new Case { Name = "", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.CreateCase(caseItem);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.AddCase(It.IsAny<Case>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCase_ValidCase_ReturnsEmptyString()
        {
            // Arrange
            var caseItem = new Case { Name = "Case1", Brand = "Brand1", Model = "Model1", ComponentId = 1, Price = 100, StockQuantity = 10, Size = "mid_tower", FormFactor = "ATX" };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.UpdateCase(It.IsAny<Case>())).Returns(true);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCase(caseItem);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.UpdateCase(It.IsAny<Case>()), Times.Once);
        }

        [TestMethod]
        public void UpdateCase_DuplicateCase_ReturnsErrorMessage()
        {
            // Arrange
            var caseItem = new Case { Name = "Case1", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string>());
            mockRepo.Setup(r => r.ExistsOtherSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCase(caseItem);

            // Assert
            Assert.AreEqual("Такой корпус уже есть (совпали Название/Бренд/Модель)", result);
            mockRepo.Verify(r => r.UpdateCase(It.IsAny<Case>()), Times.Never);
        }

        [TestMethod]
        public void UpdateCase_ValidationErrors_ReturnsAggregatedErrors()
        {
            // Arrange
            var caseItem = new Case { Name = "", Brand = "Brand1", Model = "Model1", ComponentId = 1 };
            var mockRepo = new Mock<ICaseRepository>();
            var mockValidator = new Mock<CaseValidator>();

            mockValidator.Setup(v => v.Validate(It.IsAny<Case>())).Returns(new List<string> { "Название обязательно" });
            mockRepo.Setup(r => r.ExistsOtherSameCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            var service = new CaseService(mockRepo.Object, mockValidator.Object);

            // Act
            var result = service.UpdateCase(caseItem);

            // Assert
            Assert.AreEqual("Название обязательно", result);
            mockRepo.Verify(r => r.UpdateCase(It.IsAny<Case>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCase_Success_ReturnsEmptyString()
        {
            // Arrange
            var caseItem = new Case { ComponentId = 1, Name = "Case1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICaseRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(true);

            var service = new CaseService(mockRepo.Object, new CaseValidator());

            // Act
            var result = service.DeleteCase(1);

            // Assert
            Assert.AreEqual(string.Empty, result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void DeleteCase_HasActiveOrders_ReturnsErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICaseRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(true);

            var service = new CaseService(mockRepo.Object, new CaseValidator());

            // Act
            var result = service.DeleteCase(1);

            // Assert
            Assert.AreEqual("Невозможно удалить: корпус участвует в незавершённых заказах", result);
            mockRepo.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void DeleteCase_DatabaseError_ReturnsConnectionMessage()
        {
            // Arrange
            var caseItem = new Case { ComponentId = 1, Name = "Case1", Brand = "Brand1", Model = "Model1" };
            var mockRepo = new Mock<ICaseRepository>();

            mockRepo.Setup(r => r.HasActiveOrders(It.IsAny<int>())).Returns(false);
            mockRepo.Setup(r => r.DeleteById(It.IsAny<int>())).Throws(new Exception("Timeout expired"));

            var service = new CaseService(mockRepo.Object, new CaseValidator());

            // Act
            var result = service.DeleteCase(1);

            // Assert
            Assert.AreEqual("Вероятно, проблемы в соединении с БД: Timeout expired", result);
        }
    }
}
