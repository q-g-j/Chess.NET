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
        private readonly PlayerDBContext _playerDBContext;

        public Players(PlayerDBContext playerDBContext)
        {
            _playerDBContext = playerDBContext;
        }

        #region HttpGet
        [HttpGet]
        public async Task<List<Player>> GetAllPlayers()
        {
            return await _playerDBContext.Player.ToListAsync(); ;
        }

        [HttpGet("{id}")]
        public List<Player> GetInvitations(int id)
        {
            var invitations = _playerDBContext.Invitations.Where(a => a.PlayerId == id).ToList();

            List<Player> returnList = new List<Player>();

            foreach (var inv in invitations)
            {
                var p = _playerDBContext.Player.Where(a => a.Id == inv.InvitingPlayerId).FirstOrDefault();
                returnList.Add(new Player(inv.InvitingPlayerId, p!.Name!));
            }
            return returnList;
        }
        #endregion HttpGet

        #region HttpPost
        [HttpPost]
        public async Task<ActionResult> PostNewPlayer(Player player)
        {
            if (! _playerDBContext.Player.Any(a => a.Name == player.Name))
            {
                _playerDBContext.Player.Add(player);
                await _playerDBContext.SaveChangesAsync();

                var playerInDb = _playerDBContext.Player.Where(a => a.Name == player.Name).FirstOrDefault();

                if (playerInDb != null)
                {
                    return Ok(playerInDb);
                }
            }
            return Conflict("error_nameconflict");
        }

        [HttpPost("invite/{id}")]
        public async Task<ActionResult> PostInvitePlayer(int id, Player invitingPlayer)
        {
            Player? playerInDb = _playerDBContext.Player.Where(a => a.Id == id).FirstOrDefault();
            if (playerInDb != null)
            {
                Player? invitingPlayerInDB = _playerDBContext.Player.Where(a => a.Id == invitingPlayer.Id).FirstOrDefault();
                if (invitingPlayerInDB != null)
                {
                    Invitation? invitation = _playerDBContext.Invitations.Where(a => a.PlayerId == id).Where(a => a.InvitingPlayerId == invitingPlayer.Id).FirstOrDefault();
                    
                    if (invitation == null)
                    {
                        Invitation newInvitingPlayer = new()
                        {
                            PlayerId = id,
                            InvitingPlayerId = invitingPlayer.Id,
                        };

                        _playerDBContext.Invitations.Add(newInvitingPlayer);
                        await _playerDBContext.SaveChangesAsync();
                        return Ok("success_invite");
                    }
                }
            }

            await _playerDBContext.SaveChangesAsync();
            return Conflict("error_invite");
        }
        #endregion HttpPost

        #region HttpPut
        [HttpPut("{id}")]
        public async Task<ActionResult> PutResetInactiveCounter(int id)
        {
            var playerInDb = _playerDBContext.Player.Where(a => a.Id == id).FirstOrDefault();

            if (playerInDb != null)
            {
                playerInDb.InactiveCounter = 0;
                await _playerDBContext.SaveChangesAsync();
                return Ok();
            }

            return Conflict("error_resetcounterfailed");
        }
        #endregion HttpPut

        #region HttpDelete
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            var playerInDb = _playerDBContext.Player.Where(a => a.Id == id).FirstOrDefault();

            if (playerInDb != null)
            {
                _playerDBContext.Invitations.RemoveRange(_playerDBContext.Invitations.Where(a => a.PlayerId == playerInDb.Id));
                _playerDBContext.Player.Remove(playerInDb);
                await _playerDBContext.SaveChangesAsync();
                return Ok();
            }

            return Conflict("error_resetcounterfailed");
        }
        #endregion HttpDelete
    }
}
