using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CodeCraft.Application.DTOs.AI;
using Microsoft.Extensions.Configuration;

namespace CodeCraft.Infrastructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiService(HttpClient httpClient, IConfiguration configuration)
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

        Console.WriteLine($"[AI Token] Status: {response.StatusCode}");
        Console.WriteLine($"[AI Token] Response: {content}");

        if (!response.IsSuccessStatusCode)
            return null;

        var token = System.Text.Json.JsonSerializer.Deserialize<DjangoTokenResponseDto>(
            content,
            new System.Text.Json.JsonSerializerOptions
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
            return null;

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/ai/questions/?track={Uri.EscapeDataString(track)}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[AI Questions] Status: {response.StatusCode}");
        Console.WriteLine($"[AI Questions] Response: {content}");

        if (!response.IsSuccessStatusCode)
            return null;

        var result = System.Text.Json.JsonSerializer.Deserialize<AiQuestionsResponseDto>(
            content,
            new System.Text.Json.JsonSerializerOptions
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
            Console.WriteLine("[AI Evaluate] Token is NULL");
            return null;
        }

        var json = System.Text.Json.JsonSerializer.Serialize(
            dto,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        Console.WriteLine($"[AI Evaluate] Sent Body: {json}");

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl}/ai/evaluate/")
        {
            Content = new StringContent(
                json,
                System.Text.Encoding.UTF8,
                "application/json")
        };

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[AI Evaluate] Status: {response.StatusCode}");
        Console.WriteLine($"[AI Evaluate] Response: {content}");

        if (!response.IsSuccessStatusCode)
            return null;

        return System.Text.Json.JsonSerializer.Deserialize<EvaluateResponseDto>(
            content,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}