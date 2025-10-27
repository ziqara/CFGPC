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
        }
}

