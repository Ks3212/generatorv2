namespace TechStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public string? Image { get; set; }
        public string? Company { get; set; }
        public int CategoryId { get; set; }
        public bool IsOnSale { get; set; }
        public double? SalePrice { get; set; }
        public string? Url { get; set; }
        public Category? Category { get; set; }
        public List<Review>? Reviews { get; set; }
        public ProductSpecification? Specification { get; set; }
        public List<ProductOrderRelation>? ProductOrderRelations { get; set; }
        public List<Description>? Descriptions { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}
