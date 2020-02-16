using Notes2021Blazor.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Notes2021Client
{
    public partial class ListNotes : Form, IRelistAble
    {
        private readonly NoteFile MyFile;
        private NoteAccess MyAccess;
        private readonly ListNotes mySelf;

        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight; //Used to get/set the datagridview cell height
        int iTotalWidth; //
        int iRow;//Used as counter
        bool bFirstPage; //Used to check whether we are printing first page
        bool bNewPage;// Used to check whether we are printing a new page
        int iHeaderHeight; //Used for the header height

        public ListNotes(NoteFile myFile)
        {
            InitializeComponent();
            mySelf = this;

            MyFile = myFile;

            label1.Text = MyFile.NoteFileName;
            label2.Text = MyFile.NoteFileTitle;

            List<NoteHeader> noteHeader = Actions.GetNoteList(Program.MyClient, MyFile.Id);
            MyAccess = Actions.GetAccess(Program.MyClient, MyFile.Id);

            button1.Enabled = MyAccess.Write;

            int count = 0;
            foreach (var header in noteHeader)
            {
                header.CreateDate = header.CreateDate.ToLocalTime();
                count++;
            }

            label3.Text = count + @"  Base Notes";

            dataGridView1.DataSource = noteHeader;

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[14].Visible = false;
            dataGridView1.Columns[15].Visible = false;

            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[13].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            NoteHeader myHeader = (NoteHeader)dataGridView1.Rows[e.RowIndex].DataBoundItem;

            new DisplayNote(MyFile, myHeader.NoteOrdinal, this)
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
                NoteHeader myHeader = (NoteHeader)dataGridViewRow.DataBoundItem;
                new DisplayNote(MyFile, myHeader.NoteOrdinal, this)
                {
                    Visible = true
                };
            }
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            mySelf.Width = WinObjFunctions.CountGridWidth(dataGridView1) + 90;
        }


        public void Relist()
        {
            List<NoteHeader> noteHeader = Actions.GetNoteList(Program.MyClient, MyFile.Id);
            MyAccess = Actions.GetAccess(Program.MyClient, MyFile.Id);

            button1.Enabled = MyAccess.Write;

            int count = 0;
            foreach (var header in noteHeader)
            {
                header.CreateDate = header.CreateDate.ToLocalTime();
                count++;
            }

            label3.Text = count + @"  Base Notes";

            dataGridView1.DataSource = noteHeader;

            dataGridView1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateNote(MyFile, 0, null, null, null, this)
            {
                Visible = true
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Relist();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ext = ".txt";
            bool isHTML = false;

            if (sender == button3)
            {
                ext = ".html";
                isHTML = true;
            }

            Stream myStream = Actions.Export(Program.MyClient, MyFile.Id, isHTML);

            string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            docsPath += "\\NotesFile-" + MyFile.NoteFileName + ext;

            FileStream file = new FileStream(docsPath, FileMode.Create, FileAccess.Write);
            myStream.CopyToAsync(file).GetAwaiter();

            Thread.Sleep(1000);

            file.Close();
            myStream.Close();

            MessageBox.Show(docsPath, @"See your Documents folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            printDialog1.UseEXDialog = true;

            if (DialogResult.OK == printDialog1.ShowDialog())
            {
                printDocument1.DocumentName = MyFile.NoteFileTitle;
                printDocument1.Print();
            }
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                // Calculating Total Widths
                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dataGridView1.Columns)
                {
                    if (dgvGridCol.Visible)
                        iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                //Set the left margin
                int iLeftMargin = e.MarginBounds.Left;
                //Set the top margin
                int iTopMargin = e.MarginBounds.Top;
                //Whether more pages have to print or not
                bool bMorePagesToPrint = false;
                int iTmpWidth;

                //For the first page to print set the cell width and header height
                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                    {
                        if (!GridCol.Visible)
                            continue;

                        iTmpWidth = (int)(Math.Floor(GridCol.Width /
                                                     (double)iTotalWidth * iTotalWidth *
                                                     (e.MarginBounds.Width / (double)iTotalWidth)));

                        if (GridCol.InheritedStyle != null)
                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                                GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                        // Save width and height of headres
                        arrColumnLefts.Add(iLeftMargin);
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }
                //Loop till all the grid rows not get printed
                while (iRow <= dataGridView1.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = dataGridView1.Rows[iRow];
                    //Set the cell height
                    iCellHeight = GridRow.Height + 5;
                    int iCount = 0;
                    //Check whether the current page settings allo more rows to print
                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {
                            //Draw Header
                            e.Graphics.DrawString(printDocument1.DocumentName, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString(printDocument1.DocumentName, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                            //Draw Date
                            e.Graphics.DrawString(strDate, new Font(dataGridView1.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(dataGridView1.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Customer Summary", new Font(new Font(dataGridView1.Font,
                                    FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            //Draw Columns                 
                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                            {
                                if (!GridCol.Visible)
                                    continue;
                                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                if (GridCol.InheritedStyle != null)
                                    e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                            (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;
                        //Draw Columns Contents                
                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (!Cel.Visible)
                                continue;

                            if (Cel.Value != null)
                            {
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                            new SolidBrush(Cel.InheritedStyle.ForeColor),
                                            new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                            (int)arrColumnWidths[iCount], iCellHeight), strFormat);
                            }
                            //Drawing Cells Borders 
                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                    iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                            iCount++;
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;
                }

                //If more lines exist, print another page.
                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
