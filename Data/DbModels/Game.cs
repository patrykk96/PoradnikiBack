using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DbModels
{
    public class Game : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Rating { get; set; }

        public virtual ICollection<Guide> Guides { get; set; }
        public virtual ICollection<GameReview> GameReviews { get; set; }
    }
}
