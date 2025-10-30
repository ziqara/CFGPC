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

            userRepoMock.Setup(repo => repo.FindByEmail("user1@example.com"))
                        .Returns(new User
                        {
                            Email = "user1@example.com",
                            FullName = "Иван Петров",
                            Phone = "+79991234567",
                            Address = "г. Москва, ул. Арбат, 1"
                        });

            userRepoMock.Setup(repo => repo.FindByEmail("user2@example.com"))
                        .Returns(new User
                        {
                            Email = "user2@example.com",
                            FullName = "Петр Сидоров",
                            Phone = "+79997654321",
                            Address = "г. Санкт-Петербург, Невский пр., 10"
                        });

            userRepoMock.Setup(repo => repo.FindByEmail("user3@example.com"))
                        .Returns(new User
                        {
                            Email = "user3@example.com",
                            FullName = "Мария Иванова",
                            Phone = "+79995554433",
                            Address = "г. Казань, ул. Баумана, 5"
                        });

            userRepoMock.Setup(repo => repo.FindByEmail(It.Is<string>(e =>
                e != "user1@example.com" && e != "user2@example.com" && e != "user3@example.com")))
                        .Returns((User)null);

            User result = accountService.GetUserProfile(email);

            Assert.IsNotNull(result);
            Assert.AreEqual("user1@example.com", result.Email);
            Assert.AreEqual("Иван Петров", result.FullName);
            Assert.AreEqual("+79991234567", result.Phone);
            Assert.AreEqual("г. Москва, ул. Арбат, 1", result.Address);
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

            User existingUser = new User
            {
                Email = email,
                FullName = "Иван Петров", 
                Phone = "+79991234567",   
                Address = "г. Москва, ул. Арбат, 1" 
            };

            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(existingUser);

            User updatedUser = null;
            userRepoMock.Setup(repo => repo.UpdateProfile(It.IsAny<User>()))
                        .Callback<User>(user => updatedUser = user);

            string result = accountService.UpdateProfile(email, fullName, phone, address);

            Assert.AreEqual("Профиль обновлён", result);

            userRepoMock.Verify(repo => repo.UpdateProfile(It.IsAny<User>()), Times.Once);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(email, updatedUser.Email);
            Assert.AreEqual(fullName, updatedUser.FullName);
            Assert.AreEqual(phone, updatedUser.Phone);
            Assert.AreEqual(address, updatedUser.Address);
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

            User user = new User
            {
                Email = email,
                Password = "hashed_old_password",
                FullName = "Иван Петров",
                Phone = "+79991234567"
            };

            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);
            userRepoMock.Setup(repo => repo.VerifyPassword(user, currentPassword)).Returns(true);

            string newPasswordHash = null;
            userRepoMock.Setup(repo => repo.UpdatePasswordHash(email, newPassword))
                        .Callback<string, string>((userEmail, password) =>
                        {
                            newPasswordHash = password;
                            user.Password = password;
                        })
                        .Returns(true);

            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            Assert.AreEqual("Пароль обновлён", result);

            userRepoMock.Verify(repo => repo.UpdatePasswordHash(email, newPassword), Times.Once);

            Assert.IsNotNull(newPasswordHash);
            Assert.AreEqual(newPassword, newPasswordHash);

            Assert.AreEqual(newPassword, user.Password);
        }

        [TestMethod]
        [DataRow(null, "+79991234567", "г. Москва", "ФИО не может быть пустым")]
        [DataRow("", "+79991234567", "г. Москва", "ФИО не может быть пустым")]
        [DataRow("   ", "+79991234567", "г. Москва", "ФИО не может быть пустым")]
        [DataRow("Очень длинное имя которое превышает допустимую длину в двести пятьдесят пять символов и должно вызвать ошибку валидации потому что " +
            "это слишком длинная строка для поля ФИО в нашей системе и мы должны ограничивать его размер для корректной работы базы данных и " +
            "пользовательского интерфейса", "+79991234567", "г. Москва", "Превышена допустимая длина ФИО (≤ 255)")]
        [DataRow("Иван Петров", "invalid_phone", "г. Москва", "Неверный формат телефона. Пример: +7 (999) 123-45-67")]
        [DataRow("Иван Петров", "123", "г. Москва", "Неверный формат телефона. Пример: +7 (999) 123-45-67")]
        public void TestUpdateProfile_WithInvalidData_ReturnsValidationErrors(string fullName, string phone, string address, string expectedError)
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();

            string email = "user1@example.com";

            User existingUser = new User
            {
                Email = email,
                FullName = "Иван Петров",
                Phone = "+79991234567",
                Address = "г. Москва, ул. Арбат, 1"
            };

            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(existingUser);

            string result = accountService.UpdateProfile(email, fullName, phone, address);

            Assert.AreEqual(expectedError, result);
            userRepoMock.Verify(repo => repo.UpdateProfile(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        [DataRow("OldPassw0rd!", "NewPass123", "DifferentPass123", "Пароли не совпадают")]
        [DataRow("OldPassw0rd!", "12345", "12345", "Пароль недостаточно надёжный (минимум 6 символов)")]
        [DataRow("OldPassw0rd!", "short", "short", "Пароль недостаточно надёжный (минимум 6 символов)")]
        [DataRow("WrongPassword!", "NewPassw0rd!", "NewPassw0rd!", "Неверный пароль")]
        public void TestChangePassword_WithInvalidData_ReturnsValidationErrors(string currentPassword, string newPassword, string repeatPassword, string expectedError)
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();
            AccountService accountService = new AccountService();

            string email = "user1@example.com";

            User user = new User { Email = email, Password = "hashed_old_password" };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);

            userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>()))
                        .Returns<string, string>((u, pass) => pass == "OldPassw0rd!");

            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            Assert.AreEqual(expectedError, result);

            userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}