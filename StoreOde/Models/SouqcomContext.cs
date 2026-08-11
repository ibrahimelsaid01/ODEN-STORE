using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StoreOde.Models
{
    public partial class SouqcomContext
        : IdentityDbContext<
            IdentityUser,
            IdentityRole,
            string>
    {
        public SouqcomContext(
            DbContextOptions<SouqcomContext> options)
            : base(options)
        {
        }

        public DbSet<Cart> Carts => Set<Cart>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<ContactMessage> ContactMessages =>
            Set<ContactMessage>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCart(modelBuilder);

            ConfigureCategory(modelBuilder);

            ConfigureContactMessage(modelBuilder);

            ConfigureProduct(modelBuilder);

            ConfigureReview(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        private static void ConfigureCart(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable(
                    "Cart",
                    table =>
                    {
                        table.HasCheckConstraint(
                            "CK_Cart_Qty_Positive",
                            "[Qty] > 0");
                    });

                entity.Property(cart => cart.UserId)
                    .HasMaxLength(Cart.UserIdMaxLength)
                    .IsRequired();

                entity.Property(cart => cart.ProductId)
                    .IsRequired();

                entity.Property(cart => cart.Qty)
                    .IsRequired();

                entity.HasIndex(
                        cart => new
                        {
                            cart.UserId,
                            cart.ProductId
                        })
                    .IsUnique()
                    .HasDatabaseName(
                        "UX_Cart_UserId_ProductId");

                entity.HasOne(cart => cart.Product)
                    .WithMany(product => product.Carts)
                    .HasForeignKey(cart => cart.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName(
                        "FK_Cart_Product");

                entity.HasOne<IdentityUser>()
                    .WithMany()
                    .HasForeignKey(cart => cart.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName(
                        "FK_Cart_AspNetUsers_UserId");
            });
        }

        private static void ConfigureCategory(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(category => category.Name)
                    .HasMaxLength(Category.NameMaxLength)
                    .IsRequired();

                entity.Property(category => category.IconClass)
                    .HasMaxLength(Category.IconClassMaxLength);

                entity.Property(category => category.Description)
                    .HasMaxLength(Category.DescriptionMaxLength);

                entity.HasIndex(category => category.Name)
                    .IsUnique();
            });
        }

        private static void ConfigureContactMessage(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.ToTable("ContactMessage");

                entity.Property(message => message.Name)
                    .HasMaxLength(
                        ContactMessage.NameMaxLength)
                    .IsRequired();

                entity.Property(message => message.Email)
                    .HasMaxLength(
                        ContactMessage.EmailMaxLength)
                    .IsRequired();

                entity.Property(message => message.Subject)
                    .HasMaxLength(
                        ContactMessage.SubjectMaxLength)
                    .IsRequired();

                entity.Property(message => message.Message)
                    .HasMaxLength(
                        ContactMessage.MessageMaxLength)
                    .IsRequired();

                entity.Property(message => message.CreatedAtUtc)
                    .HasColumnType("datetime2");

                entity.HasIndex(
                    message => message.CreatedAtUtc);
            });
        }

        private static void ConfigureProduct(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable(
                    "Product",
                    table =>
                    {
                        table.HasCheckConstraint(
                            "CK_Product_Price_Positive",
                            "[Price] > 0");

                        table.HasCheckConstraint(
                            "CK_Product_Quantity_NonNegative",
                            "[quantity] >= 0");

                        table.HasCheckConstraint(
                            "CK_Product_PriceAfterDiscount_Range",
                            "[priceafterdiscount] IS NULL OR ([priceafterdiscount] >= 0 AND [priceafterdiscount] <= [Price])");
                    });

                entity.Property(product => product.Id)
                    .HasColumnName("id");

                entity.Property(product => product.Name)
                    .HasMaxLength(Product.NameMaxLength)
                    .IsRequired();

                entity.Property(product => product.Description)
                    .HasMaxLength(
                        Product.DescriptionMaxLength);

                entity.Property(product => product.Photo)
                    .HasMaxLength(Product.PhotoMaxLength);

                entity.Property(product => product.Type)
                    .HasMaxLength(Product.TypeMaxLength)
                    .HasColumnName("type");

                entity.Property(product => product.SupplierName)
                    .HasMaxLength(
                        Product.SupplierNameMaxLength);

                entity.Property(product => product.ReviewUrl)
                    .HasMaxLength(
                        Product.ReviewUrlMaxLength);

                entity.Property(product => product.EntryDate)
                    .HasColumnType("date");

                entity.Property(product => product.Price)
                    .HasPrecision(18, 2);

                entity.Property(
                        product =>
                            product.Priceafterdiscount)
                    .HasPrecision(18, 2)
                    .HasColumnName(
                        "priceafterdiscount");

                entity.Property(product => product.Quantity)
                    .HasColumnName("quantity")
                    .IsRequired();

                entity.HasOne(product => product.Cat)
                    .WithMany(category => category.Products)
                    .HasForeignKey(product => product.Catid)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName(
                        "FK_Product_Category");
            });
        }

        private static void ConfigureReview(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review");

                entity.Property(review => review.Name)
                    .HasMaxLength(Review.NameMaxLength)
                    .IsRequired();

                entity.Property(review => review.Email)
                    .HasMaxLength(Review.EmailMaxLength);

                entity.Property(review => review.Subject)
                    .HasMaxLength(Review.SubjectMaxLength);

                entity.Property(review => review.Description)
                    .HasMaxLength(
                        Review.DescriptionMaxLength)
                    .IsRequired();
            });
        }

        partial void OnModelCreatingPartial(
            ModelBuilder modelBuilder);
    }
}