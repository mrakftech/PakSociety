using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Globalization;

namespace Society.Controllers
{
    public class DashboardController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        public static List<int> GetDatesOneMonthAgoShort()
        {
            lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();

            DateTime today = DateTime.Now;
            List<int> result = new List<int>();
            DateTime currentDate = new DateTime(today.Year, today.Month, today.Day);

            // Go back one month from today's date
            currentDate = currentDate.AddMonths(-1);


            // Iterate from today's date to the date one month ago and add each date to the result list
            while (currentDate <= today)
            {

                result.Add(dbm.Members.Count(o=> o.createdDate.Value.Year == currentDate.Year && o.createdDate.Value.Month == currentDate.Month && o.createdDate.Value.Day == currentDate.Day));
                currentDate = currentDate.AddDays(1); // Move to the next day
            }

            return result;
        }

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();
            dy.member = dbm.Members.Count();
            var oneMonthLater = DateTime.Now.AddMonths(-1);
            dy.paid = dbm.Installments.Where(o=>o.Paid_Month.Value.Month==DateTime.Today.Month && o.Paid_Month.Value.Year==DateTime.Today.Year && o.IsPaid==true).Count();
            dy.received = dbm.Installments.Where(o=>o.Paid_Month.Value.Month==DateTime.Today.Month && o.Paid_Month.Value.Year==DateTime.Today.Year).Sum(o=>o.Monthly_Paid);
            dy.not_paid = dbm.Installments.Where(o => o.Payment_Month.Month == DateTime.Today.Month && o.Payment_Month.Year == DateTime.Today.Year && o.IsPaid == false).Count();
            dy.defaulter = dbm.Installments.Where(o => o.Payment_Month <= DateTime.Today).Sum(o => o.Amount_Paid)-dbm.Installments.Where(k=>k.Payment_Month<=DateTime.Today).Sum(k=>k.Monthly_Paid);
            dy.toBeReceived = dbm.Installments.Where(o => o.Payment_Month <= DateTime.Today && o.Payment_Month >= oneMonthLater).Sum(o => o.Amount_Paid)-dbm.Installments.Where(k=>k.Payment_Month<=DateTime.Today && k.Payment_Month >= oneMonthLater).Sum(k=>k.Monthly_Paid);
            dy.plotSales = dbm.Members.Count();
            dy.plotSalesPer = (Convert.ToDecimal( dbm.Members.Count(o => o.createdDate <= DateTime.Now && o.createdDate >= oneMonthLater)) * 100 ) / 300;
            dy.fileTransfer = dbm.File_Transfer.Count(o=>o.Date<=DateTime.Now && o.Date>=oneMonthLater);
            dy.fileTransferPer = (Convert.ToDecimal( dbm.File_Transfer.Count(o=>o.Date<=DateTime.Now && o.Date>=oneMonthLater)) * 100) / 300;
            dy.paymentTransfer = dbm.Payment_Transfer.Count(o=>o.Date<=DateTime.Now && o.Date>=oneMonthLater);
            dy.paymentTransferPer = Convert.ToDecimal(dbm.Payment_Transfer.Count(o => o.Date <= DateTime.Now && o.Date >= oneMonthLater)) * 100 / 300;
            dy.refund = dbm.Refunds.Count(o=>o.Date<=DateTime.Now && o.Date>=oneMonthLater);
            dy.refundPer =(Convert.ToDecimal( dbm.Refunds.Count(o=>o.Date<=DateTime.Now && o.Date>=oneMonthLater)) *100) / 300;
            var refund = dbm.Refunds.Where(o => o.Date <= DateTime.Now && o.Date >= oneMonthLater);
            if (refund.Any())
            {
                dy.refundAmount = refund.Sum(o=>o.Amount);
            }
            else
            {
                dy.refundAmount = 0;
            }
            dy.monthlySales = dbm.Payment_Plan.Where(o=>o.Plot.Member.createdDate<=DateTime.Now && o.Plot.Member.createdDate>=oneMonthLater).Sum(c=>c.Net_Price_Plot);
            dy.chartdata = GetDatesOneMonthAgoShort();
                        
            return View(dy);
        }
    }
}