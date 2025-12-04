using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrySystem;

namespace SmartShelf_try
{
    public partial class register : Form
    {
        public register()
        {
            InitializeComponent();
<<<<<<< HEAD
=======
            this.Load += Register_Load;
        }

        private void Register_Load(object sender, EventArgs e)
        {
            // Set password character to hide password input
            textBox2.PasswordChar = '*';
            textBox2.UseSystemPasswordChar = false;
>>>>>>> ccd93a08a8d773fb8eadb95edc0c84be66da8ff2
        }

        private void btnregister_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
            Form1 login1 = new Form1();
            this.Hide();
            login1.ShowDialog();
=======
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TrySystem.DatabaseHelper.RegisterUser(username, password))
            {
                MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
>>>>>>> ccd93a08a8d773fb8eadb95edc0c84be66da8ff2
        }
    }
}
