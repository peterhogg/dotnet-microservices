using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            if (grpcClient is null)
            {
                return;
            }
            var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
            if (repo is null)
            {
                return;
            }
            var platforms = grpcClient.ReturnAllPlatforms();
            if (platforms is null)
            {
                return;
            }

            SeedData(repo, platforms);


        }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        foreach (Platform platform in platforms)
        {
            if (!repo.ExternalPlatformExists(platform.ExternalId))
            {
                repo.CreatePlatform(platform);
            }
            repo.SaveChanges();
        }
    }
}