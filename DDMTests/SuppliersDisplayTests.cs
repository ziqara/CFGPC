// SuppliersDisplayTests.cs
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class SuppliersDisplayTests
{
    private Mock<ISupplierRepository> _repo;
    private SupplierService _service;
    private Mock<ISuppliersView> _view;
    private SuppliersPresenter _presenter;

    [TestInitialize]
    public void Setup()
    {
        _repo = new Mock<ISupplierRepository>();
        _service = new SupplierService(_repo.Object);
        _view = new Mock<ISuppliersView>();
        _presenter = new SuppliersPresenter(_service, _view.Object);
    }

    [TestMethod]
    public void Load_WithData_ShouldPassDataToView()
    {
        var data = new List<Supplier>
        {
            new Supplier
            {
                SupplierId = 1,
                Name = "ООО Альфа",
                ContactEmail = "alpha.supply@example.com",
                Phone = "+7 (999) 123-45-67",
                Address = "г. Москва, ул. Примерная, д. 1"
            },
            new Supplier
            {
                SupplierId = 2,
                Name = "ИП Васильев",
                ContactEmail = "supply@vasiliev.biz",
                Phone = null,
                Address = null
            }
        };

        _repo.Setup(r => r.ReadAllSuppliers()).Returns(data);

        // Ловим фактически переданные в UI данные
        List<Supplier> shown = null;
        _view.Setup(v => v.ShowTable(It.IsAny<List<Supplier>>()))
             .Callback<List<Supplier>>(l => shown = l);

        _presenter.Load();

        // Проверяем, что выданы именно те данные
        Assert.IsNotNull(shown, "Список, переданный во View, не был получен");
        Assert.AreEqual(2, shown.Count);

        Assert.AreEqual(1, shown[0].SupplierId);
        Assert.AreEqual("ООО Альфа", shown[0].Name);
        Assert.AreEqual("alpha.supply@example.com", shown[0].ContactEmail);
        Assert.AreEqual("+7 (999) 123-45-67", shown[0].Phone);
        Assert.AreEqual("г. Москва, ул. Примерная, д. 1", shown[0].Address);

        Assert.AreEqual(2, shown[1].SupplierId);
        Assert.AreEqual("ИП Васильев", shown[1].Name);
        Assert.AreEqual("supply@vasiliev.biz", shown[1].ContactEmail);
        Assert.IsNull(shown[1].Phone);
        Assert.IsNull(shown[1].Address);
    }

    [TestMethod]
    public void Load_EmptyList_ShouldShowEmptyState()
    {
        _repo.Setup(r => r.ReadAllSuppliers()).Returns(new List<Supplier>());

        _presenter.Load();

        _view.Verify(v => v.ShowEmpty(), Times.Once);
        _view.Verify(v => v.ShowError(It.IsAny<string>()), Times.Never);
    }
}
