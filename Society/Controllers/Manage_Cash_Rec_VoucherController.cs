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

namespace Society.Controllers
{
    public class Manage_Cash_Rec_VoucherController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();

        public ActionResult result(string search1)
        {

            var trips = dbm.reciept_vouchers.Where(o => (o.Manual_Voucher_No.StartsWith(search1) && o.Type == "CRV")).ToList().Take(300).Select(o => new { o.ChequeDate, o.ChequeNumber, o.Comments, o.Drown_bank, o.IsApproved, o.Manual_Voucher_No, o.RV_ID, o.Type, o.VoucherDate, o.VoucherNum, o.reciept_voucher_payment.FirstOrDefault(k => k.RV_ID == o.RV_ID).Amount });
            var json = JsonConvert.SerializeObject(trips);
            return Json(json, JsonRequestBehavior.AllowGet);







        }
        // GET: Manage_Cash_Rec_Voucher
        public ActionResult Manage_Cash_Rec_Voucher()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Voucher");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }



            dynamic dy = new ExpandoObject();
            dy.Pending_Amount = dbm.reciept_vouchers.Where(o => o.Type == "CRV").OrderBy(o => o.IsApproved == 1).ToList().Take(200).Reverse();

            return View(dy);


        }
    }
}