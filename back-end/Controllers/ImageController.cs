using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetImage(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                return BadRequest();
            }

            var image = await _imageService.GetImage(filename);

            return File(image, "image/jpeg");
        }
    }
}