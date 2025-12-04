using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dacn.Models
{
    public class ChartDataViewModel
    {
        public string NgayGD { get; set; }  // Hoặc DateTime, tùy xaxis
        public decimal Thu { get; set; }
        public decimal Chi { get; set; }
    }
}