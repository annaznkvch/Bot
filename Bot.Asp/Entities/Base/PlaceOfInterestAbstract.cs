namespace ConsoleApp3.Entities.Base
{
    public abstract record PlaceOfInterestAbstract : PlaceAbstract
    {
        public string PhotoPath { get; set; }
        public string Description { get; set; } 
    }
}
