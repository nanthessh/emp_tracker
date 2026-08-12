using Dapper;
using EmpTracker.Core.DTOs;
using EmpTracker.Core.Interfaces;
using Microsoft.Data.SqlClient;

namespace EmpTracker.Infrastructure.Repositories;

public class DashboardRepository(string connectionString) : IDashboardRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<DashboardStats> GetStatsAsync(int? userId, string role)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstAsync<DashboardStats>(
            "sp_GetDashboardStats", new { UserId = userId, Role = role },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RecentTask>> GetRecentTasksAsync(int? userId, string role, int top = 5)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<RecentTask>(
            "sp_GetRecentTasks", new { UserId = userId, Role = role, Top = top },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
