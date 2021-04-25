using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrintForMe.Models.Report
{
    public class ReportModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string TimeFrame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDate { get; set; } = DateTime.Now;

        public string ReportType { get; set; }
    }
}