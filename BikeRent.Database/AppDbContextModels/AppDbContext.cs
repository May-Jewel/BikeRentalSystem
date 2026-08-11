using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRent.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace BikeRent.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBike> TblBikes { get; set; }

    public virtual DbSet<TblRental> TblRentals { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-OI6UJSEI;Database=BikeRent;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBike>(entity =>
        {
            entity.HasKey(e => e.BikeId);

            entity.ToTable("Bikes");

            entity.Property(e => e.BikeId).HasColumnName("bike_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PricePerHour)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_per_hour");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("available")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Condition)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("good")
                .HasColumnName("condition");
        });

        modelBuilder.Entity<TblRental>(entity =>
        {
            entity.HasKey(e => e.RentalId);

            entity.ToTable("Rentals");

            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.ActualReturn)
                .HasColumnType("datetime")
                .HasColumnName("actual_return");
            entity.Property(e => e.BikeId).HasColumnName("bike_id");
            entity.Property(e => e.ExpectedReturn)
                .HasColumnType("datetime")
                .HasColumnName("expected_return");
            entity.Property(e => e.LateFee)
                .HasPrecision(10, 2)
                .HasDefaultValue(0.00m)
                .HasColumnName("late_fee");
            entity.Property(e => e.RentDatetime)
                .HasColumnType("datetime")
                .HasColumnName("rent_datetime");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Bike).WithMany(p => p.TblRentals)
                .HasForeignKey(d => d.BikeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rentals__bike_id");

            entity.HasOne(d => d.User).WithMany(p => p.TblRentals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rentals__user_id");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Users");

            entity.HasIndex(e => e.Phone, "UQ__Users__phone").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
