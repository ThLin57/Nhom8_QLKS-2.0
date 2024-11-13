using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;

namespace QuanLyPhongKhachSan
{
    public partial class QuanLyPhong : Form
    {
        public string TenKH { get; set; }
        public string SoP { get; set; }
        string connetString = @"Data Source=LAPTOP-NBIQUV5E;Initial Catalog=QuanLyKhachSan1.0;Integrated Security=True;";
        //  private FlowLayoutPanel flowLayoutPanel1;
        public QuanLyPhong(string tenKH, string soP)
        {
            this.TenKH = tenKH;
            this.SoP = soP;
        }
        int id_HD = 3;


        public QuanLyPhong()
        {
            InitializeComponent();
        }

        private void button1_Load(object sender, EventArgs e)
        {
            // MessageBox.Show("000");
        }

        private void button1_Load_1(object sender, EventArgs e)
        {
            string mt;
            mt = "Thông tin mới cho ô này";
            MoTaPhong.SetToolTip((Control)sender, mt);
        }
  
        private ToolTip MoTaPhong = new ToolTip();
        private void QuanLyPhong_Load(object sender, EventArgs e)
        {
            radioButton11.Checked = true;
            radioButton6.Checked = true;
            ConfigureDataGridView();
            LoadRoomList();
          //  LoadHoaDonDetails(id_HD);
        //    LoadRoomsForInvoice(id_HD);
          
        }
        private void LoadRoomList()
        {
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();

                // Lấy danh sách tầng
                string getFloorsQuery = "SELECT ID_Tang, Tang FROM Tang WHERE KhaDung = 1";
                SqlCommand getFloorsCmd = new SqlCommand(getFloorsQuery, conn);
                SqlDataAdapter floorAdapter = new SqlDataAdapter(getFloorsCmd);
                DataTable floorTable = new DataTable();
                floorAdapter.Fill(floorTable);

                // Duyệt qua từng tầng và tạo GroupBox
                foreach (DataRow floorRow in floorTable.Rows)
                {
                    int floorId = Convert.ToInt32(floorRow["ID_Tang"]);
                    string floorName = floorRow["Tang"].ToString();

                    // Tạo GroupBox cho mỗi tầng
                    GroupBox groupBox = new GroupBox();
                    groupBox.Text = floorName;
                    groupBox.Padding = new Padding(10);
                    groupBox.Margin = new Padding(10);

                    // Create a FlowLayoutPanel for the rooms
                    FlowLayoutPanel roomPanel = new FlowLayoutPanel();
                    roomPanel.Dock = DockStyle.Fill;
                    roomPanel.AutoScroll = true;

                    // Lấy danh sách phòng cho từng tầng
                    string getRoomsQuery = @"SELECT p.ID_Phong, p.SoPhong,hp.Gia ,hp.HangPhong, p.TrangThai 
                                      FROM Phong p
                                      INNER JOIN HangPhong hp ON p.ID_HP = hp.ID_HP
                                      WHERE p.ID_Tang = @floorId AND p.KhaDung = 1";
                    SqlCommand getRoomsCmd = new SqlCommand(getRoomsQuery, conn);
                    getRoomsCmd.Parameters.AddWithValue("@floorId", floorId);
                    SqlDataAdapter roomAdapter = new SqlDataAdapter(getRoomsCmd);
                    DataTable roomTable = new DataTable();
                    roomAdapter.Fill(roomTable);

                    int maxRoomsPerRow = 6;
                    int totalRooms = roomTable.Rows.Count;
                    int totalRows = (int)Math.Ceiling((double)totalRooms / maxRoomsPerRow);

                    foreach (DataRow roomRow in roomTable.Rows)
                    {
                        string roomNumber = roomRow["SoPhong"].ToString();
                        string roomCategory = roomRow["HangPhong"].ToString();
                        int roomStatus = Convert.ToInt32(roomRow["TrangThai"]);
                        int roomId = Convert.ToInt32(roomRow["ID_Phong"]);
                        int roomPrice = Convert.ToInt32(roomRow["Gia"]);

                        Button roomButton = new Button();
                        roomButton.Text = $"Phòng {roomNumber}\n\n{roomCategory}";
                        roomButton.Size = new Size(110, 70);
                        switch (roomStatus)
                        {
                            case 1:
                                roomButton.BackColor = Color.LightGreen; // Phòng khả dụng
                                break;
                            case 0:
                                roomButton.BackColor = Color.Red; // Phòng không khả dụng
                                break;
                            case 2:
                                roomButton.BackColor = Color.Gray; // Phòng trạng thái 2
                                break;
                            case 3:
                                roomButton.BackColor = Color.Orange; // Phòng trạng thái 3
                                break;
                            default:
                                roomButton.BackColor = Color.LightGray; // Mặc định
                                break;
                        }

                        roomButton.Margin = new Padding(5);
                        roomButton.TextAlign = ContentAlignment.MiddleLeft;
                          roomPanel.Controls.Add(roomButton);
                        // Thêm hình ảnh vào nút
                        try
                        {
                            roomButton.Image = Image.FromFile("C:\\Users\\LUAN\\Pictures\\BT\\twopeople.png");
                            roomButton.ImageAlign = ContentAlignment.MiddleRight;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Không thể tải hình ảnh: {ex.Message}");
                        }

                        // Sự kiện double click để thêm thông tin vào DataGridView
                        DateTime lastClickTime = DateTime.MinValue; // biến này để lưu thời gian của lần click trước
                        roomButton.Click += (s, e) =>
                        {
                            // Tính toán khoảng thời gian giữa lần click hiện tại và lần click trước
                            TimeSpan interval = DateTime.Now - lastClickTime;
                            if (interval.TotalMilliseconds <= SystemInformation.DoubleClickTime)
                            {
                                // Nếu khoảng cách giữa hai lần click nhỏ hơn ngưỡng DoubleClickTime thì xử lý như là Double-Click
                                MessageBox.Show($"Đã them phòng {roomNumber}");
                                dataGridView1.Rows.Add(roomId, roomNumber,roomPrice);
                            }
                            // Cập nhật lại thời gian click gần nhất
                            lastClickTime = DateTime.Now;
                        };
                    }
                    // Thiết lập kích thước cho groupBox dựa trên tổng số dòng
                    groupBox.Height = totalRows * 90 + 70;
                    groupBox.Width = 850;

                    // Thêm roomPanel vào groupBox và groupBox vào flowLayoutPanel1
                    groupBox.Controls.Add(roomPanel);
                    flowLayoutPanel1.Controls.Add(groupBox);
                }

                conn.Close();
            }
        }
        private void ConfigureDataGridView()
        {
            dataGridView1.Columns.Add("ID_Phong", "ID Phòng");
            dataGridView1.Columns.Add("SoPhong", "Số Phòng");
            dataGridView1.Columns.Add("Gia", "Gia");
        }

