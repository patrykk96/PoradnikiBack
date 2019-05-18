using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GuideService : IGuideService
    {
        private readonly IRepository<Guide> _guideRepo;
        private readonly IRepository<Game> _gameRepo;
        private readonly IRepository<User> _userRepo;

        public GuideService(IRepository<Guide> guideRepo, IRepository<Game> gameRepo,
                            IRepository<User> userRepo)
        {
            _guideRepo = guideRepo;
            _gameRepo = gameRepo;
            _userRepo = userRepo;
        }

        public async Task<ResultDto<BaseDto>> AddGuide(GuideModel guideModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            bool userExists = await _userRepo.Exists(x => x.Id == guideModel.Author);

            if (!userExists)
            {
                result.Error = "Autor poradnika nie został odnaleziony";
                return result;
            }

            bool gameExists = await _gameRepo.Exists(x => x.Id == guideModel.Game);

            if (!gameExists)
            {
                result.Error = "Podana gra nie została znaleziona";
                return result;
            }

            var guide = new Guide()
            {
                AuthorId = guideModel.Author,
                Content = guideModel.Content,
                GameId = guideModel.Game,
                Name = guideModel.Name
            };

            try
            {
                _guideRepo.Add(guide);
            }
            catch(Exception e)
            {
                result.Error = e.Message;
            }

            return result; 
        }

        public async Task<ResultDto<BaseDto>> UpdateGuide(int id, GuideModel guideModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var guide = await _guideRepo.GetSingleEntity(x => x.Id == id);

            if (guide == null)
            {
                result.Error = "Nie odnaleziono wybranego poradnika";
                return result;
            }

            bool userExists = await _userRepo.Exists(x => x.Id == guideModel.Author);

            if (!userExists)
            {
                result.Error = "Autor poradnika nie został odnaleziony";
                return result;
            }

            bool gameExists = await _gameRepo.Exists(x => x.Id == guideModel.Game);

            if (!gameExists)
            {
                result.Error = "Podana gra nie została znaleziona";
                return result;
            }

            guide.Name = guideModel.Name;
            guide.Content = guideModel.Content;

            try
            {
                _guideRepo.Update(guide);
            }
            catch(Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        public async Task<ResultDto<GuideDto>> GetGuide(int id)
        {
            var result = new ResultDto<GuideDto>()
            {
                Error = null
            };

            var guide = await _guideRepo.GetSingleEntity(x => x.Id == id);

            if (guide == null)
            {
                result.Error = "Nie odnaleziono poradnika";
                return result;
            }

            var user = await _userRepo.GetSingleEntity(x => x.Id == guide.AuthorId);

            if (user == null)
            {
                result.Error = "Nie odnaleziono autora";
                return result;
            }

            var game = await _gameRepo.GetSingleEntity(x => x.Id == guide.GameId);

            if (game == null)
            {
                result.Error = "Nie odnaleziono gry";
                return result;
            }

            var guideToSend = new GuideDto()
            {
                Id = guide.Id,
                Content = guide.Content,
                Username = user.Username,
                Name = guide.Name,
                GameName = game.Name,
                GameImage = game.Image
            };

            result.SuccessResult = guideToSend;

            return result;
        }

        public async Task<ResultDto<GuidesDto>> GetGuides(int userId, int gameId)
        {
            var result = new ResultDto<GuidesDto>()
            {
                Error = null
            };

            List<GuideDto> list = new List<GuideDto>();
            List<Guide> guides = new List<Guide>();
            if (userId == 0 && gameId == 0)
            {
                guides = await _guideRepo.GetAll();
            }
            else if (userId != 0)
            {
                var user = _userRepo.GetSingleEntity(x => x.Id == userId);
                if (user == null)
                {
                    result.Error = "Nie znaleziono użytkownika";
                    return result;
                }

                guides = await _guideRepo.GetAllBy(x => x.AuthorId == userId);
                if (gameId != 0)
                {
                    guides = guides.Where(x => x.GameId == gameId).ToList();
                }
            }
            else
            {
                var game = _gameRepo.GetSingleEntity(x => x.Id == gameId);

                if (game == null)
                {
                    result.Error = "Nie znaleziono gry";
                    return result;
                }

                guides = await _guideRepo.GetAllBy(x => x.GameId == gameId);
            }

            foreach (var guide in guides)
            {
                var user = await _userRepo.GetSingleEntity(x => x.Id == guide.AuthorId);
                var game = await _gameRepo.GetSingleEntity(x => x.Id == guide.GameId);

                var g = new GuideDto()
                {
                    Id = guide.Id,
                    Content = guide.Content,
                    Username = user.Username,
                    Name = guide.Name,
                    GameName = game.Name,
                    GameImage = game.Image
                };

                list.Add(g);
            }

            result.SuccessResult = new GuidesDto()
            {
                Guides = list
            };

            return result;
        }
    }
}
