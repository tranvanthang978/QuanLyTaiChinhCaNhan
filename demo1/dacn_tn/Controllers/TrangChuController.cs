using dacn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dacn.Controllers
{
    public class TrangChuController : Controller
    {
        private ChiTiet db = new ChiTiet();
        // GET: TrangChu
        public ActionResult Index(int? thang, int? nam, string loai)
        {
            using (var db = new ChiTiet())
            {


                var query = from gd in db.GiaoDich
                            join nd in db.NguoiDung on gd.MaNguoiDung equals nd.MaNguoiDung
                            join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                            select new BaoCaoViewModel
                            {
                                MaNguoiDung = nd.MaNguoiDung,
                                HoTen = nd.HoTen,
                                Loai = dm.Loai,
                                TenDanhMuc = dm.TenDanhMuc,
                                TongTien = gd.SoTien,
                                NgayGD = gd.NgayGD
                            };

                if (nam.HasValue)
                    query = query.Where(x => x.NgayGD.Year == nam.Value);

                if (thang.HasValue)
                    query = query.Where(x => x.NgayGD.Month == thang.Value);

                if (!string.IsNullOrEmpty(loai))
                    query = query.Where(x => x.Loai == loai);

                var model = query.ToList();

                // Tách danh sách Thu / Chi
                var thuList = model.Where(x => x.Loai == "Thu").ToList();
                var chiList = model.Where(x => x.Loai == "Chi").ToList();

                // Gán cho ViewBag (tránh null thì cho List rỗng)
                ViewBag.ThuList = thuList ?? new List<BaoCaoViewModel>();
                ViewBag.ChiList = chiList ?? new List<BaoCaoViewModel>();

                // Tính tổng
                ViewBag.TongThu = thuList.Sum(x => x.TongTien);
                ViewBag.TongChi = chiList.Sum(x => x.TongTien);
                ViewBag.ChenhLech = ViewBag.TongThu - ViewBag.TongChi;

                // Tính tổng chi
                decimal tongChi = chiList.Sum(x => x.TongTien);

                // Nhóm theo TenDanhMuc và tính tổng từng nhóm
                var chiTheoNhom = chiList
                    .GroupBy(x => x.TenDanhMuc)
                    .Select(g => new TopChiViewModel
                    {
                        TenDanhMuc = g.Key,
                        TongTien = g.Sum(x => x.TongTien),
                        PhanTram = tongChi > 0 ? Math.Round((g.Sum(x => x.TongTien) / tongChi) * 100, 2) : 0
                    })
                    .OrderByDescending(x => x.PhanTram)
                    .ToList();

                // Gán vào ViewBag để View dùng
                ViewBag.TopChi = chiTheoNhom;



                // Lấy các giao dịch gần đây (ví dụ 10 giao dịch mới nhất)
                var recentGiaoDich = model
                    .OrderByDescending(x => x.NgayGD)
                    .Take(10)
                    .Select(x => new RecentActivityViewModel
                    {
                        NgayGD = x.NgayGD,
                        NoiDung = $"{x.TenDanhMuc} - {x.Loai} - {x.TongTien} VND"
                    })
                    .ToList();

                // Gán vào ViewBag
                ViewBag.RecentGiaoDich = recentGiaoDich;


                // Lấy tháng và năm hiện tại
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Query dữ liệu cho chart
                var chartQuery = from gd in db.GiaoDich
                                 join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                                 where gd.NgayGD.Month == currentMonth && gd.NgayGD.Year == currentYear
                                 select new BaoCaoViewModel
                                 {
                                     NgayGD = gd.NgayGD,
                                     Loai = dm.Loai,
                                     TongTien = gd.SoTien
                                 };

                var chartModel = chartQuery.ToList();

                // Gom dữ liệu theo ngày
                var chartData = chartModel
                    .GroupBy(x => x.NgayGD.Day)
                    .Select(g => new
                    {
                        NgayGD = g.Key,
                        Thu = g.Where(x => x.Loai == "Thu").Sum(x => x.TongTien),
                        Chi = g.Where(x => x.Loai == "Chi").Sum(x => x.TongTien)
                    })
                    .OrderBy(x => x.NgayGD)
                    .ToList();

                ViewBag.ChartData = chartData;





                return View(model);
            }
        }

        
       
    }
}