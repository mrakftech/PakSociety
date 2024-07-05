using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
namespace Society.Controllers
{
    public class Ledger_AccountController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Ledger_Account
        public ActionResult Add_Ledger_Account()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Add Accounts");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();
            dy.Element_List = dbm.Element_Account.ToList();
            dy.Control_List = dbm.Control_Account.ToList();
            dy.Ledger_List = dbm.Ledger_Account.ToList();
            return View(dy);
        }
        public JsonResult result(FormCollection form)
        {
            var element_account_code = Convert.ToInt32(form["EleAcc"]);



            var xx = dbm.Control_Account.Where(x => x.Element_Account_Code == element_account_code).Select(x => new
            {
                title = x.Control_Account_Title,
                code = x.Control_Account_Code,
               element_id=x.Element_ID,
                control_id=x.ID
            }).ToList();
            return Json(xx, JsonRequestBehavior.AllowGet);

        }
        public JsonResult result2(FormCollection f)
        {
            var elementaccount = Convert.ToInt32(f["EleAcc"]);
            var controlaccount = Convert.ToInt32(f["ConAcc"]);
            var cc = Convert.ToInt32(dbm.Ledger_Account.Where(x => x.Control_Account_Code == controlaccount && x.Element_Account_Code == elementaccount).Max(x => x.Ledger_Account_Code));


            return Json(cc, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Add_Ledger_Account(FormCollection f)
        {
            Ledger_Account l = new Ledger_Account();
            l.Ledger_Complete_Code = f["txtAccountLedCode"];
            l.Ledger_Account_Title = f["txtAccountLedTitle"];
            l.Ledger_Account_Code = Convert.ToInt32(f["txtAccountLedCodeHidd"]);
            l.Element_Account_Title = f["txtelementaccounttitle"];
            l.Control_Account_Title = f["txtcontrolaccounttitle"];
            l.Sub_Ledger_Account_Code = f["txtAccountLedCode"] + ".1";
            var subledger_title = f["txtAccountSubLedTitle"];
            if (subledger_title == "")
            {
                l.Sub_Ledger_Account_Title = f["txtAccountLedTitle"];
            }
            else
            {
                l.Sub_Ledger_Account_Title = f["txtAccountSubLedTitle"];
            }

            l.Element_Account_Code = Convert.ToInt32(f["txtAccountCode"]);
            l.Control_Account_Code = Convert.ToInt32(f["txtConAccountCode"]);
            l.Element_ID= Convert.ToInt32(f["elementid"]);
            l.Control_ID= Convert.ToInt32(f["controlid"]);
            l.Balance = 0;
            
            dbm.Ledger_Account.Add(l);
            dbm.SaveChanges();
            return RedirectToAction("Add_Ledger_Account", "Ledger_Account");
        }
        public JsonResult Update_Ledger_Account(FormCollection form)
        {
            var id = Convert.ToInt32(form["id"]);
            var title = form["title"];
            var a = dbm.Ledger_Account.FirstOrDefault(o => o.ID == id);
            a.Ledger_Account_Title = title;

            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);


        }
    }
}