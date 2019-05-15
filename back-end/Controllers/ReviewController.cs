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
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddReview(ReviewModel reviewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _reviewService.AddReview(reviewModel);

            if (result.Error == null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}