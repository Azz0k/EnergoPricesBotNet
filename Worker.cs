using EnergoPricesBotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PricesBotWorkerService
{
    public sealed class WindowsBackgroundService: BackgroundService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private readonly BotService _botService;
        private  ITelegramBotClient? _botClient;
        private ReceiverOptions? _receiverOptions;
        private byte[] _dummyBuffer;

        public WindowsBackgroundService(IOptions<AppSettings> settings, ILogger<WindowsBackgroundService> logger, BotService botService) 
        {
            _settings = settings.Value;
            _logger = logger;
            _botService = botService;
            _dummyBuffer = File.ReadAllBytes("DummyPrice.xlsx");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogCritical("Service started");
                _botClient = new TelegramBotClient(_settings.TelegramBotToken);
                _receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new[]
                    {
                            UpdateType.Message,
                            UpdateType.CallbackQuery,
                    },
                    //ThrowPendingUpdates = true,
                };
                _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, _receiverOptions, stoppingToken);
                int timer = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                    if (++timer == 60*60) 
                    {
                        timer = 0;
                    }

                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
        }
        private async Task BotMessageHandler(Telegram.Bot.Types.Message message)
        {
            var user = message?.From;
            var chat = message?.Chat;
            if (message.Type == MessageType.Text)
            {
                var text = message.Text.Trim();
                if (text == "/start")
                {
                    await _botClient.SendTextMessageAsync(user.Id, _settings.FirstGreetingText);
                }
                string brand = "";
                if (_botService.IsCatalogMenuButtonPressed(text))
                {
                    brand = _botService.GetBrandIfCatalogMenuButtonPressed(text);
                    await _botClient.SendDocument(user.Id, "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf", "Каталог " + brand);
                }
                else if (_botService.IsPriceMenuButtonPressed(text))
                {
                    brand = _botService.GetBrandIfPriceMenuButtonPressed(text); 
                    await using var ms = new MemoryStream(_dummyBuffer);
                    await _botClient.SendDocument(user.Id, InputFile.FromStream(ms, "dummy.xlsx"), "Прайс-лист "+brand);
                }
                BotResponse br = _botService.GenerateResponse(text);
                await _botClient.SendTextMessageAsync(user.Id, br.messageText, replyMarkup: br.replayMarkup);

            }
            
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await BotMessageHandler(update.Message);
                    break;
                }
                case UpdateType.CallbackQuery:
                {
 //                   await BotCallBackHandler(update.CallbackQuery);
                    break;
                }
            }
            
        }

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException are => $"Telegram API Error:\n[{are.ErrorCode}]\n{are.Message}",
                _ => exception.ToString()
            };
            _logger.LogError(errorMessage);
            await Task.CompletedTask;
        }
    }
}

