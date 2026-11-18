using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Mappings
{
    public static class StudentMappings
    {
        public static StudentDto ToDto(this Student s)
            => new(s.Id, s.FullName, s.Email);

        public static Student ToEntity(this CreateStudentDto dto)
            => new() { FullName = dto.FullName, Email = dto.Email };
    }
}