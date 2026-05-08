-- Create dan use database
CREATE DATABASE DBBengkel;
USE DBBengkel;

-- Create login admin & petugas
CREATE LOGIN admin WITH PASSWORD = 'admin123';
CREATE LOGIN petugas WITH PASSWORD = 'petugas123';

-- Create user admin & petugas
CREATE USER admin_user FOR LOGIN admin;
CREATE USER petugas_user FOR LOGIN petugas;

-- Create role admin dan grant privileges
CREATE ROLE role_admin;

GRANT SELECT, INSERT, UPDATE, DELETE ON Pelanggan TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Kendaraan TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Servis TO role_admin;

ALTER ROLE role_admin ADD MEMBER admin_user;

-- Create role petugas dan grant privileges
CREATE ROLE role_petugas;

GRANT SELECT, INSERT, UPDATE ON Pelanggan TO role_petugas;
GRANT SELECT, INSERT, UPDATE ON Kendaraan TO role_petugas;
GRANT SELECT, INSERT, UPDATE ON Users TO role_petugas;
GRANT SELECT, INSERT, UPDATE ON Servis TO role_petugas;

ALTER ROLE role_petugas ADD MEMBER petugas_user;

-- Memastikan petugas tidak bisa delete
DENY DELETE ON Pelanggan TO role_petugas;
DENY DELETE ON Kendaraan TO role_petugas;
DENY DELETE ON Users TO role_petugas;
DENY DELETE ON Servis TO role_petugas;

-- Create table pelanggan
CREATE TABLE Pelanggan(
	id_pelanggan INT IDENTITY(1,1) PRIMARY KEY,
	nama VARCHAR(100) NOT NULL,
	alamat VARCHAR(100) NOT NULL,
	no_hp VARCHAR(13)
		CHECK(
			no_hp NOT LIKE '%[^0-9]%'
			AND LEN(no_hp) BETWEEN 10 AND 13
			AND no_hp LIKE '08%'
			)
);

-- Create table kendaraan
CREATE TABLE Kendaraan(
	id_kendaraan INT IDENTITY(1,1) PRIMARY KEY,
	id_pelanggan INT,
	merk VARCHAR(50) NOT NULL,
	plat_no VARCHAR(11) NOT NULL,
	tahun INT CHECK (tahun BETWEEN 2000 AND 2040),
	FOREIGN KEY (id_pelanggan) REFERENCES Pelanggan(id_pelanggan)
);

-- Create table users
CREATE TABLE Users(
	id_user INT IDENTITY(1,1) PRIMARY KEY,
	nama VARCHAR(100) NOT NULL,
	username VARCHAR(50) NOT NULL,
	no_telp VARCHAR(13)
		CHECK(
			no_telp NOT LIKE '%[^0-9]%'
			AND LEN(no_telp) BETWEEN 10 AND 13
			AND no_telp LIKE '08%'
			),
	role VARCHAR(20)
		CHECK (role IN ('admin', 'petugas'))
);

-- Create table servis
CREATE TABLE Servis(
	id_servis INT IDENTITY(1,1) PRIMARY KEY,
	id_kendaraan INT,
	id_user INT,
	Tanggal SMALLDATETIME,
	JenisServis VARCHAR(100) NOT NULL,
	SukuCadang VARCHAR(100) NOT NULL,
	Biaya INT CHECK(Biaya >= 0 AND Biaya <= 10000000) NOT NULL,
	Catatan VARCHAR(255),
	FOREIGN KEY (id_kendaraan) REFERENCES Kendaraan (id_kendaraan),
	FOREIGN KEY (id_user) REFERENCES USERS (id_user)
);

-- =============================================
-- STORED PROCEDURES FOR PELANGGAN
-- =============================================
CREATE PROCEDURE sp_GetAllPelanggan
AS SELECT id_pelanggan AS ID, nama AS Nama, alamat AS Alamat, no_hp AS [No HP] FROM Pelanggan;
GO

CREATE PROCEDURE sp_InsertPelanggan @nama VARCHAR(100), @alamat VARCHAR(100), @no_hp VARCHAR(13)
AS INSERT INTO Pelanggan(nama, alamat, no_hp) VALUES(@nama, @alamat, @no_hp);
GO

CREATE PROCEDURE sp_UpdatePelanggan @id INT, @nama VARCHAR(100), @alamat VARCHAR(100), @no_hp VARCHAR(13)
AS UPDATE Pelanggan SET nama=@nama, alamat=@alamat, no_hp=@no_hp WHERE id_pelanggan=@id;
GO

CREATE PROCEDURE sp_DeletePelanggan @id INT
AS DELETE FROM Pelanggan WHERE id_pelanggan=@id;
GO

CREATE PROCEDURE sp_SearchPelanggan @cari VARCHAR(100)
AS SELECT id_pelanggan AS ID, nama AS Nama, alamat AS Alamat, no_hp AS [No HP] 
   FROM Pelanggan WHERE nama LIKE '%' + @cari + '%';
GO

