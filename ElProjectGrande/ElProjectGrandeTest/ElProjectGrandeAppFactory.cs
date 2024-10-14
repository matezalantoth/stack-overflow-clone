using ElProjectGrande.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ElProjectGrandeTest;

public class ElProjectGrandeAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(sc =>
        {
            sc.RemoveAll(typeof(DbContextOptions<ApiDbContext>));
            sc.AddDbContext<ApiDbContext>(options => { options.UseInMemoryDatabase(_dbName); });

            var sp = sc.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContextOptions<ApiDbContext>>();

        });
    }
}