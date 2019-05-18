using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class GuideDto : BaseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string GameName { get; set; }
        public string GameImage { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
