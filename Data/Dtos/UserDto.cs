﻿using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class UserDto : BaseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
    }
}
