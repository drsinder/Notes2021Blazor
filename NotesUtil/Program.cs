using System;
using System.Windows.Forms;

namespace NotesUtil
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OptionsMenu());
        }
    }

    public partial class Importer : Notes2021Blazor.Shared.Import.Importer
    {
        public static TextBox myTextBox;
        public override void Output(string message)
        {
            myTextBox.Text += "   " + message;
        }
    }

}
