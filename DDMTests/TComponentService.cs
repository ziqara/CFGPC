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
        }
    }
}
