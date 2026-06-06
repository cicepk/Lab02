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
        var sql = @"
            SELECT s.Id, s.Name, c.Id, c.CourseName
            FROM Students s
            JOIN StudentCourse sc ON s.Id = sc.StudentId
            JOIN Course c ON sc.CourseId = c.Id
            ORDER BY s.Id";
        var studentDictionary = new Dictionary<int, StudentWithCourses>();
        db.Query<StudentWithCourses, Course, StudentWithCourses>(
            sql,
            (student, course) =>
            {
                if (!studentDictionary.TryGetValue(student.Id, out var existing))
                {
                    existing = student;
                    studentDictionary.Add(student.Id, existing);
                }
                if (course != null)
                {
                    existing.Courses.Add(course);
                }
                return existing;
            },
            splitOn: "Id"
        ).ToList();
        return studentDictionary.Values;
    }
}