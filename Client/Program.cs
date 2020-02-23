/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: Program.cs
    **
    ** Description:
    **      Program main enrtyp point - startup
    **
    ** This program is free software: you can redistribute it and/or modify
    ** it under the terms of the GNU General Public License version 3 as
    ** published by the Free Software Foundation.   
    **
    ** This program is distributed in the hope that it will be useful,
    ** but WITHOUT ANY WARRANTY; without even the implied warranty of
    ** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    ** GNU General Public License version 3 for more details.
    **
    **  You should have received a copy of the GNU General Public License
    **  version 3 along with this program in file "license-gpl-3.0.txt".
    **  If not, see<http: //www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/


using Microsoft.AspNetCore.Blazor.Hosting;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.EJ2.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

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

            // 17.4.0.50
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjEzMTQ4QDMxMzcyZTM0MmUzMGx5SlNFVXdpdTFGMnBCQU0vOU9yRytkcWxkVG5wTkN4aUZRYzBjR2V3TGM9;MjEzMTQ5QDMxMzcyZTM0MmUzMGk1VXFYZTdMS016U1ZTTU1VYlBuOW0wRG9JK1Z1bm52OGtKSnpsdVM5RFk9;MjEzMTUwQDMxMzcyZTM0MmUzMGxuRDE5c2NRRkdhR3pBQmRuUi96KzJxMDRZb3VHVTdhMXBFaHpvM0lPUXc9;MjEzMTUxQDMxMzcyZTM0MmUzMEVRNkV4bjFwMmhGeGNhai9Vcmp2eENhMFNWaitjZUxUWkYzSzJ3bjAwN009;MjEzMTUyQDMxMzcyZTM0MmUzMFdaM0dVU1pIU28wcDZTdEdlZFB6WU1oaHJsV3ZURDJFWkFTcmNnNGk0OTA9;MjEzMTUzQDMxMzcyZTM0MmUzME1ZWGFRVFBtaXRKSlR0NmlmRytDeHlORm5Ca3lkanpzalBKSCszbnI4T289;MjEzMTU0QDMxMzcyZTM0MmUzMEV0Q28vWUk2OEpPWFN4VjlGYnBnNW04ajJwbThhQnRWWGVxaGIxNW9QNDg9;MjEzMTU1QDMxMzcyZTM0MmUzMGI1N0ZrelJWVlhyUGplZmZNWStwYUF1UUMrQUlXbzRjWm9ianlKTEJCOUE9;MjEzMTU2QDMxMzcyZTM0MmUzME5qK1NXRDM0ZWd3QVNXTHN0UzNHaCtTbCtHd2dRU1F4YVNvOFlYbFJwOUU9;NT8mJyc2IWhiZH1nfWN9ZmNoYmF8YGJ8ampqanNiYmlmamlmanMDHmggOj03NiETOj8/Oj08OiB9Njcm;MjEzMTU3QDMxMzcyZTM0MmUzMG15SEwzblV6Y05KcjBkNTdmNUlhb1BDY1c2K3ZKay9GOWc0LzltRlpZOVU9");
    
            builder.RootComponents.Add<App>("app");

            await builder
                .Build()
                .UseLocalTimeZone()
                .RunAsync();
        }
    }
}
