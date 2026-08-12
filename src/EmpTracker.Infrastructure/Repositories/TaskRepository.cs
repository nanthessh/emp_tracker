using Dapper;
using EmpTracker.Core.DTOs;
using EmpTracker.Core.Interfaces;
using Microsoft.Data.SqlClient;

namespace EmpTracker.Infrastructure.Repositories;

public class TaskRepository(string connectionString) : ITaskRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<IEnumerable<TaskDto>> GetAllAsync(string? search, string? status, string? priority)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<TaskDto>(
            "sp_GetAllTasks",
            new { Search = search, Status = status, Priority = priority },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TaskDto>> GetByUserAsync(int userId, string? search, string? status, string? priority)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<TaskDto>(
            "sp_GetTasksByUser",
            new { UserId = userId, Search = search, Status = status, Priority = priority },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<TaskDto?> GetByIdAsync(int taskId)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TaskDto>(
            "sp_GetTaskById", new { TaskId = taskId },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(CreateTaskRequest request)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "sp_CreateTask",
            new { request.Title, request.Description, request.AssignedTo, request.Priority, request.Status, request.DueDate },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(UpdateTaskRequest request)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "sp_UpdateTask",
            new { request.TaskId, request.Title, request.Description, request.AssignedTo, request.Priority, request.Status, request.DueDate },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task UpdateStatusAsync(int taskId, string status)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "sp_UpdateTaskStatus", new { TaskId = taskId, Status = status },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int taskId)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "sp_DeleteTask", new { TaskId = taskId },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
