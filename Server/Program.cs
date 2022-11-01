using Microsoft.EntityFrameworkCore;

using Server.Models;
using Server.Services;

namespace Server
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<PlayerDBContext>(opt => opt.UseInMemoryDatabase("Players"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHostedService<DeleteInactivePlayersService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}