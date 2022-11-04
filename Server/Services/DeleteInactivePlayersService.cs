using Server.Models;

namespace Server.Services
{
    public class DeleteInactivePlayersService : IHostedService, IDisposable
    {
        private readonly ILogger<DeleteInactivePlayersService> _logger;
        private Timer? _timer = null;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeleteInactivePlayersService(ILogger<DeleteInactivePlayersService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var playerContext = scope.ServiceProvider.GetRequiredService<PlayerDBContext>();

                if (playerContext != null)
                {
                    try
                    {
                        foreach (var player in playerContext.Player)
                        {
                            player.InactiveCounter += 1;
                            if (player.InactiveCounter == 5)
                            {
                                playerContext.Player.Remove(player);

                                playerContext.Invitations.RemoveRange(playerContext.Invitations.Where(a => a.PlayerId == player.Id));
                            }
                        }
                        await playerContext.SaveChangesAsync();
                    }
                    catch
                    {
                        ;
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
