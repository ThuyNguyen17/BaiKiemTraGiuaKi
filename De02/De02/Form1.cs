using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using De02.Model;

namespace De02
{
    public partial class frmSanPham : Form
    {
        public frmSanPham()
        {
            InitializeComponent();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                btLuu.Enabled = false;
                btKhongLuu.Enabled = false;
                ProductContextDB context = new ProductContextDB();
                List<Sanpham> listSanPham = context.Sanpham.ToList();
                List<LoaiSP> listLoaiSP = context.LoaiSP.ToList();
                FillFalcultyCombobox(listLoaiSP);
                BindGrid(listSanPham);             
            
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<LoaiSP> listLoaiSP)
        {
            this.cmbLoaiSP.DataSource = listLoaiSP;
            this.cmbLoaiSP.DisplayMember = "TenLoai";
            this.cmbLoaiSP.ValueMember = "MaLoai";
        }
 
        private void BindGrid(List<Sanpham> listSanPham)
        {
            dgvSanPham.Rows.Clear();
            foreach (var item in listSanPham)
            {
                int index = dgvSanPham.Rows.Add();
                dgvSanPham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanPham.Rows[index].Cells[1].Value = item.TenSP;
                dgvSanPham.Rows[index].Cells[2].Value = item.Ngaynhap;
                dgvSanPham.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
            }
        }
        


        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text) || string.IsNullOrEmpty(txtTenSP.Text))
                {
                    MessageBox.Show("Mã sản phẩm và tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.Cells[0].Value.ToString() == txtMaSP.Text)
                    {
                        row.Cells[1].Value = txtTenSP.Text;
                        row.Cells[2].Value = dateTimePicker1.Value;
                        row.Cells[3].Value = cmbLoaiSP.SelectedValue.ToString();
                        break;
                    }
                }

                btLuu.Enabled = true;
                btKhongLuu.Enabled = true;

                txtMaSP.Enabled = false;
                txtTenSP.Enabled = false;
                dateTimePicker1.Enabled = false;
                cmbLoaiSP.Enabled = false;

                MessageBox.Show("Sản phẩm đã được cập nhật trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSanPham.SelectedRows.Count > 0)
                {
                    var selectedRow = dgvSanPham.SelectedRows[0];
                    var maSP = selectedRow.Cells[0].Value.ToString();

                    var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?",
                                                        "Xác nhận xóa",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        dgvSanPham.Rows.Remove(selectedRow);

                      
                        deletedProductIDs.Add(maSP); 
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSanPham.Rows[e.RowIndex];
                txtMaSP.Text = selectedRow.Cells[0].Value.ToString();
                txtTenSP.Text = selectedRow.Cells[1].Value.ToString();
                if (DateTime.TryParse(selectedRow.Cells[2].Value?.ToString(), out DateTime ngayNhap))
                {
                    dateTimePicker1.Value = ngayNhap;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Now;
                }
                cmbLoaiSP.Text = selectedRow.Cells[3].Value.ToString();
                txtMaSP.Enabled = true;
                txtTenSP.Enabled = true;
                dateTimePicker1.Enabled = true;
                cmbLoaiSP.Enabled = true;

                btLuu.Enabled = true;
                btKhongLuu.Enabled = true;
                
            }   
        }

        private void btThoat_Click(object sender, EventArgs e)
        {           
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }


        private List<string> deletedProductIDs = new List<string>();  
        private void btLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text) || string.IsNullOrEmpty(txtTenSP.Text) || cmbLoaiSP.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (ProductContextDB db = new ProductContextDB())
                {
                    var existingProduct = db.Sanpham.FirstOrDefault(s => s.MaSP == txtMaSP.Text);

                    if (existingProduct != null)
                    {
                        existingProduct.TenSP = txtTenSP.Text;
                        existingProduct.Ngaynhap = dateTimePicker1.Value;
                        existingProduct.MaLoai = cmbLoaiSP.SelectedValue.ToString();
                    }
                    else
                    {
                        var newSanPham = new Sanpham()
                        {
                            MaSP = txtMaSP.Text,
                            TenSP = txtTenSP.Text,
                            Ngaynhap = dateTimePicker1.Value,
                            MaLoai = cmbLoaiSP.SelectedValue.ToString()
                        };
                        db.Sanpham.Add(newSanPham);
                    }
                    foreach (var maSP in deletedProductIDs)
                    {
                        var productToDelete = db.Sanpham.FirstOrDefault(s => s.MaSP == maSP);
                        if (productToDelete != null)
                        {
                            db.Sanpham.Remove(productToDelete);
                        }
                    }

                    db.SaveChanges();

                    btLuu.Enabled = false;
                    btKhongLuu.Enabled = false;
                    txtMaSP.Enabled = false;
                    txtTenSP.Enabled = false;
                    dateTimePicker1.Enabled = false;
                    cmbLoaiSP.Enabled = false;

                    deletedProductIDs.Clear();

                    MessageBox.Show("Cập nhật cơ sở dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu sản phẩm: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btKhongLuu_Click(object sender, EventArgs e)
        {
            
            txtMaSP.Clear();
            txtTenSP.Clear();
            cmbLoaiSP.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;

            btLuu.Enabled = false;
            btKhongLuu.Enabled = false;

            MessageBox.Show("Hủy bỏ thay đổi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btThem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMaSP.Text))
                {
                    MessageBox.Show("Mã sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ProductContextDB db = new ProductContextDB();
                List<Sanpham> studentList = db.Sanpham.ToList();
                if (studentList.Any(s => s.MaSP == txtMaSP.Text))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập một mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var newSanPham = new Sanpham()
                {
                    MaSP = txtMaSP.Text,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dateTimePicker1.Value,
                    MaLoai = cmbLoaiSP.SelectedValue.ToString()
                };
                dgvSanPham.Rows.Add(newSanPham.MaSP, newSanPham.TenSP, newSanPham.Ngaynhap, newSanPham.MaLoai);

                btLuu.Enabled = true;
                btKhongLuu.Enabled = true;

                txtMaSP.Enabled = false;
                txtTenSP.Enabled = false;
                dateTimePicker1.Enabled = false;
                cmbLoaiSP.Enabled = false;

                MessageBox.Show("Sản phẩm đã được thêm vào danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btTimTheoTen_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (ProductContextDB db = new ProductContextDB())
                {
                    string searchKeyword = txtTim.Text.Trim();
                    var result = db.Sanpham.Where(sp => sp.TenSP.Contains(searchKeyword)).ToList();
                    if (result.Any())
                    {
                        BindGrid(result);
                        MessageBox.Show($"Tìm thấy {result.Count} sản phẩm phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
