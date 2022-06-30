using ConsoleApp3.Entities.Base;

namespace ConsoleApp3.Entities
{
    public record Location : PlaceOfInterestAbstract
    {
        public int RegionId { get; set; }
        public bool OnWinter { get; set; }
        public bool OnSummer { get; set; }
        public bool OnSpring { get; set; }
        public bool OnAutmn { get; set; }
    }
}
