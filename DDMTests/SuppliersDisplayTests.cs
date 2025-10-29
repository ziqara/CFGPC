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
}
