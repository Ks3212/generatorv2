namespace TechStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? OrderStatus { get; set; }
        public double OrderValue { get; set; }
        public DateTime OrderDate { get; set; }
        public bool OrderConfirmation { get; set; }
        public bool CompletionConfirmation { get; set; }
        public string? ClientId { get; set; }
        public int ShippingAddressId { get; set; }
        public Client? Client { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public List<ProductOrderRelation>? ProductOrderRelations { get; set; }
    }
}
