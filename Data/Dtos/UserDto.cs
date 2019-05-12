using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class UserDto : BaseDto
    {
       public UserModel User { get; set; }
    }
}
