namespace ConsoleApp3.Entities.Base
{
    public abstract record PlaceAbstract : BaseEntity
    {
        public string Name { get; set; }
        public string Cooordinates { get; set; }
    }
}
