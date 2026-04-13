using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.TrackDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface ITrackService
{
    Task<IEnumerable<TrackResponse>> GetAllTracks();

    Task<TrackResponse?> GetTrack(int id);

    Task<TrackResponse> CreateTrack(TrackRequest request);
    Task<PaginatedResponse<TrackResponse>> GetPagedTracks(PaginationParameters parameters);
    Task UpdateTrack(int id, UpdateTrackRequest request);

    Task DeleteTrack(int id);
    Task EnrollAsync(int userId, int trackId);

    Task<List<Track>> GetUserTracksAsync(int userId);

}
