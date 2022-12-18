using System.ComponentModel.DataAnnotations;

namespace CommandsService.Dtos;

public class CommandCreateDto
{
    [Required]
    public string HowTo { get; init; } = string.Empty;
    [Required]
    public string CommandLine { get; init; } = string.Empty;
}