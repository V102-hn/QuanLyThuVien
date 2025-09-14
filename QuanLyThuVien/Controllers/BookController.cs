using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QuanLyThuVien.Models; // Quan trọng: Thêm namespace của Models
using System.Data.Entity; // Quan trọng: Thêm namespace cho Include()

namespace QuanLyThuVien.Controllers
{
    public class BookController : Controller
    {
        // Khởi tạo đối tượng DbContext để làm việc với CSDL
        private QuanLyThuVienEntities db = new QuanLyThuVienEntities(); // Tên này có thể khác tùy theo cách bạn đặt tên model

        // GET: Book
        public ActionResult Index()
        {
            // Sử dụng .Include() để tải kèm dữ liệu từ bảng TacGia và NhaXuatBan
            var danhSachSach = db.Saches
                                 .Include(s => s.TacGia)
                                 .Include(s => s.NhaXuatBan)
                                 .ToList();

            // Truyền danh sách đã có ĐẦY ĐỦ dữ liệu này đến View
            return View(danhSachSach);
        }
        // Đừng quên giải phóng tài nguyên khi Controller bị hủy
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
