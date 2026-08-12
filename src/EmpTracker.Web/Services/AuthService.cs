using Blazored.LocalStorage;
using EmpTracker.Core.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EmpTracker.Web.Services;

public class AuthService(HttpClient http, ILocalStorageService localStorage)
{
    private const string TokenKey = "jwt_token";
    private const string UserKey  = "user_info";

    public async Task<(bool Success, string? Error)> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, err?.Message ?? "Invalid email or password.");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null) return (false, "Invalid server response.");

            await localStorage.SetItemAsync(TokenKey, result.Token);
            await localStorage.SetItemAsync(UserKey, result);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await localStorage.RemoveItemAsync(TokenKey);
            await localStorage.RemoveItemAsync(UserKey);
        }
        catch { }
    }

    public async Task<LoginResponse?> GetCurrentUserAsync()
    {
        try
        {
            return await localStorage.GetItemAsync<LoginResponse>(UserKey);
        }
        catch { return null; }
    }

    public async Task SetAuthHeaderAsync(HttpClient client)
    {
        try
        {
            var token = await localStorage.GetItemAsStringAsync(TokenKey);
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Trim('"'));
        }
        catch { }
    }

    private record ErrorResponse(string Message);
}
