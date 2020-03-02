using Microsoft.Extensions.DependencyInjection;
using WebWindows.Blazor;
using Notes2021Spa.RCL;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Modal;
using Blazored.LocalStorage;

namespace Notes2021Desktop
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredModal();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            services.AddScoped<IAuthService, AuthService>();

        }

        public void Configure(DesktopApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
