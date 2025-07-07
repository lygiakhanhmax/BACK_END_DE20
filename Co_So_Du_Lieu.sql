CREATE DATABASE QuanLyLichKham;
GO
USE QuanLyLichKham;
GO

CREATE TABLE BenhNhans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TaiKhoan NVARCHAR(50) NOT NULL,
    MatKhau NVARCHAR(50) NOT NULL,
    SoDienThoai NVARCHAR(15) NOT NULL,
    DiaChiEmail NVARCHAR(100) NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE BacSis (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    ChuyenKhoa NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(15) NOT NULL,
    Email NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE LichKhams (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BenhNhanId INT FOREIGN KEY REFERENCES BenhNhans(Id),
    BacSiId INT FOREIGN KEY REFERENCES BacSis(Id),
    NgayKham DATETIME NOT NULL,
    GioKham TIME NOT NULL,
    TrieuChung NVARCHAR(500),
    TrangThai NVARCHAR(50) DEFAULT N'Chờ khám',
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO BenhNhans (TaiKhoan, MatKhau, SoDienThoai, DiaChiEmail) VALUES
(N'Lý Gia Khánh', N'1234@aA', N'0398740501', N'giakhanhmax@gmail.com'),
(N'Lai Minh Hiệp', N'4321@aA', N'0336382612', N'bnv@gmail.com'),
(N'Nguyễn Văn Khoa', N'5678@bB', N'0336382613', N'ctt@gmail.com'),
(N'Nguyễn Lê Đăng Khánh', N'9876@cC', N'0336382614', N'dlv@gmail.com');

INSERT INTO BacSis (HoTen, ChuyenKhoa, SoDienThoai, Email) VALUES
(N'BS. Nguyễn Văn Hùng', N'Nội khoa', N'0901234567', N'hungnv@benhvien.com'),
(N'BS. Trần Thị Lan', N'Nhi khoa', N'0901234568', N'lantt@benhvien.com'),
(N'BS. Lê Minh Tâm', N'Tim mạch', N'0901234569', N'tamlm@benhvien.com');

INSERT INTO LichKhams (BenhNhanId, BacSiId, NgayKham, GioKham, TrieuChung, TrangThai) VALUES
(1, 1, '2024-07-15', '08:00', N'Đau đầu, chóng mặt', N'Chờ khám'),
(2, 2, '2024-07-16', '09:30', N'Sốt, ho', N'Đã khám'),
(1, 3, '2024-07-17', '14:00', N'Đau ngực', N'Chờ khám');

-- Tạo SQL login mới
CREATE LOGIN qlkadmin WITH PASSWORD = 'Admin123!';
GO

-- Sử dụng database
USE QuanLyLichKham;
GO

-- Tạo user cho login
CREATE USER qlkadmin FOR LOGIN qlkadmin;
GO

-- Gán quyền db_owner
ALTER ROLE db_owner ADD MEMBER qlkadmin;
GO

-- Kiểm tra xem login đã tồn tại chưa
SELECT name FROM sys.sql_logins WHERE name = 'qlkadmin';

-- Kiểm tra quyền
SELECT 
    dp.name AS principal_name,
    dp.type_desc AS principal_type_desc,
    r.name AS role_name
FROM sys.database_role_members rm
INNER JOIN sys.database_principals dp ON rm.member_principal_id = dp.principal_id
INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
WHERE dp.name = 'qlkadmin';