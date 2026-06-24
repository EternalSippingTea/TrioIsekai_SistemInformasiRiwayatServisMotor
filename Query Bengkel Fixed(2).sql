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

/*
-- Data Dummy
-- =============================================
-- DATA DUMMY PELANGGAN (10 data)
-- =============================================
INSERT INTO Pelanggan (nama, alamat, no_hp) VALUES
('Budi Santoso',      'Jl. Magelang No. 12, Yogyakarta',      '081234567890'),
('Siti Rahayu',       'Jl. Kaliurang Km. 5, Sleman',          '082345678901'),
('Ahmad Fauzi',       'Jl. Solo No. 45, Klaten',              '083456789012'),
('Dewi Lestari',      'Jl. Parangtritis No. 7, Bantul',       '085678901234'),
('Eko Prasetyo',      'Jl. Godean No. 23, Yogyakarta',        '087890123456'),
('Fitria Handayani',  'Jl. Wates Km. 3, Kulon Progo',         '088901234567'),
('Gunawan Hadi',      'Jl. Imogiri No. 88, Bantul',           '081122334455'),
('Hani Permatasari',  'Jl. Ringroad Utara No. 15, Sleman',    '082233445566'),
('Irwan Kusuma',      'Jl. Wonosari No. 30, Gunungkidul',     '083344556677'),
('Joko Widiatmoko',   'Jl. Monjali No. 5, Yogyakarta',        '085566778899');
GO

-- =============================================
-- DATA DUMMY KENDARAAN (13 data)
-- =============================================
INSERT INTO Kendaraan (id_pelanggan, merk, plat_no, tahun) VALUES
(1,  'Honda Beat',        'AB 1234 AA', 2019),
(1,  'Toyota Avanza',     'AB 5678 BB', 2021),  -- Budi punya 2 kendaraan
(2,  'Yamaha NMAX',       'AB 2345 CC', 2020),
(3,  'Honda Vario 125',   'AD 3456 DD', 2018),
(4,  'Suzuki Ertiga',     'AB 4567 EE', 2022),
(5,  'Mitsubishi Xpander','AB 5678 FF', 2021),
(6,  'Honda Scoopy',      'AB 6789 GG', 2020),
(7,  'Yamaha Mio M3',     'AB 7890 HH', 2017),
(7,  'Daihatsu Xenia',    'AB 7891 HH', 2019),  -- Gunawan punya 2 kendaraan
(8,  'Honda Jazz',        'AB 8901 II', 2018),
(9,  'Toyota Kijang',     'AB 9012 JJ', 2015),
(10, 'Kawasaki KLX',      'AB 0123 KK', 2022),
(10, 'Honda Mobilio',     'AB 0124 KK', 2020);  -- Joko punya 2 kendaraan
GO

-- =============================================
-- DATA DUMMY USERS (5 data)
-- =============================================
INSERT INTO Users (nama, username, no_telp, role) VALUES
('Rizky Adriansyah',  'rizky.admin',    '081111222233', 'admin'),
('Slamet Riyadi',     'slamet.petugas', '082222333344', 'petugas'),
('Nurul Hidayah',     'nurul.petugas',  '083333444455', 'petugas'),
('Dani Firmansyah',   'dani.admin',     '084444555566', 'admin'),
('Wahyu Setiawan',    'wahyu.petugas',  '085555666677', 'petugas');
GO

-- =============================================
-- DATA DUMMY SERVIS (15 data)
-- =============================================
INSERT INTO Servis (id_kendaraan, id_user, Tanggal, JenisServis, SukuCadang, Biaya, Catatan) VALUES
(1,  2, '2024-01-05', 'Ganti Oli',            'Oli Mesin Shell 10W-40',          75000,  'Ganti oli rutin 3 bulan'),
(2,  3, '2024-01-10', 'Tune Up',              'Busi NGK, Filter Udara',           150000, 'Mesin terasa berat saat start'),
(3,  2, '2024-01-15', 'Ganti Ban',            'Ban Michelin 90/80-14',            350000, 'Ban belakang sudah botak'),
(4,  5, '2024-02-01', 'Servis Rem',           'Kampas Rem Depan Belakang',        120000, 'Rem bunyi saat ditekan'),
(5,  3, '2024-02-10', 'Ganti Oli + Tune Up',  'Oli Mobil 5W-30, Busi Denso',     250000, 'Servis rutin 10.000 km'),
(6,  2, '2024-02-20', 'Ganti Aki',            'Aki Yuasa 12V 5Ah',               350000, 'Motor susah dinyalakan'),
(7,  5, '2024-03-05', 'Ganti Oli',            'Oli Mesin Pertamina 10W-40',       65000,  'Ganti oli rutin'),
(8,  3, '2024-03-12', 'Servis AC',            'Freon AC R134a, Filter Kabin',     450000, 'AC tidak dingin'),
(9,  2, '2024-03-20', 'Tune Up',              'Filter Udara, Busi, Throttle Body',200000, 'Bensin boros'),
(10, 5, '2024-04-01', 'Ganti Kampas Kopling', 'Kampas Kopling Honda Jazz',        500000, 'Kopling selip saat akselerasi'),
(11, 3, '2024-04-15', 'Ganti Timing Belt',    'Timing Belt Toyota',               750000, 'Sudah 80.000 km belum pernah ganti'),
(12, 2, '2024-05-02', 'Ganti Oli Gardan',     'Oli Gardan Shell Advance 90',      85000,  'Servis rutin'),
(13, 5, '2024-05-10', 'Servis Suspensi',      'Shock Absorber Depan KYB',         900000, 'Mobil oleng saat menikung'),
(1,  3, '2024-05-20', 'Ganti Oli',            'Oli Mesin Shell 10W-40',           75000,  'Servis rutin kedua'),
(4,  2, '2024-06-01', 'Ganti Ban',            'Ban IRC 80/90-14',                 280000, 'Ban depan kempes terus');
GO
*/

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
        RAISERROR('No. HP sudah terdaftar!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Pelanggan WHERE nama = @nama AND alamat = @alamat)
    BEGIN
        RAISERROR('Pelanggan dengan nama dan alamat yang sama sudah terdaftar!', 16, 1);
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

    IF EXISTS (SELECT 1 FROM Pelanggan WHERE no_hp = @no_hp)
    BEGIN
        RAISERROR('No. HP sudah terdaftar!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Pelanggan WHERE nama = @nama AND alamat = @alamat)
    BEGIN
        RAISERROR('Pelanggan dengan nama dan alamat yang sama sudah terdaftar!', 16, 1);
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
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO Kendaraan(id_pelanggan, merk, plat_no, tahun)
        VALUES(@id_pel, @merk, @plat_no, @tahun);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR('Gagal menyimpan data kendaraan!', 16, 1);
    END CATCH
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
    IF EXISTS (SELECT 1 FROM Kendaraan WHERE plat_no = @plat_no AND id_kendaraan <> @id)
    BEGIN
        RAISERROR('Plat No. sudah terdaftar!', 16, 1);
        RETURN;
    END
    IF NOT EXISTS (SELECT 1 FROM Pelanggan WHERE id_pelanggan = @id_pel)
    BEGIN
        RAISERROR('Pelanggan tidak ditemukan!', 16,1);
        RETURN;
    END
    IF NOT EXISTS (SELECT 1 FROM Kendaraan WHERE id_kendaraan = @id)
    BEGIN
        RAISERROR('Kendaraan tidak ditemukan!', 16, 1);
        RETURN;
    END
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE Kendaraan
        SET
            id_pelanggan = @id_pel,
            merk = @merk,
            plat_no = @plat_no,
            tahun = @tahun
        WHERE id_kendaraan = @id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR('Gagal mengubah data kendaraan!', 16, 1);
    END CATCH
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
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM Kendaraan
        WHERE id_kendaraan = @id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RAISERROR('Gagal menghapus data kendaraan!', 16, 1);
    END CATCH
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
    
-- SP Insert User
CREATE PROCEDURE sp_InsertUser
    @nama VARCHAR(100),
    @user VARCHAR(50),
    @telp VARCHAR(13),
    @role VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE username = @user AND no_telp = @telp)
    BEGIN
        RAISERROR('Username atau No. Telepon sudah digunakan!', 16,1);
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

    IF EXISTS (SELECT 1 FROM Users WHERE username = @user AND no_telp = @telp)
    BEGIN
        RAISERROR('Username atau No. Telepon sudah digunakan!', 16,1);
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

select * from Users-- Fitur PrintCREATE PROCEDURE sp_PrintServis
    @id_kendaraan INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT
        Users.nama AS Nama_User,
        Users.role AS Role,
        Kendaraan.merk AS Merk,
        Kendaraan.tahun AS Tahun,
        Pelanggan.nama AS Nama_Pelanggan,
        Kendaraan.plat_no AS No_Plat,
        Servis.Tanggal,
        Servis.JenisServis AS Jenis_Servis,
        Servis.SukuCadang AS Suku_Cadang,
        Servis.Biaya,
        Servis.Catatan
    FROM
        Servis
    JOIN
        Kendaraan ON Servis.id_kendaraan = Kendaraan.id_kendaraan
    JOIN
        Pelanggan ON Kendaraan.id_pelanggan = Pelanggan.id_pelanggan
    JOIN
        Users ON Servis.id_user = Users.id_user
    WHERE 
        Kendaraan.id_kendaraan = @id_kendaraan
    ORDER BY 
        Servis.Tanggal DESC;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Tidak ada data servis!', 16, 1);
    END
END;EXEC sp_PrintServis;-- Alter validasi jumlah digit plat no untuk tabel kendaraanALTER TABLE Kendaraan
ADD CONSTRAINT CK_Kendaraan_plat_no
CHECK (LEN(plat_no) BETWEEN 1 AND 11);