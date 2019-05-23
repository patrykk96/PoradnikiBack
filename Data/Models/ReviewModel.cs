using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ReviewModel
    {
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
    }
}
