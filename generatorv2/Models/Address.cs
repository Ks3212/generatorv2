namespace TechStore.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? Locality { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
