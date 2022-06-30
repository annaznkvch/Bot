using ConsoleApp3;
using ConsoleApp3.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bot.Asp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : Controller
    {
        private FileStorageCollection<Location> Locations { get; }

        public LocationsController()
        {
            Locations = new("res/loc.txt", true);
        }

        [HttpGet]
        public List<Location> Get()
        {
            return Locations;
        }

        [HttpPost]
        public void Post([FromBody]Location loc)
        {
            Locations.Add(loc);
            Locations.Save();
        }

        [HttpDelete]
        public void Delete(int locationId)
        {
            var loc = Locations.FirstOrDefault(x => x.Id == locationId);
            Locations.Remove(loc);
            Locations.Save();
        }
    }
}
