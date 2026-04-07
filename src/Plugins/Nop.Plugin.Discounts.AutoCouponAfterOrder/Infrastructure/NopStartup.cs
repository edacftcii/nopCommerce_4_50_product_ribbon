using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Factories;
using Nop.Plugin.Discounts.AutoCouponAfterOrder.Services;

namespace Nop.Plugin.Discounts.AutoCouponAfterOrder.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAutoCouponService, AutoCouponService>();
            services.AddScoped<IAutoCouponModelFactory, AutoCouponModelFactory>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 3000;
    }
}