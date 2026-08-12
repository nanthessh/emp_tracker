using EmpTracker.Core.DTOs;
using System.Net.Http.Json;

namespace EmpTracker.Web.Services;

public class TaskApiService(HttpClient http, AuthService auth)
{
    private async Task SetAuth() => await auth.SetAuthHeaderAsync(http);

    public async Task<List<TaskDto>> GetTasksAsync(string? search = null, string? status = null, string? priority = null)
    {
        try
        {
            await SetAuth();
            var url = $"api/tasks?search={search}&status={status}&priority={priority}";
            var res = await http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return [];
            return await res.Content.ReadFromJsonAsync<List<TaskDto>>() ?? [];
        }
        catch { return []; }
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        try
        {
            await SetAuth();
            var res = await http.GetAsync("api/users/task");
            if (!res.IsSuccessStatusCode) return [];
            return await res.Content.ReadFromJsonAsync<List<EmployeeDto>>() ?? [];
        }
        catch { return []; }
    }

    public async Task<bool> CreateTaskAsync(CreateTaskRequest request)
    {
        try
        {
            await SetAuth();
            var res = await http.PostAsJsonAsync("api/tasks", request);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request)
    {
        try
        {
            await SetAuth();
            var res = await http.PutAsJsonAsync($"api/tasks/{id}", request);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        try
        {
            await SetAuth();
            var res = await http.PatchAsJsonAsync($"api/tasks/{id}/status", new UpdateStatusRequest(status));
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        try
        {
            await SetAuth();
            var res = await http.DeleteAsync($"api/tasks/{id}");
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        try
        {
            await SetAuth();
            var res = await http.GetAsync("api/dashboard/stats");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<DashboardStats>();
        }
        catch { return null; }
    }

    public async Task<List<RecentTask>> GetRecentTasksAsync()
    {
        try
        {
            await SetAuth();
            var res = await http.GetAsync("api/dashboard/recent");
            if (!res.IsSuccessStatusCode) return [];
            return await res.Content.ReadFromJsonAsync<List<RecentTask>>() ?? [];
        }
        catch { return []; }
    }
}