        private void chonPhongToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // Khai báo danh sách chứa ID của các phòng đã chọn
        private List<int> selectedRoomIds = new List<int>();

        private void button2_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu DataGridView có ít nhất một phòng
            if (dataGridView1.Rows.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(connetString))
                {
                    conn.Open();

                    // Duyệt qua tất cả các hàng trong DataGridView để lấy ID_Phong
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        int roomId = Convert.ToInt32(row.Cells["ID_Phong"].Value);

                        // Cập nhật trạng thái của phòng trong cơ sở dữ liệu
                        string updateRoomStatusQuery = "UPDATE Phong SET TrangThai = 0 WHERE ID_Phong = @roomId";
                        SqlCommand updateCmd = new SqlCommand(updateRoomStatusQuery, conn);
                        updateCmd.Parameters.AddWithValue("@roomId", roomId);

                        try
                        {
                            updateCmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi cập nhật trạng thái phòng ID {roomId}: {ex.Message}");
                        }
                    }

                    // Hiển thị thông báo sau khi cập nhật tất cả các phòng
                    MessageBox.Show("Đã cập nhật trạng thái phòng.");

                    // Tạo hóa đơn mới
                    string insertHoaDonQuery = @"INSERT INTO HoaDon (ID_NV, ID_KH, NgayLap, NgayToi, NgayDi, DatCoc, HinhThucThanhToan, PhuThu, TrangThai, TongTien, GhiChu, KhaDung)
                                         VALUES (@ID_NV, @ID_KH, @NgayLap, @NgayToi, @NgayDi, @DatCoc, @HinhThucThanhToan, @PhuThu, @TrangThai, @TongTien, @GhiChu, @KhaDung);
                                         SELECT SCOPE_IDENTITY();"; // Sử dụng SCOPE_IDENTITY để lấy ID_HD vừa tạo

