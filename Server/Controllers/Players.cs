using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Server.Models;
using System.Linq;
using System.Numerics;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Players : ControllerBase
    {
        private readonly ChessDBContext _playerDBContext;

        public Players(ChessDBContext playerDBContext)
        {
            _playerDBContext = playerDBContext;
        }

        [HttpGet]
        public async Task<List<Player>> GetAllPlayers()
        {
            return await _playerDBContext.Players.ToListAsync(); ;
        }

        [HttpPost]
        public async Task<ActionResult> PostNewPlayer(Player player)
        {
            if (! _playerDBContext.Players.Any(a => a.Name == player.Name))
            {
                await _playerDBContext.Players.AddAsync(player);
                await _playerDBContext.SaveChangesAsync();

                var playerInDb = await _playerDBContext.Players.Where(a => a.Name == player.Name).FirstOrDefaultAsync();

                if (playerInDb != null)
                {
                    return Ok(playerInDb);
                }
            }
            return Conflict("error_nameconflict");
        }

        [HttpPut("{playerId}")]
        public async Task<ActionResult> PutResetInactiveCounter(int playerId)
        {
            var playerInDb = await _playerDBContext.Players.Where(a => a.Id == playerId).FirstOrDefaultAsync();

            if (playerInDb != null)
            {
                playerInDb.InactiveCounter = 0;
                await _playerDBContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound("error_resetcounterfailed");
        }

        [HttpDelete("{playerId}")]
        public async Task<ActionResult> DeletePlayer(int playerId)
        {
            var playerInDb = _playerDBContext.Players.Where(a => a.Id == playerId).FirstOrDefault();

            if (playerInDb != null)
            {
                _playerDBContext.Invitations.RemoveRange(_playerDBContext.Invitations.Where(a => a.PlayerId == playerInDb.Id));
                _playerDBContext.Players.Remove(playerInDb);
                await _playerDBContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound("error_resetcounterfailed");
        }
    }
}
