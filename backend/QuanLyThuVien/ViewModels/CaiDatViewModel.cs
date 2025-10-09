using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.ViewModels
{
    public class CaiDatViewModel
    {
        [Display(Name = "Số ngày mượn tối đa")]
        public int SoNgayMuonToiDa { get; set; }

        [Display(Name = "Số sách mượn tối đa")]
        public int SoSachMuonToiDa { get; set; }

        [Display(Name = "Tiền phạt mỗi ngày")]
        public int TienPhatMoiNgay { get; set; }

        [Display(Name = "Tên thư viện")]
        public string TenThuVien { get; set; }

        [Display(Name = "Địa chỉ thư viện")]
        public string DiaChiThuVien { get; set; }
    }
}