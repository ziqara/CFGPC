using System;
using DDMLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DDMTests
{
    [TestClass]
    public class TUserService
    {
        private Mock<IUserRepository> _userRepositoryMock = null;
        private UserService _userService = null;
        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [TestMethod]
        public void TestRegisterUser_WithValidData_ShouldSaveUser_Successfully()
        {
            var email = "ivan.petrov@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Петров Иван Сергеевич";
            var phone = "+7 (999) 123-45-67";
            var address = "г. Москва, ул. Ленина, д. 10, кв. 5";
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns((User)null);
            _userRepositoryMock.Setup(r => r.Save(It.IsAny<User>())).Returns<User>(u => u);

            var result = _userService.RegisterUser(email, password, passwordConfirm, fullName, phone, address);

            Assert.AreEqual("Аккаунт успешно создан!", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.Is<User>(u =>
                u.Email == email &&
                u.FullName == fullName &&
                u.Phone == phone &&
                u.Address == address &&
                u.IsActive == true &&
                u.Password == password
            )), Times.Once);
        }

        [TestMethod]
        public void TestRegisterUser_WithDuplicateEmail_ShouldReturnError()
        {
            var email = "existing.user@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Иванов Иван Иванович";
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns(new User { Email = email });

            var result = _userService.RegisterUser(email, password, passwordConfirm, fullName, "", "");

            Assert.AreEqual("Email уже зарегистрирован", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithPasswordMismatch_ShouldReturnError()
        {
            var email = "test.user@example.com";
            var password = "Passw0rd1";
            var passwordConfirm = "Passw0rd2";
            var fullName = "Сидоров Пётр Алексеевич";
            var phone = "+7 (999) 000-00-00";
            var address = "г. СПб, ул. Невский, д. 1";

            var result = _userService.RegisterUser(email, password, passwordConfirm, fullName, phone, address);

            Assert.AreEqual("Пароли не совпадают", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithInvalidEmail_ShouldReturnError()
        {
            var email = "ivan@@example";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Петров Иван Сергеевич";

            var result = _userService.RegisterUser(email, password, passwordConfirm, fullName, "", "");

            Assert.AreEqual("Некорректный email", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void TestRegisterUser_WithInvalidPhoneFormat_ShouldReturnError()
        {
            var email = "phone.test@example.com";
            var password = "Passw0rd!";
            var passwordConfirm = "Passw0rd!";
            var fullName = "Иванов Иван Иванович";
            var phone = "invalid-phone-text";
            var address = "г. Москва, ул. Тверская, д. 1";
            _userRepositoryMock.Setup(r => r.FindByEmail(email)).Returns((User)null);

            var result = _userService.RegisterUser(email, password, passwordConfirm, fullName, phone, address);

            Assert.AreEqual("Неверный формат телефона", result);
            _userRepositoryMock.Verify(r => r.FindByEmail(email), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(It.IsAny<User>()), Times.Never);
        }
    }
}
