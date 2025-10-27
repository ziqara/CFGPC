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

        }
}

