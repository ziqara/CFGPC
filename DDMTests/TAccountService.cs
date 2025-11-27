using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDMLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

            // Создаем SessionManager с настроенной авторизацией
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            // Настраиваем куки для сессии
            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
    {
        { "SessionId", sessionId }
    });

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var sessionManager = new SessionManager(httpContextAccessorMock.Object);

            // Добавляем тестовую сессию через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

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

            // Настраиваем репозиторий
            foreach (var user in testUsers)
            {
                userRepoMock.Setup(repo => repo.FindByEmail(user.Email)).Returns(user);
            }

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

            foreach (var expectedUser in testUsers)
            {
                // Устанавливаем правильную сессию для текущего пользователя
                sessions[sessionId] = new SessionData
                {
                    SessionId = sessionId,
                    Email = expectedUser.Email,
                    UserName = expectedUser.FullName,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddHours(12)
                };

                User result = accountService.GetUserProfile(expectedUser.Email);

                Assert.IsNotNull(result, $"Пользователь {expectedUser.Email} не должен быть null");
                Assert.AreEqual(expectedUser.Email, result.Email, $"Email пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.FullName, result.FullName, $"FullName пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.Phone, result.Phone, $"Phone пользователя {expectedUser.Email} не совпадает");
                Assert.AreEqual(expectedUser.Address, result.Address, $"Address пользователя {expectedUser.Email} не совпадает");

                userRepoMock.Verify(repo => repo.FindByEmail(expectedUser.Email), Times.Once,
                    $"Метод FindByEmail должен быть вызван один раз для {expectedUser.Email}");
            }
        }

        [TestMethod]
        public void TestGetUserProfile_WithNonExistentEmail_ReturnsNull()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();

            // Создаем SessionManager с настроенной авторизацией
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
    {
        { "SessionId", sessionId }
    });

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var sessionManager = new SessionManager(httpContextAccessorMock.Object);

            // Добавляем тестовую сессию через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

            string nonExistentEmail = "nonexistent@example.com";

            // Настраиваем сессию для несуществующего пользователя
            sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Email = nonExistentEmail, // email несуществующего пользователя
                UserName = "Test User",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(12)
            };

            // Настраиваем репозиторий возвращать null для несуществующего email
            userRepoMock.Setup(repo => repo.FindByEmail(nonExistentEmail)).Returns((User)null);

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

            User result = accountService.GetUserProfile(nonExistentEmail);

            Assert.IsNull(result, "Для несуществующего email должен возвращаться null");
            userRepoMock.Verify(repo => repo.FindByEmail(nonExistentEmail), Times.Once);
        }

        [TestMethod]
        public void TestUpdateProfile_WithValidData_UpdatesProfileSuccessfully()
        {
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();

            // Создаем реальные экземпляры SessionManager и UserService
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            // Настраиваем сессию для авторизации
            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
    {
        { "SessionId", sessionId }
    });

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var sessionManager = new SessionManager(httpContextAccessorMock.Object);

            // Добавляем тестовую сессию через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

            string email = "user1@example.com";

            // Создаем сессию для тестового пользователя
            sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Email = email,
                UserName = "Иван Петров",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(12)
            };

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

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

            // Изменяем возвращаемое значение на пустую строку или "Профиль обновлён"
            userRepoMock.Setup(repo => repo.UpdateProfile(It.IsAny<User>()))
                        .Callback<User>(user => updatedUser = user)
                        .Returns(string.Empty); // Пустая строка означает успех

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
            // Arrange
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();

            // Создаем реальный SessionManager
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            // Настраиваем сессию для авторизации
            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
    {
        { "SessionId", sessionId }
    });

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            var sessionManager = new SessionManager(httpContextAccessorMock.Object);

            // Добавляем тестовую сессию через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

            string email = "user1@example.com";

            // Создаем сессию для тестового пользователя
            sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Email = email,
                UserName = "Иван Петров",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(12)
            };

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

            string currentPassword = "OldPassw0rd!";
            string newPassword = "BetterP4ss!";
            string repeatPassword = "BetterP4ss!";

            // Используем BCrypt для реального хеширования паролей
            string hashedOldPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);
            string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            User user = new User
            {
                Email = email,
                Password = hashedOldPassword,
                FullName = "Иван Петров",
                Phone = "+79991234567"
            };

            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(() => user);

            // Настраиваем проверку текущего пароля с использованием BCrypt
            userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>()))
                        .Returns<User, string>((u, password) => BCrypt.Net.BCrypt.Verify(password, hashedOldPassword));

            // Настраиваем обновление пароля
            userRepoMock.Setup(repo => repo.UpdatePasswordHash(email, It.IsAny<string>()))
                        .Callback<string, string>((userEmail, password) =>
                        {
                            // В реальной системе здесь будет хеширование пароля
                            user.Password = password; // Сохраняем "сырой" пароль для проверки в тесте
                        })
                        .Returns(true);

            // Act
            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            // Assert
            Assert.AreEqual("Пароль обновлён", result);

            userRepoMock.Verify(repo => repo.UpdatePasswordHash(email, It.IsAny<string>()), Times.Once);

            // Проверяем, что пароль был обновлен
            User updatedUser = userRepoMock.Object.FindByEmail(email);
            Assert.IsNotNull(updatedUser);

            // Пароль должен быть обновлен (в реальной системе это будет хеш)
            Assert.AreNotEqual(hashedOldPassword, updatedUser.Password, "Пароль должен быть изменен");

            // Проверяем, что остальные данные не изменились
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

            // Создаем реальный SessionManager с настроенной сессией
            var httpContext = new DefaultHttpContext();

            // Создаем тестовую сессию
            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                { "SessionId", sessionId }
            });

            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var sessionManager = new SessionManager(httpContextAccessor);

            // Добавляем тестовую сессию в статический словарь через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

            string email = "user1@example.com";
            sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Email = email,
                UserName = "Test User",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(12)
            };

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

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
            // Arrange
            Mock<IUserRepository> userRepoMock = new Mock<IUserRepository>();

            // Создаем реальный SessionManager
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();

            // Настраиваем сессию для авторизации
            string sessionId = "test-session-id";
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
    {
        { "SessionId", sessionId }
    });

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            var sessionManager = new SessionManager(httpContextAccessorMock.Object);

            // Добавляем тестовую сессию через reflection
            var sessionsField = typeof(SessionManager).GetField("sessions_",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var sessions = sessionsField.GetValue(null) as ConcurrentDictionary<string, SessionData>;
            if (sessions == null)
            {
                sessions = new ConcurrentDictionary<string, SessionData>();
                sessionsField.SetValue(null, sessions);
            }

            string email = "user1@example.com";

            // Создаем сессию для тестового пользователя
            sessions[sessionId] = new SessionData
            {
                SessionId = sessionId,
                Email = email,
                UserName = "Test User",
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(12)
            };

            // Создаем реальный UserService
            var userService = new UserService(userRepoMock.Object, sessionManager);

            AccountService accountService = new AccountService(
                userRepoMock.Object,
                sessionManager,
                userService);

            // Настраиваем данные пользователя
            string hashedOldPassword = BCrypt.Net.BCrypt.HashPassword("OldPassw0rd!");
            User user = new User { Email = email, Password = hashedOldPassword };
            userRepoMock.Setup(repo => repo.FindByEmail(email)).Returns(user);

            // Настраиваем проверку пароля
            userRepoMock.Setup(repo => repo.VerifyPassword(user, It.IsAny<string>()))
                        .Returns<User, string>((u, pass) => BCrypt.Net.BCrypt.Verify(pass, hashedOldPassword));

            // Act
            string result = accountService.ChangePassword(email, currentPassword, newPassword, repeatPassword);

            // Assert
            Assert.AreEqual(expectedError, result);
            userRepoMock.Verify(repo => repo.UpdatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}