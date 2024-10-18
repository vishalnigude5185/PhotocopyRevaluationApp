using Microsoft.EntityFrameworkCore;
using PhotocopyRevaluationApp.Models;

namespace PhotocopyRevaluationApp.Data {
    public class LoggingContext : DbContext {
        public LoggingContext() {
        }

        public LoggingContext(DbContextOptions<LoggingContext> options)
            : base(options) {
        }

        public virtual DbSet<Log> Logs { get; set; } = default!;
        //public virtual DbSet<Feedback> Feedbacks { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Log>(entity => {
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

            //OnModelCreatingPartial(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
