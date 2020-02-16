using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace NotesUtil
{
    public partial class EditAccess : Form
    {
        public EditAccess()
        {
            InitializeComponent();
        }

        private void checkedListBox1_Layout(object sender, LayoutEventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            string notefilename = comboBox1.SelectedItem.ToString();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            optionsBuilder.UseSqlServer(config["ConnectionString"]);

            NotesDbContext _db = new NotesDbContext(optionsBuilder.Options);

            NoteFile nf = _db.NoteFile.Where(p => p.NoteFileName == notefilename).SingleOrDefault();




        }
    }
}
