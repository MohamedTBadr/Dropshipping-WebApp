using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.PhoneNumber).HasMaxLength(50);
            builder.Property(c => c.DropshipperId).IsRequired().HasMaxLength(450);
            
            // Configure relationship with User (Dropshipper)
            builder.HasOne(c => c.Dropshipper)
                .WithMany()
                .HasForeignKey(c => c.DropshipperId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

