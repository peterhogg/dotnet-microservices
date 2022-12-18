using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;
public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
        }
    }

    private static void SeedData(AppDbContext context)
    {

        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not run migartions: {ex.Message}");
        }
        if (context.Platforms.Any())
        {
            Console.WriteLine("Skipping seed data");
            return;
        }
        Console.WriteLine("Seeding data");
        context.Platforms.AddRange(
            new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
            new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );
        context.SaveChanges();

    }
}