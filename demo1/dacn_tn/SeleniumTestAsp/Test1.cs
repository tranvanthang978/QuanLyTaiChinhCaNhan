using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using SeleniumExtras.WaitHelpers;

namespace dacn.SeleniumTestAsp
{
    public class Test1
    {
        public static void RunTest()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                // 1️⃣ Mở trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:44352/NguoiDungs/DangNhap");
                driver.Manage().Window.Maximize();
                Console.WriteLine("✅ Đã mở trang đăng nhập");

                Thread.Sleep(1000);

                // 2️⃣ Nhập tài khoản
                driver.FindElement(By.Id("TenDangNhap")).SendKeys("user1");  // ⚠️ đổi ID theo form thật nếu khác
                driver.FindElement(By.Id("MatKhau")).SendKeys("123456");     // ⚠️ đổi ID theo form thật nếu khác

                // 3️⃣ Nhấn nút đăng nhập
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Debug.WriteLine("✅ Đã nhấn đăng nhập");

                Thread.Sleep(1000);



                

                // 5️⃣ Nhấn vào “Quản lý ngân sách”
                driver.FindElement(By.LinkText("Quản lý Ngân sách")).Click();
                Debug.WriteLine("✅ Đã mở trang Quản lý ngân sách");

                Thread.Sleep(2000);



                // 6️⃣ Kiểm tra URL
                string currentUrl = driver.Url;
                if (currentUrl.Contains("/NganSach"))
                {
                    Debug.WriteLine("🎉 TEST PASSED: Đã điều hướng đến trang ngân sách thành công!");
                }
                else
                {
                    Debug.WriteLine("❌ TEST FAILED: URL hiện tại không đúng. URL hiện tại là: " + currentUrl);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Lỗi khi chạy test: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }
        public static void Testcase3()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                // 1️⃣ Mở trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:44352/NguoiDungs/DangNhap");
                driver.Manage().Window.Maximize();
                Console.WriteLine("✅ Đã mở trang đăng nhập");
                Thread.Sleep(1000);

                // 2️⃣ Nhập tài khoản quản trị viên
                driver.FindElement(By.Id("TenDangNhap")).SendKeys("user1");  // đổi ID nếu khác
                driver.FindElement(By.Id("MatKhau")).SendKeys("123456");     // đổi ID nếu khác

                // 3️⃣ Nhấn nút đăng nhập
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Debug.WriteLine("✅ Đã nhấn đăng nhập");
                Thread.Sleep(1000);

                // 4️⃣ Điều hướng đến trang Quản lý ngân sách
                driver.FindElement(By.LinkText("Quản lý Ngân sách")).Click();
                Debug.WriteLine("✅ Đã mở trang Quản lý ngân sách");
                Thread.Sleep(2000);

                // 5️⃣ Chờ cho đến khi nút “+Tạo ngân sách” xuất hiện
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement addButton = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(
                        By.CssSelector("button[data-bs-target='#addTransactionModal']")
                    )
                );
                addButton.Click();

                Debug.WriteLine("✅ Đã nhấn nút + Tạo ngân sách");

