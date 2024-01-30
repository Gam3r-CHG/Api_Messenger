using Microsoft.EntityFrameworkCore;

namespace UsersMessages.Db;

public partial class AppDbContext : DbContext
{

    private readonly string _connectionString;

    public AppDbContext()
    {
    }

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pk");

            entity.ToTable("messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FromId).HasColumnName("from_id");
            entity.Property(e => e.ToId).HasColumnName("to_id");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.Received)
                .HasDefaultValue(false)
                .HasColumnName("received");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
