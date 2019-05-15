using back_end.Controllers;
using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TDD
{
    public class UserTests
    {
        [Fact]
        public async void ShouldNotReturnUserIfNotFound()
        {
            int id = 1;
            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.GetUser(id);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<UserDto>>(badRequest.Value);

            string error = "Nie znaleziono takiego użytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldReturnUserIfFound()
        {
            int id = 1;
            User user = new User()
            {
                Id = 1,
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.GetUser(id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<UserDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotEditUserIfUserNotFound()
        {
            var model = new UserModel()
            {
                Id = 1
            };

            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.EditUser(model);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie ma takiego użytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldEditUserIfUserFound()
        {
            var model = new UserModel()
            {
                Id = 1
            };

            User user = new User()
            {
                Id = 1
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.EditUser(model);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotSetAvatarIfUserNotFound()
        {
            var model = new ImageModel();
            var id = 1;

            User user = null;

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.SetUserAvatar(id, model);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<ImagePathDto>>(badRequest.Value);

            string error = "Nie znaleziono użytkownika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotSetAvatarIfImageNotSent()
        {
            var model = new ImageModel()
            {
                Image = null
            };

            var id = 1;
            User user = new User()
            {
                Id = 1
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.SetUserAvatar(id, model);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<ImagePathDto>>(badRequest.Value);

            string error = "Nie udało się ustawić zdjęcia";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldSetAvatarIfDataCorrect()
        {
            var model = new ImageModel()
            {
                Image = new Mock<IFormFile>().Object
            };

            var id = 1;
            User user = new User()
            {
                Id = 1
            };

            var repo = new Mock<IRepository<User>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            var hosting = new Mock<IHostingEnvironment>();

            var userService = new UserService(repo.Object, hosting.Object);
            var userController = new UserController(userService);

            var result = await userController.SetUserAvatar(id, model);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<ImagePathDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }
    }
}
