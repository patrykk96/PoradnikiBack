using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ImageModel
    {
        public IFormFile Image { get; set; }
    }
}
