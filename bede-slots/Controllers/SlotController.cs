using Microsoft.AspNetCore.Mvc;
using bede_slots.ViewModels;
using bede_slots.Services;
using bede_slots.Models;

namespace bede_slots.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class SlotController : Controller
    {

        private readonly IGameService _gameService;
        public SlotController(IGameService gameService)
        {
            _gameService = gameService;
        }    

        [HttpGet("GetBalance")]
        public IActionResult GetBalance()
        {

            var result = _gameService.GetBalance(1);
            return Ok(ResponseModel.SuccessResponse("Balance", result));
        }

        [HttpPost("AddToBalance")]
        public IActionResult AddToBalance(decimal amountToAdd)
        {

            _gameService.Deposit(amountToAdd , 1);
            return Ok(ResponseModel.SuccessResponse("Balance", null));
        }


        [HttpGet("Player")]
        public IActionResult Player()
        {

            var result = _gameService.GetPlayer();
            return Ok(ResponseModel.SuccessResponse("player", result));
        }



        [HttpGet("Spin")]
        public async Task<IActionResult> Spin(decimal stake)
        {
            SpinResultVM spinResult = new();

            spinResult.Gameboard = await _gameService.Spin(stake, 1);
            var coefficents = _gameService.CalcuateWinningRowsCoefficents(spinResult.Gameboard );
            var finalCoefficent = 0.0m;

            foreach(var coefficent in coefficents)
            {
                finalCoefficent += coefficent;
            }
            
            spinResult.Winnings = await _gameService.CalculateWinnings(stake, 1, finalCoefficent);                  
            spinResult.Balance = _gameService.GetBalance(1);

            return Ok(ResponseModel.SuccessResponse("result", spinResult));
        }
            

    }
}
