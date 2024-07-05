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
    public class Account_SettingController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Advance
        // GET: Account_Setting

       
        public ActionResult Setting()
        {
            dynamic dy = new ExpandoObject();

            dy.element = dbm.Element_Account.ToList();
            dy.control = dbm.Control_Account.ToList();
            dy.registration_setting = dbm.Account_Setting.FirstOrDefault(o=>o.Setting_Name=="Registration");
            dy.bank_setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Bank");
            dy.cash_setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Cash");

            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Setting");
                ViewBag.Key = s.Target;
                if (s.Target == false) { 
                return RedirectToAction("UnAuthorized", "Login");
                }

            }
            return View(dy);
        }
        public JsonResult Element(FormCollection form) {
            var elementid =Convert.ToInt32( form["elementid"]);
      


            var settingname = form["settingname"];
            var update = dbm.Account_Setting.FirstOrDefault(o=> o.Setting_Name==settingname);
            update.Element_ID = elementid;
            dbm.Entry(update).State = EntityState.Modified;
            dbm.SaveChanges();

            return Json( JsonRequestBehavior.AllowGet);
        }
        public JsonResult Control(FormCollection form) {
            var controlid = Convert.ToInt32(form["controlid"]);
            var settingname = form["settingname"];
            var update = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == settingname);
            update.Control_ID = controlid;
            dbm.Entry(update).State = EntityState.Modified;
            dbm.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}