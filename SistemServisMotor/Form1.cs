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

            try
            {
                using (var conn = DatabaseHelper.GetConn())
                {
                    conn.Open();
                    string sql = "SELECT id_user, nama, role FROM Users WHERE username=@u AND no_telp=@t";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", txtusername.Text.Trim());
                    cmd.Parameters.AddWithValue("@t", txttele.Text.Trim());

                    // SqlDataReader (Bagian A)
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int id = (int)reader["id_user"];
                        string nama = reader["nama"].ToString();
                        string role = reader["role"].ToString();
                        reader.Close();
                        conn.Close(); // explicitly close before opening new form

                        MessageBox.Show("Welcome, " + nama + "!");
                        var f2 = new MainForm(id, nama, role);
                        f2.Show();
                        this.Hide();
                        return; // exit the using block cleanly
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Login gagal! Username atau No. Telp salah.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

  