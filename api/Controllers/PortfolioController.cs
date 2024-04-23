using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository stockRepository,
            IPortfolioRepository portfolioRepository
        )
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            if (username == null)
            {
                return StatusCode(500, "Failed to get username");
            }
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return NotFound("Cannot find user");
            }
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            if (username == null)
            {
                return StatusCode(500, "Failed to get username");
            }
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return StatusCode(500, "Cannot find user");
            }
            var stock = await _stockRepository.GetBySymbolAsync(symbol);
            if (stock == null)
            {
                return NotFound("Stock not found");
            }
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            if (
                userPortfolio.Any(
                    x => x.Symbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                return BadRequest("Cannot add same stock to portfolio");
            }
            var portfolioModel = new Portfolio { StockId = stock.Id, AppUserId = appUser.Id };
            var createResult = await _portfolioRepository.CreateAsync(portfolioModel);
            if (createResult == null)
            {
                return StatusCode(500, "Could not create portfolio");
            }
            return Created();
        }
    }
}
