using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBudgetManagement.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace ProjectBudgetManagement.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public IntegrationTestWebAppFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<ProjectBudgetDbContext>));
            services.RemoveAll(typeof(ProjectBudgetDbContext));

            // Add DbContext with test container connection string
            services.AddDbContext<ProjectBudgetDbContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });
    }
}
