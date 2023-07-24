using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(
                destinationMember => destinationMember.ExternalId,
                options => options.MapFrom(sourceMember => sourceMember.Id)
            );
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(
                destinationMember => destinationMember.ExternalId,
                options => options.MapFrom(sourceMember => sourceMember.PlatformId)
            )
            .ForMember(
                destinationMember => destinationMember.Name,
                options => options.MapFrom(sourceMember => sourceMember.Name)
            )
            .ForMember(
                dest => dest.Commands, opt => opt.Ignore()
            );
    }

}