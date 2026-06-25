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
        SqlConnection conn = new SqlConnection(DAL.GetConnectionString());

        public LoginForm()
        {
            InitializeComponent();
        }

        // Bagian B - Cek status koneksi sebelum form utama
        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                lblStatus.Text = "Status : Connected.";
                lblStatus.ForeColor = Color.Green;
            }
            catch
            {
                lblStatus.Text = "Status : Failed to Connect.";
                lblStatus.ForeColor = Color.Red;
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
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // =====================================================
                // [!] DEMO SQL INJECTION (UCP 2 - point #3)
                // Field username sengaja pakai konkatenasi string
                // sehingga rentan SQL Injection.
                // Contoh payload bypass: admin' OR '1'='1' --
                // =====================================================
                string sql = "SELECT id_user, nama, role FROM Users"
                           + " WHERE username='" + txtusername.Text + "'"
                           + " AND no_telp=@t";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@t", txttele.Text.Trim()));

                SqlDataReader reader = cmd.ExecuteReader();  // Bagian A

                if (reader.Read())
                {
                    int id = (int)reader["id_user"];
                    string nama = reader["nama"].ToString();
                    string role = reader["role"].ToString();
                    reader.Close();

                    MessageBox.Show("Welcome, " + nama + "!");
                    MainForm f2 = new MainForm(id, nama, role);
                    f2.Show();
                    this.Hide();
                }
                else
                {
                    reader.Close();
                    MessageBox.Show("Login gagal! Username atau No. Telp salah.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
