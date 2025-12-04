using dacn.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace dacn.Controllers
{

    public class DanhMucsController : Controller
    {
        public class QLChiTieuDbContext : DbContext
        {
            public DbSet<GiaoDich> GiaoDich { get; set; }
            public QLChiTieuDbContext() : base("name=ChiTieu") { }
            public DbSet<DanhMuc> DanhMuc { get; set; }
        }
        private QLChiTieuDbContext db = new QLChiTieuDbContext();

        // GET: DanhMucs/KhoanThu
        public ActionResult KhoanThu()
        {
             // ví dụ tạm, bạn có thể lấy từ session sau này
            var khoanThu = db.DanhMuc
                .Where(d => d.Loai == "Thu")
                .Include(d=>d.GiaoDich)
                .ToList();
            return View(khoanThu);
        }

        public ActionResult KhoanChi()
        {
            
            var khoanThu = db.DanhMuc
                .Where(d => d.Loai == "Chi")
                .Include(d => d.GiaoDich)
                .ToList();
            return View(khoanThu);
        }

        public ActionResult Create(string loai = "Thu")
        {
            ViewBag.Loai = loai;
            return View();
        }

        // POST: DanhMucs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DanhMuc danhMuc, string MoTa, decimal SoTien, DateTime NgayGD, bool taoThem = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate loại
                    if (danhMuc.Loai != "Thu" && danhMuc.Loai != "Chi")
                    {
                        ModelState.AddModelError("Loai", "Loại giao dịch không hợp lệ");
                        return View(danhMuc);
                    }

                    // Lấy mã người dùng từ session (tạm thời dùng 1)
                    danhMuc.MaNguoiDung = 1;

                    // Kiểm tra trùng tên danh mục cùng loại
                    var danhMucTrung = db.DanhMuc
                        .Any(d => d.TenDanhMuc == danhMuc.TenDanhMuc &&
                                 d.Loai == danhMuc.Loai &&
                                 d.MaNguoiDung == danhMuc.MaNguoiDung);

                    if (danhMucTrung)
                    {
                        ModelState.AddModelError("TenDanhMuc", "Đã tồn tại danh mục cùng tên và loại");
                        return View(danhMuc);
                    }

                    db.DanhMuc.Add(danhMuc);
                    db.SaveChanges();

                    // Tạo giao dịch đầu tiên
                    var giaoDich = new GiaoDich
                    {
                        MoTa = MoTa,
                        SoTien = SoTien,
                        NgayGD = NgayGD,
                        MaDanhMuc = danhMuc.MaDanhMuc,
                        MaNguoiDung = danhMuc.MaNguoiDung
                    };

                    db.GiaoDich.Add(giaoDich);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = $"Đã thêm {(danhMuc.Loai == "Thu" ? "khoản thu" : "khoản chi")} thành công!";

                    if (taoThem)
                    {
                        return RedirectToAction("Create", new { loai = danhMuc.Loai });
                    }

                    return RedirectToAction("Khoan" + danhMuc.Loai);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo giao dịch: " + ex.Message);
            }

            ViewBag.Loai = danhMuc.Loai;
            return View(danhMuc);
        }

        // GET: DanhMucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DanhMuc danhMuc = db.DanhMuc
                .Include(d => d.GiaoDich)
                .FirstOrDefault(d => d.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        // POST: DanhMucs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMuc danhMuc, string MoTa, decimal SoTien, DateTime NgayGD)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(danhMuc).State = EntityState.Modified;
                    db.SaveChanges();

                    // Cập nhật giao dịch gần nhất
                    var giaoDichGanNhat = db.GiaoDich
                        .Where(g => g.MaDanhMuc == danhMuc.MaDanhMuc)
                        .OrderByDescending(g => g.NgayGD)
                        .FirstOrDefault();

                    if (giaoDichGanNhat != null)
                    {
                        giaoDichGanNhat.MoTa = MoTa;
                        giaoDichGanNhat.SoTien = SoTien;
                        giaoDichGanNhat.NgayGD = NgayGD;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Khoan" + danhMuc.Loai);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
            }

            return View(danhMuc);
        }

        // GET: DanhMucs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DanhMuc danhMuc = db.DanhMuc
                .Include(d => d.GiaoDich)
                .FirstOrDefault(d => d.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        // POST: DanhMucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DanhMuc danhMuc = db.DanhMuc
                .Include(d => d.GiaoDich)
                .FirstOrDefault(d => d.MaDanhMuc == id);

            if (danhMuc != null)
            {
                // Xóa tất cả giao dịch liên quan
                db.GiaoDich.RemoveRange(danhMuc.GiaoDich);
                db.DanhMuc.Remove(danhMuc);
                db.SaveChanges();
            }

            return RedirectToAction("Khoan" + danhMuc.Loai);
        }

        // GET: DanhMucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DanhMuc danhMuc = db.DanhMuc
                .Include(d => d.GiaoDich)
                .FirstOrDefault(d => d.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        public ActionResult Loc(string loai = "Thu")
        {
            try
            {
                ViewBag.Loai = loai;

                // Lấy danh sách danh mục để hiển thị trong dropdown
                var danhMucs = db.DanhMuc
                    .Where(d => d.Loai == loai)
                    .Include(d => d.GiaoDich)
                    .ToList();

                return View(danhMucs);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tải trang lọc: " + ex.Message;
                return View(new List<DanhMuc>());
            }
        }

        // GET: DanhMucs/FilterGiaoDich
        public JsonResult FilterGiaoDich(string loai = "Thu", string filterType = "all",
                                        string specificDate = null, string month = null,
                                        string fromDate = null, string toDate = null,
                                        int? categoryId = null, decimal? minAmount = null,
                                        decimal? maxAmount = null)
        {
            try
            {
                // Query cơ bản - join với DanhMuc
                var query = from gd in db.GiaoDich
                            join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                            where dm.Loai == loai
                            select new { GiaoDich = gd, DanhMuc = dm };

                // Áp dụng các bộ lọc
                if (!string.IsNullOrEmpty(filterType))
                {
                    switch (filterType.ToLower())
                    {
                        case "date":
                            if (!string.IsNullOrEmpty(specificDate))
                            {
                                DateTime date = DateTime.Parse(specificDate);
                                query = query.Where(x => EntityFunctions.TruncateTime(x.GiaoDich.NgayGD) == date.Date);
                            }
                            break;

                        case "month":
                            if (!string.IsNullOrEmpty(month))
                            {
                                DateTime monthStart = DateTime.Parse(month + "-01");
                                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
                                query = query.Where(x => x.GiaoDich.NgayGD >= monthStart && x.GiaoDich.NgayGD <= monthEnd);
                            }
                            break;

                        case "range":
                            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                            {
                                DateTime start = DateTime.Parse(fromDate);
                                DateTime end = DateTime.Parse(toDate).AddDays(1).AddSeconds(-1);
                                query = query.Where(x => x.GiaoDich.NgayGD >= start && x.GiaoDich.NgayGD <= end);
                            }
                            break;

                        case "category":
                            if (categoryId.HasValue)
                            {
                                query = query.Where(x => x.GiaoDich.MaDanhMuc == categoryId.Value);
                            }
                            break;
                    }
                }

                // Lọc theo số tiền
                if (minAmount.HasValue)
                {
                    query = query.Where(x => x.GiaoDich.SoTien >= minAmount.Value);
                }
                if (maxAmount.HasValue)
                {
                    query = query.Where(x => x.GiaoDich.SoTien <= maxAmount.Value);
                }

                // Sắp xếp và lấy kết quả
                var results = query
                    .OrderByDescending(x => x.GiaoDich.NgayGD)
                    .ToList()
                    .Select(x => new
                    {
                        MaGiaoDich = x.GiaoDich.MaGiaoDich,
                        MoTa = x.GiaoDich.MoTa,
                        NgayGD = x.GiaoDich.NgayGD,
                        SoTien = x.GiaoDich.SoTien,
                        MaDanhMuc = x.GiaoDich.MaDanhMuc,
                        DanhMucTen = x.DanhMuc.TenDanhMuc,
                        Loai = x.DanhMuc.Loai
                    })
                    .ToList();

                var totalAmount = results.Sum(x => x.SoTien);

                return Json(new
                {
                    success = true,
                    giaoDichList = results,
                    totalCount = results.Count,
                    totalAmount = totalAmount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi khi lọc dữ liệu: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
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
