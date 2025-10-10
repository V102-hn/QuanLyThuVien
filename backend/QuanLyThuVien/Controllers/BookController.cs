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
        public ActionResult Create([Bind(Exclude = "MaSach")] Sach sach, int[] selectedTheLoaiIds)
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

        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // 1. Tìm sách trong database
                var sach = db.Saches.Find(id);


                // 2. Kiểm tra xem sách có tồn tại không
                if (sach == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sách để xóa." });
                }


                // 3. Thực hiện "xóa mềm" - Cập nhật trạng thái DaXoa
                sach.DaXoa = true;


                // 4. Lưu thay đổi vào database
                db.SaveChanges();


                // 5. Trả về kết quả thành công cho AJAX
                return Json(new { success = true, message = "Xóa sách thành công!" });
            }
            catch (Exception ex)
            {
                // Bắt lỗi và trả về thông báo lỗi
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa: " + ex.Message });
            }
        }

        // GET: Book/GetBookDetails/5
        // Action này dùng để lấy dữ liệu chi tiết của 1 cuốn sách và các thể loại của nó
        // Dữ liệu sẽ được trả về dưới dạng JSON cho AJAX
        public ActionResult GetBookDetails(int id)
        {
            try
            {
                // Tắt Lazy Loading để tránh lỗi tham chiếu vòng lặp khi serialize JSON
                db.Configuration.ProxyCreationEnabled = false;

                // 1. Tìm sách theo ID
                var sach = db.Saches.AsNoTracking().FirstOrDefault(s => s.MaSach == id);


                if (sach == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sách." }, JsonRequestBehavior.AllowGet);
                }


                // 2. Lấy danh sách các MaTheLoai mà sách này đang thuộc về
                var selectedTheLoaiIds = db.PhanLoaiSaches
                                            .Where(p => p.MaSach == id)
                                            .Select(p => p.MaTheLoai)
                                            .ToList();


                // 3. Trả về một đối tượng JSON chứa cả thông tin sách và danh sách ID thể loại
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        sach = sach,
                        selectedTheLoaiIds = selectedTheLoaiIds
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lấy dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: Book/Edit
        [HttpPost]
        public ActionResult Edit(Sach sach, int[] selectedTheLoaiIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Lấy đối tượng sách hiện tại từ DB
                    var existingSach = db.Saches.Find(sach.MaSach);
                    if (existingSach == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy sách để cập nhật." });
                    }


                    // 2. Cập nhật các thuộc tính của sách
                    existingSach.TenSach = sach.TenSach;
                    existingSach.MaTacGia = sach.MaTacGia;
                    existingSach.MaNXB = sach.MaNXB;
                    existingSach.SoLuongTon = sach.SoLuongTon;
                    existingSach.MoTa = sach.MoTa;
                    // Thêm các trường khác nếu có...
                    // Ví dụ: existingSach.NgayCapNhat = DateTime.Now;


                    // 3. Xử lý cập nhật quan hệ Nhiều-Nhiều với Thể Loại
                    // Cách đơn giản nhất: Xóa hết các liên kết cũ và tạo lại các liên kết mới
                    var oldPhanLoai = db.PhanLoaiSaches.Where(p => p.MaSach == sach.MaSach);
                    db.PhanLoaiSaches.RemoveRange(oldPhanLoai);


                    if (selectedTheLoaiIds != null)
                    {
                        foreach (var maTheLoai in selectedTheLoaiIds)
                        {
                            var newPhanLoai = new PhanLoaiSach
                            {
                                MaSach = sach.MaSach,
                                MaTheLoai = maTheLoai
                            };
                            db.PhanLoaiSaches.Add(newPhanLoai);
                        }
                    }


                    // 4. Lưu tất cả thay đổi vào database
                    db.SaveChanges();


                    return Json(new { success = true, message = "Cập nhật thông tin sách thành công!" });
                }


                return Json(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật: " + ex.Message });
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