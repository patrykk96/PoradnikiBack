using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DbModels
{
    public class User : Entity
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Guid VerificationCode { get; set; }
        public bool ConfirmedAccount { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
