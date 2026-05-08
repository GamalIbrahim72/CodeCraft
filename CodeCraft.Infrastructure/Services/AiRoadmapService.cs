using CodeCraft.Application.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Services;
public class AiRoadmapService: IAiRoadmapService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiRoadmapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AiRoadmapResponseDto?> GenerateRoadmapAsync(int trackId, string userLevel)
    {
        var djangoBaseUrl = _configuration["DjangoAi:BaseUrl"];

        var request = new
        {
            trackId,
            level = userLevel
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{djangoBaseUrl}/api/roadmap/generate",
            request
        );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AiRoadmapResponseDto>();
    }
}
