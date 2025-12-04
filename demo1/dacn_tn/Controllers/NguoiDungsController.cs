using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dacn.Models;

namespace dacn.Controllers
{
    public class NguoiDungsController : Controller
    {
        private ChiTiet db = new ChiTiet();

        // GET: NguoiDungs
        public ActionResult Index()
        {
            return View(db.NguoiDung.ToList());
        }
        public ActionResult DoiMatKhau()
        {
            return View();
        }
        public ActionResult ThongTinCaNhan()
        {
            return View();
        }
        public ActionResult DangNhap() => View();

        [HttpPost]
        public ActionResult DangNhap(string TenDangNhap, string MatKhau)
        {
            var user = db.NguoiDung.FirstOrDefault(u => u.TenDangNhap == TenDangNhap && u.MatKhau == MatKhau);
            if (user != null)
            {
                Session["MaNguoiDung"] = user.MaNguoiDung;
                Session["HoTen"] = user.HoTen;
                Session["TenDangNhap"] = user.TenDangNhap;
                Session["MatKhau"] = user.MatKhau;
                return RedirectToAction("Index", "TrangChu");
            }
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        // Đăng ký
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy([Bind(Include = "TenDangNhap,MatKhau,HoTen,Email")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                // Thêm vào DB
                db.NguoiDung.Add(nguoiDung);
                db.SaveChanges();

                // Tạo session cho user vừa đăng ký
                Session["MaNguoiDung"] = nguoiDung.MaNguoiDung;
                Session["TenDangNhap"] = nguoiDung.TenDangNhap;

                // Chuyển sang trang Báo cáo
                return RedirectToAction("Index", "BaoCao");
            }

            return View(nguoiDung);
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("DangNhap");
        }


        

        
       

        // GET: NguoiDungs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDung.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNguoiDung,TenDangNhap,MatKhau,HoTen,NgayTao")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                // Gán ngày tạo tự động
                nguoiDung.NgayTao = DateTime.Now;

                db.NguoiDung.Add(nguoiDung);
                db.SaveChanges();

                // Tạo session cho user vừa đăng ký
                Session["MaNguoiDung"] = nguoiDung.MaNguoiDung;
                Session["TenDangNhap"] = nguoiDung.TenDangNhap;

                // Chuyển sang trang Báo cáo
                return RedirectToAction("Index", "BaoCao");
            }

            return View(nguoiDung);
        }

        // GET: NguoiDungs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDung.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,TenDangNhap,MatKhau,HoTen,NgayTao")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoiDung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDung.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: NguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NguoiDung nguoiDung = db.NguoiDung.Find(id);
            db.NguoiDung.Remove(nguoiDung);
            db.SaveChanges();
            return RedirectToAction("Index");
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
