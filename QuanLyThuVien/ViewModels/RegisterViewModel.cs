using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập (Email hoặc Mã SV)")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}