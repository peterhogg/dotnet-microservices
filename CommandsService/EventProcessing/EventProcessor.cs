using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
        EventType eventType = DetermineEvent(message);
        switch (eventType)
        {
            case (EventType.PlatformPublished):
                addPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        return (
            eventType is not null &&
            eventType.Event == "Platform_Published"
        )
        ? EventType.PlatformPublished
        : EventType.Unknown;
    }

    private void addPlatform(string platformPublishedEventSerialzed)
    {
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedEventSerialzed);
        if (platformPublishedDto is null)
        {
            return;
        }
        using (var scope = _scopeFactory.CreateScope())
        {
            ICommandRepo repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"Platform already exists with ID ${platform.ExternalId}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add platform to DB{ex.Message}");
            }
        }

    }
}

enum EventType
{
    PlatformPublished,
    Unknown
}