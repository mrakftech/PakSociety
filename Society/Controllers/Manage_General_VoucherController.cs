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
    public class Manage_General_VoucherController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Manage_General_Voucher
        public ActionResult Manage_General_Voucher()
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
            dy.Pending_Amount = dbm.reciept_vouchers.Where(o => o.Type == "JV").ToList();

            return View(dy);


        }
    }
}