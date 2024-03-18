using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace FinalProject_Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // quan he 1 nhieu
            builder.Entity<Cart>().HasOne(cart => cart.Account).WithMany(account => account.Carts).HasForeignKey(cart => cart.AccountID);
            builder.Entity<Cart>().HasOne(cart => cart.Product).WithMany(product => product.Carts).HasForeignKey(cart => cart.ProductID);
            builder.Entity<Receipt>().HasOne(receipt => receipt.Account).WithMany(account => account.Receipts).HasForeignKey(receipt => receipt.AccountID);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

    }
}
