using System.Net.Http.Headers;
using System.Text.Json;
using CodeCraft.Application.DTOs.AI;
using CodeCraft.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeCraft.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiService> _logger;

    public AiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AiService> logger)
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

        _logger.LogInformation("[AI Token] Status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("[AI Token] Failed to obtain AI token: {Content}", content);
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

    public async Task<List<AiQuestionDto>?> GetQuestionsAsync(string track)
    {
        var baseUrl = _configuration["DjangoAi:BaseUrl"];
        var token = await GetDjangoTokenAsync();

        if (token == null)
        {
            _logger.LogError("[AI Questions] Could not obtain token for track {Track}", track);
            return null;
        }

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/ai/questions/?track={Uri.EscapeDataString(track)}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("[AI Questions] Track: {Track}, Status: {StatusCode}", track, response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("[AI Questions] Error fetching questions: {Content}", content);
            return null;
        }

        var result = JsonSerializer.Deserialize<AiQuestionsResponseDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result?.Questions;
    }

    public async Task<EvaluateResponseDto?> EvaluateAsync(EvaluateRequestDto dto)
    {
        var baseUrl = _configuration["DjangoAi:BaseUrl"];
        var token = await GetDjangoTokenAsync();

        if (token == null)
        {
            _logger.LogError("[AI Evaluate] AI Token is null");
            return null;
        }

        var json = JsonSerializer.Serialize(dto);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl}/ai/evaluate/")
        {
            Content = new StringContent(
                json,
                System.Text.Encoding.UTF8,
                "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("[AI Evaluate] Status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("[AI Evaluate] Failed evaluation: {Content}", content);
            return null;
        }

        return JsonSerializer.Deserialize<EvaluateResponseDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}