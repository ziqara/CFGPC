using DDMLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace DDMTests
{
    [TestClass]
    public class TUserService
    {
        private Mock<IUserRepository> CreateUserRepositoryMock()
        {
            return new Mock<IUserRepository>();
        }

        private SessionManager CreateSessionManager()
        {
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            return new SessionManager(httpContextAccessorMock.Object);
        }

        private UserService CreateUserService(Mock<IUserRepository> userRepoMock)
        {
            var sessionManager = CreateSessionManager();
            return new UserService(userRepoMock.Object, sessionManager);
        }

        [TestMethod]
        public void TestRegisterUser_WithValidData_ShouldSaveUser_Successfully()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "ivan.petrov@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Петров Иван Сергеевич";
            var phone = "+7 (999) 123-45-67";
            var address = "г. Москва, ул. Ленина, д. 10, кв. 5";
            User user = new User();
            user.Email = email;
            user.Password = password;
            user.Address = address;
            user.FullName = fullName;
            user.Phone = phone;
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns((User)null);
            _userRepositoryMock.Setup(r => r.Save(It.IsAny<User>())).Returns<User>(u => u);

            // Act
            var result = _userService.RegisterUser(user, passwordConfirm);

            // Assert
            Assert.AreEqual("Аккаунт успешно создан!", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.Is<User>(u =>
                u.Email == email &&
                u.FullName == fullName &&
                u.Phone == phone &&
                u.Address == address &&
                u.Password == password
            )), Times.Once);
        }

        [TestMethod]
        public void TestRegisterUser_WithDuplicateEmail_ShouldReturnError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "existing.user@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Иванов Иван Иванович";
            User user = new User();
            user.Email = email;
            user.Password = password;
            user.FullName = fullName;
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns(new User { Email = email });

            // Act
            var result = _userService.RegisterUser(user, passwordConfirm);

            // Assert
            Assert.AreEqual("Email уже зарегистрирован", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithPasswordMismatch_ShouldReturnError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "test.user@example.com";
            var password = "Passw0rd1";
            var passwordConfirm = "Passw0rd2";
            var fullName = "Сидоров Пётр Алексеевич";
            var phone = "+7 (999) 000-00-00";
            var address = "г. СПб, ул. Невский, д. 1";
            User user = new User();
            user.Email = email;
            user.Password = password;
            user.Address = address;
            user.FullName = fullName;
            user.Phone = phone;

            // Act
            var result = _userService.RegisterUser(user, passwordConfirm);

            // Assert
            Assert.AreEqual("Пароли не совпадают", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithInvalidEmail_ShouldReturnError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "ivan@@example";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Петров Иван Сергеевич";
            User user = new User();
            user.Email = email;
            user.Password = password;
            user.FullName = fullName;

            // Act
            var result = _userService.RegisterUser(user, passwordConfirm);

            // Assert
            Assert.AreEqual("Некорректный email. Требуемый формат: username@domain.com", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithInvalidPhoneFormat_ShouldReturnError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "phone.test@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Иванов Иван Иванович";
            var phone = "invalid-phone-text";
            var address = "г. Москва, ул. Тверская, д. 1";

            var user = new User
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Phone = phone,
                Address = address
            };

            // Act
            var result = _userService.RegisterUser(user, passwordConfirm);

            // Assert
            Assert.AreEqual("Неверный формат телефона. Пример: +7 (999) 123-45-67", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithCorrectData_ShouldReturnSuccessMessage()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "ivan.petrov@example.com";
            var password = "Passw0rd!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Email = email,
                Password = hashedPassword,
                FullName = "Петров Иван Сергеевич",
                RegistrationDate = DateTime.Now,
            };

            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns(user);

            // Act
            var result = _userService.LoginUser(email, password);

            // Assert
            Assert.AreEqual("", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithInvalidEmailFormat_ShouldReturnInvalidEmailError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "ivan@@example";
            var password = "Passw0rd!";

            // Act
            var result = _userService.LoginUser(email, password);

            // Assert
            Assert.AreEqual("Некорректный email. Требуемый формат: username@domain.com", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithNonExistentEmail_ShouldReturnAccountNotFoundError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "nonexistent.user@example.com";
            var password = "Passw0rd!";
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns((User)null);

            // Act
            var result = _userService.LoginUser(email, password);

            // Assert
            Assert.AreEqual("Аккаунт не найден", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithExistingEmailButWrongPassword_ShouldReturnWrongPasswordError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "ivan.petrov@example.com";
            var correctPassword = "Passw0rd!";
            var wrongPassword = "WrongPassw0rd!";

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User
            {
                Email = email,
                Password = hashedPassword,
                FullName = "Петров Иван Сергеевич",
                RegistrationDate = DateTime.Now,
            };

            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns(user);

            // Act
            var result = _userService.LoginUser(email, wrongPassword);

            // Assert
            Assert.AreEqual("Неверный пароль", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithEmptyEmail_ShouldReturnEnterEmailError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "";
            var password = "Passw0rd!";

            // Act
            var result = _userService.LoginUser(email, password);

            // Assert
            Assert.AreEqual("Введите email", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void TestLoginUser_WithEmptyPassword_ShouldReturnEnterPasswordError()
        {
            // Arrange
            var _userRepositoryMock = CreateUserRepositoryMock();
            var _userService = CreateUserService(_userRepositoryMock);

            var email = "test@example.com";
            var password = "";

            // Act
            var result = _userService.LoginUser(email, password);

            // Assert
            Assert.AreEqual("Введите пароль", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
        }
    }
}