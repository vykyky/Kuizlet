using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Infrastructure.Implementations;
using Kuizlet.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kuizlet.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<ICurrentUserInfo, CurrentUserInfo>();
            services.AddScoped<ICardSetRepository, CardSetRepository>();
            services.AddScoped<IAccessRequestRepository, AccessRequestRepository>();
            services.AddScoped<ICardRepository, CardRepository>();

            return services;
        }
    }
}
