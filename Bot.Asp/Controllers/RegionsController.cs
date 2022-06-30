using ConsoleApp3;
using ConsoleApp3.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bot.Asp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private FileStorageCollection<Region> Regions { get; }
        public RegionsController()
        {
            Regions = new("res/reg.txt", true);
        }

        [HttpGet]
        public List<Region> Get()
        {
            return Regions;
        }

        [HttpPost]
        public void Post(Region loc)
        {
            Regions.Add(loc);
            Regions.Save();
        }

        [HttpDelete]
        public void Delete(int RegionId)
        {
            var loc = Regions.FirstOrDefault(x => x.Id == RegionId);
            Regions.Remove(loc);
            Regions.Save();
        }
    }
}
