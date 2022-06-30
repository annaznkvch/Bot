using ConsoleApp3;
using ConsoleApp3.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Bot.Asp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PeriodController : ControllerBase
    {
        [HttpGet("/{region}")] //https://localhost:64133/bySeazon/Autamn
        public string GetInfo(string region)
        {
            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.openweathermap.org/data/2.5/weather?q={region}&units=metric&appid=2eddc90d57e02e513c03971871742822");

            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader reader = new StreamReader(s);

            string answer = reader.ReadToEnd();

            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(answer);

            return $"Погода в {region} сегодня:\n- показалети температуры:\n   min: {weatherResponse.Main.temp_min} °c\n   max: {weatherResponse.Main.temp_max} °c\n   ощущается как: {weatherResponse.Main.feels_like} °c\n- скорость ветра: {weatherResponse.wind.speed} м/с \n- тучность: {weatherResponse.clouds.all} %";

            //return "here is sime info about your request :)" + region;
        }
    }

}
