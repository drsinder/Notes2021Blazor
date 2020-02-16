using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Notes2021Client
{
    public partial class frmReplace : Form
    {
        // member variable pointing to main form
        CreateNote mMain;


        // default constructor
        public frmReplace()
        {
            InitializeComponent();
        }


        // overloaded constructor accepteing main form as
        // an argument
        public frmReplace(CreateNote f)
        {
            InitializeComponent();
            mMain = f;
        }



        private void btnFind_Click(object sender, System.EventArgs e)
        {
            try
            {
                int StartPosition;
                StringComparison SearchType;

                if (chkMatchCase.Checked == true)
                {
                    SearchType = StringComparison.Ordinal;
                }
                else
                {
                    SearchType = StringComparison.OrdinalIgnoreCase;
                }

                StartPosition = mMain.rtbDoc2.Text.IndexOf(txtSearchTerm.Text, SearchType);

                if (StartPosition == 0)
                {
                    MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                mMain.rtbDoc2.Select(StartPosition, txtSearchTerm.Text.Length);
                mMain.rtbDoc2.ScrollToCaret();
                mMain.Focus();
                btnFindNext.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }

        }



        private void btnFindNext_Click(object sender, System.EventArgs e)
        {
            try
            {
                int StartPosition = mMain.rtbDoc2.SelectionStart + 2;

                StringComparison SearchType;

                if (chkMatchCase.Checked == true)
                {
                    SearchType = StringComparison.Ordinal;
                }
                else
                {
                    SearchType = StringComparison.OrdinalIgnoreCase;
                }

                StartPosition = mMain.rtbDoc2.Text.IndexOf(txtSearchTerm.Text, StartPosition, SearchType);

                if (StartPosition == 0 || StartPosition < 0)
                {
                    MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                mMain.rtbDoc2.Select(StartPosition, txtSearchTerm.Text.Length);
                mMain.rtbDoc2.ScrollToCaret();
                mMain.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }




        private void btnReplace_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (mMain.rtbDoc2.SelectedText.Length != 0)
                {
                    mMain.rtbDoc2.SelectedText = txtReplacementText.Text;
                }

                int StartPosition;
                StringComparison SearchType;

                if (chkMatchCase.Checked == true)
                {
                    SearchType = StringComparison.Ordinal;
                }
                else
                {
                    SearchType = StringComparison.OrdinalIgnoreCase;
                }

                StartPosition = mMain.rtbDoc2.Text.IndexOf(txtSearchTerm.Text, SearchType);

                if (StartPosition == 0 || StartPosition < 0)
                {
                    MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }

                mMain.rtbDoc2.Select(StartPosition, txtSearchTerm.Text.Length);
                mMain.rtbDoc2.ScrollToCaret();
                mMain.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }

        }



        private void btnReplaceAll_Click(object sender, System.EventArgs e)
        {

            try
            {
                mMain.rtbDoc2.Rtf = mMain.rtbDoc2.Rtf.Replace(txtSearchTerm.Text.Trim(), txtReplacementText.Text.Trim());


                int StartPosition;
                StringComparison SearchType;

                if (chkMatchCase.Checked == true)
                {
                    SearchType = StringComparison.Ordinal;
                }
                else
                {
                    SearchType = StringComparison.OrdinalIgnoreCase;
                }

                StartPosition = mMain.rtbDoc2.Text.IndexOf(txtReplacementText.Text, SearchType);

                mMain.rtbDoc2.Select(StartPosition, txtReplacementText.Text.Length);
                mMain.rtbDoc2.ScrollToCaret();
                mMain.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }

        }

        private void txtSearchTerm_TextChanged(object sender, EventArgs e)
        {
            btnFindNext.Enabled = false;
        }


    }
}
