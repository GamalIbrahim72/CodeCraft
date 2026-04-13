using CodeCraft.Application.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class AiService:IAiService
{
    private readonly HttpClient _httpClient;

    public AiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    
    public async Task<object> StartExam()
    {
        var response = await _httpClient.PostAsync(
            "http://localhost:8000/start-exam", null);

        var result = await response.Content.ReadFromJsonAsync<object>();

        return result ?? new object();
    }
    public async Task<AiResponseDto> SendAnswer(AnswerDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:8000/answer",
            dto);

        var result = await response.Content.ReadFromJsonAsync<AiResponseDto>();

        if (result == null)
            throw new Exception("AI returned empty response");

        return result;

    }
}
