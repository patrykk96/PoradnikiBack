using back_end.Controllers;
using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace TDD
{
    public class AuthTests
    {
        [Fact]
        public async void ShouldNotRegisterIfEmailIsTaken()
        {
            var userModel = new RegisterUserModel()
            {
                Email = "test@mail.com",
                Password = "testtt",
                Username = "test"
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));

            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.Register(userModel);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Podany adres email jest zajêty";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldRegisterIfDataCorrect()
        {
            var userModel = new RegisterUserModel()
            {
                Email = "test@mail.com",
                Password = "testtt",
                Username = "test"
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(false));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.Register(userModel);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotLoginWhenUserNotFound()
        {
            var userModel = new LoginUserModel()
            {
                Username = "test",
                Password = "testowe"
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(false));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.Login(userModel);

            var badRequest = Assert.IsType<UnauthorizedObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<LoginDto>>(badRequest.Value);

            string error = "Nazwa u¿ytkownika lub has³o niepoprawne";
            Assert.Contains(error, errorResult.Error);

        }

        [Fact]
        public async void ShouldNotLoginWhenPasswordIsNotCorrect()
        {
            var userModel = new LoginUserModel()
            {
                Username = "test",
                Password = "testowe"
            };

            string password = "bardzo tajne";
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User()
            {
                Username = "test",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.Login(userModel);

            var badRequest = Assert.IsType<UnauthorizedObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<LoginDto>>(badRequest.Value);

            string error = "Nazwa u¿ytkownika lub has³o niepoprawne";
            Assert.Contains(error, errorResult.Error);

        }

        [Fact]
        public async void ShouldNotLoginAccountNotConfirmed()
        {
            string password = "testowe";
            byte[] passwordHash;
            byte[] passwordSalt;

            var userModel = new LoginUserModel()
            {
                Username = "test",
                Password = password
            };

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User()
            {
                Id = 1,
                Username = "test",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ConfirmedAccount = false
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.Login(userModel);
            var badRequest = Assert.IsType<UnauthorizedObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<LoginDto>>(badRequest.Value);

            string error = "Konto niezatwierdzone. SprawdŸ swój email";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotConfirmEmailIfUserNotFound()
        {
            string username = "test";
            string code = "kod";
            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ConfirmEmail(username, code);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Podany u¿ytkownik nie zosta³ znaleziony";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotConfirmEmailIfCodeIsWrong()
        {
            string username = "test";
            string code = Guid.NewGuid().ToString();
            User user = new User()
            {
                Username = "test",
                VerificationCode = Guid.NewGuid()
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ConfirmEmail(username, code);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Wyst¹pi³ b³¹d podczas potwierdzenia adresu email";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotConfirmEmailIfAlreadyConfirmed()
        {
            string username = "test";
            Guid codeGuid = Guid.NewGuid();
            string code = codeGuid.ToString();
            User user = new User()
            {
                Username = "test",
                VerificationCode = codeGuid,
                ConfirmedAccount = true
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ConfirmEmail(username, code);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Wyst¹pi³ b³¹d podczas potwierdzenia adresu email";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldConfirmEmailIfDataCorrect()
        {
            string username = "test";
            Guid codeGuid = Guid.NewGuid();
            string code = codeGuid.ToString();
            User user = new User()
            {
                Username = "test",
                VerificationCode = codeGuid,
                ConfirmedAccount = false
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ConfirmEmail(username, code);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotResetPasswordIfUserNotFound()
        {
            string email = "test";
            var repo = new Mock<IRepository<User>>();
            User user = null;
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));

            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ResetPassword(email);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie znaleziono wybranego u¿ytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldResetPasswordIfUserFound()
        {
            string email = "pkucinski@gmail.com";
            var repo = new Mock<IRepository<User>>();
            User user = new User()
            {
                Email = email,
                Username = "test",
                ConfirmedAccount = true
            };
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));

            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ResetPassword(email);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotSetNewPasswordIfUserNotFound()
        {
            string username = "test";
            string code = "kod";
            string password = "haslo";
            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.SetNewPassword(username, code, password);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie znaleziono podanego u¿ytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotSetNewPasswordIfCodeIsWrong()
        {
            string username = "test";
            Guid codeGuid = new Guid();
            string code = "kod";
            string password = "haslo";
            User user = new User()
            {
                Username = username,
                VerificationCode = codeGuid,
                ConfirmedAccount = true
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.SetNewPassword(username, code, password);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie uda³o siê ustawiæ nowego has³a";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldSetNewPasswordIfCodeIsWrong()
        {
            string username = "test";
            Guid codeGuid = new Guid();
            string code = codeGuid.ToString();
            string password = "haslo";
            User user = new User()
            {
                Username = username,
                VerificationCode = codeGuid,
                ConfirmedAccount = true
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.SetNewPassword(username, code, password);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotChangePasswordIfUserNotFound()
        {
            var model = new ChangePasswordModel()
            {
                OldPassword = "test",
                NewPassword = "test2"
            };
            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ChangePassword(model);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie znaleziono u¿ytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotChangePasswordIfOldPasswordNotCorrect()
        {
            var model = new ChangePasswordModel()
            {
                OldPassword = "test",
                NewPassword = "test2"
            };
            string password = "bardzo tajne";
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User()
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ChangePassword(model);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Stare has³o jest niepoprawne";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldChangePasswordIfDataIsCorrect()
        {
            var model = new ChangePasswordModel()
            {
                Id = 1,
                OldPassword = "test",
                NewPassword = "test2"
            };
            string password = "test";
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var user = new User()
            {
                Id = 1,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var configuration = new Mock<IConfiguration>();

            var authService = new AuthService(repo.Object, configuration.Object);
            var authController = new AuthController(authService, configuration.Object);

            var result = await authController.ChangePassword(model);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }

   
}
