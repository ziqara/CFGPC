using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static DDMTests.TSupplierService;

namespace DDMTests
{
        [TestClass]
        public class TSupplierService
        {
            private Mock<ISupplierRepository> _repoMock;
            private SupplierService _service;

            [TestInitialize]
            public void Setup()
            {
                _repoMock = new Mock<ISupplierRepository>(MockBehavior.Strict);
                _service = new SupplierService(_repoMock.Object);
            }

            private static string MakeString(int len) // при тестах, где нужно проверить граничные случаи длины поля name
            {
                return new string('А', len); // кириллица для проверки длины
            }

            // Корректные данные
            [TestMethod]
            public void AddSupplier_MinimalValidData_ShouldPass_And_SaveDtoWithNulls()
            {
                var dto = new SupplierDto
                {
                Name = "ООО Альфа",
                ContactEmail = "alpha.supply@example.com",
                Phone = "",       
                Address = ""      
                };

                _repoMock.Setup(r => r.ExistsByEmail("alpha.supply@example.com")).Returns(false);
                _repoMock.Setup(r => r.ExistsByNameInsensitive("ООО Альфа")).Returns(false);
                _repoMock.Setup(r => r.Save(It.IsAny<Supplier>()))
                                      .Returns<Supplier>(s => s);

                var result = _service.AddSupplier(dto);

                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
                _repoMock.Verify(r => r.ExistsByEmail("alpha.supply@example.com"), Times.Once);
                _repoMock.Verify(r => r.ExistsByNameInsensitive("ООО Альфа"), Times.Once);
                _repoMock.Verify(r => r.Save(It.Is<Supplier>(s =>
                                s.Name == "ООО Альфа" &&
                                s.ContactEmail == "alpha.supply@example.com" &&
                                s.Phone == null &&
                                s.Address == null
                                )), Times.Once);
            }
            // --- Корректные данные (все поля)
            [TestMethod]
            public void AddSupplier_AllFieldsValid_ShouldPass_And_Save()
            {
                var dto = new SupplierDto
                {
                Name = "ИП Васильев",
                ContactEmail = "supply@vasiliev.biz",
                Phone = "+7 (999) 123-45-67",
                Address = "г. Москва, ул. Примерная, д. 1"
                };

                _repoMock.Setup(r => r.ExistsByEmail(dto.ContactEmail)).Returns(false);
                _repoMock.Setup(r => r.ExistsByNameInsensitive(dto.Name)).Returns(false);
                _repoMock.Setup(r => r.Save(It.IsAny<Supplier>()))
                                      .Returns<Supplier>(s => s);

                var result = _service.AddSupplier(dto);

                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
                _repoMock.Verify(r => r.Save(It.Is<Supplier>(s =>
                                      s.Name == dto.Name &&
                                      s.ContactEmail == dto.ContactEmail &&
                                      s.Phone == dto.Phone &&
                                      s.Address == dto.Address
                                      )), Times.Once);
            }

            // Обязательность name — пустое/пробелы
            [TestMethod]
            public void AddSupplier_EmptyOrWhitespaceName_ShouldFail_WithRequiredMessage()
            {
                foreach (var badName in new[] { "", "   " })
                {
                    var dto = new SupplierDto
                    {
                        Name = badName,
                        ContactEmail = "valid@example.com"
                    };

                    var result = _service.AddSupplier(dto);

                    CollectionAssert.Contains(result.Errors, "Название обязательно");
                    Assert.IsFalse(result.IsValid);
                    _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Never);
                }
            }

            // Граница длины name — 191 валидно
            [TestMethod]
            public void AddSupplier_NameLength191_ShouldPass()
            {
                var dto = new SupplierDto
                {
                    Name = MakeString(191),
                    ContactEmail = "valid@example.com"
                };

                _repoMock.Setup(r => r.ExistsByEmail(dto.ContactEmail)).Returns(false);
                _repoMock.Setup(r => r.ExistsByNameInsensitive(dto.Name)).Returns(false);
                _repoMock.Setup(r => r.Save(It.IsAny<Supplier>()))
                                      .Returns<Supplier>(s => s);

                var result = _service.AddSupplier(dto);

                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
                _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Once);
            }

            // Длина name > 191 (невалидно)
            [TestMethod]
            public void AddSupplier_NameLength192_ShouldFail_LengthValidation()
            {
                var dto = new SupplierDto
                {
                Name = MakeString(192),
                ContactEmail = "valid@example.com"
                };

                var result = _service.AddSupplier(dto);

                Assert.IsFalse(result.IsValid);
                Assert.IsTrue(result.Errors.Exists(e => e.Contains("длина") || e.Contains("Превышена")));
                _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Never);
            }

            // Обязательность contact_email — пусто/пробелы
            [TestMethod]
            public void AddSupplier_EmptyOrWhitespaceEmail_ShouldFail_WithRequiredMessage()
            {
                foreach (var badEmail in new[] { "", "   " })
                {
                    var dto = new SupplierDto
                    {
                    Name = "ООО БезПочты",
                    ContactEmail = badEmail
                    };

                    var result = _service.AddSupplier(dto);

                    CollectionAssert.Contains(result.Errors, "Email обязателен");
                    Assert.IsFalse(result.IsValid);
                    _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Never);
                }
            }

            // Некорректный email (формат)
            [TestMethod]
            public void AddSupplier_InvalidEmailFormat_ShouldFail_WithInvalidEmailMessage()
            {
                var dto = new SupplierDto
                {
                Name = "ООО Тест",
                ContactEmail = "ivan@@example"
                };

                var result = _service.AddSupplier(dto);

                CollectionAssert.Contains(result.Errors, "Некорректный email");
                Assert.IsFalse(result.IsValid);
                _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Never);
            }

            //  Валидный вариант email
            [TestMethod]
            public void AddSupplier_ValidEmail_ShouldPass()
            {
                var dto = new SupplierDto
                {
                    Name = "ООО Почта+",
                    ContactEmail = "valid@example.com"
                };

                _repoMock.Setup(r => r.ExistsByEmail(dto.ContactEmail)).Returns(false);
                _repoMock.Setup(r => r.ExistsByNameInsensitive(dto.Name)).Returns(false);
                _repoMock.Setup(r => r.Save(It.IsAny<Supplier>()))
                                      .Returns<Supplier>(s => s);

                var result = _service.AddSupplier(dto);

                Assert.IsTrue(result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
                _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Once);
            }

            // Дубликат email
            [TestMethod]
            public void AddSupplier_DuplicateEmail_ShouldFail_WithDuplicateMessage()
            {
                var dto = new SupplierDto
                {
                Name = "ООО Дубликат",
                ContactEmail = "dup@example.com"
                };

                _repoMock.Setup(r => r.ExistsByEmail(dto.ContactEmail)).Returns(true);

                var result = _service.AddSupplier(dto);

                CollectionAssert.Contains(result.Errors, "Email уже используется");
                Assert.IsFalse(result.IsValid);
                _repoMock.Verify(r => r.Save(It.IsAny<Supplier>()), Times.Never);
            }

        }

}

