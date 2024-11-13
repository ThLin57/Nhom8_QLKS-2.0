using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuanLyPhongKhachSan
{
    public partial class DatPhong : Form
    {
        private string connetString = @"Data Source=LAPTOP-NBIQUV5E;Initial Catalog=QuanLyKhachSan1.0;Integrated Security=True;";
        public DatPhong()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void AddNewCustomer()
        {
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                string tenKH = txtTenKh.Text;
                bool gioiTinh = rdNam.Checked; // Nếu radioButtonNam được chọn thì là nam (true), nếu không thì là nữ (false)
                DateTime ngaySinh = dtpNgaySinh.Value;
                string sdt = txtSDT.Text;
                string email = txtEmail.Text;
                string soGiayTo = txtGiayTo.Text;
                string ghiChu = null;
                int khaDung = 1;
                // Câu lệnh SQL để thêm khách hàng mới
                string insertCustomerQuery = @"INSERT INTO KhachHang 
                                               (TenKH, GioiTinh, NgaySinh, SDT, Email, SoGiayTo, GhiChu, KhaDung) 
                                               VALUES 
                                               (@TenKH, @GioiTinh, @NgaySinh, @SDT, @Email, @SoGiayTo, @GhiChu, @KhaDung)";

                using (SqlCommand cmd = new SqlCommand(insertCustomerQuery, conn))
                {
                    // Thêm tham số cho câu lệnh SQL
                    cmd.Parameters.AddWithValue("@TenKH", tenKH);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@SDT", sdt);
                    cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value); // Email có thể là NULL
                    cmd.Parameters.AddWithValue("@SoGiayTo", soGiayTo);
                    cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? (object)DBNull.Value); // Ghi chú có thể là NULL
                    cmd.Parameters.AddWithValue("@KhaDung", khaDung);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Khách hàng mới đã được thêm thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddNewCustomer();
        }

        private void DatPhong_Load(object sender, EventArgs e)
        {
            dtpNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            LoadCustomerList();
            LoadAvailableRooms();
        }
        private void LoadCustomerList()
        {
            // Clear any previous items in the ListView
            listView1.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                string selectQuery = "SELECT ID_KH, TenKH, GioiTinh, NgaySinh, SDT, Email, SoGiayTo, GhiChu, KhaDung FROM KhachHang";

                using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                {
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            // Create a new ListViewItem for each row
                            ListViewItem item = new ListViewItem(reader["ID_KH"].ToString());
                            item.SubItems.Add(reader["TenKH"].ToString());
                            item.SubItems.Add((bool)reader["GioiTinh"] ? "Nam" : "Nữ"); // Convert boolean to text
                            item.SubItems.Add(Convert.ToDateTime(reader["NgaySinh"]).ToString("dd/MM/yyyy"));
                            item.SubItems.Add(reader["SDT"].ToString());
                            item.SubItems.Add(reader["Email"].ToString());
                            item.SubItems.Add(reader["SoGiayTo"].ToString());
                            item.SubItems.Add(reader["GhiChu"].ToString());
                            item.SubItems.Add(reader["KhaDung"].ToString());

                            // Add the item to the ListView
                            listView1.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải danh sách khách hàng: " + ex.Message);
                    }
                }
            }
        }

        private void txtTenKh_TextChanged(object sender, EventArgs e)
        {
        }

        private void dgvDSKH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Get the selected ListViewItem
                ListViewItem item = listView1.SelectedItems[0];

                // Populate the controls with the corresponding values
                txtTenKh.Text = item.SubItems[1].Text; // TenKH
                txtSDT.Text = item.SubItems[4].Text; // SDT
                txtEmail.Text = item.SubItems[5].Text; // Email
                txtGiayTo.Text = item.SubItems[6].Text; // SoGiayTo

                // Set the gender RadioButton
                if (item.SubItems[2].Text == "Nam")
                {
                    rdNam.Checked = true;
                    rdNu.Checked = false;
                }
                else
                {
                    rdNam.Checked = false;
                    rdNu.Checked = true;
                }
               // dtpNgaySinh.Value = DateTime.Parse(item.SubItems[3].Text);
            }
        }
        private void LoadAvailableRooms()
        {
            // Clear any previous items in the ListView
            listView2.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                string query = "SELECT Phong.ID_Phong, Phong.SoPhong,HangPhong.HangPhong FROM Phong JOIN HangPhong ON Phong.ID_HP = HangPhong.ID_HP JOIN Tang ON Phong.ID_Tang = Tang.ID_Tang  WHERE   Phong.TrangThai = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            // Create a new ListViewItem for each row
                            ListViewItem item = new ListViewItem(reader["ID_Phong"].ToString());
                            item.SubItems.Add(reader["SoPhong"].ToString());
                            item.SubItems.Add(reader["HangPhong"].ToString());

                            // Add the item to the ListView
                            listView2.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải danh sách khách hàng: " + ex.Message);
                    }
                }
            }
        }


    }
}
