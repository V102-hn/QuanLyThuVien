using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models; // <-- THÊM DÒNG NÀY để sử dụng các lớp trong thư mục Models

namespace QuanLyThuVien.Controllers
{
    public class TacGiaController : Controller
    {
        // Khởi tạo đối tượng context để kết nối đến DB
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: TacGia
        public ActionResult Index()
        {
            // Lấy danh sách tất cả tác giả từ DB
            // Điều kiện Where(tg => tg.DaXoa == false || tg.DaXoa == null) dùng để chỉ lấy những tác giả chưa bị "xóa mềm"
            var danhSachTacGia = db.TacGias.Where(tg => tg.DaXoa == false || tg.DaXoa == true).ToList();

            // Trả về View cùng với danh sách tác giả đã lấy được
            return View(danhSachTacGia);
        }

        // (Các action khác như Create, Edit, Delete sẽ được thêm vào sau)
    }
}