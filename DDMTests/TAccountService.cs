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
        private Mock<IConfigurationRepository> _configRepoMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _accountService = new AccountService();
            _configRepoMock = new Mock<IConfigurationRepository>();
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

        [TestMethod]
        public void TestUpdateProfile_WithTooLongFullName_ReturnsValidationError()
        {
            var email = "user1@example.com";
            var longName = new string('a', 256);

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

        [TestMethod]
        public void TestGetUserConfigurations_WithExistingConfigs_ReturnsUserConfigurations()
        {
            var email = "user1@example.com";
            var mockConfigs = new List<ConfigurationCard>
            {
                new ConfigurationCard { ConfigId = 10, ConfigName = "Gaming #1", Status = "draft", TotalPrice = 125000.00m, UserEmail = email },
                new ConfigurationCard { ConfigId = 11, ConfigName = "Workstation", Status = "validated", TotalPrice = 210000.00m, UserEmail = email }
            };
            _configRepoMock.Setup(repo => repo.GetUserConfigurations(email)).Returns(mockConfigs);

            var result = _accountService.GetUserConfigurations(email);

            Assert.AreEqual(2, result.Count);
            foreach (var config in result)
            {
                Assert.AreEqual(email, config.UserEmail);
            }
            _configRepoMock.Verify(repo => repo.GetUserConfigurations(email), Times.Once);
        }

        [TestMethod]
        public void TestGetUserConfigurations_WithNoConfigs_ReturnsEmptyList()
        {
            var email = "user1@example.com";
            _configRepoMock.Setup(repo => repo.GetUserConfigurations(email)).Returns(new List<ConfigurationCard>());

            var result = _accountService.GetUserConfigurations(email);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestDeleteConfiguration_WithNoRelatedOrders_DeletesSuccessfully()
        {
            var configId = 10;
            var userEmail = "user1@example.com";
            var config = new ConfigurationCard { ConfigId = configId, UserEmail = userEmail };

            _configRepoMock.Setup(repo => repo.GetConfigurationById(configId)).Returns(config);
            _configRepoMock.Setup(repo => repo.HasRelatedOrders(configId)).Returns(false);
            _configRepoMock.Setup(repo => repo.DeleteConfiguration(configId, userEmail)).Returns(true);

            var result = _accountService.DeleteConfiguration(configId, userEmail);

            Assert.AreEqual("Сборка удалена", result);
            _configRepoMock.Verify(repo => repo.DeleteConfiguration(configId, userEmail), Times.Once);
        }

        [TestMethod]
        public void TestDeleteConfiguration_WithRelatedOrders_ReturnsError()
        {
            var configId = 11;
            var userEmail = "user1@example.com";
            var config = new ConfigurationCard { ConfigId = configId, UserEmail = userEmail };

            _configRepoMock.Setup(repo => repo.GetConfigurationById(configId)).Returns(config);
            _configRepoMock.Setup(repo => repo.HasRelatedOrders(configId)).Returns(true);

            var result = _accountService.DeleteConfiguration(configId, userEmail);

            Assert.AreEqual("Вы не можете удалить данную сборку, так как на неё оформлен заказ", result);
            _configRepoMock.Verify(repo => repo.DeleteConfiguration(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }
    }
}
