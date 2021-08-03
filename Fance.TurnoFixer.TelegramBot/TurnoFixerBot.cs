using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Fance.TurnoFixer.TelegramBot
{
    public class TurnoFixerBot : BackgroundService
    {
        private static ITelegramBotClient _botClient;
        private static IUpdateHandler _updateHandler;

        public TurnoFixerBot(ITelegramBotClient botClient, IUpdateHandler updateHandler)
        {
            _botClient = botClient;
            _updateHandler = updateHandler;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _botClient.ReceiveAsync(_updateHandler, stoppingToken);
        }
    }
}