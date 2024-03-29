﻿using Data.DbModels;
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

        //metoda zwracajaca uzytkownika
        public async Task<ResultDto<UserDto>> GetUser(int id)
        {
            var result = new ResultDto<UserDto>()
            {
                Error = null
            };

            //sprawdzam czy uzytkownik istnieje
            var user = await _repo.GetSingleEntity(x => x.Id == id);

            if (user == null)
            {
                result.Error = "Nie znaleziono takiego użytkownika";
                return result;
            }

            //zwracam użytkownika
            result.SuccessResult = new UserDto()
            {
                Id = user.Id,
                Description = user.Description,
                Username = user.Username,
                Email = user.Email
            };

            return result;
        }

        //edycja użytkownika
        public async Task<ResultDto<BaseDto>> EditUser(UserModel editUserModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            //sprawdzam czy taki użytkownik istnieje
            var user = await _repo.GetSingleEntity(x => x.Id == editUserModel.Id);

            if (user == null)
            {
                result.Error = "Nie ma takiego użytkownika";
                return result;
            }

            //sprawdzam czy nowa nazwa nie jest zajeta
            if (user.Username != editUserModel.Username)
            {
                bool exists = await _repo.Exists(x => x.Username == editUserModel.Username);

                if (exists)
                {
                    result.Error = "Podana nazwa użytkownika jest już zajęta";
                    return result;
                }
            }

            //aktualizuje uzytkownika
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

        public async Task<ResultDto<ImagePathDto>> SetUserAvatar(int id, ImageModel imageModel)
        {
            var result = new ResultDto<ImagePathDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Id == id);

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
            var fileName = id + Path.GetExtension(imageModel.Image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using (FileStream fs = File.Create(filePath))
            {
                imageModel.Image.CopyTo(fs);
                fs.Flush();
            }

            user.ProfileImage = @"https://localhost:44326/api/image/" + $"{fileName}";

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
