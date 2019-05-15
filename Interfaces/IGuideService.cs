using Data.Dtos;
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
    }
}
