using System.Diagnostics.CodeAnalysis;
using HarpDataSync.Application.Constants;
using HarpDataSync.Infrastructure;
using HarpDataSync.Infrastructure.Repositories;
using HarpDataSync.Startup.Configuration;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.Contracts.Services;
using OldIrasSyncProjectData.Functions;
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

        builder.Services.AddHeaderPropagation(options => options.Headers.Add(RequestHeadersKeys.CorrelationId));

        // register dependencies
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IOldIrasProjectRepository>(sp =>
        {
            return new OldIrasProjectRepository(config.GetConnectionString("BGOHARPConnectionString")!);
        });

        builder.Services.AddDbContext<HarpProjectDataDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("HarpProjectDataConnectionString"));
            options.EnableSensitiveDataLogging();
        });

        builder.Services.AddScoped<IHarpDataSyncService, HarpDataSyncService>();
        builder.Services.AddScoped<IHarpProjectDataRepository, HarpProjectDataRepository>();
        builder.Services.AddScoped<HarpDataSyncFunction>();

        builder.Services.AddHttpContextAccessor();

        if (!builder.Environment.IsDevelopment())
        {
            // Load configuration from Azure App Configuration

            builder.Services.AddAzureAppConfiguration(config);
        }

        var app = builder.Build();

        await app.RunAsync();
    }
}