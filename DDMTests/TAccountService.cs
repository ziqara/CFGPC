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
        private Mock<ISessionManager> _sessionManagerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _accountService = new AccountService();
            _sessionManagerMock = new Mock<ISessionManager>();
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
            _sessionManagerMock.Setup(sm => sm.ValidateSession()).Returns(true);
            _sessionManagerMock.Setup(sm => sm.GetUserEmailFromSession()).Returns(email);
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(mockUser);

            var result = _accountService.GetUserProfile(email);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockUser.Email, result.Email);
            Assert.AreEqual(mockUser.FullName, result.FullName);
            Assert.AreEqual(mockUser.Phone, result.Phone);
            Assert.AreEqual(mockUser.Address, result.Address);
            _sessionManagerMock.Verify(sm => sm.ValidateSession(), Times.Once);
            _sessionManagerMock.Verify(sm => sm.GetUserEmailFromSession(), Times.Once);
            _userRepoMock.Verify(repo => repo.FindByEmail(email), Times.Once);
        }

        [TestMethod]
        public void TestGetUserProfile_WithInvalidSession_ReturnsNull()
        {
            var email = "user1@example.com";
            _sessionManagerMock.Setup(sm => sm.ValidateSession()).Returns(false);

            var result = _accountService.GetUserProfile(email);

            Assert.IsNull(result);
            _sessionManagerMock.Verify(sm => sm.ValidateSession(), Times.Once);
            _sessionManagerMock.Verify(sm => sm.GetUserEmailFromSession(), Times.Never);
            _userRepoMock.Verify(repo => repo.FindByEmail(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestUpdateProfile_WithValidData_UpdatesProfileSuccessfully()
        {
            var email = "user1@example.com";
            var fullName = "Иван Петрович";
            var phone = "+79990000000";
            var address = "г. Санкт-Петербург, Невский пр., 10";

            _sessionManagerMock.Setup(sm => sm.IsUserAuthenticated()).Returns(true);
            _sessionManagerMock.Setup(sm => sm.GetUserEmailFromSession()).Returns(email);
            var existingUser = new User { Email = email };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(existingUser);
            _userRepoMock.Setup(repo => repo.UpdateProfile(It.IsAny<User>())).Returns(true);

            var result = _accountService.UpdateProfile(email, fullName, phone, address);

            Assert.AreEqual("Профиль обновлён", result);
            _sessionManagerMock.Verify(sm => sm.IsUserAuthenticated(), Times.Once);
            _sessionManagerMock.Verify(sm => sm.GetUserEmailFromSession(), Times.Once);
            _userRepoMock.Verify(repo => repo.UpdateProfile(It.Is<User>(u =>
                u.FullName == fullName && u.Phone == phone && u.Address == address)), Times.Once);
        }

        [TestMethod]
        public void TestUpdateProfile_WithTooLongFullName_ReturnsValidationError()
        {
            var email = "user1@example.com";
            var longName = new string('a', 256);
            _sessionManagerMock.Setup(sm => sm.IsUserAuthenticated()).Returns(true);
            _sessionManagerMock.Setup(sm => sm.GetUserEmailFromSession()).Returns(email);

            var result = _accountService.UpdateProfile(email, longName, "+79991234567", "г. Москва");

            Assert.AreEqual("Превышена допустимая длина ФИО (≤ 255)", result);
            _userRepoMock.Verify(repo => repo.UpdateProfile(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithValidData_ChangesPasswordSuccessfully()
        {
            var email = "user1@example.com";
            var currentPassword = "OldPassw0rd!";
            var newPassword = "BetterP4ss!";
            var repeatPassword = "BetterP4ss!";

            var user = new User { Email = email, Password = "hashed_old_password" };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            _userRepoMock.Setup(repo => repo.VerifyPassword(user, currentPassword)).Returns(true);
            _userRepoMock.Setup(repo => repo.UpdatePasswordHash(user, newPassword)).Returns(true);

            var result = _accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            Assert.AreEqual("Пароль обновлён", result);
            _userRepoMock.Verify(repo => repo.UpdatePasswordHash(user, newPassword), Times.Once);
        }

        [TestMethod]
        public void TestChangePassword_WithWrongCurrentPassword_ReturnsError()
        {
            var email = "user1@example.com";
            var user = new User { Email = email };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            _userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>())).Returns(false);

            var result = _accountService.ChangePassword(email, "WrongPass!", "BetterP4ss!", "BetterP4ss!");

            Assert.AreEqual("Неверный пароль", result);
            _userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithWeakNewPassword_ReturnsError()
        {
            var email = "user1@example.com";
            var user = new User { Email = email };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            _userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>())).Returns(true);

            var result = _accountService.ChangePassword(email, "OldPassw0rd!", "12345", "12345");

            Assert.AreEqual("Пароль недостаточно надёжный (минимум 6 символов)", result);
            _userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithMismatchedPasswords_ReturnsError()
        {
            var email = "user1@example.com";
            var user = new User { Email = email };
            _userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);

            var result = _accountService.ChangePassword(email, "OldPassw0rd!", "BetterP4ss!", "DifferentP4ss!");

            Assert.AreEqual("Пароли не совпадают", result);
            _userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }
}
