using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DbModels
{
    public class Guide : Entity
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public double Rating { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public int GameId { get; set; }
        public virtual Game Game { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
