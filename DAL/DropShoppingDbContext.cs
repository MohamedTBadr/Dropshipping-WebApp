using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DropShoppingDbContext:IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Dropshipper> Dropshippers { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
       


        public DropShoppingDbContext(DbContextOptions<DropShoppingDbContext> options) : base(options)
        {
            
        }


        
     
              protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Product>().HasMany(x => x.Images).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
            // Dropshipper - ApplicationUser  one-to-one
            builder.Entity<Dropshipper>()
                .HasKey(d => d.UserId);
            builder.Entity<Dropshipper>()
                .HasOne(d => d.User)
                .WithOne(u => u.Dropshipper)
                .HasForeignKey<Dropshipper>(d => d.UserId);
            // Wallet - Dropshipper one-to-one
            builder.Entity<Wallet>()
                .HasOne(w => w.Dropshipper)
                .WithOne(d => d.Wallet)
                .HasForeignKey<Wallet>(w => w.DropshipperId);
            // WalletTransaction - Wallet one-to-many
            builder.Entity<WalletTransaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.WalletTransactions)
                .HasForeignKey(t => t.WalletId);
            // Order - User(Dropshipper) many-to-one
            builder.Entity<Order>()
                .HasOne(o => o.Dropshipper)
                .WithMany()
                .HasForeignKey(o => o.DropshipperId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Images one-to-many
            builder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            // OrderItem relations
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        
    }
}
