

namespace TechStore.Models
{
    public class Opinia
    {
        public int Id { get; set; }
        public int? Count { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Quality { get; set; }
        public double? Function { get; set; }
        public double? Overall { get; set; }
        public DateTime Data { get; set; }

        
    }
}
