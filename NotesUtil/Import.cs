using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notes2021Blazor.Shared;
using Notes2021Blazor.Shared.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NotesUtil
{
    public partial class Import : Form
    {
        public Import()
        {
            InitializeComponent();
        }

        private async void comboBox1_Layout(object sender, LayoutEventArgs e)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            optionsBuilder.UseSqlServer(config["ConnectionString"]);

            NotesDbContext _db = new NotesDbContext(optionsBuilder.Options);

            List<NoteFile> nfl = await _db.NoteFile
                .OrderBy(p => p.NoteFileName)
                .ToListAsync();

            if (comboBox1.Items.Count == 0)
            {
                foreach (NoteFile nf in nfl)
                {
                    comboBox1.Items.Add(nf.NoteFileName);
                }
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string notefilename = comboBox1.SelectedItem.ToString();
            string filename = textBox1.Text;

            Importer.myTextBox = textBox2;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            optionsBuilder.UseSqlServer(config["ConnectionString"]);

            NotesDbContext _db = new NotesDbContext(optionsBuilder.Options);

            NoteFile nf = _db.NoteFile.Where(p => p.NoteFileName == notefilename).SingleOrDefault();

            if (nf == null)
            {
                textBox2.Text = ("Note file" + notefilename + " not found!");
                return;
            }

            if (checkBox1.Checked)
            {
                NoteDataManager.ArchiveNoteFile(_db, nf);

                nf = _db.NoteFile.Where(p => p.NoteFileName == notefilename).SingleOrDefault();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }


            StreamReader file;
            try
            {
                file = new StreamReader(filename);
            }
            catch
            {

                textBox2.Text = ("File " + filename + " not found!");
                return;
            }

            file.Close();

            button1.Enabled = false;


            Importer imp = new Importer();

            imp.Import(_db, filename, notefilename);

            //button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string startDir = "C:\\";
                string configDir = null;

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
                configDir = config["StartDirectory"];
                if (!string.IsNullOrEmpty(configDir))
                    startDir = configDir;

                openFileDialog.InitialDirectory = startDir;
                openFileDialog.Filter = "json files (*.json)|*.json|txt files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    textBox1.Text = openFileDialog.FileName;

                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Import_Load(object sender, EventArgs e)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            string constr = config["ConnectionString"];
            int loc = constr.IndexOf("Database=");
            constr = constr.Substring(loc + 9, 25);
            constr = constr.Substring(0, constr.IndexOf(";"));

            this.Text = "Import - " + constr;
        }
    }
}

