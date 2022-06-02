using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT_QuanLySinhVien_ADO
{
    public class SinhVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public int MaLop { get; set; }

        public SinhVien() { }
        public SinhVien(DataRow row)
        {
            Id = Convert.ToInt32(row["ID"]);
            HoTen = row["Ten"].ToString();
            MaLop = Convert.ToInt32(row["MaLop"]);
        }

        public SinhVien(int id, string hoTen, int maLop)
        {
            Id = id;
            HoTen = hoTen;
            MaLop = maLop;
        }

    }
}
