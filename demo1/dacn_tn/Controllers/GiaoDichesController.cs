using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using dacn.Models;

namespace dacn.Controllers
{
    // ViewModel để truyền dữ liệu lọc
    public class GiaoDichFilterViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string LoaiGiaoDich { get; set; } // "Thu", "Chi" hoặc null
        public string Query { get; set; }
        public IEnumerable<GiaoDich> GiaoDiches { get; set; }
    }

    public class GiaoDichesController : Controller
    {
        private ChiTiet db = new ChiTiet();

        // Kiểm tra đăng nhập an toàn
        private int? GetMaNguoiDung()
        {
            if (Session["MaNguoiDung"] != null)
                return Convert.ToInt32(Session["MaNguoiDung"]);
            return null;
        }

        // GET: GiaoDiches
        public ActionResult Index(DateTime? tuNgay, DateTime? denNgay, string loai)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue)
                return RedirectToAction("DangNhap", "NguoiDung"); // sửa tên action login theo project của bạn

            var query = db.GiaoDich.Where(g => g.MaNguoiDung == maND.Value);

            // Lọc theo ngày
            if (tuNgay.HasValue)
                query = query.Where(g => g.NgayGD >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(g => g.NgayGD <= denNgay.Value);

            // Lọc theo loại giao dịch
            if (!string.IsNullOrEmpty(loai))
            {
                query = query.Where(g => g.DanhMuc.Loai == loai);
            }

            var model = new GiaoDichFilterViewModel
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                LoaiGiaoDich = loai,
                GiaoDiches = query.OrderByDescending(g => g.NgayGD).ToList()
            };

            return View(model);
        }

        // GET: GiaoDiches/Create
        public ActionResult Create()
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue) return RedirectToAction("DangNhap", "NguoiDung");

            ViewBag.MaDanhMuc = GetDanhMucFull(maND.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiaoDich giaoDich)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue) return RedirectToAction("DangNhap", "NguoiDung");

            if (ModelState.IsValid)
            {
                giaoDich.MaNguoiDung = maND.Value;
                db.GiaoDich.Add(giaoDich);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = GetDanhMucFull(maND.Value);
            return View(giaoDich);
        }

        // GET: GiaoDiches/Edit/5
        public ActionResult Edit(int? id)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue) return RedirectToAction("DangNhap", "NguoiDung");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GiaoDich giaoDich = db.GiaoDich.Find(id);
            if (giaoDich == null) return HttpNotFound();

            ViewBag.MaDanhMuc = GetDanhMucFull(maND.Value, giaoDich.MaDanhMuc);
            return View(giaoDich);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiaoDich giaoDich)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue) return RedirectToAction("DangNhap", "NguoiDung");

            if (ModelState.IsValid)
            {
                db.Entry(giaoDich).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDanhMuc = GetDanhMucFull(maND.Value, giaoDich.MaDanhMuc);
            return View(giaoDich);
        }

        // GET: GiaoDiches/Delete/5
        public ActionResult Delete(int? id)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue) return RedirectToAction("DangNhap", "NguoiDung");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GiaoDich giaoDich = db.GiaoDich.Find(id);
            if (giaoDich == null) return HttpNotFound();

            return View(giaoDich);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GiaoDich giaoDich = db.GiaoDich.Find(id);
            db.GiaoDich.Remove(giaoDich);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hàm lấy danh mục đầy đủ (của người dùng + danh mục chung)
        private SelectList GetDanhMucFull(int maND, object selectedValue = null)
        {
            var danhMuc = db.DanhMuc.ToList();
            return new SelectList(danhMuc, "MaDanhMuc", "TenDanhMuc", selectedValue);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult Details(int? id)
        {
            int? maND = GetMaNguoiDung();
            if (!maND.HasValue)
                return RedirectToAction("DangNhap", "NguoiDung");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Tìm giao dịch. PHẢI thêm .Include(g => g.DanhMuc) để View hiển thị được TenDanhMuc
            GiaoDich giaoDich = db.GiaoDich
                                    .Include(g => g.DanhMuc)
                                    .FirstOrDefault(g => g.MaGiaoDich == id && g.MaNguoiDung == maND.Value);

            if (giaoDich == null)
                return HttpNotFound(); // Trả về 404 nếu không tìm thấy giao dịch hoặc không thuộc user

            return View(giaoDich);
        }


        // 👉 Thêm mới action này:
        public ActionResult Search(string query)
        {
            var giaoDiches = db.GiaoDich.Include(g => g.DanhMuc).AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                giaoDiches = giaoDiches.Where(g =>
                    g.MoTa.Contains(query) ||
                    g.DanhMuc.TenDanhMuc.Contains(query) ||
                    
                    g.SoTien.ToString().Contains(query)
                );
            }

            // Tạo model để reuse lại View Index
            var model = new GiaoDichFilterViewModel
            {
                GiaoDiches = giaoDiches.OrderByDescending(g => g.NgayGD).ToList(),
                Query = query
            };

            // Dùng lại View Index.cshtml để hiển thị
            return View("Index", model);
        }
    }
}
