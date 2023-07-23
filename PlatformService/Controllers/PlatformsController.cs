using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
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
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
        IPlatformRepo repo,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient
    )
    {
        this._repo = repo;
        this._mapper = mapper;
        this._commandDataClient = commandDataClient;
        this._messageBusClient = messageBusClient;
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

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        // Send Sync Message alert CommandService of the new platform
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not send syncronously: {ex.Message}");
        }

        // Send Async Message
        try
        {
            PlatformPublishedDto platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not send asyncronously: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
    }
}