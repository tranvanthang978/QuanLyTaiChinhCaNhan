using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using dacn.Models; 

namespace dacn.Controllers
{
    public class SharedController : Controller
    {
        private ChiTiet db = new ChiTiet();

        public PartialViewResult Notifications()
        {
            // ⚠️ Kiểm tra người dùng đã đăng nhập chưa
            if (Session["MaNguoiDung"] == null)
            {
                return PartialView("_Notifications", new List<string>());
            }

            int maNguoiDung = (int)Session["MaNguoiDung"];
            int thang = DateTime.Now.Month;
            int nam = DateTime.Now.Year;

            // Lấy danh sách ngân sách tháng hiện tại
            var listNganSach = db.NganSach
                .Include(n => n.DanhMuc)
                .Where(n => n.MaNguoiDung == maNguoiDung && n.Thang == thang && n.Nam == nam)
                .ToList();

            var notifications = new List<string>();

            foreach (var n in listNganSach)
            {
                var DaChi = db.GiaoDich
                    .Where(gd => gd.MaDanhMuc == n.MaDanhMuc
                              && gd.MaNguoiDung == maNguoiDung
                              && gd.NgayGD.Month == thang
                              && gd.NgayGD.Year == nam)
                    .Sum(gd => (decimal?)gd.SoTien) ?? 0;

                if (n.SoTienGioiHan - DaChi < 0)
                {
                    notifications.Add($"Ngân sách \"{n.DanhMuc.TenDanhMuc}\" đã vượt quá hạn mức!");
                }
            }

            return PartialView("_Notifications", notifications);
        }
    }
}
