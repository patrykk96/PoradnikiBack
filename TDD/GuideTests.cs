using back_end.Controllers;
using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
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
    public class GuideTests
    {
        [Fact]
        public async void ShouldNotAddGuideIfAuthorNotFound()
        {
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(false));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.AddGuide(guideModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Autor poradnika nie został odnaleziony";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotAddGuideIfGameNotFound()
        {
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            gameRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(false));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.AddGuide(guideModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Podana gra nie została znaleziona";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldAddGuideIfDataCorrect()
        {
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            gameRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(true));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.AddGuide(guideModel);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotUpdateGuideIfGuideNotFound()
        {
            var id = 1;
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            Guide guide = null;
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.UpdateGuide(id, guideModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie odnaleziono wybranego poradnika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotUpdateGuideIfAuthorNotFound()
        {
            var id = 1;
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            var guide = new Guide();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(false));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.UpdateGuide(id, guideModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Autor poradnika nie został odnaleziony";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotUpdateGuideIfGameNotFound()
        {
            var id = 1;
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            var guide = new Guide();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            gameRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(false));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.UpdateGuide(id, guideModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Podana gra nie została znaleziona";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldUpdateGuideIfDataCorrect()
        {
            var id = 1;
            var guideModel = new GuideModel();
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            var guide = new Guide();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(true));
            gameRepo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(true));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.UpdateGuide(id, guideModel);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotGetGuideIfGuideNotFound()
        {
            var id = 1;
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            Guide guide = null;
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.GetGuide(id);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<GuideDto>>(badRequest.Value);

            string error = "Nie odnaleziono poradnika";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotGetGuideIfAuthorNotFound()
        {
            var id = 1;
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            Guide guide = new Guide();
            User user = null;
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.GetGuide(id);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<GuideDto>>(badRequest.Value);

            string error = "Nie odnaleziono autora";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldNotGetGuideIfGameNotFound()
        {
            var id = 1;
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            Guide guide = new Guide();
            User user = new User();
            Game game = null;
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            gameRepo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.GetGuide(id);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<GuideDto>>(badRequest.Value);

            string error = "Nie odnaleziono gry";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldGetGuideIfDataCorrect()
        {
            var id = 1;
            var repo = new Mock<IRepository<Guide>>();
            var gameRepo = new Mock<IRepository<Game>>();
            var userRepo = new Mock<IRepository<User>>();
            var reviewRepo = new Mock<IRepository<GuideReview>>();
            Guide guide = new Guide();
            User user = new User();
            Game game = new Game();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guide));
            userRepo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(user));
            gameRepo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));

            var guideService = new GuideService(repo.Object, gameRepo.Object, userRepo.Object, reviewRepo.Object);
            var guideController = new GuideController(guideService);

            var result = await guideController.GetGuide(id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<GuideDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }
    }
}
