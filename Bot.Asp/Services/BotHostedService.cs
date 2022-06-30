using ConsoleApp3;
using ConsoleApp3.Entities;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Asp.Services
{
    public class BotHostedService : IHostedService
    {
        TelegramBotClient Bot = null;
        List<string> Messages = new List<string>() { "", "", "" };

        private FileStorageCollection<ConsoleApp3.Entities.Location> Locations { get; }
        private FileStorageCollection<Region> Regions { get; }
        private FileStorageCollection<RestPlace> RestPlaces { get; }

        public BotHostedService()
        {

            Locations = new("res/loc.txt", true);
            Regions = new FileStorageCollection<Region>("res/reg.txt", true);
            RestPlaces = new("res/rest.txt", true);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Bot = new TelegramBotClient("5344907666:AAFyxfDx29DJt32zn90EWY_CWkFblVEOKw0");

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            Bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await Bot.GetMeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Bot.CloseAsync();
        }

        protected async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } messageText)
                return;

            var Id = message.Chat.Id;
            var username = message.Chat.Username;

            Console.WriteLine($"Received a '{messageText}' message in chat.");

            if (Messages[0] == "")
            {
                if (messageText == "/bySeazon" || messageText == "/placesToEat" || messageText == "/whatIsTheWeatherToday" || messageText == "/PlacesToWalk" || messageText == "/start" || messageText == "/help" || messageText == "/PlacesForOvernightStay")
                {
                    switch (messageText)
                    {

                        case "/start":
                            {
                                await Bot.SendTextMessageAsync(Id, $"{username}, бот приветствует вас!\nЧто бы получить дополнительную информацию выберите /help\nЧто бы Вы хотели сделать?", replyMarkup: keyboard());

                                break;
                            }
                        case "/help":
                            {
                                await Bot.SendTextMessageAsync(Id, $"Список доступных функций: \n/bySeazon - выбрать локацию по времени года;\n/whatIsTheWeatherToday -просмотр погоды на сегодня;\n/PlacesForOvernightStay - поиск мест для ночевки;\n/PlacesToWalk - поиск мест для прогулки;", replyMarkup: keyboard());

                                break;
                            }
                        case "/bySeazon":
                            {
                                Messages[0] = messageText;

                                await Bot.SendTextMessageAsync(Id, $"Выберите время года\n\n", replyMarkup: keyboardSeazons());

                                break;
                            }
                        case "/whatIsTheWeatherToday":
                            {
                                Messages[0] = messageText;

                                await Bot.SendTextMessageAsync(Id, $"Выберите область\n\n", replyMarkup: keyboardRegions());

                                break;
                            }
                        case "/PlacesToWalk":
                            {
                                Messages[0] = messageText;

                                await Bot.SendTextMessageAsync(Id, $"Выберите область\n\n", replyMarkup: keyboardRegions());

                                break;
                            }
                        case "/PlacesForOvernightStay":
                            {
                                Messages[0] = messageText;

                                await Bot.SendTextMessageAsync(Id, $"Выберите область\n\n", replyMarkup: keyboardRegions());

                                break;
                            }
                    }
                }
            }
            else
            {
                if (Messages[1] == "" && Messages[1] != Messages[0])
                    Messages[1] = messageText;
            }


            if (Messages[0].Contains("/PlacesToWalk") || Messages[0].Contains("/whatIsTheWeatherToday") || Messages[0].Contains("/bySeazon") || Messages[0].Contains("/PlacesForOvernightStay"))
            {

                if (Messages[0] != Messages[1] && Messages[1] != "")
                {
                    var action = Messages[0].Trim();
                    var change = Messages[1];

                    var processor = new BotActionProcessor(Bot);

                    switch (action)
                    {
                        case "/PlacesToWalk":
                            await processor.PlacesToWalk(Id, change);
                            break;
                        case "/whatIsTheWeatherToday":
                            await processor.WeatherToday(Id, change);
                            break;
                        case "/PlacesForOvernightStay":
                          await processor.PlacesForOvernightStay(Id, change);
                            break;
                        case "/bySeazon":
                            await processor.BySeazon(Id, change);
                            break;

                        default:
                            throw new NotImplementedException();
                    }


                    await Bot.SendTextMessageAsync(Id, $"Что бы вы хотели сделать еще?", replyMarkup: keyboard());
                    Messages[0] = "";
                    Messages[1] = "";

                    return;
                }


            }
        }

        protected Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        ReplyKeyboardMarkup keyboard()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
               new[]

               {
                    new[] { new KeyboardButton("/bySeazon"),new KeyboardButton("/PlacesForOvernightStay") },

                    new[] { new KeyboardButton("/whatIsTheWeatherToday"),new KeyboardButton("/PlacesToWalk")},
               });
            replyKeyboard.ResizeKeyboard = true;

            return replyKeyboard;
        }

        ReplyKeyboardMarkup keyboardSeazons()
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
               new[]

               {
                    new[] { new KeyboardButton("Autamn"),new KeyboardButton("Winter") },

                    new[] { new KeyboardButton("Spring"),new KeyboardButton("Summer") },
               });
            replyKeyboard.ResizeKeyboard = true;

            return replyKeyboard;
        }

        ReplyKeyboardMarkup keyboardRegions()
        {

            var i = 0;
            var buttons = Regions.Select(x => new KeyboardButton(x.Name)).GroupBy(x => i++ % 2 == 0);

            var replyKeyboard = new ReplyKeyboardMarkup(buttons);
            replyKeyboard.ResizeKeyboard = true;

            return replyKeyboard;
        }

    }
}
