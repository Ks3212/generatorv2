namespace TechStore.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Answered { get; set; }
        public string? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
