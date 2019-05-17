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
    public class GameReviewService : IGameReviewService
    {
        private readonly IRepository<GameReview> _reviewRepo;
        private readonly IRepository<Game> _gameRepo;

        public GameReviewService(IRepository<GameReview> reviewRepo, IRepository<Game> gameRepo)
        {
            _reviewRepo = reviewRepo;
            _gameRepo = gameRepo;
        }

        public async Task<ResultDto<BaseDto>> AddReview(GameReviewModel reviewModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var oldReview = await _reviewRepo.GetSingleEntity(x => x.GameId == reviewModel.GameId
                                                        && x.UserId == reviewModel.UserId);
           

            if (oldReview != null)
            {
                if (oldReview.Rating != reviewModel.Rating)
                {
                    oldReview.Rating = reviewModel.Rating;
                    _reviewRepo.Update(oldReview);
                }
            }
            else
            {
                var review = new GameReview()
                {
                    UserId = reviewModel.UserId,
                    GameId = reviewModel.GameId,
                    Rating = reviewModel.Rating
                };
                _reviewRepo.Add(review);
            }
             

            if (reviewModel.GameId != 0)
            {
                RecalculateGameReview(reviewModel.GameId);
            }

            return result;
        }

        private async void RecalculateGameReview(int gameId)
        {
            var reviews = await _reviewRepo.GetAllBy(x => x.GameId == gameId);

            int sum = 0;

            foreach(var review in reviews)
            {
                sum += review.Rating;
            }

            double score = (double)sum / reviews.Count;

            var game = await _gameRepo.GetSingleEntity(x => x.Id == gameId);

            if (game.Rating != score)
            {
                game.Rating = score;

                _gameRepo.Update(game);
            }
            
        }

        //private async void RecalculateGuideReview(int guideId)
        //{
        //    var reviews = await _reviewRepo.GetAllBy(x => x.GuideId == guideId);

        //    int sum = 0;

        //    foreach (var review in reviews)
        //    {
        //        sum += review.Rating;
        //    }

        //    double score = (double)sum / reviews.Count;

        //    var guide = await _guideRepo.GetSingleEntity(x => x.Id == guideId);

        //    guide.Rating = score;

        //    _guideRepo.Update(guide);
        //}
    }
}
