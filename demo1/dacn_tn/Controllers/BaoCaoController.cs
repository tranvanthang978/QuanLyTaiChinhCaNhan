using dacn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using OfficeOpenXml.Style;


namespace dacn.Controllers
{
    public class BaoCaoController : Controller
    {
        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            bool isUserFiltered = startDate.HasValue || endDate.HasValue;

            // Phạm vi mặc định: 6 tháng gần nhất
            var defaultEndDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var defaultStartDate = defaultEndDate.AddMonths(-6).AddDays(1).Date;

            var currentStartDate = startDate ?? defaultStartDate;
            var currentEndDate = endDate ?? defaultEndDate;

            if (endDate.HasValue && endDate.Value.Date == endDate.Value)
            {
                currentEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            }

            using (var db = new ChiTiet())
            {
                var query = from gd in db.GiaoDich
                            join nd in db.NguoiDung on gd.MaNguoiDung equals nd.MaNguoiDung
                            join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                            where gd.NgayGD >= currentStartDate && gd.NgayGD <= currentEndDate
                            select new
                            {
                                nd.HoTen,
                                dm.Loai,
                                dm.TenDanhMuc,
                                gd.SoTien,
                                gd.NgayGD
                            };

                var rawData = query.ToList();

                //  Gộp dữ liệu cho Doughnut chart (theo danh mục)
                
                var aggregatedData = rawData
                    .GroupBy(x => new { x.Loai, x.TenDanhMuc })
                    .Select(g => new BaoCaoViewModel
                    {
                        Loai = g.Key.Loai,
                        TenDanhMuc = g.Key.TenDanhMuc,
                        TongTien = g.Sum(x => x.SoTien),
                    })
                    .ToList();

                var thuList = aggregatedData.Where(x => x.Loai == "Thu").ToList();
                var chiList = aggregatedData.Where(x => x.Loai == "Chi").ToList();

                //  Biểu đồ đường chính: Xu hướng Thu - Chi (Tháng hoặc Ngày)
                
                List<ThongKeNgayViewModel> summaryData;

                if (isUserFiltered)
                {
                    // Nếu người dùng lọc → nhóm theo NGÀY
                    summaryData = rawData
                        .GroupBy(x => x.NgayGD.Date)
                        .Select(g => new ThongKeNgayViewModel
                        {
                            Ngay = g.Key,
                            TongThu = g.Where(x => x.Loai == "Thu").Sum(x => x.SoTien),
                            TongChi = g.Where(x => x.Loai == "Chi").Sum(x => x.SoTien)
                        })
                        .OrderBy(x => x.Ngay)
                        .ToList();
                }
                else
                {
                    // Mặc định → nhóm theo THÁNG
                    summaryData = rawData
                        .GroupBy(x => new { x.NgayGD.Year, x.NgayGD.Month })
                        .Select(g => new ThongKeNgayViewModel
                        {
                            Ngay = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TongThu = g.Where(x => x.Loai == "Thu").Sum(x => x.SoTien),
                            TongChi = g.Where(x => x.Loai == "Chi").Sum(x => x.SoTien)
                        })
                        .OrderBy(x => x.Ngay)
                        .ToList();
                }

                // Biểu đồ chi tiết: Xu hướng Thu / Chi theo Danh mục
                
                List<BaoCaoViewModel> categorySummary;

                if (isUserFiltered)
                {
                    // Khi người dùng lọc → nhóm THEO NGÀY
                    categorySummary = rawData
                        .GroupBy(x => new { x.Loai, x.TenDanhMuc, x.NgayGD.Date })
                        .Select(g => new BaoCaoViewModel
                        {
                            Loai = g.Key.Loai,
                            TenDanhMuc = g.Key.TenDanhMuc,
                            NgayGD = g.Key.Date,
                            TongTien = g.Sum(x => x.SoTien)
                        })
                        .OrderBy(x => x.NgayGD)
                        .ToList();
                }
                else
                {
                    // Mặc định → nhóm THEO THÁNG
                    categorySummary = rawData
                        .GroupBy(x => new { x.Loai, x.TenDanhMuc, x.NgayGD.Year, x.NgayGD.Month })
                        .Select(g => new BaoCaoViewModel
                        {
                            Loai = g.Key.Loai,
                            TenDanhMuc = g.Key.TenDanhMuc,
                            NgayGD = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TongTien = g.Sum(x => x.SoTien)
                        })
                        .OrderBy(x => x.NgayGD)
                        .ToList();
                }

                //  Dữ liệu bảng chi tiết
                
                var detailedTransactions = rawData
                    .Select(x => new BaoCaoViewModel
                    {
                        TenDanhMuc = x.TenDanhMuc,
                        Loai = x.Loai,
                        TongTien = x.SoTien,
                        NgayGD = x.NgayGD
                    })
                    .OrderByDescending(x => x.NgayGD)
                    .ToList();

                // Gửi dữ liệu sang View
                
                ViewBag.StartDate = currentStartDate;
                ViewBag.EndDate = currentEndDate;
                ViewBag.IsUserFiltered = isUserFiltered;

                ViewBag.ThuList = thuList;
                ViewBag.ChiList = chiList;
                ViewBag.MonthlySummary = summaryData;
                ViewBag.MonthlyCategorySummary = categorySummary;
                ViewBag.DetailedTransactions = detailedTransactions;

                ViewBag.TongThu = thuList.Sum(x => x.TongTien);
                ViewBag.TongChi = chiList.Sum(x => x.TongTien);

                ViewBag.jsonDailySummary = JsonConvert.SerializeObject(summaryData);
                ViewBag.jsonDetailedTransactions = JsonConvert.SerializeObject(categorySummary);

                return View();


            }
        }

