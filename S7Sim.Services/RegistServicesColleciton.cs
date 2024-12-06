using Microsoft.Extensions.DependencyInjection;
using S7Sim.Services.DB;
using S7Sim.Services.MB;
using S7Sim.Services.Server;

namespace S7Sim.Services
{
    public static class RegistServicesColleciton
    {
        public static void AddS7SimServices(this IServiceCollection services)
        {
            services.AddSingleton<IS7ServerService, S7ServerService>();
            services.AddSingleton<IS7DataBlockService, S7DataBlockService>();
            services.AddSingleton<IS7MBService, S7MBService>();
        }
    }
}
