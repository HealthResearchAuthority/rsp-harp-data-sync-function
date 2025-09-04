using System.Diagnostics.CodeAnalysis;
using HarpDataSync.Infrastructure;
using HarpDataSync.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.Contracts.Services;
using OldIrasSyncProjectData.Services;

namespace OldIrasSyncProjectData.Startup;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();

        var config = builder.Configuration;

        config
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables();

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IOldIrasProjectRepository>(sp =>
        {
            return new OldIrasProjectRepository(config.GetConnectionString("OldIrasConnectionString")!);
        });

        builder.Services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("HarpProjectDataConnectionString"));
        });

        builder.Services.AddScoped<IHarpDataSyncService, HarpDataSyncService>();
        builder.Services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        await app.RunAsync();
    }
}