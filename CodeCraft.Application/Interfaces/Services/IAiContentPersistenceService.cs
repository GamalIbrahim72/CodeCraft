using CodeCraft.Application.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface IAiContentPersistenceService
{
    Task SaveGeneratedContentAsync(AiRoadmapResponseDto data);
}
