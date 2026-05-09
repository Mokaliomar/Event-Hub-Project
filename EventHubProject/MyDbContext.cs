using System;
using EventHubProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace EventHubProject;

public class MyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server = localhost\SQL2022; Database = EventHub;Trusted_Connection=True;TrustServerCertificate=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Changing the default table name for the Attendee entity, and Making the `Owned Entity` properties to be stored in the same table as the Attendee entity
        modelBuilder.Entity<Attendee>(
        entity => entity.OwnsOne(a => a.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Street");
            a.Property(p => p.City).HasColumnName("City");
            a.Property(p => p.Country).HasColumnName("Country");
            a.Property(p => p.PostalCode).HasColumnName("PostalCode");
        }));

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(a => new { a.EventId, a.AttendeeId });
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.Property(p => p.DateIssued).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETDATE()");
            entity.Property<DateTime>("LastModified").HasDefaultValueSql("GETDATE()");
        });

        // ---------------------------------
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(o => o.Organizer)
                .WithMany(e => e.Events)
                .HasForeignKey(o => o.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        // 1. هنجيب كل الكائنات اللي نوعها Event وحالتها "Modified" (يعني حصل فيها تعديل)
        var modifiedEvents = ChangeTracker.Entries<Event>()
                                          .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedEvents)
        {
            // 2. هنوصل للـ Shadow Property ونديلها الوقت الحالي
            entry.Property("LastModified").CurrentValue = DateTime.Now;
            // ممكن تستخدم DateTime.UtcNow لو بتتعامل مع مناطق زمنية مختلفة
        }

        // 3. بنكمل عملية الحفظ العادية عشان التعديلات تروح للداتابيز
        return base.SaveChanges();
    }

    // وطبعاً يُفضل تعمل نفس الكلام في الـ Async version
    /* public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedEvents = ChangeTracker.Entries<Event>().Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedEvents)
        {
            entry.Property("LastModified").CurrentValue = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    } */
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ProfilePage> ProfilePages { get; set; }
    public DbSet<Organizer> Organizers { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<Badge> Badges { get; set; }
}
