using Dapper;
using EmpTracker.Core.DTOs;
using EmpTracker.Core.Interfaces;
using EmpTracker.Core.Models;
using Microsoft.Data.SqlClient;

namespace EmpTracker.Infrastructure.Repositories;

public class UserRepository(string connectionString) : IUserRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "sp_GetUserByEmail", new { Email = email },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(string name, string email, string passwordHash, string role)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "sp_CreateUser", new { Name = name, Email = email, PasswordHash = passwordHash, Role = role },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<EmployeeDto>(
            "sp_GetAllEmployees",
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
