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
        string printText;

        // ===== UCP 2 #4 - BindingSource untuk DataGridView =====
        BindingSource bsPelanggan = new BindingSource();
        BindingSource bsKendaraan = new BindingSource();
        BindingSource bsServis    = new BindingSource();
        BindingSource bsUsers     = new BindingSource();

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

            // Bagian B - Cek status koneksi
            try
            {
                using (SqlConnection c = DatabaseHelper.GetConn()) { c.Open(); }
                lblcon.Text = "Connection : Successful";
                lblcon.ForeColor = Color.Green;
            }
            catch
            {
                lblcon.Text = "Connection : Failed";
                lblcon.ForeColor = Color.Red;
            }

            // Sembunyikan tombol untuk petugas
            if (userRole == "petugas")
            {
                btndelp.Visible = false;
                btndelk.Visible = false;
                btndels.Visible = false;
                btndelu.Visible = false;
                // Petugas tidak bisa CRU di tab Users
                btnaddu.Visible = false;
                btnupu.Visible = false;
            }

            // ===== UCP 2 #5 - Connect BindingNavigator (dari Designer) ke BindingSource =====
            bnPelanggan.BindingSource = bsPelanggan;
            bnKendaraan.BindingSource = bsKendaraan;
            bnServis.BindingSource    = bsServis;
            bnUsers.BindingSource     = bsUsers;

            // Wire event: saat posisi BindingSource berubah (navigator/klik row),
            // textbox dan combobox otomatis ter-update
            bsPelanggan.PositionChanged += bsPelanggan_PositionChanged;
            bsKendaraan.PositionChanged += bsKendaraan_PositionChanged;
            bsServis.PositionChanged    += bsServis_PositionChanged;
            bsUsers.PositionChanged     += bsUsers_PositionChanged;

            // Load tab pertama + combobox
            LoadPelanggan();
            LoadCombos();
        }

        // ============ POSITION CHANGED HANDLERS ============
        // Dipanggil saat user pencet panah di BindingNavigator atau klik baris di DGV

        private void bsPelanggan_PositionChanged(object sender, EventArgs e)
        {
            if (bsPelanggan.Current == null) return;
            DataRowView row = (DataRowView)bsPelanggan.Current;
            txtnamapel.Text = row["Nama"].ToString();
            txtalamat.Text  = row["Alamat"].ToString();
            txtnohp.Text    = row["No HP"].ToString();
        }

        private void bsKendaraan_PositionChanged(object sender, EventArgs e)
        {
            if (bsKendaraan.Current == null) return;
            DataRowView row = (DataRowView)bsKendaraan.Current;
            txtmerk.Text     = row["Merk"].ToString();
            txtplano.Text    = row["Plat No"].ToString();
            txttahunken.Text = row["Tahun"] == DBNull.Value ? "" : row["Tahun"].ToString();

            // Sinkronkan ComboBox Pelanggan
            string pel = row["Pelanggan"].ToString();
            for (int i = 0; i < cmbpelanggan.Items.Count; i++)
            {
                if (cmbpelanggan.Items[i].ToString().Contains(pel))
                { cmbpelanggan.SelectedIndex = i; break; }
            }
        }

        private void bsServis_PositionChanged(object sender, EventArgs e)
        {
            if (bsServis.Current == null) return;
            DataRowView row = (DataRowView)bsServis.Current;
            txtjenisservis.Text = row["Jenis Servis"].ToString();
            txtsukucadang.Text  = row["Suku Cadang"].ToString();
            txtbiaya.Text       = row["Biaya"].ToString();
            txtcatatan.Text     = row["Catatan"] == DBNull.Value ? "" : row["Catatan"].ToString();

            if (row["Tanggal"] != DBNull.Value)
                dtptanggal.Value = Convert.ToDateTime(row["Tanggal"]);

            // Sinkronkan ComboBox Kendaraan
            string plat = row["Plat No"].ToString();
            for (int i = 0; i < cmbkendaraan.Items.Count; i++)
            {
                if (cmbkendaraan.Items[i].ToString().Contains(plat))
                { cmbkendaraan.SelectedIndex = i; break; }
            }

            // Sinkronkan ComboBox Users (Petugas)
            string pet = row["Petugas"].ToString();
            for (int i = 0; i < cmbusers.Items.Count; i++)
            {
                if (cmbusers.Items[i].ToString().Contains(pet))
                { cmbusers.SelectedIndex = i; break; }
            }
        }

        private void bsUsers_PositionChanged(object sender, EventArgs e)
        {
            if (bsUsers.Current == null) return;
            DataRowView row = (DataRowView)bsUsers.Current;
            txtnamauser.Text  = row["Nama"].ToString();
            txtusername.Text  = row["Username"].ToString();
            txtnoteluser.Text = row["No Telp"].ToString();
            cmbrole.SelectedItem = row["Role"].ToString();
        }

        private void tabcontrol1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabcontrol1.SelectedIndex)
            {
                case 0: LoadPelanggan(); break;
                case 1: LoadKendaraan(); break;
                case 2: LoadServis();    break;
                case 3: LoadUsers();     break;
            }
            LoadCombos();
        }

        // ============ HELPER METHODS ============

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

        bool IsValidPhone(string phone)
        {
            if (phone.Length < 10 || phone.Length > 13) return false;
            if (!phone.StartsWith("08")) return false;
            foreach (char c in phone)
                if (!char.IsDigit(c)) return false;
            return true;
        }

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
            using (SqlConnection conn = DatabaseHelper.GetConn())
            {
                conn.Open();
                SqlDataReader reader = new SqlCommand(sql, conn).ExecuteReader();
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

        // ============================================
        // TAB PELANGGAN
        // ============================================

        void LoadPelanggan()
        {
            try
            {
                // Load data via Stored Procedure (yang baca dari View vwPelanggan)
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllPelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();  // Bagian A
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        reader.Close();

                        // Bind ke BindingSource (UCP 2 #4)
                        bsPelanggan.DataSource = dt;
                        dgvPelanggan.DataSource = bsPelanggan;
                    }
                }

                // Bagian D - ExecuteScalar untuk hitung total
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pelanggan", conn))
                    {
                        conn.Open();
                        int total = (int)cmd.ExecuteScalar();
                        lblcountp.Text = "Total: " + total;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data pelanggan: " + ex.Message);
            }
        }

        private void btnaddp_Click(object sender, EventArgs e)
        {
            // Validasi (Bagian F)
            if (txtnamapel.Text == "" || txtalamat.Text == "" || txtnohp.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }
            if (!IsValidPhone(txtnohp.Text.Trim()))
            { MessageBox.Show("No HP harus angka, 10-13 digit, dan dimulai dengan 08!"); return; }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertPelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@nama",   txtnamapel.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@alamat", txtalamat.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@no_hp",  txtnohp.Text.Trim()));

                        conn.Open();
                        cmd.ExecuteNonQuery();  // Bagian D
                    }
                }

                MessageBox.Show("Data berhasil ditambahkan!");
                ClearP(); LoadPelanggan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tambah pelanggan: " + ex.Message);
            }
        }

        private void btnupp_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvPelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (txtnamapel.Text == "" || txtalamat.Text == "" || txtnohp.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }
            if (!IsValidPhone(txtnohp.Text.Trim()))
            { MessageBox.Show("No HP harus angka, 10-13 digit, dan dimulai dengan 08!"); return; }

            // Konfirmasi (Bagian F)
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id",     id));
                        cmd.Parameters.Add(new SqlParameter("@nama",   txtnamapel.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@alamat", txtalamat.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@no_hp",  txtnohp.Text.Trim()));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diubah!");
                ClearP(); LoadPelanggan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update pelanggan: " + ex.Message);
            }
        }

        private void btndelp_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvPelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeletePelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!");
                ClearP(); LoadPelanggan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus pelanggan: " + ex.Message);
            }
        }

        // Bagian E - Search pakai SqlDataAdapter + DataSet
        private void btncarip_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SearchPelanggan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cari", txtcarip.Text.Trim()));

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds, "Hasil");

                        bsPelanggan.DataSource = ds.Tables["Hasil"];
                        dgvPelanggan.DataSource = bsPelanggan;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari pelanggan: " + ex.Message);
            }
        }

        private void btnloadp_Click(object sender, EventArgs e) { LoadPelanggan(); }

        // Bagian E - Pilih data DGV ke TextBox
        // Klik baris -> BindingSource.Position berubah -> bsPelanggan_PositionChanged otomatis dipanggil
        private void dgvPelanggan_CellClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnClearP_Click(object sender, EventArgs e) { ClearP(); }
        void ClearP()
        {
            txtnamapel.Clear();
            txtalamat.Clear();
            txtnohp.Clear();
            txtcarip.Clear();
        }

        // ============================================
        // TAB KENDARAAN
        // ============================================

        void LoadKendaraan()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllKendaraan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        reader.Close();

                        bsKendaraan.DataSource = dt;
                        dgvKendaraan.DataSource = bsKendaraan;
                    }
                }

                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Kendaraan", conn))
                    {
                        conn.Open();
                        int total = (int)cmd.ExecuteScalar();
                        lblcountk.Text = "Total: " + total;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data kendaraan: " + ex.Message);
            }
        }

        private void btnaddk_Click(object sender, EventArgs e)
        {
            int idP = GetComboId(cmbpelanggan);
            if (idP == -1) { MessageBox.Show("Pilih pelanggan!"); return; }
            if (txtmerk.Text == "" || txtplano.Text == "")
            { MessageBox.Show("Merk dan Plat No harus diisi!"); return; }

            int tahun = 0;
            if (txttahunken.Text != "")
            {
                if (!int.TryParse(txttahunken.Text, out tahun) || tahun < 2000)
                { MessageBox.Show("Tahun harus angka dan minimal 2000!"); return; }
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertKendaraan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id_pel",  idP));
                        cmd.Parameters.Add(new SqlParameter("@merk",    txtmerk.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@plat_no", txtplano.Text.Trim()));

                        if (txttahunken.Text == "")
                            cmd.Parameters.Add(new SqlParameter("@tahun", DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter("@tahun", tahun));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil ditambahkan!");
                ClearK(); LoadKendaraan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tambah kendaraan: " + ex.Message);
            }
        }

        private void btnupk_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvKendaraan);
            int idP = GetComboId(cmbpelanggan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (idP == -1 || txtmerk.Text == "" || txtplano.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }

            int tahun = 0;
            if (txttahunken.Text != "")
            {
                if (!int.TryParse(txttahunken.Text, out tahun) || tahun < 2000)
                { MessageBox.Show("Tahun harus angka dan minimal 2000!"); return; }
            }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateKendaraan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id",      id));
                        cmd.Parameters.Add(new SqlParameter("@id_pel",  idP));
                        cmd.Parameters.Add(new SqlParameter("@merk",    txtmerk.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@plat_no", txtplano.Text.Trim()));

                        if (txttahunken.Text == "")
                            cmd.Parameters.Add(new SqlParameter("@tahun", DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter("@tahun", tahun));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diubah!");
                ClearK(); LoadKendaraan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update kendaraan: " + ex.Message);
            }
        }

        private void btndelk_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvKendaraan);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteKendaraan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!");
                ClearK(); LoadKendaraan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus kendaraan: " + ex.Message);
            }
        }

        private void btncarik_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SearchKendaraan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cari", txtcarik.Text.Trim()));

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds, "Hasil");

                        bsKendaraan.DataSource = ds.Tables["Hasil"];
                        dgvKendaraan.DataSource = bsKendaraan;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari kendaraan: " + ex.Message);
            }
        }

        private void btnloadk_Click(object sender, EventArgs e) { LoadKendaraan(); }

        // Klik baris -> bsKendaraan_PositionChanged otomatis dipanggil
        private void dgvKendaraan_CellClick(object sender, DataGridViewCellEventArgs e) { }

        private void btncleark_Click(object sender, EventArgs e) { ClearK(); }
        void ClearK()
        {
            txtmerk.Clear(); txtplano.Clear(); txttahunken.Clear();
            txtcarik.Clear();
            cmbpelanggan.SelectedIndex = 0;
        }

        // ============================================
        // TAB SERVIS
        // ============================================

        void LoadServis()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllServis", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        reader.Close();

                        bsServis.DataSource = dt;
                        dgvServis.DataSource = bsServis;
                    }
                }

                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Servis", conn))
                    {
                        conn.Open();
                        int total = (int)cmd.ExecuteScalar();
                        lblcounts.Text = "Total: " + total;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data servis: " + ex.Message);
            }
        }

        // Insert Servis - pakai OUTPUT parameter (pola dari materi kelas)
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
            if (biaya < 0 || biaya > 1000000)
            { MessageBox.Show("Biaya harus antara 0 - 1.000.000!"); return; }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertServis", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id_ken", idK));
                        cmd.Parameters.Add(new SqlParameter("@id_u",   idU));
                        cmd.Parameters.Add(new SqlParameter("@tgl",    dtptanggal.Value));
                        cmd.Parameters.Add(new SqlParameter("@jenis",  txtjenisservis.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@suku",   txtsukucadang.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@biaya",  (int)biaya));

                        if (txtcatatan.Text == "")
                            cmd.Parameters.Add(new SqlParameter("@catatan", DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter("@catatan", txtcatatan.Text.Trim()));

                        // OUTPUT parameter (pola dari materi kelas)
                        SqlParameter outputParam = new SqlParameter("@new_id", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Data berhasil ditambahkan! ID Servis baru: " + outputParam.Value.ToString());
                    }
                }

                ClearS(); LoadServis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tambah servis: " + ex.Message);
            }
        }

        private void btnups_Click(object sender, EventArgs e)
        {
            int id  = GetSelectedId(dgvServis);
            int idK = GetComboId(cmbkendaraan);
            int idU = GetComboId(cmbusers);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (idK == -1 || idU == -1 || txtjenisservis.Text == "" || txtsukucadang.Text == "" || txtbiaya.Text == "")
            { MessageBox.Show("Semua field harus diisi!"); return; }

            decimal biaya;
            if (!decimal.TryParse(txtbiaya.Text, out biaya))
            { MessageBox.Show("Biaya harus angka!"); return; }
            if (biaya < 0 || biaya > 1000000)
            { MessageBox.Show("Biaya harus antara 0 - 1.000.000!"); return; }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateServis", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id",     id));
                        cmd.Parameters.Add(new SqlParameter("@id_ken", idK));
                        cmd.Parameters.Add(new SqlParameter("@id_u",   idU));
                        cmd.Parameters.Add(new SqlParameter("@tgl",    dtptanggal.Value));
                        cmd.Parameters.Add(new SqlParameter("@jenis",  txtjenisservis.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@suku",   txtsukucadang.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@biaya",  (int)biaya));

                        if (txtcatatan.Text == "")
                            cmd.Parameters.Add(new SqlParameter("@catatan", DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter("@catatan", txtcatatan.Text.Trim()));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diubah!");
                ClearS(); LoadServis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update servis: " + ex.Message);
            }
        }

        private void btndels_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvServis);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteServis", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!");
                ClearS(); LoadServis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus servis: " + ex.Message);
            }
        }

        private void btncaris_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SearchServis", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cari", txtcaris.Text.Trim()));

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds, "Hasil");

                        bsServis.DataSource = ds.Tables["Hasil"];
                        dgvServis.DataSource = bsServis;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari servis: " + ex.Message);
            }
        }

        private void btnloads_Click(object sender, EventArgs e) { LoadServis(); }

        // Klik baris -> bsServis_PositionChanged otomatis dipanggil
        private void dgvServis_CellClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnclears_Click(object sender, EventArgs e) { ClearS(); }
        void ClearS()
        {
            txtjenisservis.Clear(); txtsukucadang.Clear(); txtbiaya.Clear();
            txtcatatan.Clear(); txtcaris.Clear();
            dtptanggal.Value = DateTime.Now;
            cmbkendaraan.SelectedIndex = 0;
            cmbusers.SelectedIndex = 0;
        }

        // Cetak Nota Servis - pakai named handler (bukan lambda)
        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (dgvServis.CurrentRow == null)
            { MessageBox.Show("Pilih data servis yang ingin dicetak!"); return; }

            DataGridViewRow r = dgvServis.CurrentRow;
            printText  = "================================\n";
            printText += "   NOTA SERVIS MOTOR BENGKEL\n";
            printText += "================================\n\n";
            printText += "ID Servis    : " + r.Cells["ID Servis"].Value + "\n";
            printText += "Plat No      : " + r.Cells["Plat No"].Value + "\n";
            printText += "Petugas      : " + r.Cells["Petugas"].Value + "\n";
            printText += "Tanggal      : " + Convert.ToDateTime(r.Cells["Tanggal"].Value).ToString("dd/MM/yyyy") + "\n";
            printText += "Jenis Servis : " + r.Cells["Jenis Servis"].Value + "\n";
            printText += "Suku Cadang  : " + r.Cells["Suku Cadang"].Value + "\n";
            printText += "Biaya        : Rp " + Convert.ToDecimal(r.Cells["Biaya"].Value).ToString("N0") + "\n";
            printText += "Catatan      : " + (r.Cells["Catatan"].Value == null ? "-" : r.Cells["Catatan"].Value.ToString()) + "\n\n";
            printText += "================================\n";
            printText += "       Terima Kasih!\n";

            PrintDocument doc = new PrintDocument();
            doc.PrintPage += new PrintPageEventHandler(doc_PrintPage);

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = doc;
            preview.ShowDialog();
        }

        private void doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(printText, new Font("Dubai", 11), Brushes.Black, 50, 50);
        }

        // ============================================
        // TAB USERS
        // ============================================

        void LoadUsers()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllUsers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        reader.Close();

                        bsUsers.DataSource = dt;
                        dgvUsers.DataSource = bsUsers;
                    }
                }

                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn))
                    {
                        conn.Open();
                        int total = (int)cmd.ExecuteScalar();
                        lblcountusers.Text = "Total: " + total;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data users: " + ex.Message);
            }
        }

        private void btnaddu_Click(object sender, EventArgs e)
        {
            if (txtnamauser.Text == "" || txtusername.Text == "" || txtnoteluser.Text == "" || cmbrole.SelectedIndex == -1)
            { MessageBox.Show("Semua field harus diisi!"); return; }
            if (!IsValidPhone(txtnoteluser.Text.Trim()))
            { MessageBox.Show("No Telp harus angka, 10-13 digit, dan dimulai dengan 08!"); return; }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@nama", txtnamauser.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@user", txtusername.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@telp", txtnoteluser.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@role", cmbrole.SelectedItem.ToString()));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil ditambahkan!");
                ClearU(); LoadUsers(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tambah user: " + ex.Message);
            }
        }

        private void btnupu_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvUsers);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (txtnamauser.Text == "" || txtusername.Text == "" || txtnoteluser.Text == "" || cmbrole.SelectedIndex == -1)
            { MessageBox.Show("Semua field harus diisi!"); return; }
            if (!IsValidPhone(txtnoteluser.Text.Trim()))
            { MessageBox.Show("No Telp harus angka, 10-13 digit, dan dimulai dengan 08!"); return; }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id",   id));
                        cmd.Parameters.Add(new SqlParameter("@nama", txtnamauser.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@user", txtusername.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@telp", txtnoteluser.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@role", cmbrole.SelectedItem.ToString()));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil diubah!");
                ClearU(); LoadUsers(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update user: " + ex.Message);
            }
        }

        private void btndelu_Click(object sender, EventArgs e)
        {
            int id = GetSelectedId(dgvUsers);
            if (id == -1) { MessageBox.Show("Pilih data dulu!"); return; }
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!");
                ClearU(); LoadUsers(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus user: " + ex.Message);
            }
        }

        private void btncariu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConn())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SearchUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cari", txtcariu.Text.Trim()));

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds, "Hasil");

                        bsUsers.DataSource = ds.Tables["Hasil"];
                        dgvUsers.DataSource = bsUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari user: " + ex.Message);
            }
        }

        private void btnTampilU_Click(object sender, EventArgs e) { LoadUsers(); }

        // Klik baris -> bsUsers_PositionChanged otomatis dipanggil
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e) { }

        void ClearU()
        {
            txtnamauser.Clear(); txtusername.Clear();
            txtnoteluser.Clear(); txtcariu.Clear();
            cmbrole.SelectedIndex = -1;
        }

        private void cmbrole_SelectedIndexChanged(object sender, EventArgs e) { }

        private void lblcountp_Click(object sender, EventArgs e)
        {

        }

        private void tabServis_Click(object sender, EventArgs e)
        {

        }

        private void btnclearu_Click(object sender, EventArgs e) { ClearU(); }
    }
}
