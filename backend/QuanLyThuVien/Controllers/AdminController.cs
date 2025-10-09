using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models; // Thêm namespace chứa DbContext và các model
using QuanLyThuVien.ViewModels; // Thêm namespace chứa ViewModel

namespace QuanLyThuVien.Controllers
{
    public class AdminController : Controller
    {
        // Khởi tạo đối tượng DbContext để truy vấn database
        // Thay "QuanLyThuVienDB" bằng tên DbContext 
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: Admin
        public ActionResult Index()
        {
            // Áp dụng "Object Initializer" để code gọn gàng và chuyên nghiệp hơn
            var viewModel = new AdminDashboardViewModel
            {
                // 1. Gán giá trị cho các thẻ thống kê
                TongSoSach = db.Saches.Count(),
                TongSoDocGia = db.DocGias.Count(),
                SachDangMuon = db.MuonTras.Count(mt => mt.TrangThai == "Đang mượn"),
                SachQuaHan = db.MuonTras.Count(mt => mt.TrangThai == "Đang mượn" && mt.NgayHenTra < DateTime.Now),

                // 2. Lấy danh sách hoạt động gần đây và gán trực tiếp
                // (Không cần biến trung gian "recentActivities" nữa)
                HoatDongGanDay = (from mt in db.MuonTras
                                      // BẮT BUỘC: Join qua bảng trung gian TheMuonSach
                                  join tms in db.TheMuonSaches on mt.MaTheMuon equals tms.MaTheMuon
                                  // Từ TheMuonSach mới join được với DocGia
                                  join dg in db.DocGias on tms.MaDocGia equals dg.MaDocGia
                                  join cfmt in db.ChiTietMuonTras on mt.MaMuonTra equals cfmt.MaMuonTra
                                  join s in db.Saches on cfmt.MaSach equals s.MaSach
                                  orderby mt.NgayMuon descending
                                  select new RecentActivityViewModel
                                  {
                                      MaSach = s.MaSach.ToString(),
                                      TenSach = s.TenSach,
                                      TenDocGia = dg.HoTen,
                                      NgayMuon = mt.NgayMuon,
                                      TrangThai = mt.TrangThai
                                  }).Take(5).ToList()
            };

            // 3. Truyền ViewModel đến View
            return View(viewModel);
        }   
        public ActionResult Reader()
        {
            return View();
        }
       
        public ActionResult SidebarPartiView()
        {
            return PartialView();
        }
        public ActionResult HeaderPartialView()
        {
            return PartialView();
        }
        // Đừng quên giải phóng DbContext
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