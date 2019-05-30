using Data.DbModels;
using Data.Dtos;
using Data.Enums;
using Data.Models;
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

        public async Task<ResultDto<BaseDto>> Register(RegisterUserModel registerUserModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            //sprawdzam czy podany mail jest zajęty
            var emailExists = await _repo.Exists(x => x.Email == registerUserModel.Email);
            if (emailExists)
            {
                result.Error = "Podany adres email jest zajęty";
                return result;
            }

            //sprawdzam czy podana nazwa użytkownika jest zajęta
            var usernameExists = await _repo.Exists(x => x.Username == registerUserModel.Username);
            if (usernameExists)
            {
                result.Error = "Podana nazwa użytkownika jest zajęta";
                return result;
            }

            //generuję hash i salt dla podanego hasła
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(registerUserModel.Password, out passwordHash, out passwordSalt);

            //tworzę nowego użytkownika
            var user = new User
            {
                Email = registerUserModel.Email,
                Username = registerUserModel.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationCode = Guid.NewGuid(),
                ConfirmedAccount = false,
                Role = (int)UserRoles.user
            };

            _repo.Add(user);
            //wysłanie wiadomości email w celu potwierdzenia konta
            string subject = "Potwierdzenie konta: " + user.Username;
            string body = "Dzień dobry " + user.Username + "! <br/><br/>" +
                        "<a href='http://localhost:3000/confirmAccount/" + user.Username + "/" + user.VerificationCode.ToString() + "'>Kliknij w link, aby potwierdzić swoje konto. </a>";
            SendEmail(user.Email, subject, body);

            return result;
        }

        //metoda do wygenerowania hash i salt dla hasla
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //metoda wysylajaca wiadomosc email na podany adres, o podanym temacie oraz treści
        private void SendEmail(string email, string subject, string body)
        {
            using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Environment.GetEnvironmentVariable("email"));
                    mail.To.Add(email);
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = body;

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("email"), Environment.GetEnvironmentVariable("haslo"));
                    smtpServer.EnableSsl = true;

                    smtpServer.Send(mail);
                };
            };
        }

        //metoda sprawdzająca czy użytkownik istnieje
        public async Task<bool> UserExists(string username)
        {
            if (await _repo.Exists(x => x.Username == username))
                return true;
            return false;
        }

        //metoda logowania
        public async Task<ResultDto<LoginDto>> Login(LoginUserModel loginUserModel)
        {
            var result = new ResultDto<LoginDto>
            {
                Error = null
            };

            //sprawdzam czy użytkownik istnieje
            var user = await _repo.GetSingleEntity(x => x.Username == loginUserModel.Username);

            if (user == null || !VerifyPasswordHash(loginUserModel.Password, user.PasswordHash, user.PasswordSalt))
            {
                result.Error = "Nazwa użytkownika lub hasło niepoprawne";
                return result;
            }

            //sprawdzam czy konto zostało potwierdzone
            if (!user.ConfirmedAccount)
            {
                result.Error = "Konto niezatwierdzone. Sprawdź swój email";
                return result;
            }

            //jesli wszystko ok tworze token i go zwracam
            var token = GenerateToken(user);

            result.SuccessResult = new LoginDto()
            {
                Token = token,
                Id = user.Id
            };

            return result;
        }

        //Metoda pozwalajaca na sprawdzenie poprawnosci hasla poprzez wygenerowanie hashu z pomocą saltu z bazy
        //Jesli nowy hash zgadza sie ze starym z bazy to haslo poprawne
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

        //metoda generujaca token uwierzytelniajacy
        private string GenerateToken(User user)
        {
            var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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

        //metoda zatwierdzająca konto po otrzymaniu wiadomosci email
        public async Task<ResultDto<BaseDto>> ConfirmEmail(string username, string code)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            //sprawdzam czy taki użytkownik istnieje
            var user = await _repo.GetSingleEntity(x => x.Username == username);
            
            if (user == null)
            {
                result.Error = "Podany użytkownik nie został znaleziony";
                return result;
            }

            //zatwierdzam konto
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

        //Metoda pozwalająca na zmiane hasła, wyśle wiadomość email z linkiem resetującym
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

            //tworzę nowy kod weryfikacyjny który posłuży do utworzenia linku resetującego
            user.VerificationCode = Guid.NewGuid();
            var code = user.VerificationCode;

            _repo.Update(user);

            //tworzę wiadomość email z linkiem resetującym
            string subject = "Zmiana hasła konta: " + user.Username;
            string body = "Dzień dobry " + user.Username + "! <br/><br/>" +
                        "<a href='http://localhost:3000/changePassword/" + user.Username + "/" + user.VerificationCode.ToString() + "'>Kliknij w link, aby ustawić nowe hasło. </a>";

            SendEmail(user.Email, subject, body);

            return result;
        }

        //metoda ustawiająca nowe hasło po zresetowaniu starego
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

            //jesli nie ma bledow ustawiam nowe haslo
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

        //logowanie za pomocą facebook
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
                else
                {
                    result.Error = "Błąd uwierzytelniania";
                    return result;
                }
            };

            var epInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseResult);
            //sprawdzam czy logujący się użytkownik już sie logował wczesniej
            var user = await _repo.GetSingleEntity(x => x.LoginProvider == "facebook" && x.ProviderKey == epInfo["id"]);

            //jesli sie nie logowal
            if (user == null)
            {
                //sprawdzam czy jego mail juz jest w bazie
                user = await _repo.GetSingleEntity(x => x.Email == epInfo["email"]);

                //jesli nie tworze nowe konto 
                if (user == null)
                {
                    var username = string.Format("FB{0}{1}",
                          epInfo["id"],
                          Guid.NewGuid().ToString("N")
                      );

                    var password = Guid.NewGuid().ToString();
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

                //jesli juz jest w bazie, dodaje informacje o facebook
                user.LoginProvider = "facebook";
                user.ProviderKey = epInfo["id"];

                _repo.Update(user);
            }

            int num;

            if (user != null)
            {
                num = user.Id;
            }
            else
            {
                var u = _repo.GetSingleEntity(x => x.Email == epInfo["email"]);
                num = u.Id;
            }
            //tworze token jwt
            var token = GenerateToken(user);

            result.SuccessResult = new LoginDto()
            {
                Token = token,
                Id = num    
            };

            return result;
        }

        //metoda zmiany hasla
        public async Task<ResultDto<BaseDto>> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            var result = new ResultDto<BaseDto>()
            {
                Error = null
            };

            var user = await _repo.GetSingleEntity(x => x.Id == changePasswordModel.Id);

            //sprawdzam czy uzytkownik istnieje
            if (user == null)
            {
                result.Error = "Nie znaleziono użytkownika";
                return result;
            }

            //sprawdzam czy stare haslo podane jest poprawne
            if (!VerifyPasswordHash(changePasswordModel.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                result.Error = "Stare hasło jest niepoprawne";
                return result;
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(changePasswordModel.NewPassword, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _repo.Update(user);

            return result;
        }

    }
}
