using ConsoleApp3;
using ConsoleApp3.Entities;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Asp.Services
{
    public class BotActionProcessor
    {
        private TelegramBotClient Bot { get; }

        private FileStorageCollection<ConsoleApp3.Entities.Location> Locations { get; }
        private FileStorageCollection<Region> Regions { get; }
        private FileStorageCollection<RestPlace> RestPlaces { get; }


        public BotActionProcessor(TelegramBotClient bot)
        {
            Bot = bot;

            Locations = new("res/loc.txt", true);
            Regions = new FileStorageCollection<Region>("res/reg.txt", true);
            RestPlaces = new ("res/rest.txt", true);
        }

        public async Task PlacesToWalk(ChatId id, string change)
        {
            var region = Regions.FirstOrDefault(x => x.Name == change);

            await Bot.SendTextMessageAsync(id, region.Cooordinates);
        }

        public async Task PlacesForOvernightStay(ChatId id, string change)
        {
            var restPlace = RestPlaces.FirstOrDefault(x => x.Region == change);
            if (restPlace.Name == "string")
            {
                await Bot.SendTextMessageAsync(id, "Мы не смогли найти подходящее место ночлега для выбраной локации");
                return;
            }
            else
            {
                await Bot.SendTextMessageAsync(id, restPlace.Name + "\n--------------\n" + restPlace.Cooordinates);
                await Bot.SendPhotoAsync(id, restPlace.PhotoPath);
            }
           
        }

        public async Task WeatherToday(ChatId id, string change)
        {
            try
            {
                await Bot.SendTextMessageAsync(id, Get($"https://localhost:7080/{change}"));
            }
            catch { await Bot.SendTextMessageAsync(id, $"Some troubles with response"); }

        }

        public async Task BySeazon(ChatId id, string seazon)
        {
            DateTime date = DateTime.UtcNow;

            switch (seazon)
            {
                case "Winter":
                    date = new DateTime(2022, 1, 1);
                    break;

                case "Autamn":
                    date = new DateTime(2022, 9, 1);
                    break;

                case "Spring":
                    date = new DateTime(2022, 3, 1);
                    break;

                case "Summer":
                    date = new DateTime(2022, 6, 1);
                    break;
            }

            var locations = Locations.Where(x => IsOnTime(x, date));

            await SendLocations(id, locations.ToArray());
        }

        private async Task SendLocations(ChatId chatId, ConsoleApp3.Entities.Location[] locations)
        {

            foreach (var location in locations)
            {
                await Bot.SendTextMessageAsync(chatId, location.Name + "\n--------------\n" + location.Description);
                await Bot.SendPhotoAsync(chatId, location.PhotoPath);
                await Bot.SendTextMessageAsync(chatId, location.Cooordinates);
            }
        }

        private async Task SendLocations(ChatId chatId, string name, string coordinates)
        {

           
                await Bot.SendTextMessageAsync(chatId, name + "\n--------------\n" + coordinates);
        }

        private bool IsOnTime(ConsoleApp3.Entities.Location location, DateTime date = default)
        {
            if (date == default)
            {
                date = DateTime.Now;
            }

            var month = date.Month;

            switch (month)
            {
                case 1:
                case 2:
                case 12:
                    return location.OnWinter;
                case 3:
                case 4:
                case 5:
                    return location.OnSpring;
                    break;
                case 6:
                case 7:
                case 8:
                    return location.OnSummer;
                    break;
                case 9:
                case 10:
                case 11:
                    return location.OnAutmn;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        public static string Get(string A)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(A);

            request.Method = "GET";

            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader reader = new StreamReader(s);

            string answer = reader.ReadToEnd();

            response.Close();

            return answer;
        }

    }
}
