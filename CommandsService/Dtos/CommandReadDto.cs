namespace CommandsService.Dtos;

public class CommandReadDto
{
    public int Id { get; init; }
    public string HowTo { get; init; } = string.Empty;
    public string CommandLine { get; init; } = string.Empty;
    public int PlatformId { get; set; }
}