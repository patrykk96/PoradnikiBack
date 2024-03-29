﻿using back_end.Controllers;
using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TDD
{
    public class GameTests
    {
        [Fact]
        public async void ShouldNotAddGameIfNameExists()
        {
            var name = "test";
            var description = "opis";
            var model = new ImageModel();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(true));
            var hosting = new Mock<IHostingEnvironment>();

            var gModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = model.Image
            };

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.AddGame(gModel);

            string error = "Gra o podanej nazwie została już utworzona";
            Assert.Contains(error, result.Error);
        }

        [Fact]
        public async void ShouldAddGameIfNotDuplicate()
        {
            var name = "test";
            var description = "opis";
            var model = new ImageModel();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(false));
            var hosting = new Mock<IHostingEnvironment>();

            var gModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = model.Image
            };

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);
 

            var result = await gameService.AddGame(gModel);

            Assert.Null(result.Error);
        }

        [Fact]
        public async void ShouldNotUpdateGameIfGameNotFound()
        {
            var id = 1;
            var name = "test";
            var description = "opis";
            var model = new ImageModel();
            Game game = null;
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            var hosting = new Mock<IHostingEnvironment>();

            var gModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = model.Image
            };

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.UpdateGame(id, gModel);
            
            string error = "Nie znaleziono podanej gry";
            Assert.Contains(error, result.Error);
        }

        [Fact]
        public async void ShouldNotUpdateGameIfNewNameDuplicate()
        {
            var id = 1;
            var name = "test";
            var description = "opis";
            var model = new ImageModel();
            Game game = new Game();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(true));
            var hosting = new Mock<IHostingEnvironment>();

            var gModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = model.Image
            };

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.UpdateGame(id, gModel);

            string error = "Podana nazwa gry jest już zajęta";
            Assert.Contains(error, result.Error);
        }

        [Fact]
        public async void ShouldUpdateGameIfDataCorrect()
        {
            var id = 1;
            var name = "test";
            var description = "opis";
            var model = new ImageModel();
            Game game = new Game();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(false));
            var hosting = new Mock<IHostingEnvironment>();

            var gModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = model.Image
            };

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.UpdateGame(id, gModel);

            Assert.Null(result.Error);
        }

        [Fact]
        public async void ShouldDeleteGameIfGameFound()
        {
            var id = 1;
            var model = new ImageModel();
            Game game = new Game();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.DeleteGame(id);

            Assert.Null(result.Error);
        }

        [Fact]
        public async void ShouldNotDeleteGameIfGameNotFound()
        {
            var id = 1;
            var model = new ImageModel();
            Game game = null;
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);

            var result = await gameService.DeleteGame(id);

            string error = "Nie odnaleziono podanej gry";
            Assert.Contains(error, result.Error);
        }

        [Fact]
        public async void ShouldNotReturnGameIfNotFound()
        {
            var id = 1;
            Game game = null;
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);
            var gameController = new GameController(gameService);

            var result = await gameController.GetGame(id);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<GameDto>>(badRequest.Value);

            string error = "Nie odnaleziono gry";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldReturnGameIfDataCorrect()
        {
            var id = 1;
            Game game = new Game();
            var guides = new List<Guide>();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            guideRepo.Setup(x => x.GetAllBy(It.IsAny<Expression<Func<Guide, bool>>>())).Returns(Task.FromResult(guides));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);
            var gameController = new GameController(gameService);

            var result = await gameController.GetGame(id);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<GameDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }

        [Fact]
        public async void ShouldNotAddReviewIfGameNotFound()
        {
            var reviewModel = new ReviewModel()
            {
                EntityId = 1,
                Rating = 1,
                UserId = 1
            };
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(false));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);
            var gameController = new GameController(gameService);

            var result = await gameController.AddReview(reviewModel);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorResult = Assert.IsAssignableFrom<ResultDto<BaseDto>>(badRequest.Value);

            string error = "Nie odnaleziono gry";
            Assert.Contains(error, errorResult.Error);
        }

        [Fact]
        public async void ShouldAddReviewIfDataCorrect()
        {
            var reviewModel = new ReviewModel()
            {
                EntityId = 1,
                Rating = 1,
                UserId = 1
            };
            var list = new List<GameReview>();
            var game = new Game();
            var repo = new Mock<IRepository<Game>>();
            var guideRepo = new Mock<IRepository<Guide>>();
            var gameReviewRepo = new Mock<IRepository<GameReview>>();
            gameReviewRepo.Setup(x => x.GetAllBy(It.IsAny<Expression<Func<GameReview, bool>>>())).Returns(Task.FromResult(list));
            repo.Setup(x => x.GetSingleEntity(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(game));
            repo.Setup(x => x.Exists(It.IsAny<Expression<Func<Game, bool>>>())).Returns(Task.FromResult(true));
            var hosting = new Mock<IHostingEnvironment>();

            var gameService = new GameService(repo.Object, guideRepo.Object, hosting.Object, gameReviewRepo.Object);
            var gameController = new GameController(gameService);

            var result = await gameController.AddReview(reviewModel);
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var ResultValue = Assert.IsAssignableFrom<ResultDto<BaseDto>>(OkResult.Value);

            Assert.Null(ResultValue.Error);
        }
    }
    
}
