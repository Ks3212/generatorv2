using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<Client>
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\TechStoreDatabase\\Database.db");
        }
        public DbSet<Opinia>? Opinie { get; set; }
		public DbSet<Address>? Addresses { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Client>? Clients { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<ShippingAddress>? ShippingAddresses { get; set; }
        public DbSet<ProductOrderRelation>? ProductOrderRelations { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Report>? Reports { get; set; }
        public DbSet<ProductSpecification>? ProductSpecification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            // Primary key definitions
        modelBuilder.Entity<Opinia>().HasKey(o => o.Id);
        modelBuilder.Entity<ShippingAddress>().HasKey(o => o.Id);
        modelBuilder.Entity<Address>().HasKey(a => a.Id);
        modelBuilder.Entity<Category>().HasKey(c => c.Id);
        modelBuilder.Entity<Client>().HasKey(c => c.Id);
        modelBuilder.Entity<Review>().HasKey(r => r.Id);
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<ProductOrderRelation>().HasKey(r => new { r.ProductId, r.OrderId });
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<Report>().HasKey(r => r.Id);
        modelBuilder.Entity<ProductSpecification>().HasKey(p => p.Id);
            
            //Problemy


            // Zdefiniowanie klucza głównego dla tabeli AspNetRoles
            modelBuilder.Entity<IdentityRole>()
                .ToTable("AspNetRoles")
                .HasKey(r => r.Id);

            // Zdefiniowanie klucza głównego dla tabeli AspNetUserRoles (klucz składkowy)
            modelBuilder.Entity<IdentityUserRole<string>>()
                .ToTable("AspNetUserRoles")
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Zdefiniowanie klucza głównego dla tabeli AspNetUserClaims
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .ToTable("AspNetUserClaims")
                .HasKey(uc => uc.Id);

            // Zdefiniowanie klucza głównego dla tabeli AspNetUserLogins
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .ToTable("AspNetUserLogins")
                .HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            // Zdefiniowanie klucza głównego dla tabeli AspNetUserTokens
            modelBuilder.Entity<IdentityUserToken<string>>()
                .ToTable("AspNetUserTokens")
                .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
            // Relationship definitions

            // Relacja jeden do jeden między klientem (Client) a adresem (Address).
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Address)
                .WithOne(a => a.Client)
                .HasForeignKey<Address>(a => a.ClientId);

            // Relacja jeden do wielu między klientem (Client) a recenzjami (Review).
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId);

            // Relacja jeden do wielu między klientem (Client) a zamówieniami (Order).
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId);

            // Relacja jeden do wielu między klientem (Client) a raportami (Report).
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reports)
                .WithOne(rp => rp.Client)
                .HasForeignKey(rp => rp.ClientId);

            // Relacja jeden do wielu między kategoriami (Category) a produktami (Product).
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c=>c.Products)
                .HasForeignKey(p => p.CategoryId);
            // Relacja jeden do wielu między produkt (Product) a opiniami (Reviews).
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(rp => rp.Product)
                .HasForeignKey(rp => rp.ProductId);
            //Relacja wiele do wielu między tabelami (Order) i (Product)
            modelBuilder.Entity<ProductOrderRelation>()
                .HasOne(por => por.Product)
                .WithMany(p => p.ProductOrderRelations)
                .HasForeignKey(por => por.ProductId);

            modelBuilder.Entity<ProductOrderRelation>()
                .HasOne(por => por.Order)
                .WithMany(o => o.ProductOrderRelations)
                .HasForeignKey(por => por.OrderId);

            // Relacja jeden do jeden między Order a ShippingAddress
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithOne()
                .HasForeignKey<Order>(o => o.ShippingAddressId);
            // Relacja jeden do jeden między Product a ProductSpecification

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Specification)
                .WithOne(ps => ps.Product)
                .HasForeignKey<ProductSpecification>(ps => ps.ProductId);

			modelBuilder.Entity<Product>()
	            .HasMany(p => p.Descriptions)
	            .WithOne(d => d.Product)
	            .HasForeignKey(d => d.PruductId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi  => pi.ProductId);









		}
	}
}