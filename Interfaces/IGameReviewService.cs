﻿using Data.Dtos;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGameReviewService
    {
        Task<ResultDto<BaseDto>> AddReview(GameReviewModel reviewModel);
    }
}