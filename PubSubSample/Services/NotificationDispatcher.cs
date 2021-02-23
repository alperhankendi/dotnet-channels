using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PubSubSample.Services.Data;

namespace PubSubSample.Services
{
    public class NotificationDispatcher : BackgroundService
    {
        private readonly Channel<string> channel;
        private readonly ILogger<NotificationDispatcher> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServiceProvider provider;

        public NotificationDispatcher(
            Channel<string> channel,
            ILogger<NotificationDispatcher> logger,
            IHttpClientFactory httpClientFactory,
            IServiceProvider provider)
        {
            this.channel = channel;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!channel.Reader.Completion.IsCompleted) 
            {
                var msg = await channel.Reader.ReadAsync(stoppingToken);
                try
                {
                    using (var scope = provider.CreateScope())
                    {
                        var database = scope.ServiceProvider.GetRequiredService<Database>();
                        if (!await database.Users.AnyAsync(cancellationToken: stoppingToken))
                        {
                            await database.Users.AddAsync(new User(), stoppingToken);
                            await database.SaveChangesAsync(stoppingToken);
                        }

                        var user = await database.Users.FirstOrDefaultAsync(cancellationToken: stoppingToken);

                        var client = httpClientFactory.CreateClient();
                        var response = await client.GetStringAsync("https://docs.microsoft.com/en-us/dotnet/core/", stoppingToken);
                        user.Message = response;

                        await database.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "notification failed");
                }
            }
        }
    }
}
