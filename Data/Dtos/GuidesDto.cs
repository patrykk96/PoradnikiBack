using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Dtos
{
    public class GuidesDto : BaseDto
    {
        public List<GuideDto> Guides { get; set; }
    }
}
