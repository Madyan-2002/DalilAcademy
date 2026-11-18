using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Mappings
{
    public static class CourseMappings
    {
        public static CourseResponseDto ToDto(this Course course)
        {
            return new CourseResponseDto(course.Id, course.Title, course.Price, course.Instructor?.FullName , course.ImageUrl);
        }

        public static Course ToEntity(this CreateCourseDto dto)
        {
            return new Course
            {
                Title = dto.Title,
                Price = dto.Price,
                InstructorId = dto.InstructorId,
                ImageUrl = dto.ImageUrl

            };
        }
    }
}
