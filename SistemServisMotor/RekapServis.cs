using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemServisMotor
{
    public partial class RekapServis : Form
    {
        SqlConnection conn = new SqlConnection(DAL.GetConnectionString());
        DataTable dtServis;

        DAL dbLogic = new DAL();

        // Simpan id_kendaraan hasil search (untuk load servis preview)
        int selectedIdKendaraan = -1;

        public RekapServis()
        {
            InitializeComponent();
        }

        // ============ FORM LOAD ============
        private void RekapServis_Load(object sender, EventArgs e)
        {
            // Setup default date range (1 tahun ke belakang sampai hari ini)
            dtpDari.Value = DateTime.Now.AddYears(-1);
            dtpSampai.Value = DateTime.Now;

            chkPakaiTanggal.Checked = false;
            UpdateDateFilterUI();

            lblTotal.Text = "Total : 0 Servis";
        }

        // Helper - enable/disable DTPicker berdasarkan checkbox
        void UpdateDateFilterUI()
        {
            dtpDari.Enabled = chkPakaiTanggal.Checked;
            dtpSampai.Enabled = chkPakaiTanggal.Checked;
        }

        private void chkPakaiTanggal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDateFilterUI();

            // Kalau sudah ada hasil search, re-apply filter
            if (selectedIdKendaraan != -1)
            {
                LoadServisPreview();
            }
        }

        // ============ BUTTON SEARCH ============
        // Search by plat nomor → lookup id_kendaraan → load semua servis untuk kendaraan tsb
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string plat = txtCari.Text.Trim();

            if (string.IsNullOrEmpty(plat))
            {
                MessageBox.Show("Masukkan plat nomor terlebih dahulu!");
                return;
            }

            try
            {
                int idKendaraan = dbLogic.GetIdKendaraanByPlatNo(plat);

                if (idKendaraan == -1)
                {
                    MessageBox.Show("Plat nomor '" + plat + "' tidak ditemukan!");
                    selectedIdKendaraan = -1;
                    dtServis = null;
                    dgvPreview.DataSource = null;
                    lblTotal.Text = "Total : 0 Servis";
                    return;
                }

                selectedIdKendaraan = idKendaraan;
                LoadServisPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal cari: " + ex.Message);
            }
        }

        // Helper untuk load preview servis dengan/tanpa filter tanggal
        // Pakai DAL.GetServisByKendaraan (query langsung dari vwServis)
        void LoadServisPreview()
        {
            try
            {
                if (chkPakaiTanggal.Checked)
                {
                    if (dtpDari.Value > dtpSampai.Value)
                    {
                        MessageBox.Show("Tanggal 'Dari' harus lebih awal dari 'Sampai'!");
                        return;
                    }

                    dtServis = dbLogic.GetServisByKendaraan(
                        selectedIdKendaraan, dtpDari.Value, dtpSampai.Value);
                }
                else
                {
                    dtServis = dbLogic.GetServisByKendaraan(selectedIdKendaraan);
                }

                dgvPreview.DataSource = dtServis;
                lblTotal.Text = "Total : " + dtServis.Rows.Count + " Servis";

                if (dtServis.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada riwayat servis untuk kendaraan ini" +
                        (chkPakaiTanggal.Checked ? " dalam rentang tanggal tersebut" : "") + ".");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load preview: " + ex.Message);
            }
        }

        // ============ BUTTON PRINT ============
        // Print 1 servis spesifik yang dipilih user di DGV
        // (sesuai sp_PrintServis baru yang filter per @id_servis)
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (selectedIdKendaraan == -1)
            {
                MessageBox.Show("Cari plat nomor kendaraan terlebih dahulu!");
                return;
            }

            if (dtServis == null || dtServis.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data servis untuk dicetak!");
                return;
            }

            if (dgvPreview.CurrentRow == null)
            {
                MessageBox.Show("Pilih baris servis yang ingin dicetak!");
                return;
            }

            try
            {
                // Ambil id_servis dari baris yang dipilih
                int idServis = Convert.ToInt32(dgvPreview.CurrentRow.Cells["ID Servis"].Value);

                // Buka Report dengan id_servis (SP filter per servis spesifik)
                Report frm = new Report(idServis);
                frm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal buka report: " + ex.Message);
            }
        }

        // ============ BUTTON BACK ============
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblDari_Click(object sender, EventArgs e)
        {

        }
    }
}
