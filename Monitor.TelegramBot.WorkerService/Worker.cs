using Monitor.Classes;
using Monitor.Classes.Impl;
using Monitor.Common;
using Monitor.Const;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Unity;

namespace Monitor.TelegramBot.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;                
        const string SummaryLogCallbackQuery = "SummaryLog";
        const string DevLogCallbackQuery = "DevLog";
        const string FullLogCallbackQuery = "FullLog";
        [Dependency]
        public TelegramConfig TelegramConfig { get; set; }
        IUnityContainer _cfg;
        [Dependency]
        public ILogHelper? LogHelper { get; set; }
        ITelegramBotClient bot;
        public Worker(ILogger<Worker> logger, IUnityContainer cfg)
        {
            _cfg = cfg;
            _cfg.BuildUp(this);
            _logger = logger;
            if (TelegramConfig == null || TelegramConfig.BotToken.IsNullOrEmpty())
                throw new ApplicationException("Не указан токен бота");
            bot = new TelegramBotClient(TelegramConfig.BotToken);

            _logger.LogInformation("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                
                receiverOptions,
                cancellationToken
            );
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            var message = update.Message;
            if (update.Type == UpdateType.Message)            {
                
                if (message?.Text?.ToLower() == "/start")
                {
                    await HandleStartCommand(message.Chat.Id, cancellationToken);
                    return;
                    
                }              
                //await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                try
                {
                    await HandleCallbackQuery(update.CallbackQuery, cancellationToken);
                    await HandleStartCommand(update.CallbackQuery.From.Id, cancellationToken);

                }
                catch(Exception ex)
                {
                    await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, $"ОЙ: {ex.Message}");
                }

            }

        }

        async Task HandleStartCommand(long chatId, CancellationToken cancellationToken)
        {            
            var ikm = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Summary", SummaryLogCallbackQuery),
                            InlineKeyboardButton.WithCallbackData("Dev", DevLogCallbackQuery),
                            InlineKeyboardButton.WithCallbackData("Full", FullLogCallbackQuery),
                        }
                    });

            await bot.SendTextMessageAsync(chatId, "Что изволите?", replyMarkup: ikm, cancellationToken: cancellationToken);
            return;
        }

        async Task HandleCallbackQuery(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            var jsonLog = await LogHelper?.GetJsonLog(Const.MinerEnum.Whatsminer);
            if (!jsonLog.IsNullOrEmpty())
            {
                //var jw = new JsonWrapper(jsonLog);               
                var msg = callbackQuery.Data switch
                {
                    SummaryLogCallbackQuery => GetLogSummary(jsonLog),
                    DevLogCallbackQuery => GetLogDev(jsonLog),
                    FullLogCallbackQuery => GetFullLog(jsonLog),
                    _ => "Не распознал команду!",
                };
                await bot.SendTextMessageAsync(callbackQuery?.From.Id, msg);
                return;
            }
        }

        string GetLogSummary(string logJson)
        {
            var jw = new JsonWrapper(logJson);
            var sb = new StringBuilder();
            var elased = jw.GetValue("Elapsed");
            var uptime = jw.GetValue("Uptime");
            sb.AppendLine($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}");
            sb.AppendLine($"Elapsed - {TimeSpan.FromSeconds(Convert.ToUInt32(elased)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s")}");
            sb.AppendLine($"Uptime - {TimeSpan.FromSeconds(Convert.ToUInt32(uptime)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s")}");
            foreach (var field in LogerConst.LogSummaryJsonFields.Where(x=>x!= "Elapsed" && x!="Uptime"))
            {
                sb.AppendLine($"{field} - {jw.GetValue(field)}");
            }
            return sb.ToString();
        }

        string GetLogDev(string logJson)
        {
            var jw = new JsonWrapper(logJson);
            var sb = new StringBuilder();
            foreach (var field in LogerConst.LogDevsJsonFields)
            {
                sb.AppendLine($"{field} - {string.Join(" | ", jw.GetValuesFromJArray<string>("details",field))}");
            }
            return sb.ToString();
        }

        string GetFullLog(string logJson)
        {
            var summary = GetLogSummary(logJson);
            var dev = GetLogDev(logJson);
            return $"Summary \r\n{summary} \r\nDev \r\n{dev}";
        }

        string FormatOutputString2(string logJson)
        {
            var jw = new JsonWrapper(logJson);
            var sb = new StringBuilder();
            var elased = jw.GetValue("Elapsed");
            var uptime = jw.GetValue("Uptime");

            sb.AppendLine($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}");
            sb.AppendLine($"Elapsed - {TimeSpan.FromSeconds(Convert.ToUInt32(elased)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s")}");
            sb.AppendLine($"Uptime - {TimeSpan.FromSeconds(Convert.ToUInt32(uptime)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s")}");
            sb.AppendLine($"PowerMode - {jw.GetValue("PowerMode")}");
            sb.AppendLine($"GHSav - {jw.GetValue("AvgHashRate")}");
            sb.AppendLine($"FanSpeedIn - {jw.GetValue("FanSpeedIn")}");
            sb.AppendLine($"FanSpeedOut - {jw.GetValue("FanSpeedOut")}");
            sb.AppendLine($"Power - {jw.GetValue("Power")}");
            sb.AppendLine($"PowerFanSpeed - {jw.GetValue("PowerFanSpeed")}");
            sb.Append($"Temperature - {jw.GetValue("Temperature")}");
            sb.AppendLine("(" + string.Join('|', jw.GetValuesFromJArray<decimal>("details", "Temperature")) + ")");
            return sb.ToString();
        }

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            _logger.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

       
    }
}