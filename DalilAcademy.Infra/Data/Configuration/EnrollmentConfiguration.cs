using DalilAcademy.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DalilAcademy.Infra.Data.Configuration
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Configure the Enrollment entity
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Student)
       .WithMany(c => c.Enrollments)
       .HasForeignKey(e => e.StudentId)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.EnrolledAt)
                   .IsRequired()
                   .HasDefaultValueSql("now()");
            builder.Property(e => e.Status)
       .HasConversion<string>()
       .HasMaxLength(20)
       .HasDefaultValue(EnrollmentStatus.Pending);

        }
    }
}
