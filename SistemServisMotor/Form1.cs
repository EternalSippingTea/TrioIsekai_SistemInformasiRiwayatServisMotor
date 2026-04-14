using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SistemServisMotor
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConn())
                {
                    conn.Open();
                    lblStatus.Text = "Status : Connected.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch
            {
                lblStatus.Text = "Status : Failed to Connect.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                btnlogin.Enabled = false;
            }
        }


        private void btnlogin_Click(object sender, EventArgs e)
        {
            // Validasi (Bagian F)
            if (txtusername.Text.Trim() == "" || txttele.Text.Trim() == "")
            {
                MessageBox.Show("Username dan No. Telp harus diisi!");
                return;
            }
        }
    }
}

  