using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace API_Pharmacy.Model;

public partial class PharmacyDbContext : DbContext
{
    public PharmacyDbContext()
    {
    }

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketItem> BasketItems { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=217.60.37.17;user=full;password=root;database=pharmacy_db", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql")).UseLazyLoadingProxies();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.BasketId).HasName("PRIMARY");

            entity.ToTable("basket");

            entity.HasIndex(e => e.BasketClientId, "fk_basket_client_idx");

            entity.Property(e => e.BasketId).HasColumnName("basket_id");
            entity.Property(e => e.BasketClientId).HasColumnName("basket_client_id");
            entity.Property(e => e.BasketDate)
                .HasColumnType("datetime")
                .HasColumnName("basket_date");
            entity.Property(e => e.BasketStatus)
                .HasColumnType("enum('активная','оформлена','отменена')")
                .HasColumnName("basket_status");

            entity.HasOne(d => d.BasketClient).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.BasketClientId)
                .HasConstraintName("fk_basket_client");
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.HasKey(e => e.BasketItemId).HasName("PRIMARY");

            entity.ToTable("basket_item");

            entity.HasIndex(e => e.BasketId, "fk_basket_idx");

            entity.HasIndex(e => e.ItemId, "fk_item_idx");

            entity.Property(e => e.BasketItemId).HasColumnName("basket_item_id");
            entity.Property(e => e.BasketId).HasColumnName("basket_id");
            entity.Property(e => e.BasketItemCount).HasColumnName("basket_item_count");
            entity.Property(e => e.ItemId).HasColumnName("item_id");

            entity.HasOne(d => d.Basket).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.BasketId)
                .HasConstraintName("fk_basket");

            entity.HasOne(d => d.Item).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("fk_item");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PRIMARY");

            entity.ToTable("brand");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PRIMARY");

            entity.ToTable("client");

            entity.HasIndex(e => e.ClientEmail, "client_email_UNIQUE").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.ClientEmail).HasColumnName("client_email");
            entity.Property(e => e.ClientLastName)
                .HasMaxLength(100)
                .HasColumnName("client_last_name");
            entity.Property(e => e.ClientName)
                .HasMaxLength(100)
                .HasColumnName("client_name");
            entity.Property(e => e.ClientPassword)
                .HasMaxLength(255)
                .HasColumnName("client_password");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("item");

            entity.HasIndex(e => e.ItemBrandId, "fk_item_brand_idx");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemBrandId).HasColumnName("item_brand_id");
            entity.Property(e => e.ItemCount).HasColumnName("item_count");
            entity.Property(e => e.ItemDesc)
                .HasColumnType("text")
                .HasColumnName("item_desc");
            entity.Property(e => e.ItemImg)
                .HasColumnType("text")
                .HasColumnName("item_img");
            entity.Property(e => e.ItemPrice).HasColumnName("item_price");
            entity.Property(e => e.ItemStatus)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("item_status");
            entity.Property(e => e.ItemTitle)
                .HasColumnType("text")
                .HasColumnName("item_title");

            entity.HasOne(d => d.ItemBrand).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemBrandId)
                .HasConstraintName("fk_item_brand");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
