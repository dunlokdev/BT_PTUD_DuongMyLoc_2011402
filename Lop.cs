using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT_QuanLySinhVien_ADO
{
    public class Lop
    {
        public int Id { get; set; }
        public string TenLop { get; set; }

        public Lop(DataRow row)
        {
            Id = Convert.ToInt32(row["ID"]);
            TenLop = row["TenLop"].ToString();
        }

        public Lop(int id, string tenLop)
        {
            Id = id;
            TenLop = tenLop;
        }
    }
}