-- =============================================
-- STORED PROCEDURES FOR KENDARAAN
-- =============================================
CREATE PROCEDURE sp_GetAllKendaraan
AS SELECT k.id_kendaraan AS ID, p.nama AS Pelanggan, k.merk AS Merk, k.plat_no AS [Plat No], k.tahun AS Tahun
   FROM Kendaraan k JOIN Pelanggan p ON k.id_pelanggan = p.id_pelanggan;
GO

CREATE PROCEDURE sp_InsertKendaraan @id_pel INT, @merk VARCHAR(50), @plat VARCHAR(10), @tahun INT
AS INSERT INTO Kendaraan(id_pelanggan, merk, plat_no, tahun) VALUES(@id_pel, @merk, @plat, @tahun);
GO

CREATE PROCEDURE sp_UpdateKendaraan @id INT, @id_pel INT, @merk VARCHAR(50), @plat VARCHAR(10), @tahun INT
AS UPDATE Kendaraan SET id_pelanggan=@id_pel, merk=@merk, plat_no=@plat, tahun=@tahun WHERE id_kendaraan=@id;
GO

CREATE PROCEDURE sp_DeleteKendaraan @id INT
AS DELETE FROM Kendaraan WHERE id_kendaraan=@id;
GO

CREATE PROCEDURE sp_SearchKendaraan @cari VARCHAR(50)
AS SELECT k.id_kendaraan AS ID, p.nama AS Pelanggan, k.merk AS Merk, k.plat_no AS [Plat No], k.tahun AS Tahun
   FROM Kendaraan k JOIN Pelanggan p ON k.id_pelanggan=p.id_pelanggan
   WHERE k.plat_no LIKE '%' + @cari + '%' OR k.merk LIKE '%' + @cari + '%';
GO

-- =============================================
-- STORED PROCEDURES FOR USERS
-- =============================================
CREATE PROCEDURE sp_GetAllUsers
AS SELECT id_user AS ID, nama AS Nama, username AS Username, no_telp AS [No Telp], role AS Role FROM Users;
GO

CREATE PROCEDURE sp_InsertUser @nama VARCHAR(100), @user VARCHAR(50), @telp VARCHAR(13), @role VARCHAR(20)
AS INSERT INTO Users(nama, username, no_telp, role) VALUES(@nama, @user, @telp, @role);
GO

CREATE PROCEDURE sp_UpdateUser @id INT, @nama VARCHAR(100), @user VARCHAR(50), @telp VARCHAR(13), @role VARCHAR(20)
AS UPDATE Users SET nama=@nama, username=@user, no_telp=@telp, role=@role WHERE id_user=@id;
GO

CREATE PROCEDURE sp_DeleteUser @id INT
AS DELETE FROM Users WHERE id_user=@id;
GO

CREATE PROCEDURE sp_SearchUser @cari VARCHAR(50)
AS SELECT id_user AS ID, nama AS Nama, username AS Username, no_telp AS [No Telp], role AS Role 
   FROM Users WHERE nama LIKE '%' + @cari + '%' OR username LIKE '%' + @cari + '%';
GO

-- =============================================
-- STORED PROCEDURES FOR SERVIS
-- =============================================
CREATE PROCEDURE sp_GetAllServis
AS SELECT s.id_servis AS ID, k.plat_no AS [Plat No], u.nama AS Petugas, s.Tanggal, 
          s.JenisServis AS [Jenis Servis], s.SukuCadang AS [Suku Cadang], s.Biaya, s.Catatan
   FROM Servis s JOIN Kendaraan k ON s.id_kendaraan = k.id_kendaraan JOIN Users u ON s.id_user = u.id_user;
GO

CREATE PROCEDURE sp_InsertServis @id_ken INT, @id_u INT, @tgl DATETIME, @jenis VARCHAR(100), @suku VARCHAR(100), @biaya INT, @catatan VARCHAR(255)
AS INSERT INTO Servis(id_kendaraan, id_user, Tanggal, JenisServis, SukuCadang, Biaya, Catatan) 
   VALUES(@id_ken, @id_u, @tgl, @jenis, @suku, @biaya, @catatan);
GO

CREATE PROCEDURE sp_UpdateServis @id INT, @id_ken INT, @id_u INT, @tgl DATETIME, @jenis VARCHAR(100), @suku VARCHAR(100), @biaya INT, @catatan VARCHAR(255)
AS UPDATE Servis SET id_kendaraan=@id_ken, id_user=@id_u, Tanggal=@tgl, JenisServis=@jenis, SukuCadang=@suku, Biaya=@biaya, Catatan=@catatan WHERE id_servis=@id;
GO

CREATE PROCEDURE sp_DeleteServis @id INT
AS DELETE FROM Servis WHERE id_servis=@id;
GO

CREATE PROCEDURE sp_SearchServis @cari VARCHAR(100)
AS SELECT s.id_servis AS ID, k.plat_no AS [Plat No], u.nama AS Petugas, s.Tanggal, 
          s.JenisServis AS [Jenis Servis], s.SukuCadang AS [Suku Cadang], s.Biaya, s.Catatan
   FROM Servis s JOIN Kendaraan k ON s.id_kendaraan=k.id_kendaraan JOIN Users u ON s.id_user=u.id_user
   WHERE k.plat_no LIKE '%' + @cari + '%' OR s.JenisServis LIKE '%' + @cari + '%';
GO
