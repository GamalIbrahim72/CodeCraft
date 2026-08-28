using System.Net.Http.Headers;
using System.Text.Json;
using CodeCraft.Application.DTOs.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeCraft.Infrastructure.Services;

public class AiRoadmapService : IAiRoadmapService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiRoadmapService> _logger;

    public AiRoadmapService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AiRoadmapService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<string?> GetDjangoTokenAsync()
    {
        var baseUrl = _configuration["DjangoAi:BaseUrl"];
        var username = _configuration["DjangoAi:Username"];
        var password = _configuration["DjangoAi:Password"];

        var formData = new Dictionary<string, string>
        {
            { "username", username ?? string.Empty },
            { "password", password ?? string.Empty }
        };

        var response = await _httpClient.PostAsync(
            $"{baseUrl}/api/token/",
            new FormUrlEncodedContent(formData));

        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("[AI Roadmap Token] Status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("[AI Roadmap Token] Failed to obtain token: {Content}", content);
            return null;
        }

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
        {
            _logger.LogError("[AI Roadmap] AI Token is null for track {Track}", track);
            return null;
        }

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/ai/roadmap/?track={Uri.EscapeDataString(track)}&level={Uri.EscapeDataString(level)}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("[AI Roadmap] Status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("[AI Roadmap] Failed to generate roadmap: {Content}", content);
            return null;
        }

        return JsonSerializer.Deserialize<AiRoadmapResponseDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}