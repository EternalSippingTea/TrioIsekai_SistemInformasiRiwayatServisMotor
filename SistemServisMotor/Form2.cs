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
using System.Drawing.Printing;


namespace SistemServisMotor
{
    public partial class MainForm: Form
    {
        int userID;
        string userName, userRole;

        public MainForm(int id, string name, string role)
        {
            InitializeComponent();
            userID = id;
            userName = name;
            userRole = role;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblwelcome.Text = "Halo, " + userName + " (" + userRole + ")";

            // Connection status (Bagian B)
            try
            {
                using (var c = DatabaseHelper.GetConn()) { c.Open(); }
                lblcon.Text = "Koneksi: Terhubung";
                lblcon.ForeColor = Color.Green;
            }
            catch
            {
                lblcon.Text = "Koneksi: Gagal";
                lblcon.ForeColor = Color.Red;
            }

            // Hide delete buttons for petugas
            if (userRole == "petugas")
            {
                btndelp.Visible = false;
                btndelk.Visible = false;
                btndels.Visible = false;
                btndelu.Visible = false;
            }

            // Load first tab data + combos on startup
            LoadPelanggan();
            LoadCombos();
        }        

        private void tabPelanggan_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }




        private void tabServis_Click(object sender, EventArgs e)
        {

        }



        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

 


        private void btnupp_Click(object sender, EventArgs e)
        {

        }
    }
}
