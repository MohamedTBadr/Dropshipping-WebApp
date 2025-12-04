using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Dropshipper)
                .WithMany()
                .HasForeignKey(o => o.DropshipperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.OrderPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.OrderDiscount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippedDate)
                .HasColumnType("date");

            builder.Property(o => o.DropshipperId)
                .IsRequired()
                .HasMaxLength(450);
        }
    }
}