                    SqlCommand cmd = new SqlCommand(insertHoaDonQuery, conn);
                    cmd.Parameters.AddWithValue("@ID_NV" , 3);
                    cmd.Parameters.AddWithValue("@ID_KH", 1); // Giả sử lấy ID_KH từ TextBox hoặc combobox khách hàng
                    cmd.Parameters.AddWithValue("@NgayLap", DateTime.Now);
                    cmd.Parameters.AddWithValue("@NgayToi", DateTime.Now.AddDays(1)); // Ngày trả phòng
                    cmd.Parameters.AddWithValue("@NgayDi", DateTime.Now.AddDays(2)); // Ngày trả phòng
                    cmd.Parameters.AddWithValue("@DatCoc", 500000); // Số tiền đặt cọc
                    cmd.Parameters.AddWithValue("@HinhThucThanhToan", 1); // Giả sử hình thức thanh toán 1
                    cmd.Parameters.AddWithValue("@PhuThu", 0); // Phụ thu
                    cmd.Parameters.AddWithValue("@TrangThai", 1); // Trạng thái hóa đơn
                    cmd.Parameters.AddWithValue("@TongTien", 1000000); // Tổng tiền (Tính tổng theo phòng)
                    cmd.Parameters.AddWithValue("@GhiChu", "Ghi chú hóa đơn");
                    cmd.Parameters.AddWithValue("@KhaDung", 1); // Khả dụng

                    int idHoaDon = Convert.ToInt32(cmd.ExecuteScalar()); // Lấy ID của hóa đơn mới

                    // Lưu danh sách phòng vào CT_HD
                    foreach (int roomId in selectedRoomIds)
                    {
                        string insertCTHDQuery = @"INSERT INTO CT_HD (ID_HD, ID_Phong, Gia, SoNgay, KhaDung)
                                            VALUES (@ID_HD, @ID_Phong, @Gia, @SoNgay, @KhaDung)";
                        SqlCommand cmdCTHD = new SqlCommand(insertCTHDQuery, conn);
                        cmdCTHD.Parameters.AddWithValue("@ID_HD", idHoaDon);
                        cmdCTHD.Parameters.AddWithValue("@ID_Phong", roomId);
                        cmdCTHD.Parameters.AddWithValue("@Gia", 500000); // Giá phòng
                        cmdCTHD.Parameters.AddWithValue("@SoNgay", 1); // Số ngày lưu trú
                        cmdCTHD.Parameters.AddWithValue("@KhaDung", 1); // Khả dụng

                        cmdCTHD.ExecuteNonQuery();
                    }

                    conn.Close();
                    MessageBox.Show("Hóa đơn đã được tạo thành công!");
                }

                // Sau khi cập nhật, tải lại danh sách phòng để hiển thị trạng thái mới
                flowLayoutPanel1.Controls.Clear(); // Xóa các phòng hiện tại
                LoadRoomList(); // Tải lại danh sách phòng
            }
            else
            {
                MessageBox.Show("Không có phòng nào trong danh sách để chuyển trạng thái.");
            }
        }
        public void LoadHoaDonDetails(int idHoaDon)
        {
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                string query = @"SELECT k.TenKH, k.SDT, k.SoGiayTo 
                             FROM HoaDon h
                             JOIN KhachHang k ON h.ID_KH = k.ID_KH
                             WHERE h.ID_HD = @idHoaDon";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idHoaDon", idHoaDon);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {   
                    // Gán thông tin khách hàng vào các TextBox
                    txtTenKhach.Text = reader["TenKH"].ToString();
                    txtSoDienThoai.Text = reader["SDT"].ToString();
                    textBox11.Text = reader["SoGiayTo"].ToString();
                }
                conn.Close();
            }
        }
        public void LoadRoomsForInvoice(int idHoaDon)
        {
            using (SqlConnection conn = new SqlConnection(connetString))
            {
                conn.Open();
                string query = @"SELECT p.SoPhong, hp.HangPhong, ct.Gia, ct.SoNgay
                             FROM CT_HD ct
                             JOIN Phong p ON ct.ID_Phong = p.ID_Phong
                             JOIN HangPhong hp ON p.ID_HP = hp.ID_HP
                             WHERE ct.ID_HD = @idHoaDon";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idHoaDon", idHoaDon);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Bind data to DataGridView
                dataGridView1.DataSource = dt;
                conn.Close();
            }
        }

        private void đặtPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatPhong datPhong = new DatPhong();
            datPhong.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

