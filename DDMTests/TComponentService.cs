using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib.Component;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DDMTests
{
    [TestClass]
    public class TComponentService
    {
        [TestMethod]
        public void TestGetComponentsByCategory_EmptyCategory_ReturnsEmptyList()
        {
            Mock<IComponentRepository> mockRepository_ = new Mock<IComponentRepository>();
            ComponentService service = new ComponentService(mockRepository_.Object);

            mockRepository_.Setup(repo => repo.GetComponentsByCategory("gpu"))
               .Returns(new List<ComponentDto>());

            List<ComponentDto> result = service.GetComponentsByCategory("gpu");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

        }

        [TestMethod]
        public void TestGetComponentsByCategory_ValidCategory_ReturnsCorrectList()
        {
            Mock<IComponentRepository> mockRepository_ = new Mock<IComponentRepository>();
            ComponentService service = new ComponentService(mockRepository_.Object);

            ComponentDto expectedComponent = new ComponentDto
            {
                Component = new Component
                {
                    ComponentId = 205,
                    Name = "Corsair RM850x",
                    Brand = "Corsair",
                    Model = "RM850x",
                    Type = "psu",
                    Price = 8900,
                    StockQuantity = 10,
                    Description = "Полностью модульный блок питания",
                    IsAvailable = true,
                    PhotoUrl = "/img/psu-rm850x.jpg",
                    SupplierId = 1
                },
                Specs = new PsuSpec
                {
                    Wattage = 850,
                    EfficiencyRating = "80+ Gold"
                }
            };

            mockRepository_.Setup(repo => repo.GetComponentsByCategory("psu"))
                           .Returns(new List<ComponentDto> { expectedComponent });

            List<ComponentDto> result = service.GetComponentsByCategory("psu");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Component component = result[0].Component;
            Assert.AreEqual("Corsair RM850x", component.Name);
            Assert.AreEqual(8900, component.Price);
        }
    }
}
