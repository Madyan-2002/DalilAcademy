using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Core.Entites
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; } // Foreign key to the Student entity
        public Student? Student { get; set; } // Navigation property to the Student entity
        public int CourseId { get; set; } // Foreign key to the Course entity
        public Course? Course { get; set; } // Navigation property to the Course entity
        public DateTime EnrolledAt { get; set; } // Date of enrollment
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
        public List<Payment> Payments { get; set; } = [];

    }
}
