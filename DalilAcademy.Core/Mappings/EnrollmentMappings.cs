using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Mappings
{
    public static class EnrollmentMappings
    {
        public static EnrollmentResponseDto ToDto(this Enrollment e)
            => new(e.Id, e.StudentId, e.Student.FullName,
                   e.CourseId, e.Course.Title, e.EnrolledAt, e.Status.ToString());
    }
}