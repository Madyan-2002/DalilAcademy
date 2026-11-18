using DalilAcademy.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Infra.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        //DBSets
     public DbSet<Course> Courses => Set<Course>();
     public DbSet<Enrollment> Enrollments => Set<Enrollment>();
     public DbSet<Student> Students => Set<Student>();
     public DbSet<Instructor> Instructors => Set<Instructor>();
     public DbSet<User> Users => Set<User>();
     public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSeeding((context, _) =>
            {
                if (!context.Set<Instructor>().Any())
                {
                    var ahmad = new Instructor { FullName = "Ahmad Ali" };
                    var raghad = new Instructor { FullName = "Raghad Amjad" };

                    context.Set<Course>().AddRange(
                        new Course { Title = "C# Fundamentals", Price = 500, Instructor = ahmad },
                        new Course { Title = "ASP.NET Core Web API", Price = 600, Instructor = raghad }
                    );
                    context.SaveChanges();
                }

                if (!context.Set<User>().Any())
                {
                    context.Set<User>().Add(new User
                    {
                        Email = "admin@dalil.jo",
                        Role = Roles.Admin,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
                    });
                    context.SaveChanges();
                }
            });
        }
    }
}
