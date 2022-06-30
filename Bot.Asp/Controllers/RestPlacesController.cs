using ConsoleApp3;
using ConsoleApp3.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bot.Asp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestPlacesController : ControllerBase
    {
        private FileStorageCollection<RestPlace> RestPlaces { get; }

        public RestPlacesController()
        {
            RestPlaces = new("res/rest.txt", true);
        }

        [HttpGet]
        public List<RestPlace> Get()
        {
            return RestPlaces;
        }

        [HttpPost]
        public void Post(RestPlace loc)
        {
            RestPlaces.Add(loc);
            RestPlaces.Save();
        }

        [HttpDelete]
        public void Delete(int RestPlaceId)
        {
            var loc = RestPlaces.FirstOrDefault(x => x.Id == RestPlaceId);
            RestPlaces.Remove(loc);
            RestPlaces.Save();
        }
    }
}
