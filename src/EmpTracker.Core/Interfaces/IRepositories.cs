using EmpTracker.Core.DTOs;
using EmpTracker.Core.Models;

namespace EmpTracker.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<int> CreateAsync(string name, string email, string passwordHash, string role);
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
}

public interface ITaskRepository
{
    Task<IEnumerable<TaskDto>> GetAllAsync(string? search, string? status, string? priority);
    Task<IEnumerable<TaskDto>> GetByUserAsync(int userId, string? search, string? status, string? priority);
    Task<TaskDto?> GetByIdAsync(int taskId);
    Task<int> CreateAsync(CreateTaskRequest request);
    Task UpdateAsync(UpdateTaskRequest request);
    Task UpdateStatusAsync(int taskId, string status);
    Task DeleteAsync(int taskId);
}

public interface IDashboardRepository
{
    Task<DashboardStats> GetStatsAsync(int? userId, string role);
    Task<IEnumerable<RecentTask>> GetRecentTasksAsync(int? userId, string role, int top = 5);
}
