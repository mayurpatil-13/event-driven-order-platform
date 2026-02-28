namespace api_gateway.authorization
{
    public static class AuthorizationPolicies
    {
        public static IServiceCollection AddAuthorizationPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("ADMIN"));
            });

            return services;
        }
    }
}