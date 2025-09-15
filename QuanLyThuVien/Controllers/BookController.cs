using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using System.Data.Entity;
using QuanLyThuVien.ViewModels;

namespace QuanLyThuVien.Controllers
{
    public class BookController : Controller
    {
        private readonly QuanLyThuVienEntities db = new QuanLyThuVienEntities();

        // GET: Book
        public ActionResult Index()
        {
            // 1. Tạo ViewModel
            var viewModel = new SachViewModel
            {

                // 2. Lấy danh sách sách để hiển thị (bao gồm cả Tác giả và NXB)
                DanhSachSach = db.Saches
                                      .Include(s => s.TacGia)
                                      .Include(s => s.NhaXuatBan)
                                      .Where(s => s.DaXoa == false) // Chỉ lấy sách chưa bị xóa
                                      .ToList(),

                // 3. Lấy dữ liệu cho các dropdown trong Modal và gán vào ViewModel
                DanhSachNXB = db.NhaXuatBans
                                     .Where(nxb => nxb.DaXoa == false)
                                     .Select(nxb => new SelectListItem
                                     {
                                         Value = nxb.MaNXB.ToString(),
                                         Text = nxb.TenNXB
                                     }).ToList(),

                DanhSachTheLoai = db.TheLoais
                                        .Where(tl => tl.DaXoa == false)
                                        .Select(tl => new SelectListItem
                                        {
                                            Value = tl.MaTheLoai.ToString(),
                                            Text = tl.TenTheLoai
                                        }).ToList()
            };

            // 4. Truyền ViewModel đến View
            return View(viewModel);
        }

        // POST: Book/Create
        [HttpPost]
        public ActionResult Create(Sach sach, int[] selectedTheLoaiIds)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (ModelState.IsValid)
                {
                    // Thêm sách mới vào DB context
                    db.Saches.Add(sach);
                    db.SaveChanges(); // Lưu để lấy được MaSach vừa tạo

                    // Xử lý quan hệ Nhiều-Nhiều với Thể Loại
                    if (selectedTheLoaiIds != null)
                    {
                        foreach (var maTheLoai in selectedTheLoaiIds)
                        {
                            var phanLoai = new PhanLoaiSach
                            {
                                MaSach = sach.MaSach,
                                MaTheLoai = maTheLoai
                            };
                            db.PhanLoaiSaches.Add(phanLoai);
                        }
                        db.SaveChanges(); // Lưu quan hệ
                    }

                    // Trả về kết quả thành công dưới dạng JSON cho AJAX
                    return Json(new { success = true, message = "Thêm sách mới thành công!" });
                }

                // Nếu dữ liệu không hợp lệ
                return Json(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại." });
            }
            catch (Exception ex)
            {
                // Bắt lỗi và trả về thông báo lỗi
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

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