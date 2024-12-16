namespace TechStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
		public int? Rating { get; set; }
		public int ProductId { get; set; }
        public string? ClientId { get; set; }
        public Product? Product { get; set; }
        public Client? Client { get; set; }
    }
}
