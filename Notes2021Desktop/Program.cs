using System;
using WebWindows.Blazor;

namespace Notes2021Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            ComponentsDesktop.Run<Startup>("Notes2021 Desktop", "wwwroot/index.html");
        }
    }
}
