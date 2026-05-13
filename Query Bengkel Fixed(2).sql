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

-- =============================================
-- STORED PROCEDURES FOR KENDARAAN
-- =============================================

-- View Kendaraan
CREATE VIEW vwKendaraan
AS
SELECT
    k.id_kendaraan,
	p.nama,
	k.merk,
	k.plat_no,
	k.tahun
FROM Kendaraan k
JOIN Pelanggan p
    ON k.id_pelanggan = p.id_pelanggan;
GO 

-- View Get Kendaraan    
CREATE PROCEDURE sp_GetAllKendaraan
AS
BEGIN
    SELECT
        id_kendaraan AS [ID Kendaraan],
        nama AS Pelanggan,
        merk AS Merk,
        plat_no AS [Plat No],
        tahun AS Tahun
    FROM vwKendaraan;
END
GO

-- SP Insert Kendaraan
CREATE PROCEDURE sp_InsertKendaraan
    @id_pel INT,
    @merk VARCHAR(50),
    @plat_no VARCHAR(11),
    @tahun INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Kendaraan WHERE plat_no = @plat_no)
    BEGIN
        RAISERROR('Plat No. sudah terdaftar!', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Pelanggan WHERE id_pelanggan = @id_pel)
    BEGIN
        RAISERROR('Pelanggan tidak ditemukan!', 16,1);
        RETURN;
    END

    INSERT INTO Kendaraan(id_pelanggan, merk, plat_no, tahun)
    VALUES(@id_pel, @merk, @plat_no, @tahun);
END
GO

-- SP Update Kendaraan
CREATE PROCEDURE sp_UpdateKendaraan
    @id INT,
    @id_pel INT,
    @merk VARCHAR(50),
    @plat_no VARCHAR(11),
    @tahun INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Pelanggan WHERE id_pelanggan = @id_pel)
    BEGIN
        RAISERROR('Pelanggan tidak ditemukan!', 16,1);
        RETURN;
    END

    UPDATE Kendaraan
    SET
        id_pelanggan = @id_pel,
        merk = @merk,
        plat_no = @plat_no,
        tahun = @tahun
    WHERE id_kendaraan = @id;
END
GO

-- SP Delete Kendaraan
CREATE PROCEDURE sp_DeleteKendaraan
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Kendaraan WHERE id_kendaraan = @id)
    BEGIN
        RAISERROR('Kendaraan tidak ditemukan!', 16,1);
        RETURN;
    END

    DELETE FROM Kendaraan
    WHERE id_kendaraan = @id;
END
GO

-- SP Search Kendaraan
CREATE PROCEDURE sp_SearchKendaraan
    @cari VARCHAR(50)
AS
BEGIN
    SELECT
        id_kendaraan AS [ID Kendaraan],
        nama AS Pelanggan,
        merk AS Merk,
        plat_no AS [Plat No],
        tahun AS Tahun
    FROM vwKendaraan
    WHERE plat_no LIKE '%' + @cari + '%'
       OR merk LIKE '%' + @cari + '%';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Data Kendaraan tidak ditemukan!', 16, 1);
    END
END
GO


-- =============================================
-- STORED PROCEDURES FOR USERS
-- =============================================

-- View Users
CREATE VIEW vwUsers
AS
SELECT
	id_user,
	nama,
	username,
	no_telp,
	role
FROM Users;
GO 
    
-- SP Get Users
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SELECT
        id_user AS ID,
        nama AS Nama,
        username AS Username,
        no_telp AS [No Telp],
        role AS Role
    FROM vwUsers;
END
GO
-- SP Insert User
CREATE PROCEDURE sp_InsertUser
    @nama VARCHAR(100),
    @user VARCHAR(50),
    @telp VARCHAR(13),
    @role VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE username = @user)
    BEGIN
        RAISERROR('Username sudah digunakan!', 16,1);
        RETURN;
    END

    INSERT INTO Users(nama, username, no_telp, role)
    VALUES(@nama, @user, @telp, @role);
END
GO

-- SP Update User
CREATE PROCEDURE sp_UpdateUser
    @id INT,
    @nama VARCHAR(100),
    @user VARCHAR(50),
    @telp VARCHAR(13),
    @role VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Users WHERE id_user = @id)
    BEGIN
        RAISERROR('User tidak ditemukan!', 16,1);
        RETURN;
    END

    UPDATE Users
    SET
        nama = @nama,
        username = @user,
        no_telp = @telp,
        role = @role
    WHERE id_user = @id;
END
GO

-- SP Delete User
CREATE PROCEDURE sp_DeleteUser
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Users WHERE id_user = @id)
    BEGIN
        RAISERROR('User tidak ditemukan!', 16,1);
        RETURN;
    END

    DELETE FROM Users
    WHERE id_user = @id;
END
GO

