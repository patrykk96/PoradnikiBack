using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ChangePasswordModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
