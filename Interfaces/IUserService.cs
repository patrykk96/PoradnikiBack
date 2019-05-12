using Data.Dtos;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<UserDto>> GetUser(int id);
        Task<ResultDto<BaseDto>> EditUser(UserModel editUserModel);
        Task<ResultDto<ImagePathDto>> SetUserAvatar(ImageModel imageModel);
    }
}
