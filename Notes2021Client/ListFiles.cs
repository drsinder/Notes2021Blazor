using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Notes2021Client
{
    public partial class ListFiles : Form
    {
        private readonly ListFiles mySelf;
        public ListFiles()
        {
            InitializeComponent();
            mySelf = this;
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var noteFiles = Actions.GetFileList(Program.MyClient);

            foreach (var file in noteFiles)
            {
                file.LastEdited = file.LastEdited.ToLocalTime();
            }

            dataGridView1.DataSource = noteFiles;
            label1.Visible = true;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            NoteFile myFile = (NoteFile)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            new ListNotes(myFile)
            {
                Visible = true
            };
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            var dataGridViewRow = ((DataGridView)(sender)).CurrentRow;
            if (dataGridViewRow != null)
            {
                NoteFile myFile = (NoteFile)dataGridViewRow.DataBoundItem;
                new ListNotes(myFile)
                {
                    Visible = true
                };
            }
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            mySelf.Width = WinObjFunctions.CountGridWidth(dataGridView1) + 90;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
