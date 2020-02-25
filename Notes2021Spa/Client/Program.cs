/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: Program.cs
    **
    ** Description:
    **      Program main entry point - startup
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

namespace Notes2021Spa.Client
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

            // 17.4.0.51
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjE1Nzc1QDMxMzcyZTM0MmUzMFMwSDI5d0lkbGRONkloQUhsOWxVVWJWbnlReGRmR0JWVGJ5djF2b01VT1E9;MjE1Nzc2QDMxMzcyZTM0MmUzMFpTZ1RoQ2t3R3ZmU21oNnZjK1dBK0dZYTk0RzErN2JuazhhWUF2UTducDA9;MjE1Nzc3QDMxMzcyZTM0MmUzMElwZGhCWTNQUmtzQlNXTEZ5cEVVeitTZ0gxMmF6QXdHcmNsNGI1ejN3SUk9;MjE1Nzc4QDMxMzcyZTM0MmUzMFRNcVdtRjB1c3c0a3NTUlMrTnMyS0NPb1dBM1J1L3VoYWxMQ1VCTCtNVXM9;MjE1Nzc5QDMxMzcyZTM0MmUzMFcrSW9qRlRLUjNCYTMrTWI3RjR0a2xUYXR3TmQ3RjFEYkZNUnR1QWNDWHc9;MjE1NzgwQDMxMzcyZTM0MmUzMGpRNWgzTzV0d3plZGxObE5NckVxR0lLU2RBWjM0ZEVpYTBKcUlXRGwzKzA9;MjE1NzgxQDMxMzcyZTM0MmUzMFc5aDBkTG9sR1ppQ3dINi8wYXNMVHk4RSsrKzEvbFZyYXJKV1d0eXA0Vnc9;MjE1NzgyQDMxMzcyZTM0MmUzMFdabWd3d3NoOHh5M051aWFaSjUzVnBjS1V5ajZkZXdmdnRMY1J5enlNa289;MjE1NzgzQDMxMzcyZTM0MmUzMFNVUS83SEk2K08rbTJmYXJJdk12NElMb3NNcjRuQzNLN3JLYXczbVRyQXM9;NT8mJyc2IWhiZH1nfWN9ZmJoYmF8YGJ8ampqanNiYmlmamlmanMDHmggOj03NiETOj8/Oj08OiB9Njcm;MjE1Nzg0QDMxMzcyZTM0MmUzMENGNTNmV1NQZkNrMkg5enZ6bk55Q2YvWXRmQ1p6WnRjVTRuNHZPWm1WY3c9");

            builder.RootComponents.Add<App>("app");

            await builder
                .Build()
                .UseLocalTimeZone()
                .RunAsync();
        }
    }
}
