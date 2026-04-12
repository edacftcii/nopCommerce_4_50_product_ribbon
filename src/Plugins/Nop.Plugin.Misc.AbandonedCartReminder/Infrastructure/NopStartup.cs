using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AbandonedCartReminder.Factories;
using Nop.Plugin.Misc.AbandonedCartReminder.Services;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAbandonedCartReminderService, AbandonedCartReminderService>();
        services.AddScoped<IAbandonedCartReminderModelFactory, AbandonedCartReminderModelFactory>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 3000;
}