using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformsController(
        IPlatformRepo repo,
        IMapper mapper,
        ICommandDataClient commandDataClient
    )
    {
        this._repo = repo;
        this._mapper = mapper;
        this._commandDataClient = commandDataClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platforms = _repo.GetPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = _repo.GetPlatformById(id);
        if (platform is null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformCreateDto)
    {
        var platform = _mapper.Map<Platform>(platformCreateDto);
        _repo.CreatePlatform(platform);
        _repo.SaveChanges();

        var platformDto = _mapper.Map<PlatformReadDto>(platform);

        // Alert CommandService of the new platform
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not send syncronously: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetPlatformById), new { id = platformDto.Id }, platformDto);
    }
}