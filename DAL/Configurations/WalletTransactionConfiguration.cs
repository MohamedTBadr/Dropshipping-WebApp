using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.TransactionDate)
                .HasColumnType("datetime2");
        }
    }
}

