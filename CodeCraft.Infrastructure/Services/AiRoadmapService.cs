using System.Net.Http.Headers;
using System.Text.Json;
using CodeCraft.Application.DTOs.AI;
using Microsoft.Extensions.Configuration;

namespace CodeCraft.Infrastructure.Services;

public class AiRoadmapService : IAiRoadmapService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiRoadmapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<string?> GetDjangoTokenAsync()
    {
        var baseUrl = _configuration["DjangoAi:BaseUrl"];
        var username = _configuration["DjangoAi:Username"];
        var password = _configuration["DjangoAi:Password"];

        var formData = new Dictionary<string, string>
        {
            { "username", username! },
            { "password", password! }
        };

        var response = await _httpClient.PostAsync(
            $"{baseUrl}/api/token/",
            new FormUrlEncodedContent(formData));

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[AI Roadmap Token] Status: {response.StatusCode}");
        Console.WriteLine($"[AI Roadmap Token] Response: {content}");

        if (!response.IsSuccessStatusCode)
            return null;

        var token = JsonSerializer.Deserialize<DjangoTokenResponseDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return token?.Access;
    }

    public async Task<AiRoadmapResponseDto?> GenerateRoadmapAsync(
        string track,
        string level)
    {
        var baseUrl = _configuration["DjangoAi:BaseUrl"];

        var token = await GetDjangoTokenAsync();

        if (token == null)
            return null;

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/ai/roadmap/?track={Uri.EscapeDataString(track)}&level={Uri.EscapeDataString(level)}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[AI Roadmap] Status: {response.StatusCode}");
        Console.WriteLine($"[AI Roadmap] Response: {content}");

        if (!response.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<AiRoadmapResponseDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}