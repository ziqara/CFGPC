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

        [TestMethod]
        public void TestGetComponentsByCategory_SwitchingCategories_ReturnsCorrectData()
        {
            Mock<IComponentRepository> mockRepository_ = new Mock<IComponentRepository>();
            ComponentService service = new ComponentService(mockRepository_.Object);

            ComponentDto cpuComponent = new ComponentDto
            {
                Component = new Component
                {
                    Name = "Intel Core i5-13600K",
                    Brand = "Intel",
                    Model = "i5-13600K",
                    Type = "cpu",
                    Price = 25000,
                    StockQuantity = 5,
                    Description = "Процессор Intel",
                    IsAvailable = true,
                    PhotoUrl = "/img/cpu-i5.jpg",
                    SupplierId = 1
                },
                Specs = new { Cores = 14, BaseClock = "3.5 GHz" }
            };

            ComponentDto caseComponent = new ComponentDto
            {
                Component = new Component
                {
                    Name = "Fractal Design Meshify C",
                    Brand = "Fractal Design",
                    Model = "Meshify C",
                    Type = "case",
                    Price = 7500,
                    StockQuantity = 3,
                    Description = "Корпус",
                    IsAvailable = true,
                    PhotoUrl = "/img/case-meshify.jpg",
                    SupplierId = 1
                },
                Specs = new CaseSpec
                {
                    FormFactor = "ATX Mid Tower",
                    Size = "450×210×470 мм"
                }
            };

            mockRepository_.Setup(repo => repo.GetComponentsByCategory("cpu"))
                           .Returns(new List<ComponentDto> { cpuComponent });

            mockRepository_.Setup(repo => repo.GetComponentsByCategory("case"))
                           .Returns(new List<ComponentDto> { caseComponent });

            mockRepository_.Setup(repo => repo.GetComponentsByCategory("cooling"))
                           .Returns(new List<ComponentDto>());

            List<ComponentDto> cpuResult = service.GetComponentsByCategory("cpu");
            List<ComponentDto> caseResult = service.GetComponentsByCategory("case");
            List<ComponentDto> coolingResult = service.GetComponentsByCategory("cooling");

            Assert.AreEqual(1, cpuResult.Count);
            Assert.AreEqual(1, caseResult.Count);
            Assert.AreEqual(0, coolingResult.Count);
        }
    }
}
