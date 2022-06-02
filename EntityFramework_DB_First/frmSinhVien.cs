using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace EntityFramework_DB_First
{
    public partial class frmSinhVien : Form
    {
        private QlSinhVienContext context;
        private const string _defaulSearchText = "Nhập tên sinh viên";

        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            context = new QlSinhVienContext();
            SetDefaultSearchText();
            GetLopFromDB();
            GetSinhVienFromDB();
        }
        private void TxtSearchOnGotFocus(object sender, EventArgs e)
        {
            txtSearch.Text = "";
        }
        private void TxtSearchOnLostFocus(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = _defaulSearchText;
                btnReset.PerformClick();
            }
        }
        private void SetDefaultSearchText()
        {
            txtSearch.Text = _defaulSearchText;
            txtSearch.GotFocus += TxtSearchOnGotFocus;
            txtSearch.LostFocus += TxtSearchOnLostFocus;
        }

        private void GetLopFromDB()
        {
            var listLop = context.Lops.ToList();
            cboLop.DisplayMember = "TenLop"; // Hiển thị lên ComboBox
            cboLop.ValueMember = "Id"; // Khi chọn thì sẽ lưu ID
            cboLop.DataSource = listLop;
        }
        private void GetSinhVienFromDB()
        {
            // Chú ý khai báo using System.Data.Entity;
            var listSinhVien = context.SinhViens.Include(sv => sv.Lop).ToList();

            LoadListView(listSinhVien);
        }
        private void AddSinhVienToListView(SinhVien sinhVien)
        {
            string[] row = { sinhVien.ID.ToString(), sinhVien.Ten, sinhVien.Lop.TenLop };
            var item = new ListViewItem(row);
            lvSinhVien.Items.Add(item);
        }

        private void LoadListView(List<SinhVien> listSinhVien)
        {
            lvSinhVien.Items.Clear();
            foreach (SinhVien sinhVien in listSinhVien)
            {
                AddSinhVienToListView(sinhVien);
            }
        }
        private SinhVien GetSinhVienFromControl()
        {
            if (String.IsNullOrWhiteSpace(txtHoTen.Text)) return null;

            var sinhVien = new SinhVien();
            sinhVien.ID = String.IsNullOrWhiteSpace(txtId.Text) ? -1 : int.Parse(txtId.Text);
            sinhVien.Ten = txtHoTen.Text;
            sinhVien.MaLop = Convert.ToInt32(cboLop.SelectedValue); // Lấy value thay vì items

            return sinhVien;
        }
        private void ThietLapThongTinControls(SinhVien sinhVien)
        {
            txtHoTen.Text = sinhVien.Ten;
            txtId.Text = sinhVien.ID.ToString();
            cboLop.SelectedValue = sinhVien.MaLop;
        }

        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            var count = lvSinhVien.SelectedItems.Count;
            if (count > 0)
            {
                var id = int.Parse(lvSinhVien.SelectedItems[0].Text);
                //var sv = context.SinhViens.FirstOrDefault(item => item.ID == id);
                // Cach 2: Dung ham Find : se tim theo ID tuong ung
                var sv = context.SinhViens.Find(id);
                ThietLapThongTinControls(sv);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            GetSinhVienFromDB();
            var listSV = context.SinhViens.ToList();
            LoadListView(listSV);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var sinhVien = GetSinhVienFromControl();

            if (sinhVien == null)
            {
                MessageBox.Show("Chưa nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            if (sinhVien.ID < 0)
            {
                // Add SinhVien to Database
                context.SinhViens.Add(sinhVien);
            }
            else
            {
                // Nếu đã tồn tại Id thì chỉ update dữ liệu lại lên DB
                var svUpdate = context.SinhViens.Find(sinhVien.ID);
                svUpdate.ID = sinhVien.ID;
                svUpdate.Ten = sinhVien.Ten;
            }
            context.SaveChanges();
            btnReset.PerformClick();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var searchText = txtSearch.Text;
            var list = context.SinhViens
                .Where(sv => sv.Ten.Contains(searchText))
                .ToList();

            LoadListView(list);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
