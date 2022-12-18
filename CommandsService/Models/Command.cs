using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models;
public class Command
{
    [Key]
    [Required]
    public int Id { get; init; }
    [Required]
    public string HowTo { get; init; } = string.Empty;
    [Required]
    public string CommandLine { get; init; } = string.Empty;
    [Required]
    public int PlatformId { get; set; }
    public Platform? Platform { get; init; }
}