        public ActionResult ExportToExcelServer(DateTime? startDate, DateTime? endDate)
        {
            // Lấy ID người dùng
            int currentUserId = 0;
            if (Session["MaNguoiDung"] != null)
            {
                if (int.TryParse(Session["MaNguoiDung"].ToString(), out int parsedId))
                {
                    currentUserId = parsedId;
                }
            }

            var currentEndDate = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var currentStartDate = startDate ?? currentEndDate.AddMonths(-6).AddDays(1).Date;

            if (endDate.HasValue && endDate.Value.Date == endDate.Value)
            {
                currentEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            }

            using (var db = new ChiTiet())
            {
                // Lọc dữ liệu theo ID người dùng và phạm vi thời gian
                var rawData = (from gd in db.GiaoDich
                               join dm in db.DanhMuc on gd.MaDanhMuc equals dm.MaDanhMuc
                               where gd.MaNguoiDung == currentUserId
                                     && gd.NgayGD >= currentStartDate && gd.NgayGD <= currentEndDate
                               select new
                               {
                                   dm.Loai,
                                   dm.TenDanhMuc,
                                   gd.SoTien,
                                   gd.NgayGD
                               }).ToList();

                var tongThu = rawData.Where(x => x.Loai == "Thu").Sum(x => x.SoTien);
                var tongChi = rawData.Where(x => x.Loai == "Chi").Sum(x => x.SoTien);
                var chenhlech = tongThu - tongChi;

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Báo cáo Chi tiêu");

                    int row = 1;

                    // --- BẢNG 1: TIÊU ĐỀ BÁO CÁO ---
                    ws.Cells[row, 1].Value = "BÁO CÁO TỔNG HỢP THU - CHI";
                    ws.Cells[row, 1, row, 3].Merge = true;
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 14;
                    row++;
                    ws.Cells[row, 1].Value = $"Phạm vi: Từ {currentStartDate:dd/MM/yyyy} đến {currentEndDate:dd/MM/yyyy}";
                    ws.Cells[row, 1, row, 3].Merge = true;
                    row++;

                    // --- BẢNG 2: TỔNG HỢP KẾT QUẢ (Có Viền) ---
                    row++;
                    int summary_title_row = row++; // Bắt đầu từ dòng 4
                    ws.Cells[summary_title_row, 1].Value = "TỔNG HỢP KẾT QUẢ";

                    int summary_start_row = row;

                    ws.Cells[row, 1].Value = "Tổng Thu"; ws.Cells[row, 2].Value = tongThu; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                    row++;
                    ws.Cells[row, 1].Value = "Tổng Chi"; ws.Cells[row, 2].Value = tongChi; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                    row++;
                    ws.Cells[row, 1].Value = "Chênh lệch"; ws.Cells[row, 2].Value = chenhlech; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";

                    int summary_end_row = row;

                    // Áp dụng viền cho Bảng Tổng hợp (3 dòng dữ liệu, 2 cột)
                    var summaryRange = ws.Cells[summary_start_row, 1, summary_end_row, 2];
                    summaryRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    summaryRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    summaryRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    summaryRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++; row++;

                    // Chuẩn bị dữ liệu cho các bảng chi tiết
                    var thuList = rawData.Where(x => x.Loai == "Thu")
                                         .GroupBy(x => x.TenDanhMuc)
                                         .Select(g => new { Key = g.Key, TongTien = g.Sum(x => x.SoTien) })
                                         .ToList();

                    var chiList = rawData.Where(x => x.Loai == "Chi")
                                         .GroupBy(x => x.TenDanhMuc)
                                         .Select(g => new { Key = g.Key, TongTien = g.Sum(x => x.SoTien) })
                                         .ToList();

                    var isUserFiltered = startDate.HasValue || endDate.HasValue;
                    var dailyOrMonthlySummary = rawData
                        .GroupBy(x => isUserFiltered ? x.NgayGD.Date : new DateTime(x.NgayGD.Year, x.NgayGD.Month, 1))
                        .Select(g => new
                        {
                            Ngay = g.Key,
                            Thu = g.Where(x => x.Loai == "Thu").Sum(x => x.SoTien),
                            Chi = g.Where(x => x.Loai == "Chi").Sum(x => x.SoTien)
                        })
                        .OrderBy(x => x.Ngay)
                        .ToList();

                    // --- BẢNG 3: KHOẢN THU THEO DANH MỤC (Có Viền) ---
                    ws.Cells[row++, 1].Value = "KHOẢN THU (Tổng hợp theo Danh mục)";
                    int thu_header_row = row;
                    ws.Cells[row, 1].Value = "Danh mục"; ws.Cells[row, 2].Value = "Tổng tiền";
                    ws.Cells[row, 1, row, 2].Style.Font.Bold = true;
                    row++;

                    int thu_start_data_row = row;
                    foreach (var item in thuList)
                    {
                        ws.Cells[row, 1].Value = item.Key;
                        ws.Cells[row, 2].Value = item.TongTien; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        row++;
                    }

                    int thu_end_data_row = row - 1;

                    // Áp dụng viền cho Bảng Thu (bao gồm cả header)
                    var thuRange = ws.Cells[thu_header_row, 1, thu_end_data_row, 2];
                    thuRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    thuRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    thuRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    thuRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;

                    // --- BẢNG 4: KHOẢN CHI THEO DANH MỤC (Có Viền) ---
                    ws.Cells[row++, 1].Value = "KHOẢN CHI (Tổng hợp theo Danh mục)";
                    int chi_header_row = row;
                    ws.Cells[row, 1].Value = "Danh mục"; ws.Cells[row, 2].Value = "Tổng tiền";
                    ws.Cells[row, 1, row, 2].Style.Font.Bold = true;
                    row++;

                    int chi_start_data_row = row;
                    foreach (var item in chiList)
                    {
                        ws.Cells[row, 1].Value = item.Key;
                        ws.Cells[row, 2].Value = item.TongTien; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        row++;
                    }

                    int chi_end_data_row = row - 1;

                    // Áp dụng viền cho Bảng Chi (bao gồm cả header)
                    var chiRange = ws.Cells[chi_header_row, 1, chi_end_data_row, 2];
                    chiRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    chiRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    chiRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    chiRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;

                    // --- BẢNG 5: CHI TIẾT THEO NGÀY/THÁNG (Có Viền) ---
                    ws.Cells[row++, 1].Value = isUserFiltered ? "CHI TIẾT THEO NGÀY" : "CHI TIẾT THEO THÁNG";
                    int detail_header_row = row;
                    ws.Cells[row, 1].Value = isUserFiltered ? "Ngày" : "Tháng";
                    ws.Cells[row, 2].Value = "Thu";
                    ws.Cells[row, 3].Value = "Chi";
                    ws.Cells[row, 1, row, 3].Style.Font.Bold = true;
                    row++;

                    int detail_start_data_row = row;
                    foreach (var item in dailyOrMonthlySummary)
                    {
                        string dateFormat = isUserFiltered ? "dd/MM/yyyy" : "MM/yyyy";
                        ws.Cells[row, 1].Value = item.Ngay.ToString(dateFormat);
                        ws.Cells[row, 2].Value = item.Thu; ws.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        ws.Cells[row, 3].Value = item.Chi; ws.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                        row++;
                    }

                    int detail_end_data_row = row - 1;

                    // Áp dụng viền cho Bảng Chi tiết (bao gồm cả header)
                    var detailRange = ws.Cells[detail_header_row, 1, detail_end_data_row, 3];
                    detailRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    detailRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    detailRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    detailRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    row++;

                    // Tự động điều chỉnh độ rộng cột
                    ws.Cells[1, 1, row, 3].AutoFitColumns();

                    // Gửi file Excel về trình duyệt
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = $"BaoCaoTongHop_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }


}
