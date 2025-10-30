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
        [TestMethod]
        public void TestGetUserProfile_WithValidEmail_ReturnsUserProfile()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            User mockUser = new User
            {
                Email = email,
                FullName = "Иван Петров",
                Phone = "+79991234567",
                Address = "г. Москва, ул. Арбат, 1"
            };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(mockUser);

            User result = accountService.GetUserProfile(email);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockUser.Email, result.Email);
            Assert.AreEqual(mockUser.FullName, result.FullName);
            Assert.AreEqual(mockUser.Phone, result.Phone);
            Assert.AreEqual(mockUser.Address, result.Address);
            userRepoMock.Verify(repo => repo.FindByEmail(email), Times.Once);
        }

        [TestMethod]
        public void TestUpdateProfile_WithValidData_UpdatesProfileSuccessfully()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            string fullName = "Иван Петрович";
            string phone = "+79990000000";
            string address = "г. Санкт-Петербург, Невский пр., 10";

            User existingUser = new User { Email = email };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(existingUser);
            userRepoMock.Setup(repo => repo.UpdateProfile(It.IsAny<User>())).Returns(true);

            string result = accountService.UpdateProfile(email, fullName, phone, address);

            Assert.AreEqual("Профиль обновлён", result);
            userRepoMock.Verify(repo => repo.UpdateProfile(It.Is<User>(u =>
                u.FullName == fullName && u.Phone == phone && u.Address == address)), Times.Once);
        }

        [TestMethod]
        public void TestUpdateProfile_WithTooLongFullName_ReturnsValidationError()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            string longName = new string('a', 256);

            string result = accountService.UpdateProfile(email, longName, "+79991234567", "г. Москва");

            Assert.AreEqual("Превышена допустимая длина ФИО (≤ 255)", result);
            userRepoMock.Verify(repo => repo.UpdateProfile(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithValidData_ChangesPasswordSuccessfully()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            string currentPassword = "OldPassw0rd!";
            string newPassword = "BetterP4ss!";
            string repeatPassword = "BetterP4ss!";

            User user = new User { Email = email, Password = "hashed_old_password" };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            userRepoMock.Setup(repo => repo.VerifyPassword(user, currentPassword)).Returns(true);
            userRepoMock.Setup(repo => repo.UpdatePasswordHash(user, newPassword)).Returns(true);

            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            Assert.AreEqual("Пароль обновлён", result);
            userRepoMock.Verify(repo => repo.UpdatePasswordHash(user, newPassword), Times.Once);
        }

        [TestMethod]
        public void TestChangePassword_WithWrongCurrentPassword_ReturnsError()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            User user = new User { Email = email };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>())).Returns(false);

            string result = accountService.ChangePassword(email, "WrongPass!", "BetterP4ss!", "BetterP4ss!");

            Assert.AreEqual("Неверный пароль", result);
            userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithWeakNewPassword_ReturnsError()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            User user = new User { Email = email };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);

            string result = accountService.ChangePassword(email, "OldPassw0rd!", "12345", "12345");

            Assert.AreEqual("Пароль недостаточно надёжный (минимум 6 символов, буквы и цифры)", result);
            userRepoMock.Verify(repo => repo.VerifyPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestChangePassword_WithMismatchedPasswords_ReturnsError()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();
            string email = "user1@example.com";
            User user = new User { Email = email };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);

            string result = accountService.ChangePassword(email, "OldPassw0rd!", "BetterP4ss!", "DifferentP4ss!");

            Assert.AreEqual("Пароли не совпадают", result);
            userRepoMock.Verify(repo => repo.VerifyPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }
}