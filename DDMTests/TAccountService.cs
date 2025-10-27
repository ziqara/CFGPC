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
    public class TAccountService
    {
        private Mock<IUserRepository> _userRepoMock;
        private AccountService _accountService;

        [TestInitialize]
        public void TestInitialize()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _accountService = new AccountService();
        }

        [TestMethod]
        public void TestGetUserProfile_WithValidEmail_ReturnsUserProfile()
        {
            var email = "user1@example.com";
            var mockUser = new User
            {
                Email = email,
                FullName = "Иван Петров",
                Phone = "+79991234567",
                Address = "г. Москва, ул. Арбат, 1"
            };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(mockUser);

            var result = _accountService.GetUserProfile(email);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockUser.Email, result.Email);
            Assert.AreEqual(mockUser.FullName, result.FullName);
            Assert.AreEqual(mockUser.Phone, result.Phone);
            Assert.AreEqual(mockUser.Address, result.Address);
            _userRepoMock.Verify(repo => repo.FindByEmail(email), Times.Once);
        }

        [TestMethod]
        public void TestUpdateProfile_WithValidData_UpdatesProfileSuccessfully()
        {
            var email = "user1@example.com";
            var fullName = "Иван Петрович";
            var phone = "+79990000000";
            var address = "г. Санкт-Петербург, Невский пр., 10";

            var existingUser = new User { Email = email };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(existingUser);
            _userRepoMock.Setup(repo => repo.UpdateProfile(It.IsAny<User>())).Returns(true);

            var result = _accountService.UpdateProfile(email, fullName, phone, address);

            Assert.AreEqual("Профиль обновлён", result);
            _userRepoMock.Verify(repo => repo.UpdateProfile(It.Is<User>(u =>
                u.FullName == fullName && u.Phone == phone && u.Address == address)), Times.Once);
        }

    }
}
