using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Data;

public class Program
{
	public static void Main(string[] args)
	{
		using var context = new ApplicationDbContext();
		context.ChangeTracker.AutoDetectChangesEnabled = false;

		Console.WriteLine("Wybierz co chcesz generować:");
		Console.WriteLine("1. Klienci");
		Console.WriteLine("2. Kategorie");
		Console.WriteLine("3. Produkty");
		Console.WriteLine("4. Recenzje");
		Console.WriteLine("5. Zamówienia");
		Console.WriteLine("6. Raporty");
		Console.WriteLine("7. Wszystko");

		var choice = Console.ReadLine();

		Console.WriteLine("Ile rekordów chcesz wygenerować?");
		int recordCount = int.Parse(Console.ReadLine());

		using var transaction = context.Database.BeginTransaction();

		try
		{
			if (choice == "1" || choice == "7")
				GenerateClients(context, recordCount);

			if (choice == "2" || choice == "7")
				GenerateCategories(context, recordCount);

			if (choice == "3" || choice == "7")
				GenerateProducts(context, recordCount);

			if (choice == "4" || choice == "7")
				GenerateReviews(context, recordCount);

			if (choice == "5" || choice == "7")
				GenerateOrders(context, recordCount);

			if (choice == "6" || choice == "7")
				GenerateReports(context, recordCount);

			transaction.Commit();
			Console.WriteLine("Wszystkie dane zostały wygenerowane.");
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			Console.WriteLine($"Błąd: {ex.Message}");
			Console.WriteLine($"Szczegóły: {ex.InnerException?.Message}");
		}
		finally
		{
			context.ChangeTracker.AutoDetectChangesEnabled = true;
		}
	}

	private static void GenerateClients(ApplicationDbContext context, int recordCount)
	{
		var clientFaker = new Faker<Client>()
			.RuleFor(c => c.Id, f => Guid.NewGuid().ToString())
			.RuleFor(c => c.FirstName, f => f.Name.FirstName())
			.RuleFor(c => c.LastName, f => f.Name.LastName())
			.RuleFor(c => c.Email, f => f.Internet.Email())
			.RuleFor(c => c.PasswordHash, f => f.Internet.Password())
			.RuleFor(c => c.UserName, (f, c) => c.Email)
			.RuleFor(c => c.NormalizedEmail, (f, c) => c.Email.ToUpper())
			.RuleFor(c => c.NormalizedUserName, (f, c) => c.Email.ToUpper())
			.RuleFor(c => c.EmailConfirmed, c => true)
			.RuleFor(c => c.Address, f => new Address
			{
				Street = f.Address.StreetName(),
				BuildingNumber = f.Address.BuildingNumber(),
				ApartmentNumber = f.Address.SecondaryAddress(),
				PostalCode = f.Address.ZipCode(),
				Locality = f.Address.City()
			});

		SaveDataInBatches(context, clientFaker, recordCount, context.Clients);
		Console.WriteLine("Klienci wygenerowani.");
	}

	private static void GenerateCategories(ApplicationDbContext context, int recordCount)
	{
		var categoryFaker = new Faker<Category>()
			.RuleFor(c => c.Name, f => f.Commerce.Categories(1).First());

		SaveDataInBatches(context, categoryFaker, recordCount, context.Categories);
		Console.WriteLine("Kategorie wygenerowane.");
	}

	private static void GenerateProducts(ApplicationDbContext context, int recordCount)
	{
		var categoryIds = context.Categories.Select(c => c.Id).ToList();

		var productFaker = new Faker<Product>()
			.RuleFor(p => p.Name, f => f.Commerce.ProductName())
			.RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
			.RuleFor(p => p.Description, f => f.Lorem.Paragraph())
			.RuleFor(p => p.Quantity, f => f.Random.Int(1, 100))
			.RuleFor(p => p.Image, f => f.Image.PicsumUrl())
			.RuleFor(p => p.Company, f => f.Company.CompanyName())
			.RuleFor(p => p.IsOnSale, f => f.Random.Bool())
			.RuleFor(p => p.SalePrice, (f, p) => p.IsOnSale ? (double?)(p.Price * 0.9M) : null)
			.RuleFor(p => p.Url, f => f.Internet.Url())
			.RuleFor(p => p.CategoryId, f => f.PickRandom(categoryIds));

		SaveDataInBatches(context, productFaker, recordCount, context.Products);
		Console.WriteLine("Produkty wygenerowane.");
	}

	private static void GenerateReviews(ApplicationDbContext context, int recordCount)
	{
		var clientIds = context.Clients.Select(c => c.Id).ToList();
		var productIds = context.Products.Select(p => p.Id).ToList();

		var reviewFaker = new Faker<Review>()
			.RuleFor(r => r.Comment, f => f.Lorem.Sentence())
			.RuleFor(r => r.Rating, f => f.Random.Int(1, 5))
			.RuleFor(r => r.ProductId, f => f.PickRandom(productIds))
			.RuleFor(r => r.ClientId, f => f.PickRandom(clientIds));

		SaveDataInBatches(context, reviewFaker, recordCount, context.Reviews);
		Console.WriteLine("Recenzje wygenerowane.");
	}

