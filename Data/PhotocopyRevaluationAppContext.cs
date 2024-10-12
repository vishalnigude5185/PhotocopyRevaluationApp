using Microsoft.EntityFrameworkCore;
using PhotocopyRevaluationAppMVC.Models;

namespace PhotocopyRevaluationAppMVC.Data;

public partial class PhotocopyRevaluationAppContext : DbContext
{
    public PhotocopyRevaluationAppContext()
    {
    }

    public PhotocopyRevaluationAppContext(DbContextOptions<PhotocopyRevaluationAppContext> options)
        : base(options)
    {
    }
    public virtual DbSet<ApplicationUser> Users { get; set; } = default!;
    public virtual DbSet<Photocopy> Photocopies { get; set; } = default!;
    public virtual DbSet<Revaluation> Revaluations { get; set; } = default!;
    public virtual DbSet<Notification> Notifications { get; set; } = default!;
    public virtual DbSet<SessionData> SessionData { get; set; } = default!;

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Photocopy>(entity =>
        {
            entity.HasKey(e => e.SNO);

            entity.ToTable("photocopy");
        });

        modelBuilder.Entity<Revaluation>(entity =>
        {
            entity.HasKey(e => e.SNO);

            entity.ToTable("reValuation");

            //entity.Property(e => e.ReverificationCount).HasDefaultValue("");
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.ApplicationUserId);

            entity.ToTable("ApplicationUsers");
        });

        // Setup relationships and constraints
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.ApplicationUser)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.ApplicationUserId);

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Exception);
            entity.Property(e => e.Context);
            entity.Property(e => e.CorrelationId);
            entity.Property(e => e.IpAddress);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        OnModelCreatingPartial(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
