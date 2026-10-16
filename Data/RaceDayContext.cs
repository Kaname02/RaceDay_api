using Microsoft.EntityFrameworkCore;
using RaceDay_api.Models;

namespace RaceDay_api.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique email
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Unique bib per event
            modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.EventId, e.BibNumber }).IsUnique();

            // Configure relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser).WithMany(u => u.OrganisedEvents).HasForeignKey(e => e.OrganiserId);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.User).WithMany(u => u.Enrolments).HasForeignKey(e => e.UserId);

            // ---> ADD THIS TO FIX THE ERROR <---
            // Prevent cascade delete cycle: Do not allow deleting an Event if it has Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrolments)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete cycle: Do not allow deleting a Category if it has Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}