using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notes2021Client
{
    static class Program
    {
        public static readonly HttpClient MyClient = new HttpClient();
        public static string AuthToken;
        public static string baseUri;
        public static string DefaultLogin = string.Empty;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjEwODMxQDMxMzcyZTM0MmUzMEFiMDlQVmRINkpDdW5GcWdCTmhsZ250d0NBNXczeUIwSzl1TXhVTFFzSzQ9;MjEwODMyQDMxMzcyZTM0MmUzMGEzbVE3OVVFWGo3alFQa3MvamdSck5ON09WVjl1ak5xQm9aRDRMYnRtSjA9;MjEwODMzQDMxMzcyZTM0MmUzMENSWnBVcUNmV0pNMjd4WW9nUDhady9mQ2dwZVhxa3Z4U05GYzZuS2U4ZVU9;MjEwODM0QDMxMzcyZTM0MmUzMFVHOWFTZC9HMjI2a1FlZE5MS3gyYU53OWFWdERmblg0TDN0bC9ZTTIvZWc9;MjEwODM1QDMxMzcyZTM0MmUzME9ZclpGdjZQSmNScjZlY2N1VmxOU3ZpL2ZhNWZndllzOHlkTXdKQTZhU1E9;MjEwODM2QDMxMzcyZTM0MmUzMGdHQ29FUnNSS1U3N3Z0SHZlL2piZ05HcU0wdU5sZjlId0FFd0tiTndlM009;MjEwODM3QDMxMzcyZTM0MmUzMGpyV0QzRmpITTlSWEs1RTZ2VXZDZmJLd0lsWm04VFZSVlltd3ZyRkdxam89");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            DefaultLogin = config["DefaultLogin"];
            baseUri = config["Server"];
            MyClient.BaseAddress = new Uri(baseUri);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Login login = new Login();
            tryagain:
            if (login.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(login.token))
                {
                    login.RestPassword();
                    goto tryagain;
                }

                AuthToken = login.token;
                MyClient.DefaultRequestHeaders.Add("authentication", login.token);
                Application.Run(new ListFiles());
            }


        }
    }
}
