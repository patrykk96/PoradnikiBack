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
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepo;
        private readonly IRepository<Game> _gameRepo;
        private readonly IRepository<Guide> _guideRepo;

        public ReviewService(IRepository<Review> reviewRepo, IRepository<Game> gameRepo, IRepository<Guide> guideRepo)
        {
            _reviewRepo = reviewRepo;
            _gameRepo = gameRepo;
            _guideRepo = guideRepo;
        }

        public async Task<ResultDto<BaseDto>> AddReview(ReviewModel reviewModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var reviewExists = await _reviewRepo.Exists(x => x.GameId == reviewModel.GameId
                                                        && x.GuideId == reviewModel.GuideId
                                                        && x.UserId == reviewModel.UserId);
            var review = new Review()
            {
                UserId = reviewModel.UserId,
                GuideId = reviewModel.GuideId,
                GameId = reviewModel.GameId,
                Rating = reviewModel.Rating
            };

            if (reviewExists)
            {
                _reviewRepo.Update(review);
            }
            else
            {
                _reviewRepo.Add(review);
            }
             

            if (reviewModel.GameId != 0)
            {
                RecalculateGameReview(reviewModel.GameId);
            }

            if (reviewModel.GuideId != 0)
            {
                RecalculateGuideReview(reviewModel.GuideId);
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

            game.Rating = score;

            _gameRepo.Update(game);
        }

        private async void RecalculateGuideReview(int guideId)
        {
            var reviews = await _reviewRepo.GetAllBy(x => x.GuideId == guideId);

            int sum = 0;

            foreach (var review in reviews)
            {
                sum += review.Rating;
            }

            double score = (double)sum / reviews.Count;

            var guide = await _guideRepo.GetSingleEntity(x => x.Id == guideId);

            guide.Rating = score;

            _guideRepo.Update(guide);
        }
    }
}
