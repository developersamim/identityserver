using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace common.api
{
    public static class ServiceCollectionExtensions        
    {
        public static TSettings AddAndBindConfigurationSection<TSettings>(this IServiceCollection services, IConfiguration configuration, string sectionPath)
            where TSettings : class, new()
        {
            var section = configuration.GetSection(sectionPath);
            services.Configure<TSettings>(section);
            var instance = new TSettings();
            section.Bind(instance);

            return instance;
        }
    }
}
