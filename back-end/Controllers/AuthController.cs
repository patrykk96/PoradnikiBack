using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.DbModels;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserModel registerUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.Register(registerUserModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserModel loginUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.Login(loginUserModel);

            if (result.Error != null)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("confirmEmail/{username}/{code}")]
        public async Task<IActionResult> ConfirmEmail(string username, string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ConfirmEmail(username, code);

            if (result.Error != null)
            {
                return BadRequest(result);
            }


            return Ok(result);
        }

        [HttpPost("resetPassword/{email}")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPassword(email);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("setNewPassword/{username}/{code}/{newPassword}")]
        public async Task<IActionResult> SetNewPassword(string username, string code, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SetNewPassword(username, code, newPassword);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("facebookLogin")]
        public async Task<IActionResult> FacebookLogin(ExternalLoginModel externalLoginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.FacebookLogin(externalLoginModel.access_token, externalLoginModel.client_id);

            if (result.Error != null)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPatch("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _authService.ChangePassword(changePasswordModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}