namespace DalilAcademy.Core.Entites
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public int InstructorId { get; set; }
        public string? ImageUrl { get; set; } // Optional property for the course image URL
        public Instructor? Instructor { get; set; } // Navigation property to the Instructor entity 
        public List<Enrollment> Enrollments { get; set; } = []; // Navigation property to the Enrollment entity

    }
}
