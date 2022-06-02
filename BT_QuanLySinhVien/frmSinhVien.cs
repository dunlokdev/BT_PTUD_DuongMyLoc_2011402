using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace BT_QuanLySinhVien_ADO
{
    public partial class frmSinhVien : Form
    {
        private string _connectionString;
        private const string _defaulSearchText = "Nhập tên sinh viên";
        private List<Lop> _listLop;
        private List<SinhVien> _listSinhVien;

        public frmSinhVien()
        {
            InitializeComponent();
        }

        #region SỰ KIỆN
        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            _listLop = new List<Lop>();
            _listSinhVien = new List<SinhVien>();
            _connectionString = ConfigurationManager.ConnectionStrings["BT_QuanLySinhVien"].ConnectionString;
            SetDefaultSearchText();
            GetLopFromDB();
            GetSinhVienFromDB();
            LoadListView(_listSinhVien);
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
        private void BtnDefault_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            txtHoTen.Text = "";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _listSinhVien = new List<SinhVien>();
            GetSinhVienFromDB();
            LoadListView(_listSinhVien);
        }
        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = lvSinhVien.SelectedItems.Count;
            if (count > 0)
            {
                var id = Convert.ToInt32(lvSinhVien.SelectedItems[0].Text);
                SinhVien sinhVien = _listSinhVien.FirstOrDefault(sv => sv.Id == id);
                ThietLapThongTinControls(sinhVien);
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var resultList = _listSinhVien.Where(sv => sv.HoTen.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) > 1).ToList();
                LoadListView(resultList);
            }
            else
            {
                LoadListView(_listSinhVien);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region CÁC PHƯƠNG THỨC XỬ LÝ
        private void SetDefaultSearchText()
        {
            txtSearch.Text = _defaulSearchText;
            txtSearch.GotFocus += TxtSearchOnGotFocus;
            txtSearch.LostFocus += TxtSearchOnLostFocus;
        }

        private void GetSinhVienFromDB()
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            //command.CommandText = "SELECT sv.ID, sv.Ten, l.TenLop FROM SinhVien sv, Lop l WHERE l.Id = sv.MaLop ";
            command.CommandText = "SELECT * FROM SinhVien";
            var adapter = new SqlDataAdapter(command);
            var table = new DataTable("SinhVien");

            connection.Open();
            adapter.Fill(table);
            connection.Close();

            foreach (DataRow row in table.Rows)
            {
                _listSinhVien.Add(new SinhVien(row));
            }
        }

        private void GetLopFromDB()
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Lop";

            var adapter = new SqlDataAdapter(command);
            var table = new DataTable("Lop");

            connection.Open();
            adapter.Fill(table);
            connection.Close();

            foreach (DataRow row in table.Rows)
            {
                _listLop.Add(new Lop(row));
            }

            cboLop.DisplayMember = "TenLop"; // Hiển thị lên ComboBox
            cboLop.ValueMember = "Id"; // Khi chọn thì sẽ lưu ID
            cboLop.DataSource = _listLop;

        }

        private SinhVien GetSinhVienFromControl()
        {
            if (String.IsNullOrWhiteSpace(txtHoTen.Text)) return null;

            var sinhVien = new SinhVien();
            sinhVien.Id = String.IsNullOrWhiteSpace(txtId.Text) ? -1 : int.Parse(txtId.Text);
            sinhVien.HoTen = txtHoTen.Text;
            sinhVien.MaLop = Convert.ToInt32(cboLop.SelectedValue); // Lấy value thay vì items

            return sinhVien;
        }

        private void AddSinhVienToListView(SinhVien sinhVien)
        {
            string[] row = { sinhVien.Id.ToString(), sinhVien.HoTen, sinhVien.MaLop.ToString() };
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var sinhVien = GetSinhVienFromControl();

            if (sinhVien == null)
            {
                MessageBox.Show("Chưa nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();

            if (sinhVien.Id < 0)
            {
                // Add SinhVien to Database
                command.CommandText = "EXEC usp_InsertSinhVien @Ten, @Lop";
            }
            else
            {
                // Nếu đã tồn tại Id thì chỉ update dữ liệu lại lên DB
                command.CommandText = "UPDATE SinhVien SET Ten = @Ten, MaLop = @Lop WHERE Id = @Id";
            }

            command.Parameters.AddWithValue("@Ten", sinhVien.HoTen);
            command.Parameters.AddWithValue("@Lop", sinhVien.MaLop);
            command.Parameters.AddWithValue("@Id", sinhVien.Id);

            connection.Open();
            var result = command.ExecuteNonQuery(); // return number 

            if (result > 0) btnReset.PerformClick();

            connection.Close();

        }

        //private SinhVien GetSinhVienListView(ListViewItem lvitem)
        //{
        //    SinhVien sinhVien = new SinhVien();
        //    sinhVien.Id = Convert.ToInt32(lvitem.SubItems[0].Text);
        //    sinhVien.HoTen = lvitem.SubItems[1].Text;
        //    sinhVien.MaLop = Convert.ToInt32(lvitem.SubItems[2].Text);

        //    return sinhVien;
        //}

        private void ThietLapThongTinControls(SinhVien sinhVien)
        {
            txtHoTen.Text = sinhVien.HoTen;
            txtId.Text = sinhVien.Id.ToString();
            cboLop.SelectedValue = sinhVien.MaLop;
        }
        #endregion


    }
}
