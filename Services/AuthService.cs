using Data.DbModels;
using Data.Dtos;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _repo;
        private readonly IConfiguration _config;

        public AuthService(IRepository<User> repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<ResultDto<BaseDto>> Register(User user, string password)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var emailExists = await _repo.Exists(x => x.Email == user.Email);
            if (emailExists)
            {
                result.Error = "Podany adres email jest zajęty";
                return result;
            }

            var usernameExists = await _repo.Exists(x => x.Username == user.Username);
            if (emailExists)
            {
                result.Error = "Podana nazwa użytkownika jest zajęta";
                return result;
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.VerificationCode = Guid.NewGuid();
            user.ConfirmedAccount = false;

            _repo.Add(user);
            SendConfirmationEmail(user.Email, user.Username, user.VerificationCode);

            return result;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private void SendConfirmationEmail(string email, string username, Guid code)
        {
            using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("testowymail145@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Potwierdzenie konta: " + username;
                    mail.IsBodyHtml = true;
                    mail.Body = "Dzień dobry " + username + "! <br/><br/>" +
                        "<a href='http://localhost:3000/confirmAccount/" + username + "/" + code.ToString() + "'>Kliknij w link, aby potwierdzić swoje konto. </a>";

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new NetworkCredential("testowymail145@gmail.com", "testowehaslo135");
                    smtpServer.EnableSsl = true;

                    smtpServer.Send(mail);
                };
            };
        }


        public async Task<bool> UserExists(string username)
        {
            if (await _repo.Exists(x => x.Username == username))
                return true;
            return false;
        }

        public async Task<ResultDto<LoginDto>> Login(string username, string password)
        {
            var result = new ResultDto<LoginDto>
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                result.Error = "Nazwa użytkownika lub hasło niepoprawne";
                return result;
            }

            if (!user.ConfirmedAccount)
            {
                result.Error = "Konto niezatwierdzone. Sprawdź swój email";
                return result;
            }

            var token = GenerateToken(user);

            result.SuccessResult = new LoginDto()
            {
                Token = token
            };

            return result;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var ctoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(ctoken);

            return token;
        }

        public async Task<ResultDto<BaseDto>> ConfirmEmail(string username, string code)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Username == username);
            
            if (user == null)
            {
                result.Error = "Podany użytkownik nie został znaleziony";
                return result;
            }

            if (user.VerificationCode.ToString() == code && !user.ConfirmedAccount)
            {
                user.ConfirmedAccount = true;
                user.VerificationCode = Guid.Empty;
                _repo.Update(user);
                return result;
            }

            result.Error = "Wystąpił błąd podczas potwierdzenia adresu email";
            return result;
        }

        public async Task<ResultDto<BaseDto>> ResetPassword(string email)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Email == email);

            if (user == null || !user.ConfirmedAccount)
            {
                result.Error = "Nie znaleziono wybranego użytkownika";
                return result;
            }

            user.VerificationCode = Guid.NewGuid();
            var code = user.VerificationCode;

            _repo.Update(user);

            using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("testowymail145@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Zmiana hasła konta: " + user.Username;
                    mail.IsBodyHtml = true;
                    mail.Body = "Dzień dobry " + user.Username + "! <br/><br/>" +
                        "<a href='http://localhost:3000/changePassword/" + user.Username + "/" + code.ToString() + "'>Kliknij w link, aby ustawić nowe hasło. </a>";

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new NetworkCredential("testowymail145@gmail.com", "testowehaslo135");
                    smtpServer.EnableSsl = true;

                    smtpServer.Send(mail);
                };
            };

            return result;
        }

        public async Task<ResultDto<BaseDto>> SetNewPassword(string username, string code, string newPassword)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Username == username);

            if (user == null)
            {
                result.Error = "Nie znaleziono podanego użytkownika";
                return result;
            }

            if (user.VerificationCode.ToString() == code && user.ConfirmedAccount)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.VerificationCode = Guid.Empty;
                _repo.Update(user);
                return result;
            }

            result.Error = "Nie udało się ustawić nowego hasła";
            return result;
        }

        public async Task<ResultDto<LoginDto>> FacebookLogin(string fbToken, string id)
        {
            var result = new ResultDto<LoginDto>()
            {
                Error = null
            };

            string responseResult = null;
            var fbAPIUrl = "https://graph.facebook.com/v2.12";
            var fbAPIQueryString = string.Format(
                "me?scope=email&access_token={0}&fields=id,name,email",
                fbToken);

            using (var c = new HttpClient())
            {
                c.BaseAddress = new Uri(fbAPIUrl);
                var response = await c.GetAsync(fbAPIQueryString);
                if (response.IsSuccessStatusCode)
                {
                    responseResult = await response.Content.ReadAsStringAsync();
                }
                else throw new Exception("Błąd uwierzytelniania");
            };

            var epInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseResult);
            var users = await _repo.GetAll(x => x.LoginProvider == "facebook");
            var user = await _repo.GetSingleEntity(x => x.ProviderKey == epInfo["id"]);

            if (user == null)
            {
                user = await _repo.GetSingleEntity(x => x.Email == epInfo["email"]);

                if (user == null)
                {
                    var username = string.Format("FB{0}{1}",
                          epInfo["id"],
                          Guid.NewGuid().ToString("N")
                      );

                    var password = "123456";
                    byte[] passwordHash, passwordSalt;

                    CreatePasswordHash(password, out passwordHash, out passwordSalt);

                    user = new User()
                    {
                        Email = epInfo["email"],
                        Username = username,
                        LoginProvider = "facebook",
                        ProviderKey = epInfo["id"],
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        ConfirmedAccount = true
                    };

                    _repo.Add(user);
                }

                user.LoginProvider = "facebook";
                user.ProviderKey = epInfo["id"];

                _repo.Update(user);
            }


            var token = GenerateToken(user);

            result.SuccessResult = new LoginDto()
            {
                Token = token
            };

            return result;
        }
    }
}
