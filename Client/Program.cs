using Microsoft.AspNetCore.Blazor.Hosting;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.EJ2.Blazor;

namespace Notes2021Blazor.Client
{
    public class Program
    {
 

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddOptions();
            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddSyncfusionBlazor();

            // 17.4.0.49
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjEwODgyQDMxMzcyZTM0MmUzMEFiMDlQVmRINkpDdW5GcWdCTmhsZ250d0NBNXczeUIwSzl1TXhVTFFzSzQ9;MjEwODgzQDMxMzcyZTM0MmUzMFFwcEVPVzlxZ3J6WmZ0d21LVWtUTXpwQ3VFOWMyY3BYSkdrY2FpMktRK3M9;MjEwODg0QDMxMzcyZTM0MmUzMGEzbVE3OVVFWGo3alFQa3MvamdSck5ON09WVjl1ak5xQm9aRDRMYnRtSjA9;MjEwODg1QDMxMzcyZTM0MmUzMENSWnBVcUNmV0pNMjd4WW9nUDhady9mQ2dwZVhxa3Z4U05GYzZuS2U4ZVU9;MjEwODg2QDMxMzcyZTM0MmUzMFVHOWFTZC9HMjI2a1FlZE5MS3gyYU53OWFWdERmblg0TDN0bC9ZTTIvZWc9;MjEwODg3QDMxMzcyZTM0MmUzME9ZclpGdjZQSmNScjZlY2N1VmxOU3ZpL2ZhNWZndllzOHlkTXdKQTZhU1E9;MjEwODg4QDMxMzcyZTM0MmUzMG4rRkNOSGJvK2NYZWhTbEJ5NlNXYUdGb25xc0pOcHhNRURycjRPdmd4Mnc9;MjEwODg5QDMxMzcyZTM0MmUzMFlGNlVqZ1ozZ25xdjYwWllQVEJzRktFYWt3WHJmeDJXYmk3eXlMbUg5cTQ9;MjEwODkwQDMxMzcyZTM0MmUzMGdHQ29FUnNSS1U3N3Z0SHZlL2piZ05HcU0wdU5sZjlId0FFd0tiTndlM009;NT8mJyc2IWhiZH1nfWN9Z2poYmF8YGJ8ampqanNiYmlmamlmanMDHmggOj03NiETOj8/Oj08OiB9Njcm;MjEwODkxQDMxMzcyZTM0MmUzMGpyV0QzRmpITTlSWEs1RTZ2VXZDZmJLd0lsWm04VFZSVlltd3ZyRkdxam89");
    
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
