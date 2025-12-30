using Microsoft.EntityFrameworkCore;
using RestaurantApp.Entities;

namespace RestaurantApp.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext() { }

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options) { }

        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<RestaurantTable> Tables { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Waiter> Waiters { get; set; }
        public virtual DbSet<MenuCategory> MenuCategories { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=OWCA\\OWCA;Database=RestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItemIngredient>()
                .HasKey(mii => new { mii.MenuItemId, mii.IngredientId });

            modelBuilder.Entity<RestaurantTable>()
                .HasKey(t => t.TableId);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.PaymentId);

                entity.Property(p => p.Amount)
                      .HasColumnType("decimal(10,2)");

                entity.Property(p => p.Status)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasOne(p => p.Order)
                      .WithMany(o => o.Payments)
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.PaymentMethod)
                      .WithMany(pm => pm.Payments)
                      .HasForeignKey(p => p.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);

                entity.Property(o => o.Status)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(o => o.TotalAmount)
                      .HasColumnType("decimal(10,2)");

                entity.HasOne(o => o.Table)
                      .WithMany(t => t.Orders)
                      .HasForeignKey(o => o.TableId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Waiter)
                      .WithMany(w => w.Orders)
                      .HasForeignKey(o => o.WaiterId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(pm => pm.PaymentMethodId);

                entity.Property(pm => pm.Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(pm => pm.IsActive)
                      .HasDefaultValue(true);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
