using ClosedXML.Excel;
using CMS.Ecommerce;
using CMS.Membership;
using PrintForMe.Models.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers.Admin
{
    public class ReportingController : Controller
    {
        public ActionResult DownloadReport()
        {
            return View();
        }

        [HttpPost]
        public FileContentResult DownloadReport(ReportModel model)
        {
            if (model.ReportType.ToLower() == "users")
            {
                return UsersReport(model.TimeFrame, model.ReportType);
            }
            if (model.ReportType.ToLower() == "orders")
            {
                return OrdersReport(model.TimeFrame, model.ReportType);
            }
            return UsersReport("Today", "Users");
        }

        public FileContentResult UsersReport(string duration, string type)
        {

            //var rDriver = UserRoleInfoProvider.GetUserRoles().Column("UserID").WhereEquals("RoleID", 4);
            //var rPremiumDriver = UserRoleInfoProvider.GetUserRoles().Column("UserID").WhereEquals("RoleID", 10);
            string[] role = { "5" };
            var uRole = UserRoleInfoProvider.GetUserRoles().Column("UserID").WhereNotIn("RoleID", role);
            List<PieChartData> chartData = new List<PieChartData>();
            if (duration == "Daily")
            {//
                //
                var tusers = UserInfoProvider.GetUsers().WhereNotIn("UserID", uRole).Where(x => x.UserCreated.ToString("MMddyyyy") == DateTime.Now.ToString("MMddyyyy")).Count();
                chartData = new List<PieChartData>()
                    {
                        new PieChartData(){ Date=DateTime.Now,Data=tusers}
                    };
                return CreateExcel("Users" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Users");
            }

            if (duration == "Weekly")
            {
                var currentWeek = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(i - 7));
                //if (type == "Users")
                //{
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tusers = UserInfoProvider.GetUsers().WhereNotIn("UserID", uRole).Where(x => x.UserCreated.ToString("MMddyyyy") == obj.ToString("MMddyyyy")).Count();//.WhereNotIn("UserID", uRole)
                    chartData.Add(new PieChartData() { Date = obj, Data = tusers });
                }
                return CreateExcel("Users" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Users");
                // }
            }

            if (duration == "Monthly")
            {
                var currentWeek = Enumerable.Range(0, 30).Select(i => DateTime.Now.AddDays(i - 30)).OrderBy(x => x.Date);
                //if (type == "Users")
                //{
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tusers = UserInfoProvider.GetUsers().WhereNotIn("UserID", uRole).Where(x => x.UserCreated.ToString("MMddyyyy") == obj.ToString("MMddyyyy")).Count();//.WhereNotIn("UserID", uRole)
                    chartData.Add(new PieChartData() { Date = obj, Data = tusers });
                }
                return CreateExcel("Users" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Users");
                //}
            }

            if (duration == "Custom")
            {
                var currentWeek = Enumerable.Range(0, 12).Select(i => DateTime.Now.AddMonths(i - 12)).OrderBy(x => x.Date);

                //if (type == "Users")
                // {
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tusers = UserInfoProvider.GetUsers().WhereNotIn("UserID", uRole).Where(x => x.UserCreated.Month == obj.Month && x.UserCreated.Year == obj.Year).Count();//.WhereNotIn("UserID", uRole)
                    chartData.Add(new PieChartData() { Date = obj, Data = tusers });
                }
                return CreateExcel("Users" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Users");
                //}
            }

            return CreateExcel(DateTime.Now.ToString("ddMMyyyy"), new List<PieChartData>(), "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="type"></param>
        /// <returns></returns>


        public FileContentResult OrdersReport(string duration, string type)
        {
            List<PieChartData> chartData = new List<PieChartData>();
            if (duration == "Daily")
            {
                var tpaidOrders = OrderInfoProvider.GetOrders().Where(x => x.OrderDate.ToString("MMddyyyy") == DateTime.Now.ToString("MMddyyyy")).Count();
                chartData = new List<PieChartData>()
                    {
                        new PieChartData(){ Date=DateTime.Now,Data = tpaidOrders}
                    };
                return CreateExcel("Orders" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Orders");

            }

            if (duration == "Weekly")
            {
                var currentWeek = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(i - 7));
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tpaidOrders = OrderInfoProvider.GetOrders().Where(x => x.OrderDate.ToString("MMddyyyy") == obj.ToString("MMddyyyy")).Count();
                    chartData.Add(new PieChartData() { Date = obj, Data = tpaidOrders });
                }
                return CreateExcel("Orders" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Orders");
            }

            if (duration == "Monthly")
            {
                var currentWeek = Enumerable.Range(0, 30).Select(i => DateTime.Now.AddDays(i - 30)).OrderBy(x => x.Date);
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tpaidOrders = OrderInfoProvider.GetOrders().Where(x => x.OrderDate.ToString("MMddyyyy") == obj.ToString("MMddyyyy")).Count();
                    chartData.Add(new PieChartData() { Date = obj, Data = tpaidOrders });
                }

                return CreateExcel("Orders" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Orders");
            }

            if (duration == "Custom")
            {
                var currentWeek = Enumerable.Range(0, 12).Select(i => DateTime.Now.AddMonths(i - 12)).OrderBy(x => x.Date);
                chartData = new List<PieChartData>();
                foreach (DateTime obj in currentWeek)
                {
                    var tpaidOrders = OrderInfoProvider.GetOrders().Where(x => x.OrderDate.Month == obj.Month && x.OrderDate.Year == obj.Year).Count();
                    chartData.Add(new PieChartData() { Date = new DateTime(obj.Year, obj.Month, DateTime.DaysInMonth(obj.Year, obj.Month)), Data = tpaidOrders });
                }

                return CreateExcel("Orders" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Orders");
            }

            return CreateExcel("Orders" + DateTime.Now.ToString("ddMMyyyy"), chartData, "Orders");
        }

        private FileContentResult CreateExcel(string fileName, List<PieChartData> data, string recordType)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(recordType);
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Date";
                worksheet.Cell(currentRow, 2).Value = recordType;
                //worksheet.Cell(currentRow, 3).Value = otherCol;
                foreach (var objRecord in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = objRecord.Date.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 2).Value = objRecord.Data;
                    //worksheet.Cell(currentRow, 3).Value = objRecord.OtherCol;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
                }
            }
        }

        public class PDataSource
        {
            public string xValue;
            public double yValue;
            public string text;
        }

        public class PieChartData
        {
            public DateTime Date;
            public int Data;
            public string OtherCol;
            public string Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public class LineChartData
        {
            public DateTime xValue;
            public int Users;
            public int Orders;
            public decimal Income;
        }
    }
}
