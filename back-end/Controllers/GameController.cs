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
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("addGame/{name}/{description}")]
        public async Task<IActionResult> AddGame(string name, string description, ImageModel imageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var gameModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = imageModel.Image
            };

            var result = await _gameService.AddGame(gameModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("updateGame/{id}/{name}/{description}")]
        public async Task<IActionResult> UpdateGame(int id, string name, string description, ImageModel imageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var gameModel = new GameModel()
            {
                Name = name,
                Description = description,
                Image = imageModel.Image
            };

            var result = await _gameService.UpdateGame(id, gameModel);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("deleteGame/{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _gameService.DeleteGame(id);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("getGame/{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _gameService.GetGame(id);

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("getGames")]
        public async Task<IActionResult> GetGames()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _gameService.GetGames();

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}