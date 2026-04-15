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
    public partial class MainForm : Form
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
                lblcon.Text = "Connection : Successful";
                lblcon.ForeColor = Color.Green;
            }
            catch
            {
                lblcon.Text = "Connection : Failed";
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

        private void tabcontrol1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabcontrol1.SelectedIndex)
            {
                case 0: LoadPelanggan(); break;
                case 1: LoadKendaraan(); break;
                case 2: LoadServis(); break;
                case 3: LoadUsers(); break;
            }
            LoadCombos();
        }

        void LoadData(string sql, DataGridView dgv)
        {
            using (var conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                reader.Close();
                dgv.DataSource = dt;
            }
        }

        void SearchData(string sql, DataGridView dgv)
        {
            using (var conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        void RunQuery(string sql, params SqlParameter[] pars)
        {
            using (var conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(pars);
                cmd.ExecuteNonQuery();
            }
        }

        int CountRows(string table)
        {
            using (var conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM " + table, conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        int GetSelectedId(DataGridView dgv)
        {
            if (dgv.CurrentRow == null) return -1;
            return Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
        }

        int GetComboId(ComboBox cmb)
        {
            if (cmb.SelectedIndex <= 0) return -1;
            return int.Parse(cmb.SelectedItem.ToString().Split('-')[0].Trim());
        }


        // Combos 
        void LoadCombos()
        {
            FillCombo(cmbpelanggan,
                "SELECT id_pelanggan, nama FROM Pelanggan",
                "id_pelanggan", "nama", "-- Pilih Pelanggan --");

            FillCombo(cmbkendaraan,
                "SELECT id_kendaraan, plat_no + ' - ' + merk AS info FROM Kendaraan",
                "id_kendaraan", "info", "-- Pilih Kendaraan --");

            FillCombo(cmbusers,
                "SELECT id_user, nama FROM Users",
                "id_user", "nama", "-- Pilih Petugas --");
        }

        void FillCombo(ComboBox cmb, string sql, string idCol, string nameCol, string placeholder)
        {
            cmb.Items.Clear();
            cmb.Items.Add(placeholder);
            using (var conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                SqlDataReader reader = new SqlCommand(sql, conn).ExecuteReader();  // SqlDataReader
                while (reader.Read())
                    cmb.Items.Add(reader[idCol] + " - " + reader[nameCol]);
                reader.Close();
            }
            cmb.SelectedIndex = 0;
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Close();
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


        //TAB PELANGGAN

        void LoadPelanggan()
        {
            LoadData("SELECT id_pelanggan AS ID, nama AS Nama, alamat AS Alamat, no_hp AS [No HP] FROM Pelanggan", dgvPelanggan);
            lblcountp.Text = "Total: " + CountRows("Pelanggan");
        }

        private void btnaddp_Click(object sender, EventArgs e)
        {
            // Validasi (Bagian F)
            if (txtnamapel.Text == "" || txtalamat.Text == "" || txtnohp.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }

            RunQuery("INSERT INTO Pelanggan(nama, alamat, no_hp) VALUES(@a, @b, @c)",
                new SqlParameter("@a", txtnamapel.Text.Trim()),
                new SqlParameter("@b", txtalamat.Text.Trim()),
                new SqlParameter("@c", txtnohp.Text.Trim()));

            MessageBox.Show("Data berhasil ditambahkan!");
            ClearP(); LoadPelanggan(); LoadCombos();
        }

        private void btnupp_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvPelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (txtnamapel.Text == "" || txtalamat.Text == "" || txtnohp.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }

            // Konfirmasi (Bagian F)
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery("UPDATE Pelanggan SET nama=@a, alamat=@b, no_hp=@c WHERE id_pelanggan=@id",
                new SqlParameter("@a", txtnamapel.Text.Trim()),
                new SqlParameter("@b", txtalamat.Text.Trim()),
                new SqlParameter("@c", txtnohp.Text.Trim()),
                new SqlParameter("@id", id));

            MessageBox.Show("Data berhasil diubah!");
            ClearP(); LoadPelanggan(); LoadCombos();
        }

        private void btndelp_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvPelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }

            // Konfirmasi (Bagian F)
            if (MessageBox.Show("Yakin hapus?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery("DELETE FROM Pelanggan WHERE id_pelanggan=@id", new SqlParameter("@id", id));
            MessageBox.Show("Data berhasil dihapus!");
            ClearP(); LoadPelanggan(); LoadCombos();
        }

        private void btncarip_Click(object sender, EventArgs e)
        {
            string cari = txtcarip.Text.Trim();
            SearchData("SELECT id_pelanggan AS ID, nama AS Nama, alamat AS Alamat, no_hp AS [No HP] " +
                       "FROM Pelanggan WHERE nama LIKE '%" + cari + "%'", dgvPelanggan);
        }

        private void btnloadp_Click(object sender, EventArgs e) { LoadPelanggan(); }

        // CLICK ROW → FILL TEXTBOXES (Bagian E)
        private void dgvPelanggan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvPelanggan.Rows[e.RowIndex];
            txtnamapel.Text = row.Cells["Nama"].Value.ToString();
            txtalamat.Text = row.Cells["Alamat"].Value.ToString();
            txtnohp.Text = row.Cells["No HP"].Value.ToString();
        }

        private void btnClearP_Click(object sender, EventArgs e) { ClearP(); }
        void ClearP()
        {
            txtnamapel.Clear();
            txtalamat.Clear();
            txtnohp.Clear();
            txtcarip.Clear();
        }



    }
}
      