using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class ListGameDto : BaseDto
    {
        public List<GameDto> List { get; set; }
    }
}
