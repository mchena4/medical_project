using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Models;

namespace MedicalClinicAPI.Data;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Patient> Patients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed Status Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Doctor" },
            new Role { Id = 3, Name = "Receptionist" },
            new Role { Id = 4, Name = "Patient" }
        );  

        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Pending" },
            new Status { Id = 2, Name = "Completed" },
            new Status { Id = 3, Name = "Cancelled" }
        );

        // Unique Email 
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Dni)
            .IsUnique();
    }

}