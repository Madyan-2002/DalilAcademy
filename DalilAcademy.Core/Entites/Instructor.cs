using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Core.Entites
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public List<Course> Courses { get; set; } = []; // Inverse navigation property to the Course entity
    }
}