-- SP Search User
CREATE PROCEDURE sp_SearchUser
    @cari VARCHAR(50)
AS
BEGIN
    SELECT
        id_user AS ID,
        nama AS Nama,
        username AS Username,
        no_telp AS [No Telp],
        role AS Role
    FROM vwUsers
    WHERE nama LIKE '%' + @cari + '%'
       OR username LIKE '%' + @cari + '%';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Data User tidak ditemukan!', 16, 1);
    END
END
GO


-- =============================================
-- STORED PROCEDURES FOR SERVIS
-- =============================================

-- View Servis
CREATE VIEW vwServis
AS
SELECT
	s.id_servis,
	k.plat_no,
	u.nama,
	Tanggal,
	JenisServis,
	SukuCadang,
	Biaya,
	Catatan
FROM Servis s
JOIN Kendaraan k
    ON s.id_kendaraan = k.id_kendaraan
JOIN Users u
    ON s.id_user = u.id_user;
GO 

-- SP Get Servis
CREATE PROCEDURE sp_GetAllServis
AS
BEGIN
    SELECT
	    id_servis AS [ID Servis],
	    plat_no AS [Plat No],
		Nama AS Petugas,
	    Tanggal AS Tanggal,
	    JenisServis AS [Jenis Servis],
	    SukuCadang AS [Suku Cadang],
	    Biaya AS [Biaya],
	    Catatan AS Catatan
    FROM vwServis;
END
GO

-- SP Insert Servis
CREATE PROCEDURE sp_InsertServis
    @id_ken INT,
    @id_u INT,
    @tgl DATETIME,
    @jenis VARCHAR(100),
    @suku VARCHAR(100),
    @biaya INT,
    @catatan VARCHAR(255),
    @new_id INT OUTPUT       
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Kendaraan WHERE id_kendaraan = @id_ken)
    BEGIN
        RAISERROR('Kendaraan tidak ditemukan!', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Users WHERE id_user = @id_u)
    BEGIN
        RAISERROR('User tidak ditemukan!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO Servis(id_kendaraan, id_user, Tanggal, JenisServis,
                           SukuCadang, Biaya, Catatan)
        VALUES(@id_ken, @id_u, @tgl, @jenis, @suku, @biaya, @catatan);

        SET @new_id = SCOPE_IDENTITY();  

        COMMIT TRANSACTION;            
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR('Gagal menyimpan data servis!', 16, 1);
    END CATCH
END
GO

-- SP Update Servis
CREATE PROCEDURE sp_UpdateServis
    @id     INT,
    @id_ken INT,
    @id_u   INT,
    @tgl    DATETIME,
    @jenis  VARCHAR(100),
    @suku   VARCHAR(100),
    @biaya  INT,
    @catatan VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Servis WHERE id_servis = @id)
    BEGIN
        RAISERROR('Data servis tidak ditemukan!', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Kendaraan WHERE id_kendaraan = @id_ken)
    BEGIN
        RAISERROR('Kendaraan tidak ditemukan!', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Users WHERE id_user = @id_u)
    BEGIN
        RAISERROR('User tidak ditemukan!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;

    BEGIN TRY

        UPDATE Servis
        SET
            id_kendaraan = @id_ken,
            id_user      = @id_u,
            Tanggal      = @tgl,
            JenisServis  = @jenis,
            SukuCadang   = @suku,
            Biaya        = @biaya,
            Catatan      = @catatan
        WHERE id_servis = @id;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR('Gagal mengupdate data servis!', 16, 1);
    END CATCH

END
GO


-- SP Delete Servis
CREATE PROCEDURE sp_DeleteServis
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Servis WHERE id_servis = @id)
    BEGIN
        RAISERROR('Servis tidak ditemukan!', 16,1);
        RETURN;
    END;

    DELETE FROM Servis
    WHERE id_servis = @id;
END
GO

-- SP Search Servis
CREATE PROCEDURE sp_SearchServis
    @cari VARCHAR(100)
AS
BEGIN
    SELECT
	    id_servis AS [ID Servis],
	    plat_no AS [No. Plat],
	    nama AS Petugas,
	    Tanggal AS [Tanggal Servis],
	    JenisServis AS [Jenis Servis],
	    SukuCadang AS [Suku Cadang],
	    Biaya AS [Biaya],
	    Catatan AS Catatan
    FROM vwServis
    WHERE plat_no LIKE '%' + @cari + '%'
       OR nama LIKE '%' + @cari + '%';

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Data Servis tidak ditemukan!', 16, 1);
    END
END
GO


SELECT * INTO Pelanggan_Backup FROM Pelanggan;
SELECT * INTO Kendaraan_Backup FROM Kendaraan;
SELECT * INTO Users_Backup FROM Users;
SELECT * INTO Servis_Backup FROM Servis;

