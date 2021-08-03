using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fance.TurnoFixer.Models;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Fance.TurnoFixer.TelegramBot
{
    public class TurnoFixerBot : BackgroundService
    {
        private Dictionary<long, ChatStatus> chats;
        private static ITelegramBotClient botClient;
        private static IUpdateHandler updateHandler;
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            chats = new Dictionary<long, ChatStatus>();
            botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"));
            updateHandler = new UpdateHandler();
            
            return botClient.ReceiveAsync(updateHandler, stoppingToken);
        }
        
    }

    class UpdateHandler : IUpdateHandler
    {
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                await botClient.SendTextMessageAsync(message.Chat, "Hola :3");
            }
        }

        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                // await botClient.SendTextMessageAsync(-1, apiRequestException.ToString());
            }
        }

        public UpdateType[]? AllowedUpdates { get; }
    }
}