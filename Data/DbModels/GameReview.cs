﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DbModels
{
    public class GameReview : Entity
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}
