using DalilAcademy.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Data.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Student> builder)
        {
            // Configure the Student entity
            builder.HasKey(s => s.Id);
            builder.Property(s => s.FullName).IsRequired().HasMaxLength(100);
            builder.HasIndex(s => s.Email).IsUnique();
            builder.Property(s => s.Email).IsRequired().HasMaxLength(100);
            builder.HasMany(s => s.Enrollments)
                   .WithOne(i => i.Student)
                   .HasForeignKey(i => i.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);


           builder.HasOne(s => s.User)
                  .WithOne()
                  .HasForeignKey<Student>(s => s.UserId)
                  .OnDelete(DeleteBehavior.SetNull);

          builder.HasIndex(s => s.UserId).IsUnique();
        }

    }
}
