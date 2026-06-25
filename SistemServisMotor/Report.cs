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
    public partial class Report : Form
    {
        DAL dbLogic = new DAL();
        SqlConnection conn = new SqlConnection(DAL.GetConnectionString());
        DataTable dtServis;

        CetakServis listRiwayat = new CetakServis();

        int idServis { get; set; }

        // Constructor: terima id_servis (1 servis spesifik)
        // sesuai sp_PrintServis yang baru filter per @id_servis
        public Report(int IdServis)
        {
            InitializeComponent();

            idServis = IdServis;

            try
            {
                // Ambil data 1 servis (1 row nota)
                dtServis = dbLogic.GetRiwayatServis(idServis);

                listRiwayat.SetDataSource(dtServis);

                crystalReportViewer1.ReportSource = listRiwayat;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }
    }
}
