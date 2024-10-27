using BackendServer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackendTest;

public class BackendServerAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(sc =>
        {
            sc.RemoveAll(typeof(DbContextOptions<ApiDbContext>));
            sc.AddDbContext<ApiDbContext>(options => { options.UseInMemoryDatabase(_dbName); });

            var sp = sc.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
    }
}