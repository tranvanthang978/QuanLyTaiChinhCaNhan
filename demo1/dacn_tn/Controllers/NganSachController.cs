using dacn.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace dacn.Controllers
{
    public class NganSachController : Controller
    {
        private ChiTiet db = new ChiTiet();

        // GET: NganSach
        public ActionResult Index()
        {
            int thang = DateTime.Now.Month;
            int nam = DateTime.Now.Year;
            int maNguoiDung = 1; // tạm lấy người dùng ID = 1, sau này có thể dùng Session

            // Lấy danh sách ngân sách của người dùng tháng hiện tại
            var listNganSach = db.NganSach
                .Include(n => n.DanhMuc)  // load bảng liên kết
                .Where(n => n.MaNguoiDung == maNguoiDung && n.Thang == thang && n.Nam == nam)
                .ToList();

            // Tổng ngân sách tháng hiện tại
            var tongNganSach = listNganSach.Sum(n => n.SoTienGioiHan);
           
            // Tổng đã chi tháng hiện tại
            var tongDaChi = (from gd in db.GiaoDich
                             join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                             where dm.Loai == "Chi"
                                   && gd.MaNguoiDung == maNguoiDung
                                   && gd.NgayGD.Month == thang
                                   && gd.NgayGD.Year == nam
                             select gd.SoTien).Sum();
            
            

            // Số ngày còn lại đến cuối tháng
            int ngayConLai = DateTime.DaysInMonth(nam, thang) - DateTime.Now.Day;

            // Gửi dữ liệu sang View
            ViewBag.TongNganSach = tongNganSach;
            ViewBag.TongDaChi = tongDaChi;
            ViewBag.NgayConLai = ngayConLai;


            foreach (var n in listNganSach)
            {
                var DaChi = db.GiaoDich
                    .Where(gd => gd.MaDanhMuc == n.MaDanhMuc
                              && gd.MaNguoiDung == maNguoiDung
                              && gd.NgayGD.Month == thang
                              && gd.NgayGD.Year == nam)
                    .Sum(gd => (decimal?)gd.SoTien) ?? 0;

                n.DaChi = DaChi;
                n.ConLai = n.SoTienGioiHan - DaChi;
            }


            // Tạo danh sách notification
            var notifications = new List<string>();
            foreach (var n in listNganSach)
            {
                var DaChi = db.GiaoDich
                    .Where(gd => gd.MaDanhMuc == n.MaDanhMuc
                              && gd.MaNguoiDung == maNguoiDung
                              && gd.NgayGD.Month == thang
                              && gd.NgayGD.Year == nam)
                    .Sum(gd => (decimal?)gd.SoTien) ?? 0;

                n.DaChi = DaChi;
                n.ConLai = n.SoTienGioiHan - DaChi;

                // Assuming this is the C# logic where you generate your List<string> notifications:

                if (n.ConLai < 0)
                {
                    // (1) Over Budget
                    notifications.Add($"TYPE:OVER | ⚠️ Ngân sách \"{n.DanhMuc.TenDanhMuc}\" đã vượt quá hạn mức!");
                }
               


            }

            ViewBag.Notifications = notifications;
            ViewBag.TongDaChi = tongDaChi;

            // Lấy tất cả danh mục chi
            ViewBag.DanhMucChi = db.DanhMuc
                .Where(dm => dm.Loai == "Chi") // hoặc loại bạn muốn
                .ToList();

            return View(listNganSach); // Trả về list
        }


        

    


        [HttpPost]
        public ActionResult Create(NganSach model)
        {
            if (ModelState.IsValid)
            {
                model.Thang = model.NgayTao.Month;
                model.Nam = model.NgayTao.Year;
                model.MaNguoiDung = 1;

                db.NganSach.Add(model);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "NganSach")
                });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        [HttpPost]
        public ActionResult Edit(NganSach model)
        {
            if (ModelState.IsValid)
            {
                var existing = db.NganSach.Find(model.MaNganSach); // Id là khóa chính
                if (existing == null)
                    return Json(new { success = false, message = "Không tìm thấy ngân sách" });

                // Cập nhật các trường
                existing.MaDanhMuc = model.MaDanhMuc;
                existing.SoTienGioiHan = model.SoTienGioiHan;
                existing.NgayTao = model.NgayTao;
                existing.Thang = model.NgayTao.Month;
                existing.Nam = model.NgayTao.Year;

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "NganSach")
                });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // GET: NganSach/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var nganSach = db.NganSach
                .Include(n => n.DanhMuc)
                .FirstOrDefault(n => n.MaNganSach == id);

            if (nganSach == null)
                return HttpNotFound();

            return View(nganSach); // nếu muốn hiện trang xác nhận
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            var nganSach = db.NganSach.Find(id);
            if (nganSach == null)
                return Json(new { success = false, message = "Không tìm thấy ngân sách" });

            try
            {
                db.NganSach.Remove(nganSach);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public ActionResult ExportToCsv()
        {
            var nganSachs = db.NganSach.ToList();

            var csv = new StringBuilder();

            // Ghi header
            csv.AppendLine("ID,Tên ngân sách,Số tiền,Ngày tạo");

            // Ghi từng dòng dữ liệu
            foreach (var item in nganSachs)
            {
                csv.AppendLine($"{item.MaNganSach},{EscapeCsv(item.DanhMuc.TenDanhMuc)},{item.SoTienGioiHan},{item.NgayTao:yyyy-MM-dd}");
            }

            // Thêm BOM UTF-8 để Excel nhận đúng tiếng Việt
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
            var buffer = bom.Concat(csvBytes).ToArray();

            return File(buffer, "text/csv", "DanhSachNganSach.csv");
        }

        // Hàm để xử lý chuỗi có dấu phẩy hoặc dấu nháy trong CSV
        private string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }

            return input;
        }




    }
}