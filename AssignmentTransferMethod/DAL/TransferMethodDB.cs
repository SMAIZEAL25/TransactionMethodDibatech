using AssignmentTransferMethod.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;
using System.Security.Principal;

namespace AssignmentTransferMethod.DAL

{

    public class TransferMethodDB : DbContext
    {
        public TransferMethodDB(DbContextOptions<TransferMethodDB> options) : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transfer>()
        .HasKey(t => t.IdempotencyKey);

            modelBuilder.Entity<Transfer>()
        .HasIndex(t => t.IdempotencyKey)
        .IsUnique();

            modelBuilder.Entity<Account>()
        .Property(a => a.RowVersion)
        .IsRowVersion();
        }
    }
}

