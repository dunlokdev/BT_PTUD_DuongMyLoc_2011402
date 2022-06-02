create database QlSinhVien
go 

use QlSinhVien
go

create table Lop (
ID int identity(1,1) primary key,
TenLop varchar(15)
)

create table SinhVien (
ID int identity(1,1) primary key,
Ten nvarchar(100),
MaLop int REFERENCES Lop(ID)
)

insert into Lop values ('CTK45A')
insert into Lop values ('CTK45B')
insert into Lop values ('CTK44A')
insert into Lop values ('CTK44B')
insert into Lop values ('CTK43')
insert into Lop values ('CTK42')

insert into SinhVien values (N'Dương Mỹ Lộc', 7)
insert into SinhVien values (N'Nguyễn Thành Long', 8)
insert into SinhVien values (N'Nguyễn Thị Huế', 7)
insert into SinhVien values (N'Trần Thị Ngọc Ánh', 8)
insert into SinhVien values (N'Vũ Ngọc Tuấn Anh', 8)
insert into SinhVien values (N'Nguyễn Việt Hoàng', 9)
insert into SinhVien values (N'Vũ Quang Thanh', 10)
insert into SinhVien values (N'Lâm Ngọc Yến', 11)
insert into SinhVien values (N'Triệu Trọng Hậu', 12)
insert into SinhVien values (N'Trần Diệu Đông', 9)


select * from Lop
select * from SinhVien
go

-- 
CREATE PROC usp_InsertSinhVien @Ten nvarchar(100), @Lop int
as 
	INSERT INTO SinhVien VALUES(@Ten, @Lop)	
go

