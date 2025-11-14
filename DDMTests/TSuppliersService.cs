using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DDMTests
{
    [TestClass]
    public class TSuppliersService
    {
        [TestMethod]
        [DataRow(new int[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { })] // Empty list
        [DataRow(new int[] { 300000001, 300000002 }, new string[] { "ООО ПустоNull", "ООО ПустоStr" }, new string[] { "null@example.com", "empty@example.com" }, new string[] { null, "" }, new string[] { null, "" })] // Optional fields null or empty
        [DataRow(new int[] { 123456789 }, new string[] { "ООО Альфа" }, new string[] { "alpha.supply@example.com" }, new string[] { "+7 (999) 123-45-67" }, new string[] { "г. Москва, ул. Примерная, д. 1" })] // Single record
        [DataRow(new int[] { 111111111, 222222222, 333333333 }, new string[] { "ООО Бета", "ИП Васильев", "ООО Гамма" }, new string[] { "beta@example.com", "supply@vasiliev.biz", "gamma@example.com" }, new string[] { "+7 (900) 000-00-01", null, "+7 (495) 111-22-33" }, new string[] { "г. Казань, ул. Примерная, д. 5", "", "г. Москва, пр. Тестовый, д. 2" })] // Multiple records
        public void GetAllSuppliers_Test(int[] inns, string[] names, string[] emails, string[] phones, string[] addresses)
        {
            List<Supplier> data = new List<Supplier>();
            for (int i = 0; i < inns.Length; i++)
            {
                data.Add(new Supplier(inns[i])
                {
                    Name = names[i],
                    ContactEmail = emails[i],
                    Phone = phones[i],
                    Address = addresses[i]
                });
            }
            List<Supplier> expected = data;

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);
            repo.Setup(r => r.ReadAllSuppliers()).Returns(data);

            List<Supplier> result = service.GetAllSuppliers();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expected, result);
            repo.Verify(r => r.ReadAllSuppliers(), Times.Once);
        }

        [TestMethod]
        [DataRow(123456789, "ООО Альфа", "alpha@example.com", null, null)]
        [DataRow(223344556, "ООО Бета", "beta@example.com", "", "")]
        [DataRow(998877665, "ООО Гамма", "gamma@example.com", "+7 (999) 123-45-67", "г. Москва, ул. Ленина, д. 5")]
        public void CreateSupplier_ValidData(int[] inns, string[] names, string[] emails, string[] phones, string[] addresses)
        {
            for (int i = 0; i < inns.Length; i++)
            {
                // Arrange
                Supplier supplier = new Supplier(inns[i])
                {
                    Name = names[i],
                    ContactEmail = emails[i],
                    Phone = phones[i],
                    Address = addresses[i]
                };

                Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();

                repo.Setup(r => r.existsByNameInsensitive(names[i])).Returns(false);
                repo.Setup(r => r.existsByEmail(emails[i])).Returns(false);
                repo.Setup(r => r.existsByInn(inns[i])).Returns(false);
                repo.Setup(r => r.AddSupplier(supplier)).Returns(true);

                SupplierService service = new SupplierService(repo.Object);

                // Act
                string result = service.CreateSupplier(supplier);

                // Assert
                Assert.AreEqual("", result, "Valid supplier should produce no errors");
                repo.Verify(r => r.AddSupplier(supplier), Times.Once);
            }
        }

        [TestMethod]
        public void CreateSupplier_DuplicateEmail_ReturnsErrorMessage()
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = "ООО Альфа",
                ContactEmail = "dup@example.com",
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            repo.Setup(r => r.existsByEmail("dup@example.com")).Returns(true);

            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual("Email уже используется", result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        public void CreateSupplier_DuplicateName_ReturnsErrorMessage()
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = "ооо ромашка",
                ContactEmail = "new@example.com",
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            repo.Setup(r => r.existsByNameInsensitive("ооо ромашка")).Returns(true);

            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual("Поставщик с таким названием уже есть", result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        [DataRow("123")]                
        [DataRow("123456789123")]       
        [DataRow("88ABC777")]           
        public void CreateSupplier_InvalidPhone_ReturnsErrorMessage(string phone)
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = "ООО Тест",
                ContactEmail = "valid@example.com",
                Phone = phone,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual("Некорректный номер телефона", result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        [DataRow("", "Название обязательно")]
        [DataRow("   ", "Название обязательно")]
        public void CreateSupplier_InvalidName_EmptyOrSpaces_ReturnsError(string badName, string expectedError)
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = badName,
                ContactEmail = "valid@example.com",
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual(expectedError, result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        public void CreateSupplier_NameTooLong_ReturnsErrorMessage()
        {
            // Arrange
            string longName = new string('A', 51); // 51 символ
            Supplier supplier = new Supplier(123456789)
            {
                Name = longName,
                ContactEmail = "valid@example.com",
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual("Название не должно превышать 50 символов", result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        [DataRow("", "Email обязателен")]
        [DataRow("   ", "Email обязателен")]
        public void CreateSupplier_EmptyEmail_ReturnsErrorMessage(string email, string expectedError)
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = "ООО БезПочты",
                ContactEmail = email,
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual(expectedError, result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }

        [TestMethod]
        [DataRow("ivan@@example.com")]
        [DataRow("bad@@example")]
        [DataRow("no-at-symbol.com")]
        public void CreateSupplier_InvalidEmailFormat_ReturnsErrorMessage(string badEmail)
        {
            // Arrange
            Supplier supplier = new Supplier(123456789)
            {
                Name = "ООО Тест",
                ContactEmail = badEmail,
                Phone = null,
                Address = null
            };

            Mock<ISupplierRepository> repo = new Mock<ISupplierRepository>();
            SupplierService service = new SupplierService(repo.Object);

            // Act
            string result = service.CreateSupplier(supplier);

            // Assert
            Assert.AreEqual("Некорректный email", result);
            repo.Verify(r => r.AddSupplier(It.IsAny<Supplier>()), Times.Never);
        }
    }
}
