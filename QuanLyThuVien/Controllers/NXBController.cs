using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models; // Quan trọng: Thêm namespace của Models

namespace QuanLyThuVien.Controllers
{
    public class NXBController : Controller
    {
        // Khởi tạo đối tượng DbContext để làm việc với CSDL
        private QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: NXB
        public ActionResult Index()
        {
            // Lấy toàn bộ danh sách Nhà Xuất Bản từ CSDL
            // Thêm .Where() để chỉ lấy những NXB chưa bị xóa (nếu bạn có cột DaXoa)
            var danhSachNXB = db.NhaXuatBans.Where(nxb => nxb.DaXoa != true).ToList();

            // Gửi danh sách này đến View
            return View(danhSachNXB);
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