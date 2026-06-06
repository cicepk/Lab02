namespace DapperApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
    }
    public class Course
    {
        public int Id { get; set; }
        public string? CourseName { get; set; }
    }
    public class StudentWithCourses
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}