using Microsoft.AspNetCore.Identity.UI.Services;

namespace Notes2021
{
    public partial class Globals : Notes2021Blazor.Shared.Globals
    {
        public static IEmailSender EmailSender { get; set; }

     }
}
