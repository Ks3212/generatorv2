using Microsoft.AspNetCore.Identity;
using System.Net;

namespace TechStore.Models
{
    public class Client :IdentityUser

    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Address? Address { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Report>? Reports { get; set; }
    }
}
