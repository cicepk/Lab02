using Dapper;
using DapperApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
namespace DapperApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connStr;
    public StudentRepository(IConfiguration configuration)
    {
        _connStr = configuration.GetConnectionString("DefaultConnection");
    }
    private IDbConnection NewConnection() 
        => new SqlConnection(_connStr);

    public IEnumerable<Student> GetAll()
    {
        using var db = NewConnection();
        return db.Query<Student>("SELECT * FROM Students");
    }
    public Student? GetById(int id)
    {
        using var db = NewConnection();
        return db.QueryFirstOrDefault<Student>("SELECT * FROM Students WHERE Id = @Id", new { Id = id });
    }
    public void Create(Student student)
    {
        using var db = NewConnection();
        db.Execute("INSERT INTO Students (Name, Age, Email) VALUES (@Name, @Age, @Email)", student);
    }
    public void Update(Student student)
    {
        using var db = NewConnection();
        db.Execute("UPDATE Students SET Name = @Name, Age = @Age, Email = @Email WHERE Id = @Id", student);
    }
    public void Delete(int id)
    {
        using var db = NewConnection();
        db.Execute("DELETE FROM Students WHERE Id = @Id", new { Id = id });
    }
    public IEnumerable<Student> SearchByName(string name)
    {
        using var db = NewConnection();
        return db.Query<Student>("SELECT * FROM Students WHERE Name LIKE @Name", new { Name = $"%{name}%" });
    }
    public IEnumerable<StudentWithCourses> GetAllWithCourses()
    {
        using var db = NewConnection();
        var studentDictionary = new Dictionary<int, StudentWithCourses>();

        db.Query<StudentWithCourses, Course, StudentWithCourses>(
            "SELECT s.Id, s.Name, s.Age, s.Email, c.Id, c.CourseName " +
            "FROM Students s " +
            "LEFT JOIN StudentCourse sc ON s.Id = sc.StudentId " +
            "LEFT JOIN Course c ON sc.CourseId = c.Id " +
            "ORDER BY s.Id",
            (student, course) =>
            {
                if (!studentDictionary.TryGetValue(student.Id, out var Existing))
                {
                    Existing = student;
                    studentDictionary.Add(student.Id, Existing);
                }

                if (course != null)
                {
                    Existing.Courses.Add(course);
                }

                return Existing;
            },
            splitOn: "CourseName"
        );

        return studentDictionary.Values;
    }
}