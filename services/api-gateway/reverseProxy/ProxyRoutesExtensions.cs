namespace api_gateway.reverseProxy
{
    public static class ProxyRoutesExtensions
    {
        public static IServiceCollection AddProxyRoutes(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddReverseProxy()
                .LoadFromConfig(config.GetSection("ReverseProxy"));

            return services;
        }
    }
}