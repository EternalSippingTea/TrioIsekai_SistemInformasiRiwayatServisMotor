-- Create dan use database
CREATE DATABASE DBBengkel;
USE DBBengkel;

-- Create login admin & petugas
CREATE LOGIN admin WITH PASSWORD = 'admin123',
DEFAULT_DATABASE = DBBengkel;
CREATE LOGIN petugas WITH PASSWORD = 'petugas123',
DEFAULT_DATABASE = DBBengkel;

-- Create user admin & petugas
CREATE USER admin_user FOR LOGIN admin;
CREATE USER petugas_user FOR LOGIN petugas;

-- Create role admin dan grant privileges
CREATE ROLE role_admin;

GRANT SELECT, INSERT, UPDATE, DELETE ON Pelanggan TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Kendaraan TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Servis TO role_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO role_admin;

ALTER ROLE role_admin ADD MEMBER admin_user;

-- Create role petugas dan grant privileges
CREATE ROLE role_petugas;

GRANT SELECT, INSERT, UPDATE ON Pelanggan TO role_petugas;
GRANT SELECT, INSERT, UPDATE ON Kendaraan TO role_petugas;
GRANT SELECT, INSERT, UPDATE ON Servis TO role_petugas;
GRANT SELECT ON Users TO role_petugas;

DENY DELETE ON Pelanggan TO role_petugas;
DENY DELETE ON Kendaraan TO role_petugas;
DENY DELETE ON Servis TO role_petugas;
DENY INSERT, UPDATE, DELETE ON Users TO role_petugas;

ALTER ROLE role_petugas ADD MEMBER petugas_user;


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
	plat_no VARCHAR(11) UNIQUE NOT NULL,
	tahun INT CHECK (tahun BETWEEN 2000 AND 2040),
	FOREIGN KEY (id_pelanggan) REFERENCES Pelanggan(id_pelanggan)
    ON DELETE CASCADE
);

-- Create table users
CREATE TABLE Users(
	id_user INT IDENTITY(1,1) PRIMARY KEY,
	nama VARCHAR(100) NOT NULL,
	username VARCHAR(50) UNIQUE NOT NULL,
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
	FOREIGN KEY (id_kendaraan) REFERENCES Kendaraan (id_kendaraan)
    ON DELETE CASCADE,
	FOREIGN KEY (id_user) REFERENCES USERS (id_user)
    ON DELETE SET NULL
);
GO




-- =============================================
-- STORED PROCEDURES FOR PELANGGAN
-- =============================================

-- view Pelanggan
CREATE VIEW vwPelanggan
AS
SELECT
    id_pelanggan,
	nama,
    alamat,
	no_hp
FROM Pelanggan;
GO

-- SP Get Pelanggan
CREATE PROCEDURE sp_GetAllPelanggan
AS
BEGIN
    SELECT
        id_pelanggan AS ID,
        nama AS Nama,
        alamat AS Alamat,
        no_hp AS [No HP]
    FROM vwPelanggan;
END
GO

-- SP Insert Pelanggan
CREATE PROCEDURE sp_InsertPelanggan
    @nama VARCHAR(100),
    @alamat VARCHAR(100),
    @no_hp VARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Pelanggan WHERE no_hp = @no_hp)
    BEGIN
        RAISERROR('No. HP sudah terdaftar!', 16,1);
        RETURN;
    END

    INSERT INTO Pelanggan(nama, alamat, no_hp)
    VALUES(@nama, @alamat, @no_hp);
END
GO

-- SP Update Pelanggan
CREATE PROCEDURE sp_UpdatePelanggan
    @id INT,
    @nama VARCHAR(100),
    @alamat VARCHAR(100),
    @no_hp VARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT * FROM Pelanggan WHERE id_pelanggan = @id)
    BEGIN
        RAISERROR('Pelanggan tidak ditemukan!', 16, 1);
        RETURN;
    END

    UPDATE Pelanggan
    SET
        nama = @nama,
        alamat = @alamat,
        no_hp = @no_hp
    WHERE id_pelanggan = @id;
END
GO

-- SP Delete Pelanggan
CREATE PROCEDURE sp_DeletePelanggan
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Pelanggan WHERE id_pelanggan = @id)
    BEGIN
        RAISERROR('Pelanggan tidak ditemukan!', 16, 1);
        RETURN;
    END

    DELETE FROM Pelanggan
    WHERE id_pelanggan = @id;
END
GO

-- SP Search Pelanggan
CREATE PROCEDURE sp_SearchPelanggan
    @cari VARCHAR(100)
AS
BEGIN
    SELECT
        id_pelanggan AS ID,
        nama AS Nama,
        alamat AS Alamat,
        no_hp AS [No HP]
    FROM vwPelanggan
    WHERE nama LIKE '%' + @cari + '%';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Data Pelanggan tidak ditemukan!', 16, 1);
    END
END
GO

