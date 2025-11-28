using SmartShelf_try;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrySystem
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void login_Load(object sender, EventArgs e)
        {
            // Set password character to hide password input
            textBox1.PasswordChar = '*';
            textBox1.UseSystemPasswordChar = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text.Trim();
            string password = textBox1.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DatabaseHelper.AuthenticateUser(username, password))
            {
                Form1 form1 = new Form1(username);
                form1.FormClosed += (s, args) =>
                {
                    textBox2.Clear();
                    textBox1.Clear();
                    textBox2.Focus();
                    this.Show();
                };

                this.Hide();
                form1.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            register form = new register();
            this.Hide();
            form.ShowDialog();
            this.Show();
        }
    }
}