	private static void GenerateOrders(ApplicationDbContext context, int recordCount)
	{
		var clientIds = context.Clients.Select(c => c.Id).ToList();
		var productIds = context.Products.Select(p => p.Id).ToList();

		// Generowanie zamówień
		var orderFaker = new Faker<Order>()
			.RuleFor(o => o.OrderStatus, f => f.PickRandom("Pending", "Completed", "Shipped", "Cancelled"))
			.RuleFor(o => o.OrderValue, f => Convert.ToDouble(f.Commerce.Price(100, 100000)))
			.RuleFor(o => o.OrderDate, f => f.Date.Past(5))
			.RuleFor(o => o.OrderConfirmation, f => f.Random.Bool())
			.RuleFor(o => o.CompletionConfirmation, f => f.Random.Bool())
			.RuleFor(o => o.ClientId, f => f.PickRandom(clientIds))
			.RuleFor(o => o.ShippingAddress, f => new ShippingAddress
			{
				Locality = f.Address.City(),
				Street = f.Address.StreetName(),
				BuildingNumber = f.Address.BuildingNumber(),
				ApartmentNumber = f.Address.SecondaryAddress(),
				PostalCode = f.Address.ZipCode()
			});

		var orders = SaveDataInBatches(context, orderFaker, recordCount, context.Orders);

		// Generowanie relacji z informacją o postępie
		Console.WriteLine("Rozpoczęcie generowania relacji produktów dla zamówień...");
		GenerateProductRelationsWithProgress(context, orders, productIds);
		Console.WriteLine("Relacje między zamówieniami a produktami wygenerowane.");
	}

	private static void GenerateProductRelationsWithProgress(ApplicationDbContext context, List<Order> orders, List<int> productIds)
	{
		var productOrderRelations = new List<ProductOrderRelation>();
		int totalOrders = orders.Count;
		int processedOrders = 0;
		int batchSize = 100; // Process in batches

		// Create a list of tasks to run in parallel
		var tasks = new List<Task>();

		foreach (var order in orders)
		{
			// Capture the current order and product IDs for the task
			var currentOrder = order;
			var currentProductIds = productIds;

			var task = Task.Run(() =>
			{
				var random = new Random();
				var selectedProductIds = currentProductIds
					.OrderBy(x => random.Next())
					.Take(random.Next(1, Math.Min(5, currentProductIds.Count)))
					.ToList();

				var relations = selectedProductIds.Select(productId => new ProductOrderRelation
				{
					OrderId = currentOrder.Id,
					ProductId = productId
				}).ToList();

				lock (context)
				{
					productOrderRelations.AddRange(relations);
				}

				Interlocked.Increment(ref processedOrders);

				// Output progress to the console in a thread-safe way
				if (processedOrders % batchSize == 0 || processedOrders == totalOrders)
				{
					Console.WriteLine($"Przetworzono {processedOrders}/{totalOrders} zamówień...");
				}
			});

			tasks.Add(task);

			// Save data in batches to reduce memory consumption
			if (tasks.Count >= batchSize || processedOrders == totalOrders)
			{
				Task.WhenAll(tasks).Wait();  // Ensure all tasks are completed before saving
				lock (context)
				{
					context.ProductOrderRelations.AddRange(productOrderRelations);
					context.SaveChanges();
					productOrderRelations.Clear();  // Clear the list for the next batch
				}

				tasks.Clear();  // Clear the list of tasks for the next batch
			}
		}

		// Wait for all remaining tasks to complete if there are any left
		if (tasks.Any())
		{
			Task.WhenAll(tasks).Wait();
			lock (context)
			{
				context.ProductOrderRelations.AddRange(productOrderRelations);
				context.SaveChanges();
			}
		}

		Console.WriteLine("Relacje między zamówieniami a produktami wygenerowane.");
	}
	private static List<T> SaveDataInBatchesWithProgress<T>(ApplicationDbContext context, Faker<T> faker, int recordCount, DbSet<T> dbSet) where T : class
	{
		var data = new List<T>();
		int batchSize = 1000;
		int totalGenerated = 0;

		for (int i = 0; i < recordCount; i += batchSize)
		{
			var batch = faker.Generate(Math.Min(batchSize, recordCount - i));
			dbSet.AddRange(batch);
			context.SaveChanges();
			data.AddRange(batch);

			totalGenerated += batch.Count;
			Console.WriteLine($"Wygenerowano {totalGenerated}/{recordCount} rekordów...");
		}

		return data;
	}

	private static void GenerateReports(ApplicationDbContext context, int recordCount)
	{
		var clientIds = context.Clients.Select(c => c.Id).ToList();
		var productNames = context.Products.Select(p => p.Name).ToList();

		var reportFaker = new Faker<Report>()
			.RuleFor(r => r.Title, f => f.PickRandom(productNames))
			.RuleFor(r => r.Description, f => f.Lorem.Sentence())
			.RuleFor(r => r.Answered, f => f.Random.Bool())
			.RuleFor(r => r.ClientId, f => f.PickRandom(clientIds));

		SaveDataInBatches(context, reportFaker, recordCount, context.Reports);
		Console.WriteLine("Raporty wygenerowane.");
	}

	private static List<T> SaveDataInBatches<T>(ApplicationDbContext context, Faker<T> faker, int recordCount, DbSet<T> dbSet) where T : class
	{
		var data = new List<T>();
		int batchSize = 1000;
		for (int i = 0; i < recordCount; i += batchSize)
		{
			var batch = faker.Generate(Math.Min(batchSize, recordCount - i));
			dbSet.AddRange(batch);
			context.SaveChanges();
			data.AddRange(batch);
		}
		return data;
	}
}
