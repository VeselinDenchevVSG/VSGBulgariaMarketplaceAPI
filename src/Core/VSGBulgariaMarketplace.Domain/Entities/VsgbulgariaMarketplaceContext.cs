using Microsoft.EntityFrameworkCore;

namespace VSGBulgariaMarketplace.Domain.Entities;

public class VsgbulgariaMarketplaceContext : DbContext
{
    public VsgbulgariaMarketplaceContext()
    {
    }

    public VsgbulgariaMarketplaceContext(DbContextOptions<VsgbulgariaMarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    //public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    //public virtual DbSet<VersionInfo> VersionInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=VDENCHEV-PC;Initial Catalog=VSGBulgariaMarketplace;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_[Items]");

            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ModifiedAtUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PicturePublicId).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
        });

        //modelBuilder.Entity<Log>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK_[Logs]");

        //    entity.Property(e => e.Level).HasMaxLength(11);
        //    entity.Property(e => e.Type).HasMaxLength(50);
        //});

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_[Orders]");

            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(30);
            entity.Property(e => e.ModifiedAtUtc).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Item).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_[Orders]_[ItemId]_[Items]_[Id]");
        });

        //modelBuilder.Entity<VersionInfo>(entity =>
        //{
        //    entity
        //        .HasNoKey()
        //        .ToTable("VersionInfo");

        //    entity.HasIndex(e => e.Version, "UC_Version")
        //        .IsUnique()
        //        .IsClustered();

        //    entity.Property(e => e.AppliedOn).HasColumnType("datetime");
        //    entity.Property(e => e.Description).HasMaxLength(1024);
        //});

        OnModelCreatingPartial(modelBuilder);
    }
}
