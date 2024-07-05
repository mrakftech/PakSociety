using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using Society.Data;

namespace Society.Controllers
{
    public class Element_AccountController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Element_Account
        public ActionResult Add_Element_Account()
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
            Element_Account model = new Element_Account();
            int count1 = dbm.Element_Account.Count();
            if (count1 > 0)
            {
                int id = (int)dbm.Element_Account.Max(t => t.Element_Account_Code);

                model = dbm.Element_Account.FirstOrDefault(x => x.Element_Account_Code == id);
                dy.Max_Code = model.Element_Account_Code;
            }
            else
            {
                dy.Max_Code = 0;
            }



            return View(dy);
        }
        public JsonResult Add_Element_Account_Json(FormCollection form)
        {
            var accountcode = Convert.ToInt32(form["accountcode"]);
            var accounttitle = form["accounttitle"];
            Element_Account e = new Element_Account();
            e.Element_Account_Code = accountcode;
            e.Account_Title = accounttitle;
            dbm.Element_Account.Add(e);
            if (dbm.SaveChanges() > 0)
            {
                var a = dbm.Element_Account.Select(o=> new { ID=o.ID, Element_Account_Code=o.Element_Account_Code, Account_Title=o.Account_Title}).ToList();
                var json = JsonConvert.SerializeObject(a);

                return Json(json, JsonRequestBehavior.AllowGet);

            }
            return Json(JsonRequestBehavior.AllowGet);


        }
        public JsonResult Update_Element_Account(FormCollection form)
        {
            var id = Convert.ToInt32(form["id"]);
            var title = form["title"];
            var a = dbm.Element_Account.FirstOrDefault(o=>o.ID==id);
            a.Account_Title = title;

            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            return Json(true,JsonRequestBehavior.AllowGet);


        }
    }
}