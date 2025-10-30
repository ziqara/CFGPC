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
    }
}
