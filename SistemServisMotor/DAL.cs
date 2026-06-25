using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemServisMotor
{
    internal class DAL
    {
        // ============================================================
        // CONNECTION STRING
        // ============================================================
        public static string GetConnectionString()
        {
            string connectionString =
                $"Data Source=SipTea-LT,1433;Initial Catalog=DBBengkel;User ID=sa;Password=123;";

            return connectionString;
        }

        // Shared connection & adapter (pattern dari reference)
        SqlConnection conn = new SqlConnection(GetConnectionString());

        SqlDataAdapter da;
        DataTable dtPelanggan;
        DataTable dtKendaraan;
        DataTable dtUsers;
        DataTable dtServis;


        // ============================================================
        // PELANGGAN
        // ============================================================

        public DataTable GetAllPelanggan()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_pelanggan AS ID, nama AS Nama, alamat AS Alamat, no_hp AS [No HP] FROM vwPelanggan",
                conn);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);

            dtPelanggan = new DataTable();
            da.Fill(dtPelanggan);

            return dtPelanggan;
        }

        public int CountPelanggan()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pelanggan", conn);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void InsertPelanggan(string nama, string alamat, string no_hp)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_InsertPelanggan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@alamat", alamat);
            cmd.Parameters.AddWithValue("@no_hp", no_hp);

            cmd.ExecuteNonQuery();
        }

        public void UpdatePelanggan(int id, string nama, string alamat, string no_hp)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_UpdatePelanggan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@alamat", alamat);
            cmd.Parameters.AddWithValue("@no_hp", no_hp);

            cmd.ExecuteNonQuery();
        }

        public void DeletePelanggan(int id)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_DeletePelanggan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public DataTable SearchPelanggan(string cari)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_SearchPelanggan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@cari", cari);

            da = new SqlDataAdapter(cmd);

            dtPelanggan = new DataTable();
            da.Fill(dtPelanggan);

            return dtPelanggan;
        }


        // ============================================================
        // KENDARAAN
        // ============================================================

        public DataTable GetAllKendaraan()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_kendaraan AS [ID Kendaraan], nama_pemilik AS Pelanggan, merk AS Merk, plat_no AS [Plat No], tahun AS Tahun FROM vwKendaraan ORDER BY plat_no",
                conn);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);

            dtKendaraan = new DataTable();
            da.Fill(dtKendaraan);

            return dtKendaraan;
        }

        public int CountKendaraan()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Kendaraan", conn);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void InsertKendaraan(int id_pel, string merk, string plat_no, object tahun)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_InsertKendaraan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id_pel", id_pel);
            cmd.Parameters.AddWithValue("@merk", merk);
            cmd.Parameters.AddWithValue("@plat_no", plat_no);
            cmd.Parameters.AddWithValue("@tahun", tahun ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void UpdateKendaraan(int id, int id_pel, string merk, string plat_no, object tahun)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_UpdateKendaraan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@id_pel", id_pel);
            cmd.Parameters.AddWithValue("@merk", merk);
            cmd.Parameters.AddWithValue("@plat_no", plat_no);
            cmd.Parameters.AddWithValue("@tahun", tahun ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void DeleteKendaraan(int id)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_DeleteKendaraan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public DataTable SearchKendaraan(string cari)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_SearchKendaraan", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@cari", cari);

            da = new SqlDataAdapter(cmd);

            dtKendaraan = new DataTable();
            da.Fill(dtKendaraan);

            return dtKendaraan;
        }


        // ============================================================
        // USERS
        // ============================================================

        public DataTable GetAllUsers()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_user AS ID, nama AS Nama, username AS Username, no_telp AS [No Telp], role AS Role FROM vwUsers",
                conn);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);

            dtUsers = new DataTable();
            da.Fill(dtUsers);

            return dtUsers;
        }

        public int CountUsers()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void InsertUser(string nama, string username, string no_telp, string role)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_InsertUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.Parameters.AddWithValue("@telp", no_telp);
            cmd.Parameters.AddWithValue("@role", role);

            cmd.ExecuteNonQuery();
        }

        public void UpdateUser(int id, string nama, string username, string no_telp, string role)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_UpdateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.Parameters.AddWithValue("@telp", no_telp);
            cmd.Parameters.AddWithValue("@role", role);

            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_DeleteUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public DataTable SearchUser(string cari)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_SearchUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@cari", cari);

            da = new SqlDataAdapter(cmd);

            dtUsers = new DataTable();
            da.Fill(dtUsers);

            return dtUsers;
        }


        // ============================================================
        // SERVIS
        // ============================================================

        public DataTable GetAllServis()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_servis AS [ID Servis], plat_no AS [Plat No], nama_petugas AS Petugas, Tanggal, JenisServis AS [Jenis Servis], SukuCadang AS [Suku Cadang], Biaya, Catatan FROM vwServis ORDER BY Tanggal DESC",
                conn);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);

            dtServis = new DataTable();
            da.Fill(dtServis);

            return dtServis;
        }

        public int CountServis()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Servis", conn);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int InsertServis(int id_ken, int id_u, DateTime tgl, string jenis, string suku, int biaya, object catatan)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_InsertServis", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id_ken", id_ken);
            cmd.Parameters.AddWithValue("@id_u", id_u);
            cmd.Parameters.AddWithValue("@tgl", tgl);
            cmd.Parameters.AddWithValue("@jenis", jenis);
            cmd.Parameters.AddWithValue("@suku", suku);
            cmd.Parameters.AddWithValue("@biaya", biaya);
            cmd.Parameters.AddWithValue("@catatan", catatan ?? DBNull.Value);

            // OUTPUT parameter
            SqlParameter outputParam = new SqlParameter("@new_id", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return Convert.ToInt32(outputParam.Value);
        }

        public void UpdateServis(int id, int id_ken, int id_u, DateTime tgl, string jenis, string suku, int biaya, object catatan)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_UpdateServis", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@id_ken", id_ken);
            cmd.Parameters.AddWithValue("@id_u", id_u);
            cmd.Parameters.AddWithValue("@tgl", tgl);
            cmd.Parameters.AddWithValue("@jenis", jenis);
            cmd.Parameters.AddWithValue("@suku", suku);
            cmd.Parameters.AddWithValue("@biaya", biaya);
            cmd.Parameters.AddWithValue("@catatan", catatan ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void DeleteServis(int id)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_DeleteServis", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public DataTable SearchServis(string cari)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_SearchServis", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@cari", cari);

            da = new SqlDataAdapter(cmd);

            dtServis = new DataTable();
            da.Fill(dtServis);

            return dtServis;
        }


        // ============================================================
        // COMBOBOX HELPERS
        // ============================================================

        public DataTable GetPelangganForCombo()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_pelanggan, nama FROM Pelanggan ORDER BY nama",
                conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        public DataTable GetKendaraanForCombo()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_kendaraan, plat_no + ' - ' + merk AS info FROM Kendaraan ORDER BY plat_no",
                conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        public DataTable GetUsersForCombo()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_user, nama FROM Users ORDER BY nama",
                conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }


        // ============================================================
        // REPORTING (Crystal Report)
        // ============================================================

        // sp_PrintServis sekarang filter per id_servis (1 nota spesifik)
        // sesuai SP teammate yang baru
        public DataTable GetRiwayatServis(int id_servis)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_PrintServis", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id_servis", id_servis);

            da = new SqlDataAdapter(cmd);

            dtServis = new DataTable();
            da.Fill(dtServis);

            return dtServis;
        }

        // Ambil semua servis untuk 1 kendaraan tertentu (untuk preview di RekapServis)
        // Query langsung dari vwServis dengan filter id_kendaraan
        public DataTable GetServisByKendaraan(int id_kendaraan)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                @"SELECT id_servis AS [ID Servis],
                         plat_no AS [Plat No],
                         nama_petugas AS Petugas,
                         Tanggal,
                         JenisServis AS [Jenis Servis],
                         SukuCadang AS [Suku Cadang],
                         Biaya,
                         Catatan
                  FROM vwServis
                  WHERE id_kendaraan = @id_ken
                  ORDER BY Tanggal DESC",
                conn);
            cmd.Parameters.AddWithValue("@id_ken", id_kendaraan);

            da = new SqlDataAdapter(cmd);

            dtServis = new DataTable();
            da.Fill(dtServis);

            return dtServis;
        }

        // Lookup id_kendaraan dari plat_no
        // Return -1 kalau tidak ditemukan
        public int GetIdKendaraanByPlatNo(string plat_no)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT id_kendaraan FROM Kendaraan WHERE plat_no = @plat",
                conn);
            cmd.Parameters.AddWithValue("@plat", plat_no);

            object result = cmd.ExecuteScalar();
            return result == null ? -1 : Convert.ToInt32(result);
        }

        // Overload dengan filter tanggal - filter di C# pakai DataView
        public DataTable GetServisByKendaraan(int id_kendaraan, DateTime startDate, DateTime endDate)
        {
            DataTable dt = GetServisByKendaraan(id_kendaraan);

            DataView dv = dt.DefaultView;
            dv.RowFilter = "Tanggal >= #" + startDate.ToString("MM/dd/yyyy") +
                           "# AND Tanggal <= #" + endDate.ToString("MM/dd/yyyy 23:59:59") + "#";

            return dv.ToTable();
        }
    }
}
