using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DbModels
{
    public class GuideReview : Entity
    {
        public int GuideId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }

        public virtual Guide Guide { get; set; }
        public virtual User User { get; set; }
    }
}
