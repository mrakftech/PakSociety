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
    public class Control_AccountController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Control_Account
        public ActionResult Add_Control_Account()
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

            return View(dy);
        }
        [HttpPost]
        public ActionResult Add_Control_Account(FormCollection form)
        {
            Control_Account c = new Control_Account();

            var ecode = Convert.ToInt32(form["txtAccountCode"]);
            string control_title = form["txtAccountConTitle"];
            string element_account_title = form["elementaccounttitle"];
            var code = Convert.ToInt32(form["txtAccountConCodeHidd"]);
            string complete_code = form["txtAccountConCode"];
            c.Element_Account_Code = ecode;
            c.Control_Account_Title = control_title;
            c.Control_Account_Code = code;
            c.Control_Complete_Code = complete_code;
            c.Element_Account_Title = element_account_title;
            c.Element_ID= Convert.ToInt32(form["elementid"]);
            dbm.Control_Account.Add(c);
            dbm.SaveChanges();
            return RedirectToAction("Add_Control_Account", "Control_Account");
        }
        public JsonResult Add_Control_Account_Json(FormCollection form)
        {
            var element_account_code = Convert.ToInt32(form["EleAcc"]);
            var cc = dbm.Control_Account.Where(x => x.Element_Account_Code == element_account_code).Max(x => x.Control_Account_Code);
            if (cc == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(cc, JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult Update_Control_Account(FormCollection form)
        {
            var id = Convert.ToInt32(form["id"]);
            var title = form["title"];
            var a = dbm.Control_Account.FirstOrDefault(o => o.ID == id);
            a.Control_Account_Title = title;

            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);


        }
    }
}