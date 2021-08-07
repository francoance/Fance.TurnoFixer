using System;
using Fance.TurnoFixer.ImageHandler.Interfaces;
using Fance.TurnoFixer.Storage;
using Fance.TurnoFixer.Storage.Interfaces;
using Fance.TurnoFixer.TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Fance.TurnoFixer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TurnoFixerBot>();
                    services.AddTransient<IUpdateHandler, UpdateHandler>();
                    services.AddTransient<IImageHandler, ImageHandler.ImageHandler>();
                    services.AddTransient<IObjectStorage, OracleObjectStorage>();
                    Console.WriteLine(hostContext.Configuration.GetSection("TelegramBotToken").Value);
                    services.AddTransient<ITelegramBotClient>(_ => new TelegramBotClient(hostContext.Configuration.GetSection("TelegramBotToken").Value));
                });
    }
}