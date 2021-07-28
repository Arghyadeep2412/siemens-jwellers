using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace JewelleryStore.Models.DBModels
{
    public partial class sakilaContext : DbContext
    {
        public sakilaContext()
        {
        }

        public sakilaContext(DbContextOptions<sakilaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<UsersLoginCred> UsersLoginCreds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=Change123;database=sakila;port=3308");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("customers");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CustomerType)
                    .HasColumnType("tinyint")
                    .HasColumnName("customer_type");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");

                entity.HasIndex(e => e.UserId, "fk_customer");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.ActualPrice)
                    .HasColumnType("decimal(10,2)")
                    .HasColumnName("actual_price");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("currency");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(5,2)")
                    .HasColumnName("discount_percentage");

                entity.Property(e => e.FinalPrice)
                    .HasColumnType("decimal(10,2)")
                    .HasColumnName("final_price");

                entity.Property(e => e.ItmeType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("itme_type");

                entity.Property(e => e.PaymentStatus)
                    .HasColumnType("tinyint")
                    .HasColumnName("payment_status");

                entity.Property(e => e.PricePerUnit)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("price_per_unit");

                entity.Property(e => e.Rate)
                    .HasColumnType("decimal(8,2)")
                    .HasColumnName("rate");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Weight)
                    .HasColumnType("decimal(5,2)")
                    .HasColumnName("weight");

                entity.Property(e => e.WeightUnit)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("weight_unit");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_customer");
            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.ToTable("rates");

                entity.Property(e => e.RateId).HasColumnName("rate_id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("currency");

                entity.Property(e => e.ItemType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("item_type");

                entity.Property(e => e.RateAmount)
                    .HasColumnType("decimal(8,2)")
                    .HasColumnName("rate_amount");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("unit");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<UsersLoginCred>(entity =>
            {
                entity.HasKey(e => e.UserLoginId)
                    .HasName("PRIMARY");

                entity.ToTable("users_login_creds");

                entity.HasIndex(e => e.UserId, "fk_customer_login");

                entity.Property(e => e.UserLoginId).HasColumnName("user_login_id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .HasColumnName("password");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("user_name");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersLoginCreds)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_customer_login");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
