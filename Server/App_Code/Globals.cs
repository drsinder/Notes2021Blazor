using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;

namespace Notes2021Blazor.Server
{
    public class Globals : Notes2021Blazor.Shared.Globals
    {
        public static IWebHostEnvironment Env { get; set; }
    }
}
