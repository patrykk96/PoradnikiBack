using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using System;
using System.Collections.Generic;
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
            _guideRepo = guideRepo;
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
    }
}
