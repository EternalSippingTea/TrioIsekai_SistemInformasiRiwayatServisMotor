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
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

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


        // TAB KENDARAAN

        void LoadKendaraan()
        {
            LoadData(@"SELECT k.id_kendaraan AS ID, p.nama AS Pelanggan, 
                       k.merk AS Merk, k.plat_no AS [Plat No], k.tahun AS Tahun
                       FROM Kendaraan k JOIN Pelanggan p ON k.id_pelanggan = p.id_pelanggan",
                     dgvKendaraan);
            lblcountk.Text = "Total: " + CountRows("Kendaraan");
        }

        private void btnaddk_Click(object sender, EventArgs e)
        {
            int idP = GetComboId(cmbpelanggan);
            if (idP == -1) { MessageBox.Show("Pilih pelanggan!"); return; }
            if (txtmerk.Text == "" || txtplano.Text == "")
            { MessageBox.Show("Merk dan Plat No harus diisi!"); return; }

            RunQuery("INSERT INTO Kendaraan(id_pelanggan, merk, plat_no, tahun) VALUES(@a,@b,@c,@d)",
                new SqlParameter("@a", idP),
                new SqlParameter("@b", txtmerk.Text.Trim()),
                new SqlParameter("@c", txtplano.Text.Trim()),
                new SqlParameter("@d", txttahunken.Text == "" ? (object)DBNull.Value : int.Parse(txttahunken.Text)));

            MessageBox.Show("Data berhasil ditambahkan!");
            ClearK(); LoadKendaraan(); LoadCombos();
        }

        private void btnupk_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvKendaraan);
            int idP = GetComboId(cmbpelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (idP == -1 || txtmerk.Text == "" || txtplano.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery("UPDATE Kendaraan SET id_pelanggan=@a, merk=@b, plat_no=@c, tahun=@d WHERE id_kendaraan=@id",
                new SqlParameter("@a", idP),
                new SqlParameter("@b", txtmerk.Text.Trim()),
                new SqlParameter("@c", txtplano.Text.Trim()),
                new SqlParameter("@d", txttahunken.Text == "" ? (object)DBNull.Value : int.Parse(txttahunken.Text)),
                new SqlParameter("@id", id));

            MessageBox.Show("Data berhasil diubah!");
            ClearK(); LoadKendaraan(); LoadCombos();
        }

        private void btndelk_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvKendaraan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery("DELETE FROM Kendaraan WHERE id_kendaraan=@id", new SqlParameter("@id", id));
            MessageBox.Show("Data berhasil dihapus!");
            ClearK(); LoadKendaraan(); LoadCombos();
        }

        // SEARCH using SqlDataAdapter (Bagian A)
        private void btncarik_Click(object sender, EventArgs e)
        {
            string cari = txtcarik.Text.Trim();
            SearchData(@"SELECT k.id_kendaraan AS ID, p.nama AS Pelanggan, 
                         k.merk AS Merk, k.plat_no AS [Plat No], k.tahun AS Tahun
                         FROM Kendaraan k JOIN Pelanggan p ON k.id_pelanggan=p.id_pelanggan
                         WHERE k.plat_no LIKE '%" + cari + "%' OR k.merk LIKE '%" + cari + "%'",
                       dgvKendaraan);
        }

        private void btnloadk_Click(object sender, EventArgs e) { LoadKendaraan(); }

        private void dgvKendaraan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvKendaraan.Rows[e.RowIndex];
            txtmerk.Text = row.Cells["Merk"].Value.ToString();
            txtplano.Text = row.Cells["Plat No"].Value.ToString();
            txttahunken.Text = row.Cells["Tahun"].Value?.ToString() ?? "";
            string pel = row.Cells["Pelanggan"].Value.ToString();
            for (int i = 0; i < cmbpelanggan.Items.Count; i++)
                if (cmbpelanggan.Items[i].ToString().Contains(pel))
                { cmbpelanggan.SelectedIndex = i; break; }
        }

        private void btncleark_Click(object sender, EventArgs e) { ClearK(); }
        void ClearK()
        {
            txtmerk.Clear(); txtplano.Clear(); txttahunken.Clear();
            txtcarik.Clear(); cmbpelanggan.SelectedIndex = 0;
        }


        // TAB SERVIS

        void LoadServis()
        {
            LoadData(@"SELECT s.id_servis AS ID, k.plat_no AS [Plat No], u.nama AS Petugas,
                       s.Tanggal, s.JenisServis AS [Jenis Servis], 
                       s.SukuCadang AS [Suku Cadang], s.Biaya, s.Catatan
                       FROM Servis s 
                       JOIN Kendaraan k ON s.id_kendaraan = k.id_kendaraan
                       JOIN Users u ON s.id_user = u.id_user",
                     dgvServis);
            lblcounts.Text = "Total: " + CountRows("Servis");
        }

        private void btnadds_Click(object sender, EventArgs e)
        {
            int idK = GetComboId(cmbkendaraan);
            int idU = GetComboId(cmbusers);
            if (idK == -1 || idU == -1)
            { MessageBox.Show("Pilih kendaraan dan petugas!"); return; }
            if (txtjenisservis.Text == "" || txtsukucadang.Text == "" || txtbiaya.Text == "")
            { MessageBox.Show("Jenis Servis, Suku Cadang, dan Biaya harus diisi!"); return; }

            decimal biaya;
            if (!decimal.TryParse(txtbiaya.Text, out biaya))
            { MessageBox.Show("Biaya harus angka!"); return; }

            RunQuery(@"INSERT INTO Servis(id_kendaraan, id_user, Tanggal, JenisServis, SukuCadang, Biaya, Catatan) 
                       VALUES(@a,@b,@c,@d,@e,@f,@g)",
                new SqlParameter("@a", idK),
                new SqlParameter("@b", idU),
                new SqlParameter("@c", dtptanggal.Value),
                new SqlParameter("@d", txtjenisservis.Text.Trim()),
                new SqlParameter("@e", txtsukucadang.Text.Trim()),
                new SqlParameter("@f", biaya),
                new SqlParameter("@g", txtcatatan.Text == "" ? (object)DBNull.Value : txtcatatan.Text.Trim()));

            MessageBox.Show("Data berhasil ditambahkan!");
            ClearS(); LoadServis();
        }

        private void btnups_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvServis);
            int idK = GetComboId(cmbkendaraan);
            int idU = GetComboId(cmbusers);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (idK == -1 || idU == -1 || txtjenisservis.Text == "" || txtsukucadang.Text == "" || txtbiaya.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }

            decimal biaya;
            if (!decimal.TryParse(txtbiaya.Text, out biaya))
            { MessageBox.Show("Biaya harus angka!"); return; }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery(@"UPDATE Servis SET id_kendaraan=@a, id_user=@b, Tanggal=@c, 
                       JenisServis=@d, SukuCadang=@e, Biaya=@f, Catatan=@g 
                       WHERE id_servis=@id",
                new SqlParameter("@a", idK),
                new SqlParameter("@b", idU),
                new SqlParameter("@c", dtptanggal.Value),
                new SqlParameter("@d", txtjenisservis.Text.Trim()),
                new SqlParameter("@e", txtsukucadang.Text.Trim()),
                new SqlParameter("@f", biaya),
                new SqlParameter("@g", txtcatatan.Text == "" ? (object)DBNull.Value : txtcatatan.Text.Trim()),
                new SqlParameter("@id", id));

            MessageBox.Show("Data berhasil diubah!");
            ClearS(); LoadServis();
        }

        private void btndels_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvServis);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            RunQuery("DELETE FROM Servis WHERE id_servis=@id", new SqlParameter("@id", id));
            MessageBox.Show("Data berhasil dihapus!");
            ClearS(); LoadServis();
        }

        private void btncaris_Click(object sender, EventArgs e)
        {
            string cari = txtcaris.Text.Trim();
            SearchData(@"SELECT s.id_servis AS ID, k.plat_no AS [Plat No], u.nama AS Petugas,
                         s.Tanggal, s.JenisServis AS [Jenis Servis], 
                         s.SukuCadang AS [Suku Cadang], s.Biaya, s.Catatan
                         FROM Servis s JOIN Kendaraan k ON s.id_kendaraan=k.id_kendaraan
                         JOIN Users u ON s.id_user=u.id_user
                         WHERE k.plat_no LIKE '%" + cari + "%' OR s.JenisServis LIKE '%" + cari + "%'",
                       dgvServis);
        }

        private void btnloads_Click(object sender, EventArgs e) { LoadServis(); }

        private void dgvServis_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvServis.Rows[e.RowIndex];
            txtjenisservis.Text = row.Cells["Jenis Servis"].Value.ToString();
            txtsukucadang.Text = row.Cells["Suku Cadang"].Value.ToString();
            txtbiaya.Text = row.Cells["Biaya"].Value.ToString();
            txtcatatan.Text = row.Cells["Catatan"].Value?.ToString() ?? "";

            if (row.Cells["Tanggal"].Value != null)
                dtptanggal.Value = Convert.ToDateTime(row.Cells["Tanggal"].Value);

            string plat = row.Cells["Plat No"].Value.ToString();
            for (int i = 0; i < cmbkendaraan.Items.Count; i++)
                if (cmbkendaraan.Items[i].ToString().Contains(plat))
                { cmbkendaraan.SelectedIndex = i; break; }

            string pet = row.Cells["Petugas"].Value.ToString();
            for (int i = 0; i < cmbusers.Items.Count; i++)
                if (cmbusers.Items[i].ToString().Contains(pet))
                { cmbusers.SelectedIndex = i; break; }
        }

        private void btnclears_Click(object sender, EventArgs e) { ClearS(); }
        void ClearS()
        {
            txtjenisservis.Clear(); txtsukucadang.Clear(); txtbiaya.Clear();
            txtcatatan.Clear(); txtcaris.Clear();
            dtptanggal.Value = DateTime.Now;
            cmbkendaraan.SelectedIndex = 0; cmbusers.SelectedIndex = 0;
        }

        


    }
}
      