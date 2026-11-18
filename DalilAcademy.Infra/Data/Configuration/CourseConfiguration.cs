using DalilAcademy.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Infra.Data.Configuration
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Configure the Course entity 
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.Title).IsUnique();
            builder.Property(c => c.Price).HasPrecision(10, 2);
            builder.HasOne(c => c.Instructor).WithMany(i => i.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.ImageUrl).HasMaxLength(500);
        }
    }
}
