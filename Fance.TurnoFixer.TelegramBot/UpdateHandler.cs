#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fance.TurnoFixer.ImageHandler.Interfaces;
using Fance.TurnoFixer.Models;
using Fance.TurnoFixer.Storage;
using Fance.TurnoFixer.Storage.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Fance.TurnoFixer.TelegramBot
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly Dictionary<long, ChatStatus> _chats;
        private readonly OracleObjectStorage _storage;
        private readonly IImageHandler _imageHandler;
        public UpdateType[]? AllowedUpdates { get; }

        public UpdateHandler(IImageHandler imageHandler, IObjectStorage objectStorage)
        {
            _imageHandler = imageHandler;
            _chats = new Dictionary<long, ChatStatus>();
            _storage = new OracleObjectStorage();
            AllowedUpdates = Array.Empty<UpdateType>();
        }
        
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                var currentChat = _chats.GetValueOrDefault(message.Chat.Id);
                if (currentChat is null)
                {
                    currentChat = new ChatStatus(message.Chat.Id);
                    _chats.Add(message.Chat.Id, currentChat);
                }
                
                if (message.Photo != null)
                {
                    await using MemoryStream ms = new();
                    var fileInfo = await botClient.GetInfoAndDownloadFileAsync(message.Photo.OrderByDescending(x => x.Height).First().FileId, ms, cancellationToken);
                    var fileName = $"{message.Chat.Id}_{message.Chat.Username}_{fileInfo.FilePath.Split('/').Last()}";
                    await _storage.PutObjectAsync(ms.ToArray(), fileName);
                    
                    if (string.IsNullOrEmpty(currentChat.FirstImageName))
                    {
                        currentChat.FirstImageName = fileName;
                        await botClient.SendTextMessageAsync(message.Chat, "Ya me bajé la primera foto", cancellationToken: cancellationToken);
                    }
                    else
                    {
                        currentChat.SecondImageName = fileName;
                        await botClient.SendTextMessageAsync(message.Chat, "Ya me bajé la segunda foto, ahí te preparo el mix", cancellationToken: cancellationToken);
                        var newFileName = await _imageHandler.ProcessImagesAsync(currentChat.FirstImageName,
                            currentChat.SecondImageName);

                        await botClient.SendPhotoAsync(message.Chat, photo: await _storage.GetObjectLocationAsync(newFileName));
                        await botClient.SendTextMessageAsync(message.Chat, "Chau :3", cancellationToken: cancellationToken);
                        _chats.Remove(message.Chat.Id);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Hola :3", cancellationToken: cancellationToken);
                }
            }
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                // await botClient.SendTextMessageAsync(-1, apiRequestException.ToString());
            }

            return Task.CompletedTask;
        }
    }
}