# Dự án Website Quản lý Thư viện (QuanLyThuVien)

Đây là một dự án ứng dụng web hoàn chỉnh với mục đích số hóa và quản lý các hoạt động trong một thư viện, từ quản lý sách, quản lý độc giả cho đến theo dõi quá trình mượn-trả sách.

Dự án được xây dựng với kiến trúc phân tách rõ ràng giữa Backend (API) và Frontend (Giao diện người dùng), thể hiện khả năng làm việc với các công nghệ web hiện đại.

## 🚀 Các chức năng chính

### 👨‍💻 Phía người dùng (Độc giả)
- [x] Đăng ký / Đăng nhập tài khoản.
- [x] Tìm kiếm sách theo tên, tác giả, thể loại.
- [x] Xem lịch sử mượn-trả sách của cá nhân.

### 🔐 Phía quản trị viên (Thủ thư)
- [x] Đăng nhập vào trang quản trị.
- **Quản lý Sách:**
  - [x] Thêm / Sửa / Xóa thông tin sách.
  - [x] Cập nhật số lượng sách trong kho.
- **Quản lý Độc giả:**
  - [x] Thêm / Sửa / Xóa thông tin độc giả.
- **Quản lý Mượn-Trả:**
  - [x] Tạo phiếu mượn sách.
  - [x] Ghi nhận trả sách và tính phí phạt (nếu có).
  - [x] Xem danh sách các sách đang được mượn và sách quá hạn.


## 🛠️ Công nghệ sử dụng

- **Backend:**
  - **Ngôn ngữ:** C#
  - **Framework:** ASP.NET MVC
  - **Cơ sở dữ liệu:** SQL Server (hoặc MySQL, PostgreSQL...)
- **Frontend:**
  - **Ngôn ngữ:** HTML, CSS, JavaScript
- **Công cụ khác:**
  - **Quản lý phiên bản:** Git & GitHub
  - **CI/CD:** GitHub Actions

## 📸 Ảnh chụp màn hình


**Giao diện người dùng:**
<img width="818" height="435" alt="image" src="https://github.com/user-attachments/assets/af26d5a2-975d-44f1-97b2-ed0c4624d292" />


**Giao diện quản trị:**
<img width="811" height="431" alt="image" src="https://github.com/user-attachments/assets/ca16dbcd-39cb-47d6-9746-1252d7a368b9" />


## Hướng dẫn cài đặt và chạy dự án

1.  **Clone repository:**
    ```bash
    git clone https://github.com/nguyen-huu-van/QuanLyThuVien.git
    ```
2.  **Cấu hình Backend:**
    - Mở project backend bằng Visual Studio.
    - Cập nhật chuỗi kết nối (connection string) trong fileWeb.config.
    - Chạy file `database_qlthuvien.sql` để tạo cơ sở dữ liệu.
    - Build và Run project.
3.  **Cấu hình Frontend:**
    - Mở terminal trong thư mục frontend.
    - Chạy lệnh `npm install` để cài đặt các thư viện.
    - Chạy lệnh `npm start` để khởi động.
