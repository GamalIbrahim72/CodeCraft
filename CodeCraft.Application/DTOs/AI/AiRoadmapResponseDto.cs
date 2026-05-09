using System.Text.Json.Serialization;

namespace CodeCraft.Application.DTOs.AI;

public class AiRoadmapResponseDto
{
    [JsonPropertyName("data")]
    public AiRoadmapDataDto? Data { get; set; }
}

public class AiRoadmapDataDto
{
    [JsonPropertyName("track")]
    public string? Track { get; set; }

    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("roadmap")]
    public List<RoadmapStepDto> Roadmap { get; set; } = new();
}

public class RoadmapStepDto
{
    [JsonPropertyName("step")]
    public int Step { get; set; }

    [JsonPropertyName("topic_id")]
    public string? TopicId { get; set; }

    [JsonPropertyName("main_topic")]
    public string? MainTopic { get; set; }

    [JsonPropertyName("lessons")]
    public List<RoadmapLessonDto> Lessons { get; set; } = new();
}

public class RoadmapLessonDto
{
    [JsonPropertyName("lesson_id")]
    public string? LessonId { get; set; }

    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("subtopic")]
    public string? Subtopic { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("resources")]
    public RoadmapResourcesDto? Resources { get; set; }
}

public class RoadmapResourcesDto
{
    [JsonPropertyName("video")]
    public string? Video { get; set; }

    [JsonPropertyName("article")]
    public string? Article { get; set; }
}