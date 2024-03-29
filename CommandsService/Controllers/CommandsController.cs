using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        if (!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }
        var commands = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        if (!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }
        var command = _repository.GetCommand(platformId, commandId);
        if (command is null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        if (!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandCreateDto);

        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtAction(
            nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = command.Id },
            commandReadDto
        );
    }
}