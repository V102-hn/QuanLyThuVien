using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class ReadersController : Controller
    {
        // Khởi tạo DbContext để làm việc với CSDL
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();
        // GET: Readers
        public ActionResult Index()
        {
            // Lấy toàn bộ danh sách độc giả từ bảng DocGia
            // Chúng ta cần hiển thị cả độc giả 'Hoạt động' và 'Bị khóa' nên không cần lọc theo DaXoa
            var danhSachDocGia = db.DocGias.ToList();

            // Gửi danh sách này đến View
            return View(danhSachDocGia);
        }
    }
}