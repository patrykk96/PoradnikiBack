using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getUser/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.GetUser(userId);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("editUser")]
        public async Task<IActionResult> EditUser(UserModel editUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.EditUser(editUserModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("setAvatar/{id}")]
        public async Task<IActionResult> SetUserAvatar(int id, ImageModel imageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _userService.SetUserAvatar(id, imageModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        
    }
}