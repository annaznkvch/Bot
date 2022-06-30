using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

var Bot = new TelegramBotClient("5344907666:AAFyxfDx29DJt32zn90EWY_CWkFblVEOKw0");
List<string> Messages = new List<string>() { "", "", "" };

static List<string> ClearMessages(List<string> Messages)
{
    Messages.Clear();

    Messages.Add("");

    Messages.Add("");

    Messages.Add("");

    return Messages;
}

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

//Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

static ReplyKeyboardMarkup keyboard()
{
    var replyKeyboard = new ReplyKeyboardMarkup(
       new[]

       {
                    new[] { new KeyboardButton("/help"),new KeyboardButton("/bySeazon") },

                    new[] { new KeyboardButton("/placesForOvernightStay"),new KeyboardButton("/PlacesToWalk") },
       });
    replyKeyboard.ResizeKeyboard = true;

    return replyKeyboard;
}

static ReplyKeyboardMarkup keyboardSeazons()
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

static ReplyKeyboardMarkup keyboardRegions()
{
    var replyKeyboard = new ReplyKeyboardMarkup(
       new[]

       {
                    new[] { new KeyboardButton("Vinnytsia"),new KeyboardButton("Volyn") },

                    new[] { new KeyboardButton("Dnipropetrovsk"),new KeyboardButton("Donetsk") },

                     new[] { new KeyboardButton("Zhytomyr"),new KeyboardButton("Transcarpathian") },

                    new[] { new KeyboardButton("Zaporizhzhia"),new KeyboardButton("Ivano-Frankivsk") },

                     new[] { new KeyboardButton("Kyiv"),new KeyboardButton("Kirovohrad") },

                    new[] { new KeyboardButton("Crimea"),new KeyboardButton("Luhansk") },

                     new[] { new KeyboardButton("Lviv"),new KeyboardButton("Mykolayiv") },

                    new[] { new KeyboardButton("Odesa"),new KeyboardButton("Poltava") },

                     new[] { new KeyboardButton("Rivne"),new KeyboardButton("Sumy") },

                    new[] { new KeyboardButton("Ternopil"),new KeyboardButton("Kharkiv") },

                     new[] { new KeyboardButton("Kherson"),new KeyboardButton("Khmelnytsky") },

                    new[] { new KeyboardButton("Cherkasy"),new KeyboardButton("Chernivtsi") },

                     new [] {new KeyboardButton("Chernihiv") },
       });
    replyKeyboard.ResizeKeyboard = true;

    return replyKeyboard;
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
        if (messageText == "/bySeazon" || messageText == "/placesToEat" || messageText == "/placesForOvernightStay" || messageText == "/PlacesToWalk" || messageText == "/start" || messageText == "/help")
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
                        await Bot.SendTextMessageAsync(Id, $"Список доступных функций: \n/bySeazon - выбрать локацию по времени года;\n/placesForOvernightStay - поиск мест для ночевки;\n/PlacesToWalk - поискм мест для ночевки;", replyMarkup: keyboard());

                        break;
                    }
                case "/bySeazon":
                    {
                        Messages[0] = messageText;

                        await Bot.SendTextMessageAsync(Id, $"Выберите время года\n\n", replyMarkup: keyboardSeazons());

                        break;
                    }
                case "/placesForOvernightStay":
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
            }

        }
    }
    else
    {
        if (Messages[1] == "" && Messages[1]!= Messages[0])
            Messages[1] = messageText;
    }
    

    if (Messages[0].Contains("/PlacesToWalk") || Messages[0].Contains("/placesForOvernightStay") || Messages[0].Contains("/placesToEat") || Messages[0].Contains("/bySeazon"))
    {
       
        if (Messages[0] != Messages[1] && Messages[1] != "")
        {
            var action = Messages[0];
            var change = Messages[1];
            await Bot.SendTextMessageAsync(Id, $"Reference to API your action {action} your change {change}");
            await Bot.SendTextMessageAsync(Id, $"Что бы вы хотели сделать еще?", replyMarkup: keyboard());
            Messages[0] = "";
            Messages[1] = "";

            return;
        }


    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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