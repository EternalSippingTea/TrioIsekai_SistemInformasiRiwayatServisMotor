# 🛠️ Sistem Pencatat Servis Motor (Bengkel)

Sistem Informasi berbasis Desktop untuk mengelola data operasional bengkel motor, mulai dari manajemen pelanggan, kendaraan, hingga pencatatan transaksi servis dan cetak nota.

## 🌟 Fitur Utama

- **🔑 Sistem Login & Keamanan**: 
  - Login menggunakan Username dan Nomor Telepon.
  - Role-based Access: Membedakan hak akses antara **Admin** (Full Access) dan **Petugas** (View Only/Limited Delete).
- **📋 Manajemen Pelanggan (CRM)**: Input, Update, Delete, dan Cari data pelanggan.
- **🏍️ Manajemen Kendaraan**: Pencatatan data kendaraan yang terhubung langsung dengan ID pelanggan pemiliknya.
- **🔧 Pencatatan Servis**:
  - Riwayat servis lengkap (Tanggal, Jenis Servis, Suku Cadang, Biaya, Catatan).
  - Integrasi pemilihan Kendaraan dan Petugas melalui ComboBox.
  - **🖨️ Cetak Nota**: Fitur print preview untuk mencetak nota servis secara fisik/digital.
- **👥 Manajemen User**: Kelola petugas yang memiliki akses ke sistem.
- **📊 Dashboard Interaktif**: 
  - Navigasi antar modul menggunakan Tab Control.
  - Pencarian data (Search) di setiap modul dengan update real-time.
  - Total data counter untuk masing-masing modul.

## 🚀 Teknologi yang Digunakan

- **Language**: C# (C-Sharp)
- **Framework**: .NET Framework (Windows Forms)
- **Database**: SQL Server
- **Tools**: 
  - ADO.NET (SqlDataReader, SqlDataAdapter) untuk komunikasi data.
  - `System.Drawing.Printing` untuk fitur cetak nota.

## 📂 Struktur Proyek

- `Form1.cs`: Halaman Login dan validasi koneksi database.
- `Form2.cs`: Dashboard Utama dengan 4 Tab utama (Pelanggan, Kendaraan, Servis, Users).
- `DatabaseHelper.cs`: Helper class untuk mengelola koneksi string ke database.
- `App.config`: Konfigurasi aplikasi termasuk connection string.

## 🛠️ Cara Menjalankan

1. Pastikan Anda memiliki **SQL Server** yang berjalan.
2. Buat database sesuai dengan skema (Pelanggan, Kendaraan, Servis, Users).
3. Sesuaikan `connectionString` di file `App.config`.
4. Buka file `.slnx` atau `.csproj` menggunakan **Visual Studio**.
5. Build dan Run (F5).

## 📸 Dokumentasi Screenshot

Berikut adalah gambaran sistem yang sedang berjalan:

### 1. Form Koneksi & Login
*Menampilkan status koneksi database saat aplikasi dijalankan dan halaman autentikasi.*
![Form Koneksi & Login](../screenshots/login_con.png)

### 2. Form Input Data
*Proses pengisian data pada modul Pelanggan, Kendaraan, atau Servis.*
![Form Input Data](../screenshots/input_data.png)

### 3. Form Tampilan Data
*Tampilan DataGridView yang menampilkan seluruh record dari database.*
![Form Tampilan Data](../screenshots/tampil_data.png)

### 4. Bukti Operasi CRUD & Search
*Dokumentasi hasil setelah melakukan Insert, Update, Delete, maupun fitur pencarian.*
![Bukti CRUD & Search](../screenshots/crud_search_logic.png)

---
*Dibuat untuk Tugas PA Basis Data - SMT 4.*
