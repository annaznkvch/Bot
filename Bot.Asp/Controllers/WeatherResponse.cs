namespace Bot.Asp.Controllers
{
    public class WeatherResponse
    {
        public Info Main { get; set; }
        public string Name { get; set; }
        public Clouds wind { get; set; }
        public Cloudsperc clouds { get; set; }
    }
}
