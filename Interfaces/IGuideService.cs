﻿using Data.Dtos;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGuideService
    {
        Task<ResultDto<BaseDto>> AddGuide(GuideModel guideModel);
        Task<ResultDto<BaseDto>> UpdateGuide(int id, GuideModel guideModel);
        Task<ResultDto<GuideDto>> GetGuide(int id);
        Task<ResultDto<GuidesDto>> GetGuides(int userId, int gameId);
        Task<ResultDto<BaseDto>> AddReview(ReviewModel reviewModel);
        Task<ResultDto<BaseDto>> DeleteGuide(int id);
    }
}
