using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System.Numerics;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Players : ControllerBase
    {
        private readonly PlayerDBContext _dbContext;

        public Players(PlayerDBContext playerDBContext)
        {
            _dbContext = playerDBContext;
        }

        #region HttpGet
        [HttpGet]
        public async Task<List<Player>> Get()
        {
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Player.ToListAsync();
        }
        #endregion HttpGet

        #region HttpPost
        [HttpPost]
        public async Task<ActionResult> Post(Player player)
        {
            if (! _dbContext.Player.Any(a => a.Name == player.Name))
            {
                player.Invitations = new List<Player>();
                _dbContext.Player.Add(player);
                await _dbContext.SaveChangesAsync();

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
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id)
        {
            if (_dbContext.Player.Any(a => a.Id == id))
            {
                var playerInDb = _dbContext.Player.Where(a => a.Id == id).FirstOrDefault();

                if (playerInDb != null)
                {
                    playerInDb.InactiveCounter = 0;
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            return Conflict("error_resetcounterfailed");
        }

        [HttpPut("invite/{id}")]
        public async Task<ActionResult> Put(int id, int invitingId)
        {
            if (_dbContext.Player.Any(a => a.Id == id))
            {
                var playerInDb = _dbContext.Player.Where(a => a.Id == id).FirstOrDefault();
                if (playerInDb != null)
                {
                    if (_dbContext.Player.Any(a => a.Id == invitingId))
                    {
                        var invitingPlayerInDB = _dbContext.Player.Where(a => a.Id == invitingId).FirstOrDefault();
                        if (invitingPlayerInDB != null)
                        {
                            playerInDb?.Invitations?.Add(invitingPlayerInDB);
                            await _dbContext.SaveChangesAsync();
                            return Ok();
                        }
                    }
                }
            }
            return Conflict("error_resetcounterfailed");
        }
        #endregion HttpPut

        #region HttpDelete
        #endregion HttpDelete
    }
}
