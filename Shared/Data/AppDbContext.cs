using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<NotificationSetup> NotificationSetups => Set<NotificationSetup>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NotificationSetup
            modelBuilder.Entity<NotificationSetup>(entity =>
            {
                entity.ToTable("tbl_Notification_Setup");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NotificationType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Template).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Channel).HasMaxLength(50);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("tbl_Notifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.Property(e => e.TransactionRef).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AccountNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ErrorMessage).HasMaxLength(500);
                entity.HasOne(e => e.Setup)
                      .WithMany(s => s.Notifications)
                      .HasForeignKey(e => e.SetupId);
            });

            // Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("tbl_Accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.AccountNumber).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.AccountNumber).IsUnique();
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("tbl_Transactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BalanceBefore).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(200).IsRequired();
                entity.Property(e => e.TransactionRef).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.HasOne(e => e.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(e => e.AccountId);
            });
        }
    }
}