using Data.DbModels;
using Data.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IAuthService
    {
        Task<ResultDto<BaseDto>> Register(User user, string password);
        Task<bool> UserExists(string username);
        Task<ResultDto<LoginDto>> Login(string username, string password);
        Task<ResultDto<BaseDto>> ConfirmEmail(string username, string code);
        Task<ResultDto<BaseDto>> ResetPassword(string email);
        Task<ResultDto<BaseDto>> SetNewPassword(string username, string code, string newPassword);
        Task<ResultDto<LoginDto>> FacebookLogin(string token, string id);
    }
}
