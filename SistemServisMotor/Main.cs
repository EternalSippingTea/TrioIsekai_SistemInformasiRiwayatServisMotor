using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;

namespace SistemServisMotor
{
    public partial class MainForm : Form
    {
        int userID;
        string userName, userRole;

        // Flag untuk membedakan logout vs close beneran
        bool isLogout = false;

        // ===== Connection & DAL =====
        SqlConnection conn = new SqlConnection(DAL.GetConnectionString());
        DAL dbLogic = new DAL();

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
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
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

            // ===== UCP 2 #5 - Connect BindingNavigator ke BindingSource =====
            bnPelanggan.BindingSource = bsPelanggan;
            bnKendaraan.BindingSource = bsKendaraan;
            bnServis.BindingSource    = bsServis;
            bnUsers.BindingSource     = bsUsers;

            // Wire PositionChanged events
            bsPelanggan.PositionChanged += bsPelanggan_PositionChanged;
            bsKendaraan.PositionChanged += bsKendaraan_PositionChanged;
            bsServis.PositionChanged    += bsServis_PositionChanged;
            bsUsers.PositionChanged     += bsUsers_PositionChanged;

            // Load tab pertama + combobox
            LoadPelanggan();
            LoadCombos();
        }

        // ============ POSITION CHANGED HANDLERS ============

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

