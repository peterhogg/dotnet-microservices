using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data;
public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }
    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _context.Platforms.Add(platform);

    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .Where(command => command.PlatformId == platformId && command.Id == commandId)
            .Single();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(command => command.PlatformId == platformId);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(platform => platform.Id == platformId);
    }

    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }
}