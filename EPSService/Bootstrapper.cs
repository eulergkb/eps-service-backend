using ESPService.Data;

namespace ESPService;

public static class Bootstrapper
{
    public static void InitDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
