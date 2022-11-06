using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Invitations : ControllerBase
    {
        private readonly ChessDBContext _playerDBContext;

        public Invitations(ChessDBContext playerDBContext)
        {
            _playerDBContext = playerDBContext;
        }

        [HttpGet("{playerId}")]
        public async Task<List<Player>> GetInvitations(int playerId)
        {
            var invitations = await _playerDBContext.Invitations.Where(a => a.PlayerId == playerId).ToListAsync();

            List<Player> returnList = new();

            foreach (var inv in invitations)
            {
                var p = await _playerDBContext.Players.Where(a => a.Id == inv.InvitingPlayerId).FirstOrDefaultAsync();
                returnList.Add(new Player(inv.InvitingPlayerId, p!.Name!));
            }
            return returnList;
        }

        [HttpPost("{invitedId}")]
        public async Task<ActionResult> PostInvitePlayer(int invitedId, Player invitingPlayer)
        {
            Player? playerInDb = await _playerDBContext.Players.Where(a => a.Id == invitedId).FirstOrDefaultAsync();
            if (playerInDb != null)
            {
                Player? invitingPlayerInDB = await _playerDBContext.Players.Where(a => a.Id == invitingPlayer.Id).FirstOrDefaultAsync();
                if (invitingPlayerInDB != null)
                {
                    Invitation? invitation = await _playerDBContext.Invitations.Where(a => a.PlayerId == invitedId).Where(a => a.InvitingPlayerId == invitingPlayer.Id).FirstOrDefaultAsync();

                    if (invitation == null)
                    {
                        Invitation newInvitingPlayer = new()
                        {
                            PlayerId = invitedId,
                            InvitingPlayerId = invitingPlayer.Id,
                        };

                        await _playerDBContext.Invitations.AddAsync(newInvitingPlayer);
                        await _playerDBContext.SaveChangesAsync();
                        return Ok(playerInDb);
                    }
                }
            }

            await _playerDBContext.SaveChangesAsync();
            return NotFound("error_invitationfailed");
        }

        [HttpPut("cancel/{invitedId}")]
        public async Task<ActionResult> PutCancelInvitation(int invitedId, Player invitingPlayer)
        {
            var playerInDb = await _playerDBContext.Players.Where(a => a.Id == invitedId).FirstOrDefaultAsync();

            if (playerInDb != null)
            {
                _playerDBContext.Invitations.RemoveRange(
                    _playerDBContext.Invitations!.Where(a => a.PlayerId == playerInDb.Id).Where(a => a.InvitingPlayerId == invitingPlayer.Id));
                await _playerDBContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound("error_cancelinvitationfailed");
        }
    }
}
