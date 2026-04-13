using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.TrackDTOs;
using CodeCraft.Application.Interfaces.Repositories;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class TrackService: ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly IMapper _mapper;
    private readonly IUserTrackRepository _userTrackRepository;

    public TrackService(ITrackRepository trackRepository, IMapper mapper, IUserTrackRepository userTrackRepository)
    {
        _trackRepository = trackRepository;
        _mapper = mapper;
        _userTrackRepository = userTrackRepository;
    }

    public async Task<IEnumerable<TrackResponse>> GetAllTracks()
    {
        var tracks = await _trackRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<TrackResponse>>(tracks);
    }

    public async Task<TrackResponse?> GetTrack(int id)
    {
        var track = await _trackRepository.GetByIdAsync(id);

        return track == null ? null : _mapper.Map<TrackResponse>(track);
    }

    public async Task<TrackResponse> CreateTrack(TrackRequest request)
    {
        var track = _mapper.Map<Track>(request);

        await _trackRepository.AddAsync(track);

        return _mapper.Map<TrackResponse>(track);
    }

    public async Task<PaginatedResponse<TrackResponse>> GetPagedTracks(PaginationParameters parameters)
    {
        var (tracks, totalCount) =
            await _trackRepository.GetPagedAsync(parameters.Page, parameters.PageSize);

        var mapped = _mapper.Map<IEnumerable<TrackResponse>>(tracks);

        return new PaginatedResponse<TrackResponse>
        {
            Data = mapped,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount
        };
    }



    public async Task UpdateTrack(int id, UpdateTrackRequest request)
    {
        var track = await _trackRepository.GetByIdAsync(id);

        if (track == null)
            throw new Exception("Track not found");

        track.Name = request.Name;
        track.Description = request.Description;

        await _trackRepository.UpdateAsync(track);
    }



    public async Task DeleteTrack(int id)
    {
        var track = await _trackRepository.GetByIdAsync(id);

        if (track == null)
            throw new Exception("Track not found");

        await _trackRepository.DeleteAsync(track);
    }

    public async Task EnrollAsync(int userId, int trackId)
    {
        var track = await _trackRepository.GetByIdAsync(trackId);

        if (track == null)
            throw new Exception("Track not found");

        await _userTrackRepository.EnrollUserAsync(userId, trackId);
    }

    public async Task<List<Track>> GetUserTracksAsync(int userId)
    {
        return await _userTrackRepository.GetUserTracksAsync(userId);
    }

}
