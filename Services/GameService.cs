using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> _repo;
        private readonly IRepository<Guide> _guideRepo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public GameService(IRepository<Game> repo, IRepository<Guide> guideRepo, IHostingEnvironment hostingEnvironment)
        {
            _repo = repo;
            _guideRepo = guideRepo;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ResultDto<BaseDto>> AddGame(GameModel gameModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            bool exists = await _repo.Exists(x => x.Name == gameModel.Name);

            if (exists)
            {
                result.Error = "Gra o podanej nazwie została już utworzona";
                return result;
            }

            string path = null;

            if (gameModel.Image != null)
            {
                path = SaveFile(gameModel.Image, gameModel.Name);
            }

            var game = new Game()
            {
                Name = gameModel.Name,
                Description = gameModel.Description,
                Image = path
            };

            try
            {
                _repo.Add(game);
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            return result;

        }

        public async Task<ResultDto<BaseDto>> UpdateGame(int id, GameModel gameModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var game = await _repo.GetSingleEntity(x => x.Id == id);

            if (game == null)
            {
                result.Error = "Nie znaleziono podanej gry";
                return result;
            }

            if (game.Name != gameModel.Name)
            {
                bool exists = await _repo.Exists(x => x.Name == gameModel.Name);

                if (exists)
                {
                    result.Error = "Podana nazwa gry jest już zajęta";
                    return result;
                }
            }

            string path = "";
            if (gameModel.Image != null)
            {
                path = SaveFile(gameModel.Image, gameModel.Name);
            }

            if (gameModel.Name != "null")
            {
                game.Name = gameModel.Name;
            }
            if (gameModel.Description != "null")
            {
                game.Description = gameModel.Description;
            }

            if (path.Length > 0) game.Image = path;

            try
            {
                _repo.Update(game);
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        public async Task<ResultDto<BaseDto>> DeleteGame(int id)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var game = await _repo.GetSingleEntity(x => x.Id == id);

            if (game == null)
            {
                result.Error = "Nie odnaleziono podanej gry";
                return result;
            }

            try
            {
                _repo.Delete(game);
            }
            catch(Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        public async Task<ResultDto<GameDto>> GetGame(int id)
        {
            var result = new ResultDto<GameDto>()
            {
                Error = null
            };

            var game = await _repo.GetSingleEntity(x => x.Id == id);

            if (game == null)
            {
                result.Error = "Nie odnaleziono gry";
                return result;
            }

            var guides = await _guideRepo.GetAllBy(x => x.GameId == id);

            var gameToSend = new GameDto()
            {
                Id = game.Id,
                Description = game.Description,
                Name = game.Name,
                Image = game.Image,
                Rating = game.Rating,
                GuidesCount = guides.Count
            };

            result.SuccessResult = gameToSend;

            return result;
        }

        public async Task<ResultDto<ListGameDto>> GetGames()
        {
            var result = new ResultDto<ListGameDto>()
            {
                Error = null
            };

            var games = await _repo.GetAll();

            List<GameDto> gamesToSend = new List<GameDto>();

            foreach(var game in games)
            {
                var guides = await _guideRepo.GetAllBy(x => x.GameId == game.Id);
                var g = new GameDto()
                {
                    Id = game.Id,
                    Description = game.Description,
                    Image = game.Image,
                    Name = game.Name,
                    Rating = game.Rating,
                    GuidesCount = guides.Count
                };

                gamesToSend.Add(g);
            }

            var gameList = new ListGameDto()
            {
                List = gamesToSend
            };

            result.SuccessResult = gameList;

            return result;
        }

        private string SaveFile(IFormFile image, string name)
        {
            string path = "";
          
            var folderPath = _hostingEnvironment.WebRootPath + "\\uploads\\";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                name = name.Replace(c.ToString(), "");
            }
            var fileName = name + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using (FileStream fs = File.Create(filePath))
            {
                image.CopyTo(fs);
                fs.Flush();
            }

            path = @"https://localhost:44326/api/image/" + $"{fileName}";

            return path;
        }
    }
}
