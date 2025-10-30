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
    public class SuppliersServiceOnlyTests
    {
        [TestMethod]
        public void GetAllSuppliers_EmptyList_ReturnsEmptyModel()
        {
            // Arrange
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(r => r.ReadAllSuppliers()).Returns(new List<Supplier>());

            // Act
            var result = service.GetAllSuppliers();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            repo.Verify(r => r.ReadAllSuppliers(), Times.Once);
        }

        [TestMethod]
        public void GetAllSuppliers_OptionalFieldsNullOrEmpty_PassedAsIs()
        {
            // Arrange
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);

            List<Supplier> data = new List<Supplier>
            {
                new Supplier { Inn=300000001, Name="ООО ПустоNull", ContactEmail="null@example.com",
                            Phone=null, Address=null },
                new Supplier { Inn=300000002, Name="ООО ПустоStr",  ContactEmail="empty@example.com",
                           Phone="", Address="" }
            };

            repo.Setup(r => r.ReadAllSuppliers()).Returns(data);

            // Act
            var result = service.GetAllSuppliers();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            // Запись №1
            Assert.AreEqual(300000001, result[0].Inn);
            Assert.AreEqual("ООО ПустоNull", result[0].Name);
            Assert.AreEqual("null@example.com", result[0].ContactEmail);
            Assert.IsNull(result[0].Phone);
            Assert.IsNull(result[0].Address);

            // Запись №2
            Assert.AreEqual(300000002, result[1].Inn);
            Assert.AreEqual("ООО ПустоStr", result[1].Name);
            Assert.AreEqual("empty@example.com", result[1].ContactEmail);
            Assert.AreEqual("", result[1].Phone);
            Assert.AreEqual("", result[1].Address);

            repo.Verify(r => r.ReadAllSuppliers(), Times.Once);
        }
    }
}
