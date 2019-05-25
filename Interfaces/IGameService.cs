using Data.Dtos;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGameService
    {
        Task<ResultDto<BaseDto>> AddGame(GameModel gameModel);
        Task<ResultDto<BaseDto>> UpdateGame(int id, GameModel gameModel);
        Task<ResultDto<BaseDto>> DeleteGame(int id);
        Task<ResultDto<GameDto>> GetGame(int id);
        Task<ResultDto<ListGameDto>> GetGames(int id);
        Task<ResultDto<BaseDto>> AddReview(ReviewModel reviewModel);
        Task<ResultDto<ReviewDto>> GetReview(int userId, int gameId);
    }
}
