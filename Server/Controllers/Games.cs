using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Games : ControllerBase
    {
        private readonly ChessDBContext _playerDBContext;

        public Games(ChessDBContext playerDBContext)
        {
            _playerDBContext = playerDBContext;
        }

        [HttpGet("{invitingId}")]
        public async Task<ActionResult> GetGame(int invitingId)
        {
            Game? game = await _playerDBContext.Games.Where(a => a.BlackId == invitingId).FirstOrDefaultAsync();
            if (game != null)
            {
                return Ok(game);
            }

            return NotFound("error_nogamefound");
        }

        [HttpGet("current/{currentGameId}")]
        public async Task<ActionResult> GetCurrentGame(int currentGameId)
        {
            Game? game = await _playerDBContext.Games.Where(a => a.Id == currentGameId).FirstOrDefaultAsync();
            if (game != null)
            {
                return Ok(game);
            }

            return NotFound("error_nogamefound");
        }

        [HttpPost]
        public async Task<ActionResult> PostNewGame(Game newGame)
        {
            Player? whiteId = await _playerDBContext.Players.Where(a => a.Id == newGame.WhiteId).FirstOrDefaultAsync();
            if (whiteId != null)
            {
                Player? blackId = await _playerDBContext.Players.Where(a => a.Id == newGame.BlackId).FirstOrDefaultAsync();
                if (blackId != null)
                {
                    Game? gameInDbCheck = await _playerDBContext.Games.Where(a => a.WhiteId == newGame.WhiteId).Where(a => a.BlackId == newGame.BlackId).FirstOrDefaultAsync();

                    if (gameInDbCheck == null)
                    {
                        await _playerDBContext.Games.AddAsync(newGame);
                        await _playerDBContext.SaveChangesAsync();

                        Game? newGameInDb = await _playerDBContext.Games.Where(a => a.WhiteId == newGame.WhiteId).Where(a => a.BlackId == newGame.BlackId).FirstOrDefaultAsync();

                        return Ok(newGameInDb);
                    }
                }
            }

            return NotFound("error_newgamefailed");
        }

        [HttpPut("current/{currentGameId}")]
        public async Task<ActionResult> PutCurrentGame(int currentGameId, Game currentGame)
        {
            Game? gameInDb = await _playerDBContext.Games.Where(a => a.Id == currentGameId).FirstOrDefaultAsync();
            if (gameInDb != null)
            {
                gameInDb.LastMoveStartWhite = currentGame.LastMoveStartWhite;
                gameInDb.LastMoveEndWhite = currentGame.LastMoveEndWhite;
                gameInDb.LastMoveStartBlack = currentGame.LastMoveStartBlack;
                gameInDb.LastMoveEndBlack = currentGame.LastMoveEndBlack;
                gameInDb.MoveInfo = currentGame.MoveInfo;

                await _playerDBContext.SaveChangesAsync();

                return Ok();
            }

            return NotFound("error_gamenotfound");
        }

        [HttpPut("current/counter/white/{currentGameId}")]
        public async Task<ActionResult> PutResetWhiteInactivityCounter(int currentGameId)
        {
            Game? gameInDb = await _playerDBContext.Games.Where(a => a.Id == currentGameId).FirstOrDefaultAsync();
            if (gameInDb != null)
            {
                gameInDb.WhiteInactivityCounter = 0;

                await _playerDBContext.SaveChangesAsync();

                return Ok();
            }

            return NotFound("error_gamenotfound");
        }

        [HttpPut("current/counter/black/{currentGameId}")]
        public async Task<ActionResult> PutResetBlackInactivityCounter(int currentGameId)
        {
            Game? gameInDb = await _playerDBContext.Games.Where(a => a.Id == currentGameId).FirstOrDefaultAsync();
            if (gameInDb != null)
            {
                gameInDb.BlackInactivityCounter = 0;

                await _playerDBContext.SaveChangesAsync();

                return Ok();
            }

            return NotFound("error_gamenotfound");
        }
    }
}
