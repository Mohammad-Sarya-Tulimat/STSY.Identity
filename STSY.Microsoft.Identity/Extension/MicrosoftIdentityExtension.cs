using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.DBContext;
using STSY.Microsoft.Identity.Services;
using STSY.Microsoft.Identity.Services.Authenticators;
using System;

namespace STSY.Microsoft.Identity.Extension
{
    public static class MicrosoftIdentityExtension
    {
        public static IServiceCollection AddSTSYMicrosoftIdentity(this IServiceCollection services, Action<IdentityOptions> identityOptions, Action<DbContextOptionsBuilder> action)
        {
            services.AddDbContext<STSYIdentityDbContext>(action);
            services.AddIdentityCore<MicrosoftIdentityUser>(identityOptions)
                  .AddSignInManager()
                  .AddRoles<MicrosoftIdentityRole>()
                  .AddEntityFrameworkStores<STSYIdentityDbContext>()
                  .AddDefaultTokenProviders();
            services.AddDataProtection();

            AddSTSYMicrosoftIdentityServices(services);
            return services;
        }
        public static IServiceCollection AddSTSYMicrosoftIdentity<T>(this IServiceCollection services, Action<IdentityOptions> identityOptions, Action<DbContextOptionsBuilder> action) where T : STSYIdentityDbContext
        {
            services.AddIdentityCore<T>(identityOptions)
                  .AddSignInManager()
                  .AddRoles<MicrosoftIdentityRole>()
                  .AddEntityFrameworkStores<T>()
                  .AddDefaultTokenProviders();
            AddSTSYMicrosoftIdentityServices(services);
            return services;
        }
        /// <summary>
        /// you must call services.AddDbContext out of this method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="identityOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddSTSYMicrosoftIdentity<T>(this IServiceCollection services, Action<IdentityOptions> identityOptions) where T : STSYIdentityDbContext
        {
            services.AddIdentityCore<MicrosoftIdentityUser>(identityOptions)
                  .AddSignInManager()
                  .AddRoles<MicrosoftIdentityRole>()
                  .AddEntityFrameworkStores<T>()
                  .AddDefaultTokenProviders();
            AddSTSYMicrosoftIdentityServices(services);
            return services;
        }

        private static IServiceCollection AddSTSYMicrosoftIdentityServices(IServiceCollection services)
        {
            services.AddScoped<IReadUsers, MicrosoftIdentityUserRepo>();
            services.AddScoped<ISessionStorage, SessionStorage>();
            services.AddScoped<IPasswordManager, MicrosoftIdentityUserManager>();
            services.AddScoped<IUserManager, MicrosoftIdentityUserManager>();
            services.AddScoped<ITwoFactorManager, MicrosoftIdentityUserManager>();
            services.AddScoped<IPassKeyManager, PassKeyAuthenticator>();
            #region Authenticators 
            services.AddScoped<IAuthenticator, PasswordAuthenticator>();
            services.AddScoped<IAuthenticator, PassKeyAuthenticator>();
            services.AddScoped<IMFAuthenticator, EmailOTPAuthenticator>();
            services.AddScoped<IMFAuthenticator, OTPRecoveryAuthenticator>();
            services.AddScoped<IMFAuthenticator, PassKeyAuthenticator>();
            services.AddScoped<IMFAuthenticator, SMSOTPAuthenticator>();
            services.AddScoped<IMFAuthenticator, TOTPAuthenticator>();
            services.AddScoped<IChallengeAuthenticator, PassKeyAuthenticator>();
            services.AddScoped<IChallengeAuthenticator, EmailOTPAuthenticator>();
            services.AddScoped<IChallengeAuthenticator, SMSOTPAuthenticator>();
            #endregion 
            return services;
        }
        public static IServiceCollection UseSTSYMicrosoftIdentitySessionManager(this IServiceCollection services)
        {
            services.AddScoped<ISessionManager, ASPIdentitySession>();
            return services;
        }
    }
}
