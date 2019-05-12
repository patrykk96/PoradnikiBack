using Data.DbModels;
using Data.Dtos;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repo;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserService(IRepository<User> repo, IHostingEnvironment hostingEnvironment)
        {
            _repo = repo;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ResultDto<UserDto>> GetUser(int id)
        {
            var result = new ResultDto<UserDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Id == id);

            if (user == null)
            {
                result.Error = "Nie znaleziono takiego użytkownika";
                return result;
            }

            var userToSend = new UserModel()
            {
                Id = user.Id,
                Description = user.Description,
                Username = user.Username
            };

            result.SuccessResult = new UserDto
            {
                User = userToSend
            };

            return result;
        }

        public async Task<ResultDto<BaseDto>> EditUser(UserModel editUserModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Id == editUserModel.Id);

            if (user == null)
            {
                result.Error = "Nie ma takiego użytkownika";
                return result;
            }

            user.Username = editUserModel.Username;
            user.Description = editUserModel.Description;

            try
            {
                _repo.Update(user);
            }
            catch(Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }

        public async Task<ResultDto<ImagePathDto>> SetUserAvatar(ImageModel imageModel)
        {
            var result = new ResultDto<ImagePathDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Id == imageModel.Id);

            if (user == null)
            {
                result.Error = "Nie znaleziono użytkownika";
                return result;
            }

            if (imageModel.Image == null)
            {
                result.Error = "Nie udało się ustawić zdjęcia";
                return result;
            }

            var folderPath = _hostingEnvironment.WebRootPath + "\\uploads\\";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            var fileName = Guid.NewGuid() + Path.GetExtension(imageModel.Image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using (FileStream fs = File.Create(filePath))
            {
                imageModel.Image.CopyTo(fs);
                fs.Flush();
            }

            user.ProfileImage = @"http://localhost:8080/api/photo/" + $"{fileName}";

            try
            {
                _repo.Update(user);
            }
            catch (Exception e)
            {
                result.Error = e.Message;
            }
            return result;

        }  
    }
}
