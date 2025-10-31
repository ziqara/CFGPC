﻿using System;
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

            var testUsers = new[]
            {
                new User
                {
                    Email = "user1@example.com",
                    FullName = "Иван Петров",
                    Phone = "+79991234567",
                    Address = "г. Москва, ул. Арбат, 1"
                },
                new User
                {
                    Email = "user2@example.com",
                    FullName = "Петр Сидоров",
                    Phone = "+79997654321",
                    Address = "г. Санкт-Петербург, Невский пр., 10"
                },
                new User
                {
                    Email = "user3@example.com",
                    FullName = "Мария Иванова",
                    Phone = "+79995554433",
                    Address = "г. Казань, ул. Баумана, 5"
                }
            };

            foreach (var user in testUsers)
            {
                userRepoMock.Setup(repo => repo.FindByEmail(user.Email)).Returns(user);
            }

            userRepoMock.Setup(repo => repo.FindByEmail(It.Is<string>(e =>
                !testUsers.Any(u => u.Email == e))))
                        .Returns((User)null);

            foreach (var expectedUser in testUsers)
            {
                User result = accountService.GetUserProfile(expectedUser.Email);

                Assert.IsNotNull(result, $"Пользователь {expectedUser.Email} не должен быть null");
                Assert.AreEqual(expectedUser.Email, result.Email, $"Email пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.FullName, result.FullName, $"FullName пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.Phone, result.Phone, $"Phone пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.Address, result.Address, $"Address пользователя {expectedUser.Email} не совпадает");

                userRepoMock.Verify(repo => repo.FindByEmail(expectedUser.Email), Times.Once,
                    $"Метод FindByEmail должен быть вызван один раз для {expectedUser.Email}");
            }

            string nonExistentEmail = "nonexistent@example.com";
            User nonExistentResult = accountService.GetUserProfile(nonExistentEmail);
            Assert.IsNull(nonExistentResult, "Для несуществующего email должен возвращаться null");
            userRepoMock.Verify(repo => repo.FindByEmail(nonExistentEmail), Times.Once);
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

            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(() => user);

            userRepoMock.Setup(repo => repo.VerifyPassword(user, currentPassword)).Returns(true);

            userRepoMock.Setup(repo => repo.UpdatePasswordHash(email, newPassword))
                        .Callback<string, string>((userEmail, password) =>
                        {
                            user.Password = password;
                        })
                        .Returns(true);

            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            Assert.AreEqual("Пароль обновлён", result);

            userRepoMock.Verify(repo => repo.UpdatePasswordHash(email, newPassword), Times.Once);

            User updatedUser = userRepoMock.Object.FindByEmail(email);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(newPassword, updatedUser.Password, "Пароль должен быть обновлен в репозитории");

            Assert.AreEqual("Иван Петров", updatedUser.FullName);
            Assert.AreEqual("+79991234567", updatedUser.Phone);
            Assert.AreEqual(email, updatedUser.Email);
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