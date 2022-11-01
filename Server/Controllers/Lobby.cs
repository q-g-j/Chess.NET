using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Numerics;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Lobby : ControllerBase
    {
        private readonly PlayerDBContext _dbContext;

        public Lobby(PlayerDBContext playerDBContext)
        {
            _dbContext = playerDBContext;
        }

        #region HttpGet
        [HttpGet(Name = "lobby")]
        public async Task<List<Player>> Get()
        {
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Player.ToListAsync();
        }
        #endregion HttpGet

        #region HttpPost
        [HttpPost(Name = "lobby")]
        public async Task<ActionResult> Post(Player player)
        {
            var playerInDbQueryResult = _dbContext.Player.Where(a => a.Name == player.Name);

            if (!playerInDbQueryResult.Any())
            {
                player.InactiveCounter = 0;
                _dbContext.Player.Add(player);
                await _dbContext.SaveChangesAsync();

                Globals.InvitationsDict[player.Id] = new List<int>();

                var playerInDb = _dbContext.Player.Where(a => a.Name == player.Name).FirstOrDefault();

                if (playerInDb != null)
                {
                    return Ok(playerInDb);
                }
            }
            return Conflict("error_nameconflict");
        }
        #endregion HttpPost

        #region HttpPut
        [HttpPut(Name = "lobby")]
        public async Task<ActionResult> Put(Player player)
        {
            var playerInDbQueryResult = _dbContext.Player.Where(a => a.Name == player.Name);

            if (playerInDbQueryResult.Any())
            {
                var playerInDb = _dbContext.Player.Where(a => a.Name == player.Name).FirstOrDefault();

                if (playerInDb != null)
                {
                    playerInDb.InactiveCounter = 0;
                    await _dbContext.SaveChangesAsync();
                    return Ok(player);
                }
            }
            return Conflict("error_resetcounterfailed");
        }
        #endregion HttpPut

        #region HttpDelete
        #endregion HttpDelete
    }
}
