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
        db.Execute("INSERT INTO Students (Name, Age) VALUES (@Name, @Age)", student);
    }
    public void Update(Student student)
    {
        using var db = NewConnection();
        db.Execute("UPDATE Students SET Name = @Name, Age = @Age WHERE Id = @Id", student);
    }
    public void Delete(int id)
    {
        using var db = NewConnection();
        db.Execute("DELETE FROM Students WHERE Id = @Id", new { Id = id });
    }
}