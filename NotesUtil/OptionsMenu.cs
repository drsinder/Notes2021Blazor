using Microsoft.Extensions.Configuration;
using System;
using System.Windows.Forms;

namespace NotesUtil
{
    public partial class OptionsMenu : Form
    {
        public OptionsMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Import dlg = new Import();

            dlg.ShowDialog();
        }

        private void OptionsMenu_Load(object sender, EventArgs e)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            string constr = config["ConnectionString"];
            int loc = constr.IndexOf("Database=");
            constr = constr.Substring(loc + 9, 25);
            constr = constr.Substring(0, constr.IndexOf(";"));

            this.Text = "NotesUtil Menu - " + constr;

        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    EditAccess dlg = new EditAccess();

        //    dlg.ShowDialog();
        //}
    }
}
