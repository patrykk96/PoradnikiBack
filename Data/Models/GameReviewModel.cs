using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class GameReviewModel
    {
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
    }
}
