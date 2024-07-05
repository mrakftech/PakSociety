using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Linq.Expressions;

namespace Society.Controllers
{
    public class Daily_PaymentsController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Daily_Payments
        public ActionResult Report()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Report(Nullable<DateTime> recStartDateFrontend, Nullable<DateTime> createdStartDateFrontend, Nullable<DateTime> createdEndDateFrontend, Nullable<DateTime> recEndDateFrontend)
        {
            // ViewBag.allotments = dbm.Payment_Plan.AsQueryable().WhereIf(recStartDateFrontend != null, o => o.Plot.Date >= recStartDateFrontend && o.Plot.Date <= createdEndDateFrontend).WhereIf(createdStartDateFrontend != null, o => o.Plot.Member.createdDate >= createdStartDateFrontend && o.Plot.Member.createdDate <= createdEndDateFrontend).ToList();
            // ViewBag.allotments = dbm.Payment_Plan.Where( o => o.Plot.Date >= recStartDateFrontend && o.Plot.Date <= createdEndDateFrontend)edDate <= createdEndDateFrontend).ToList();
            ViewBag.allotments = dbm.Payment_Plan
     .AsQueryable()
     .Where(o => (recStartDateFrontend != null || (o.Plot.Date >= recStartDateFrontend && o.Plot.Date <= createdEndDateFrontend))).ToList();

            //ViewBag.adjustpayment = dbm.Payment_Transfer.AsQueryable().WhereIf(recStartDateFrontend != null, o => o.Date >= recStartDateFrontend && o.Date <= recEndDateFrontend).WhereIf(createdStartDateFrontend != null,o =>o.createdDate >=createdStartDateFrontend && o.createdDate <= createdEndDateFrontend).ToList();
            //  ViewBag.adjustpayment = dbm.Payment_Transfer.AsQueryable().Where(o=> ( recStartDateFrontend != null, o => o.Date >= recStartDateFrontend && o.Date <= recEndDateFrontend)).;

            ViewBag.adjustpayment = dbm.Payment_Transfer.AsQueryable().Where(o => (recEndDateFrontend != null || (o.Date >= recStartDateFrontend && o.Date <= recEndDateFrontend))).ToList();


            //ViewBag.Cash_rec = dbm.reciept_vouchers.WhereIf(recStartDateFrontend != null, o => o.VoucherDate >= recStartDateFrontend && o.VoucherDate <= recStartDateFrontend && o.Type == "CRV" && o.IsApproved == 1).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <= createdEndDateFrontend && o.Type == "CRV" && o.IsApproved == 1).ToList();

            ViewBag.Cash_rec = dbm.reciept_vouchers.Where(x => x.VoucherDate >= recStartDateFrontend && x.VoucherDate <= recEndDateFrontend && x.Type == "CRV" && x.IsApproved == 1).ToList();
            //ViewBag.Bank_rec = dbm.reciept_vouchers.WhereIf(recStartDateFrontend != null, o => o.VoucherDate >= recStartDateFrontend &&  o.VoucherDate <= recStartDateFrontend && (o.Type == "BRV" || o.Type == "ONLINETRANSFER" || o.Type == "CASHATBANK") && o.IsApproved == 1).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <=createdEndDateFrontend && o.Type == "BRV" && o.IsApproved == 1).ToList();
            ViewBag.Bank_rec = dbm.reciept_vouchers.Where(x => x.VoucherDate >= recStartDateFrontend && x.VoucherDate <= recEndDateFrontend && x.Type == "BRV" || x.Type == "ONLINETRANSFER" || x.Type == "CASHATBANK" && x.IsApproved == 1).ToList();


           // ViewBag.Cash_Pay = dbm.reciept_vouchers.WhereIf(recStartDateFrontend != null, o => o.VoucherDate >= recStartDateFrontend && o.VoucherDate <= recStartDateFrontend && o.Type == "CPV" && o.IsApproved == 1).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <= createdEndDateFrontend && o.Type == "CPV" && o.IsApproved == 1).ToList();

            ViewBag.Cash_Pay = dbm.reciept_vouchers.Where(x => x.VoucherDate >= recStartDateFrontend && x.VoucherDate <= recEndDateFrontend && x.Type == "CPV"  && x.IsApproved == 1).ToList();

            // ViewBag.Bank_pay = dbm.reciept_vouchers.WhereIf(recStartDateFrontend != null, o => o.VoucherDate >= recStartDateFrontend && o.VoucherDate <= recStartDateFrontend && o.Type == "BPV" && o.IsApproved == 1).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <= createdEndDateFrontend && o.Type == "BPV" && o.IsApproved == 1).ToList();

            ViewBag.Bank_pay = dbm.reciept_vouchers.Where(x => x.VoucherDate >= recStartDateFrontend && x.VoucherDate <= recEndDateFrontend && x.Type == "BPV" && x.IsApproved == 1).ToList();

            // ViewBag.adjustment = dbm.reciept_vouchers.WhereIf(recStartDateFrontend != null, o => o.VoucherDate >= recStartDateFrontend && o.VoucherDate <= recStartDateFrontend && o.Type == "ADJ" && o.IsApproved == 1).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <= createdEndDateFrontend && o.Type == "ADJ" && o.IsApproved == 1).ToList();
            ViewBag.Bank_pay = dbm.reciept_vouchers.Where(x => x.VoucherDate >= recStartDateFrontend && x.VoucherDate <= recEndDateFrontend && x.Type == "ADJ" && x.IsApproved == 1).ToList();

            // ViewBag.Transfer_file = dbm.File_Transfer.WhereIf(recStartDateFrontend != null, o => o.Date >= recStartDateFrontend && o.Date <= recEndDateFrontend).WhereIf(createdStartDateFrontend != null, o => o.createdDate >= createdStartDateFrontend && o.createdDate <= createdEndDateFrontend).ToList();
            ViewBag.Transfer_file = dbm.File_Transfer.AsQueryable().Where(o => (recEndDateFrontend != null || (o.Date >= recStartDateFrontend && o.Date <= recEndDateFrontend))).ToList();

            TempData["Message"] = recStartDateFrontend;
            return View();
        }


    }
}
public static class test
{
    public static IQueryable<TSource> WhereIf<TSource>(
this IQueryable<TSource> source,
bool condition,
Expression<Func<TSource, bool>> predicate)
    {
        if (condition)
            return source.Where(predicate);
        else
            return source;
    }

}