            string plat = row["Plat No"].ToString();
            for (int i = 0; i < cmbkendaraan.Items.Count; i++)
            {
                if (cmbkendaraan.Items[i].ToString().Contains(plat))
                { cmbkendaraan.SelectedIndex = i; break; }
            }

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
            try
            {
                // === Pelanggan ===
                DataTable dtPel = dbLogic.GetPelangganForCombo();
                cmbpelanggan.Items.Clear();
                cmbpelanggan.Items.Add("-- Pilih Pelanggan --");
                foreach (DataRow row in dtPel.Rows)
                {
                    cmbpelanggan.Items.Add(row["id_pelanggan"] + " - " + row["nama"]);
                }
                cmbpelanggan.SelectedIndex = 0;

                // === Kendaraan ===
                DataTable dtKen = dbLogic.GetKendaraanForCombo();
                cmbkendaraan.Items.Clear();
                cmbkendaraan.Items.Add("-- Pilih Kendaraan --");
                foreach (DataRow row in dtKen.Rows)
                {
                    cmbkendaraan.Items.Add(row["id_kendaraan"] + " - " + row["info"]);
                }
                cmbkendaraan.SelectedIndex = 0;

                // === Users ===
                DataTable dtUsr = dbLogic.GetUsersForCombo();
                cmbusers.Items.Clear();
                cmbusers.Items.Add("-- Pilih Petugas --");
                foreach (DataRow row in dtUsr.Rows)
                {
                    cmbusers.Items.Add(row["id_user"] + " - " + row["nama"]);
                }
                cmbusers.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load combo: " + ex.Message);
            }
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                isLogout = true;            // tandai bahwa close ini karena logout
                new LoginForm().Show();
                this.Close();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Hanya exit aplikasi kalau bukan logout
            // (kalau logout, biarkan LoginForm yang lanjut)
            if (!isLogout)
                Application.Exit();
        }

        // ============================================
        // TAB PELANGGAN
        // ============================================

        void LoadPelanggan()
        {
            try
            {
                DataTable dt = dbLogic.GetAllPelanggan();

                bsPelanggan.DataSource = dt;
                dgvPelanggan.DataSource = bsPelanggan;

                lblcountp.Text = "Total: " + dbLogic.CountPelanggan();
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
                dbLogic.InsertPelanggan(
                    txtnamapel.Text.Trim(),
                    txtalamat.Text.Trim(),
                    txtnohp.Text.Trim());

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
                dbLogic.UpdatePelanggan(id,
                    txtnamapel.Text.Trim(),
                    txtalamat.Text.Trim(),
                    txtnohp.Text.Trim());

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
                dbLogic.DeletePelanggan(id);

                MessageBox.Show("Data berhasil dihapus!");
                ClearP(); LoadPelanggan(); LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus pelanggan: " + ex.Message);
            }
        }

        private void btncarip_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = dbLogic.SearchPelanggan(txtcarip.Text.Trim());
                bsPelanggan.DataSource = dt;
                dgvPelanggan.DataSource = bsPelanggan;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari pelanggan: " + ex.Message);
            }
        }

        private void btnloadp_Click(object sender, EventArgs e) { LoadPelanggan(); }

        // Klik baris -> bsPelanggan_PositionChanged otomatis dipanggil
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
                DataTable dt = dbLogic.GetAllKendaraan();

                bsKendaraan.DataSource = dt;
                dgvKendaraan.DataSource = bsKendaraan;

                lblcountk.Text = "Total: " + dbLogic.CountKendaraan();
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
            if (txtplano.Text.Trim().Length > 11)
            { MessageBox.Show("Plat Nomor maksimal 11 karakter!"); return; }
            if (txtmerk.Text.Trim().Length > 50)
            { MessageBox.Show("Merk maksimal 50 karakter!"); return; }

            int tahun = 0;
            object tahunObj = null;
            if (txttahunken.Text != "")
            {
                if (!int.TryParse(txttahunken.Text, out tahun) || tahun < 2000 || tahun > 2040)
                { MessageBox.Show("Tahun harus angka antara 2000 - 2040!"); return; }
                tahunObj = tahun;
            }

            try
            {
                dbLogic.InsertKendaraan(idP, txtmerk.Text.Trim(), txtplano.Text.Trim(), tahunObj);

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
            if (txtplano.Text.Trim().Length > 11)
            { MessageBox.Show("Plat Nomor maksimal 11 karakter!"); return; }
            if (txtmerk.Text.Trim().Length > 50)
            { MessageBox.Show("Merk maksimal 50 karakter!"); return; }

            int tahun = 0;
            object tahunObj = null;
            if (txttahunken.Text != "")
            {
                if (!int.TryParse(txttahunken.Text, out tahun) || tahun < 2000 || tahun > 2040)
                { MessageBox.Show("Tahun harus angka antara 2000 - 2040!"); return; }
                tahunObj = tahun;
            }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                dbLogic.UpdateKendaraan(id, idP, txtmerk.Text.Trim(), txtplano.Text.Trim(), tahunObj);

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
                dbLogic.DeleteKendaraan(id);

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
                DataTable dt = dbLogic.SearchKendaraan(txtcarik.Text.Trim());
                bsKendaraan.DataSource = dt;
                dgvKendaraan.DataSource = bsKendaraan;
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
                DataTable dt = dbLogic.GetAllServis();

                bsServis.DataSource = dt;
                dgvServis.DataSource = bsServis;

                lblcounts.Text = "Total: " + dbLogic.CountServis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data servis: " + ex.Message);
            }
        }

        // Insert Servis - pakai OUTPUT parameter (di dalam DAL)
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
            if (biaya < 0 || biaya > 10000000)
            { MessageBox.Show("Biaya harus antara 0 - 10.000.000!"); return; }

            try
            {
                object catatanObj = txtcatatan.Text == "" ? null : (object)txtcatatan.Text.Trim();

                int newId = dbLogic.InsertServis(
                    idK, idU, dtptanggal.Value,
                    txtjenisservis.Text.Trim(),
                    txtsukucadang.Text.Trim(),
                    (int)biaya,
                    catatanObj);

                MessageBox.Show("Data berhasil ditambahkan! ID Servis baru: " + newId);
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
            if (biaya < 0 || biaya > 10000000)
            { MessageBox.Show("Biaya harus antara 0 - 10.000.000!"); return; }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                object catatanObj = txtcatatan.Text == "" ? null : (object)txtcatatan.Text.Trim();

                dbLogic.UpdateServis(id, idK, idU, dtptanggal.Value,
                    txtjenisservis.Text.Trim(),
                    txtsukucadang.Text.Trim(),
                    (int)biaya,
                    catatanObj);

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
                dbLogic.DeleteServis(id);

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
                DataTable dt = dbLogic.SearchServis(txtcaris.Text.Trim());
                bsServis.DataSource = dt;
                dgvServis.DataSource = bsServis;
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

        // Cetak Riwayat Servis - buka RekapServis form (filter kendaraan + tanggal -> Crystal Report)
        private void btnPrint_Click(object sender, EventArgs e)
        {
            RekapServis rekap = new RekapServis();
            rekap.Show();
        }

        // ============================================
        // TAB USERS
        // ============================================

        void LoadUsers()
        {
            try
            {
                DataTable dt = dbLogic.GetAllUsers();

                bsUsers.DataSource = dt;
                dgvUsers.DataSource = bsUsers;

                lblcountusers.Text = "Total: " + dbLogic.CountUsers();
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
                dbLogic.InsertUser(
                    txtnamauser.Text.Trim(),
                    txtusername.Text.Trim(),
                    txtnoteluser.Text.Trim(),
                    cmbrole.SelectedItem.ToString());

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
                dbLogic.UpdateUser(id,
                    txtnamauser.Text.Trim(),
                    txtusername.Text.Trim(),
                    txtnoteluser.Text.Trim(),
                    cmbrole.SelectedItem.ToString());

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
                dbLogic.DeleteUser(id);

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
                DataTable dt = dbLogic.SearchUser(txtcariu.Text.Trim());
                bsUsers.DataSource = dt;
                dgvUsers.DataSource = bsUsers;
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

        // ============ IMPORT EXCEL ============
        // Pattern dari reference: 2 step
        // Step 1: btnImpExcel �� load file Excel ke DGV (preview)
        // Step 2: btnImpDb �� insert data dari DGV ke database

        private void btnImpExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog =
                new OpenFileDialog { Filter = "Excel Workbook|*.xlsx" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    using (var stream = System.IO.File.Open(filePath,
                                                            System.IO.FileMode.Open,
                                                            System.IO.FileAccess.Read))
                    {
                        using (var reader =
                            ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(
                                new ExcelDataReader.ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) =>
                                        new ExcelDataReader.ExcelDataTableConfiguration()
                                        {
                                            UseHeaderRow = true   // baris 1 = header
                                        }
                                });

                            DataTable dt = result.Tables[0];

                            // Tampilkan ke DGV (replace data DB sementara)
                            dgvPelanggan.DataSource = dt;
                            dgvPelanggan.Enabled = false;   // disable edit

                            // Disable tombol CRUD biar nggak gangguan
                            btnImpDb.Enabled = true;
                            btnaddp.Enabled = false;
                            btnupp.Enabled = false;
                            btndelp.Enabled = false;
                            btncarip.Enabled = false;
                            btnloadp.Enabled = false;
                        }
                    }
                }
            }
        }

        private void btnImpDb_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)dgvPelanggan.DataSource;

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk diimport.");
                    return;
                }

                int sukses = 0;

                foreach (DataRow row in dt.Rows)
                {
                    // Pakai column INDEX (bukan name) supaya tidak case-sensitive
                    // dan tidak peduli dengan whitespace/format header Excel
                    string nama = row[0].ToString().Trim();    // kolom 1
                    string alamat = row[1].ToString().Trim();  // kolom 2
                    string no_hp = row[2].ToString().Trim();   // kolom 3

                    // Auto-prefix "0" kalau hilang
                    // (Excel auto-strip leading zero kalau cell di-treat sebagai Number)
                    if (!string.IsNullOrEmpty(no_hp) && !no_hp.StartsWith("0"))
                        no_hp = "0" + no_hp;

                    // Skip baris kosong
                    if (string.IsNullOrEmpty(nama)) continue;

                    try
                    {
                        dbLogic.InsertPelanggan(nama, alamat, no_hp);
                        sukses++;
                    }
                    catch
                    {
                        // Skip baris yg gagal (duplikat dll), lanjut ke berikutnya
                        continue;
                    }
                }

                MessageBox.Show("Berhasil import " + sukses + " data pelanggan.");

                // Re-enable tombol CRUD
                dgvPelanggan.Enabled = true;
                btnImpDb.Enabled = false;
                btnaddp.Enabled = true;
                btnupp.Enabled = true;
                btndelp.Enabled = (userRole == "admin");   // tetap respect role
                btncarip.Enabled = true;
                btnloadp.Enabled = true;

                // Reload data asli dari DB
                ClearP();
                LoadPelanggan();
                LoadCombos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal import: " + ex.Message);
            }
        }

        private void cmbrole_SelectedIndexChanged(object sender, EventArgs e) { }


        private void btnclearu_Click(object sender, EventArgs e) { ClearU(); }
    }
}
