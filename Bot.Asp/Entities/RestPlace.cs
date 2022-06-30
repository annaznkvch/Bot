using ConsoleApp3.Entities.Base;

namespace ConsoleApp3.Entities
{
    public record RestPlace : PlaceOfInterestAbstract
    {
        public string Region { get; set; }
    }
}
