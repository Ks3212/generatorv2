namespace TechStore.Models
{
	public class Description
	{
		public int Id { get; set; }
		public string? Opis { get; set; }
		public string? Image { get; set; }
		public int PruductId { get; set; }
		public Product? Product { get; set; }
	}
}
