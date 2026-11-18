using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Core.Entites
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; } // Optional phone number
        public string? Address { get; set; } // Optional address
        public List<Enrollment> Enrollments { get; set; } = []; // Navigation property to the Enrollment entity
        public int? UserId { get; set; }
        public User? User { get; set; }

    }
}
