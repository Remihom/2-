using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;


public interface IProduct
{
    int Id { get; set; }
    string Name { get; set; }
}

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public List<Order> Orders { get; set; }
}

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; }

    public List<Product> Products { get; set; }
}

public class Product : IProduct
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; }
}

public class ProductBase : IProduct
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [Required]
    public decimal Price { get; set; }

    [MaxLength(50)]
    public string Brand { get; set; }
}

public class ClothingProduct : ProductBase
{
    [MaxLength(20)]
    public string ClothingSize { get; set; }

    [MaxLength(20)]
    public string Color { get; set; }
}

namespace YourProject.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
 
            migrationBuilder.DropTable(
                name: "Customers");
        
        }
    }
}
public class OnlineStoreContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Products)
            .HasForeignKey(p => p.OrderId);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Customer>().HasData(new Customer { Id = 1, Name = "Микола Цибуля", Email = "mikola.cibylya@gmail.com" });
        modelBuilder.Entity<Order>().HasData(new Order { OrderId = 1, CustomerId = 1 });
        modelBuilder.Entity<Product>().HasData(new Product { Id = 1, Name = "Running Shoes", OrderId = 1 });
    }

    public void AddProduct(Product product)
    {
        Products.Add(product);
        SaveChanges();
    }

    public void UpdateProduct(Product product)
    {
        Products.Update(product);
        SaveChanges();
    }

    public void DeleteProduct(Product product)
    {
        Products.Remove(product);
        SaveChanges();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        using (var context = new OnlineStoreContext())
        {
            var product = new Product
            {
                Name = "Shoes",
                OrderId = 1 
            };

            context.AddProduct(product);

            var retrievedProduct = context.Products.Find(1);
            if (retrievedProduct != null)
            {
                retrievedProduct.Name = "Running Shoes";
                context.UpdateProduct(retrievedProduct);
            }

            var productToDelete = context.Products.Find(2); 
            if (productToDelete != null)
            {
                context.DeleteProduct(productToDelete);
            }

            context.SaveChanges();
        }
    }
}
