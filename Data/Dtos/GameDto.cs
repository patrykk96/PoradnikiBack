using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class GameDto : BaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double Rating { get; set; }
        public int GuidesCount { get; set; }
    }
}
