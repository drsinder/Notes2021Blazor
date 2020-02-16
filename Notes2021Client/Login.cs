using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace Notes2021Client
{
    public partial class Login : Form
    {
        public string token;

        public Login()
        {
            InitializeComponent();
            textBoxEmail.Text = Program.DefaultLogin;
        }

        private void Login_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpResponseMessage resp = Actions.Login(Program.MyClient, textBoxEmail.Text, textBoxPassword.Text);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                token = resp.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrEmpty(token))
                {
                    RestPassword();
                    button1.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                RestPassword();
            }
        }

        public void RestPassword()
        {
            textBoxPassword.Text = String.Empty;
            textBoxPassword.Focus();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            linkLabel1.Text = Program.MyClient.BaseAddress.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