                // 6️⃣ Chờ modal hiển thị
                WebDriverWait wait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(By.Id("addDanhMuc")).Displayed);

                // 7️⃣ Nhập thông tin ngân sách mới trong modal
                var danhMucSelect = new SelectElement(wait.Until(d => d.FindElement(By.Id("addDanhMuc"))));
                danhMucSelect.SelectByText("Mua sắm");  // hoặc SelectByValue("1")

                driver.FindElement(By.Id("addAmount")).SendKeys("5000000");
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('addDate').value = arguments[0];", DateTime.Now.ToString("yyyy-MM-dd"));

                Debug.WriteLine("✅ Đã nhập thông tin ngân sách mới");

                // Chờ modal hiển thị và nút Lưu xuất hiện
             
                var saveBtn = wait.Until(d => d.FindElement(By.Id("addSaveBtn")));

                // Nhấn nút "Lưu"
                saveBtn.Click();

                Debug.WriteLine("✅ Đã nhấn nút Lưu trong modal");

               

                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Chờ alert xuất hiện
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());

                // Lấy nội dung alert (nếu muốn)
                string alertText = alert.Text;
                Debug.WriteLine("📢 Thông báo: " + alertText);

                // Nhấn OK để đóng
                alert.Accept();

                // Sau đó chờ điều hướng về Index
                wait.Until(d => d.Url.Contains("/NganSach/Index"));
                Debug.WriteLine("✅ Đã điều hướng về trang Index sau khi thêm ngân sách");





                //98️⃣ Kiểm tra xem ngân sách mới đã xuất hiện trong danh sách chưa
                var danhSachNganSach = driver.PageSource;
                if (danhSachNganSach.Contains("Mua sắm"))
                {
                    Debug.WriteLine("🎉 TEST PASSED: Ngân sách mới đã được tạo thành công!");
                }
                else
                {
                    Debug.WriteLine("❌ TEST FAILED: Ngân sách mới chưa xuất hiện trong danh sách.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Lỗi khi chạy test: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }
        public static void Testcase4()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                // 1️⃣ Mở trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:44352/NguoiDungs/DangNhap");
                driver.Manage().Window.Maximize();
                Console.WriteLine("✅ Đã mở trang đăng nhập");
                Thread.Sleep(1000);

                // 2️⃣ Nhập tài khoản quản trị viên
                driver.FindElement(By.Id("TenDangNhap")).SendKeys("user1");  // đổi ID nếu khác
                driver.FindElement(By.Id("MatKhau")).SendKeys("123456");     // đổi ID nếu khác

                // 3️⃣ Nhấn nút đăng nhập
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Debug.WriteLine("✅ Đã nhấn đăng nhập");
                Thread.Sleep(1000);

                // 4️⃣ Điều hướng đến trang Quản lý ngân sách
                driver.FindElement(By.LinkText("Quản lý Ngân sách")).Click();
                Debug.WriteLine("✅ Đã mở trang Quản lý ngân sách");
                Thread.Sleep(2000);

                // 5️⃣ Chờ cho đến khi nút "Sửa" xuất hiện
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement editButton = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(
                        By.CssSelector("button.btn.btn-sm.btn-warning.edit-btn")
                    )
                );

                // Nhấn vào nút "Sửa"
                editButton.Click();
                Debug.WriteLine("✅ Đã nhấn nút Sửa (edit)");


                // 6️⃣ Chờ modal hiển thị
                WebDriverWait wait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(By.Id("editName")).Displayed);

                // 7️⃣ Nhập thông tin ngân sách mới trong modal
                var danhMucSelect = new SelectElement(wait.Until(d => d.FindElement(By.Id("editName"))));
                danhMucSelect.SelectByText("Mua sắm");  // hoặc SelectByValue("1")

                var amountInput = driver.FindElement(By.Id("editAmount"));
                amountInput.Clear();  // 🧹 Xóa toàn bộ nội dung cũ
                amountInput.SendKeys("5000000");  // ✍️ Gõ giá trị mới

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('editDate').value = arguments[0];", DateTime.Now.ToString("yyyy-MM-dd"));

                Debug.WriteLine("✅ Đã nhập thông tin ngân sách mới");

                // Chờ modal hiển thị và nút Cập nhật xuất hiện

                var saveBtn = wait.Until(d => d.FindElement(By.Id("editSaveBtn")));

                // Nhấn nút "Lưu"
                saveBtn.Click();

                Debug.WriteLine("✅ Đã nhấn nút Lưu trong modal");



                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Chờ alert xuất hiện
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());

                // Lấy nội dung alert (nếu muốn)
                string alertText = alert.Text;
                Debug.WriteLine("📢 Thông báo: " + alertText);

                // Nhấn OK để đóng
                alert.Accept();

                // Sau đó chờ điều hướng về Index
                wait.Until(d => d.Url.Contains("/NganSach/Index"));
                Debug.WriteLine("✅ Đã điều hướng về trang Index sau khi thêm ngân sách");





                //98️⃣ Kiểm tra xem ngân sách mới đã xuất hiện trong danh sách chưa
                var danhSachNganSach = driver.PageSource;
                if (danhSachNganSach.Contains("Mua sắm"))
                {
                    Debug.WriteLine("🎉 TEST PASSED: Ngân sách mới đã được tạo thành công!");
                }
                else
                {
                    Debug.WriteLine("❌ TEST FAILED: Ngân sách mới chưa xuất hiện trong danh sách.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Lỗi khi chạy test: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }
        public static void Testcase5()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                // 1️⃣ Mở trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:44352/NguoiDungs/DangNhap");
                driver.Manage().Window.Maximize();
                Console.WriteLine("✅ Đã mở trang đăng nhập");
                Thread.Sleep(1000);

                // 2️⃣ Nhập tài khoản quản trị viên
                driver.FindElement(By.Id("TenDangNhap")).SendKeys("user1");  // đổi ID nếu khác
                driver.FindElement(By.Id("MatKhau")).SendKeys("123456");     // đổi ID nếu khác

                // 3️⃣ Nhấn nút đăng nhập
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                Debug.WriteLine("✅ Đã nhấn đăng nhập");
                Thread.Sleep(1000);

                // 4️⃣ Điều hướng đến trang Quản lý ngân sách
                driver.FindElement(By.LinkText("Quản lý Ngân sách")).Click();
                Debug.WriteLine("✅ Đã mở trang Quản lý ngân sách");
                Thread.Sleep(2000);

                // 5️⃣ Chờ cho đến khi nút "Xoá" xuất hiện
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement deleteButton = wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(
                        By.CssSelector("button.btn.btn-sm.btn-danger.delete-btn")
                    )
                );

                // Nhấn vào nút "Xoá"
                deleteButton.Click();
                Debug.WriteLine("✅ Đã nhấn nút Xoá (delete)");


               

                // Chờ modal hiển thị và nút Cập nhật xuất hiện

                var saveBtn = wait.Until(d => d.FindElement(By.Id("deleteConfirmBtn")));

                // Nhấn nút "Lưu"
                saveBtn.Click();

                Debug.WriteLine("✅ Đã nhấn nút Lưu trong modal");



                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Chờ alert xuất hiện
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());

                // Lấy nội dung alert (nếu muốn)
                string alertText = alert.Text;
                Debug.WriteLine("📢 Thông báo: " + alertText);

                // Nhấn OK để đóng
                alert.Accept();

                // Sau đó chờ điều hướng về Index
                wait.Until(d => d.Url.Contains("/NganSach/Index"));
                Debug.WriteLine("✅ Đã điều hướng về trang Index sau khi thêm ngân sách");





                //98️⃣ Kiểm tra xem ngân sách mới đã xuất hiện trong danh sách chưa
                var danhSachNganSach = driver.PageSource;
                if (danhSachNganSach.Contains("Mua sắm"))
                {
                    Debug.WriteLine("🎉 TEST PASSED: Ngân sách mới đã được tạo thành công!");
                }
                else
                {
                    Debug.WriteLine("❌ TEST FAILED: Ngân sách mới chưa xuất hiện trong danh sách.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Lỗi khi chạy test: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}

