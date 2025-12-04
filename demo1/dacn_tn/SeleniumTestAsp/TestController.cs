using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dacn.SeleniumTestAsp
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult RunSeleniumTest()
        {
            Test1.RunTest();
            return Content("✅ Selenium test đã chạy xong! (Kiểm tra console để xem log chi tiết)");
        }
        public ActionResult RunSeleniumTest3()
        {
            Test1.Testcase3();
            return Content("✅ Selenium test đã chạy xong! (Kiểm tra console để xem log chi tiết)");
        }
        public ActionResult RunSeleniumTest4()
        {
            Test1.Testcase4();
            return Content("✅ Selenium test đã chạy xong! (Kiểm tra console để xem log chi tiết)");
        }
        public ActionResult RunSeleniumTest5()
        {
            Test1.Testcase5();
            return Content("✅ Selenium test đã chạy xong! (Kiểm tra console để xem log chi tiết)");
        }
    }
}