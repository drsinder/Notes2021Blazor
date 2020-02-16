using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Notes2021Client
{
    public partial class DisplayNote : Form, IRelistAble
    {
        private readonly NoteFile MyFile;
        private readonly int baseOrdinal;
        private readonly IRelistAble MyParentListNotes;
        private List<NoteHeader> Myheadlist;
        private int currentResp;
        private int respCount;
        private NoteAccess MyAccess;
        //private int checkPrint;


        public DisplayNote(NoteFile myFile, int noteOrdinal, IRelistAble myParentListNotes)
        {
            InitializeComponent();

            MyFile = myFile;
            baseOrdinal = noteOrdinal;
            MyParentListNotes = myParentListNotes;
            currentResp = 0;

            label1.Text = MyFile.NoteFileName;
            label2.Text = MyFile.NoteFileTitle;

            var noteHeader = Actions.GetDislayNoteHeadersWithResponses(Program.MyClient, MyFile.Id, noteOrdinal);

            Myheadlist = noteHeader.ToList();
            respCount = Myheadlist.Count - 1;

            MyAccess = Actions.GetAccess(Program.MyClient, MyFile.Id);

            SetStuff();
        }

        public void Relist()
        {
            SetStuff();
        }

        private void SetStuff()
        {
            string[] autparts = Program.AuthToken.Split(',');

            string authorID = autparts[1];

            var noteHeader = Actions.GetDislayNoteHeadersWithResponses(Program.MyClient, MyFile.Id, baseOrdinal);

            Myheadlist = noteHeader.ToList();
            respCount = Myheadlist.Count - 1;

            MyAccess = Actions.GetAccess(Program.MyClient, MyFile.Id);

            NoteContent noteContent = Actions.GetNoteContent(Program.MyClient, MyFile.Id, baseOrdinal, currentResp);

            IEnumerable<Tags> myTags = Actions.GetTags(Program.MyClient, MyFile.Id, baseOrdinal, currentResp);

            label9.Text = "";

            if (myTags != null && ((List<Tags>)myTags).Any())
            {
                label9.Text = @"Tags: ";
                foreach (var tag in myTags)
                {
                    label9.Text += tag.Tag + @" ";
                }
            }

            label3.Text = Myheadlist[currentResp].NoteSubject;
            label4.Text = Myheadlist[currentResp].LastEdited.ToLocalTime().ToLongTimeString() + @" "
                        + Myheadlist[0].LastEdited.ToLocalTime().ToShortDateString();
            label5.Text = string.IsNullOrEmpty(noteContent.DirectorMessage) ? "" : noteContent.DirectorMessage;
            label6.Text = respCount + @"  Responses";

            string title = "<p>File " + MyFile.NoteFileName + " - Subject: " +
                           Myheadlist[currentResp].NoteSubject + " - By "
                           + Myheadlist[currentResp].AuthorName + "  " + Myheadlist[currentResp].CreateDate
                               .ToLocalTime().ToShortTimeString()
                           + "  " + Myheadlist[currentResp].CreateDate
                               .ToLocalTime().ToShortDateString() + "</p>";

            if (!string.IsNullOrEmpty(noteContent.DirectorMessage))
            {
                title += "<p>Director Message: " + noteContent.DirectorMessage + "</p>";
            }

            if (label9.Text.Length > 1)
            {
                title += "<p>" + label9.Text + "</p>";
            }

            webBrowser1.DocumentText = noteContent.NoteBody;
            webBrowserPrint.DocumentText = title + noteContent.NoteBody;
            label7.Text = currentResp == 0 ? respCount + " Responses" : "Response  " + currentResp + " of " + respCount;
            label8.Text = Myheadlist[currentResp].AuthorName;

            buttonNResp.Enabled = currentResp < respCount;
            buttonPResp.Enabled = currentResp > 0;
            buttonBase.Enabled = currentResp > 0;
            buttonReply.Enabled = MyAccess.Respond || MyAccess.Write;
            buttonEdit.Enabled = (authorID == Myheadlist[currentResp].AuthorID) && (currentResp == respCount);
            buttonDelete.Enabled = buttonEdit.Enabled;

        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
        }
        private void printDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            //checkPrint = 0;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Relist();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            webBrowserPrint.ShowPrintDialog();
        }

        private void buttonReply_Click(object sender, EventArgs e)
        {
            new CreateNote(MyFile, Myheadlist[0].Id, null, null, null, this)
            {
                Visible = true
            };
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            NoteContent noteContent = Actions.GetNoteContent(Program.MyClient, MyFile.Id, baseOrdinal, currentResp);

            IEnumerable<Tags> myTags = Actions.GetTags(Program.MyClient, MyFile.Id, baseOrdinal, currentResp);

            new CreateNote(MyFile, Myheadlist[0].Id, Myheadlist[currentResp], noteContent, myTags, this)
            {
                Visible = true
            };

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            NoteContent noteContent = Actions.GetNoteContent(Program.MyClient, MyFile.Id, baseOrdinal, currentResp);

            Actions.DeleteNote(Program.MyClient, noteContent.NoteHeaderId);

            Thread.Sleep(400);

            MyParentListNotes.Relist();

            Close();
        }

        private void buttonBase_Click(object sender, EventArgs e)
        {
            currentResp = 0;
            SetStuff();
        }

        private void buttonPResp_Click(object sender, EventArgs e)
        {
            currentResp--;
            SetStuff();
        }

        private void buttonNResp_Click(object sender, EventArgs e)
        {
            currentResp++;
            SetStuff();
        }
    }
}
