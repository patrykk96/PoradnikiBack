using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GuideController : Controller
    {
        private readonly IGuideService _guideService;

        public GuideController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        [HttpPost("addGuide")]
        public async Task<IActionResult> AddGuide(GuideModel guideModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _guideService.AddGuide(guideModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("updateGuide/{id}")]
        public async Task<IActionResult> UpdateGuide(int id, GuideModel guideModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _guideService.UpdateGuide(id, guideModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}