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
        }

        private void btnregister_Click(object sender, EventArgs e)
        {
            Form1 login1 = new Form1();
            this.Hide();
            login1.ShowDialog();
        }
    }
}
