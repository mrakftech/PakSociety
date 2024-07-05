using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using System.Data.Entity;

namespace Society.Controllers
{
    public class Payment_PlanController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Payment_Plan
        public ActionResult Manage_Plan()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Payment Plan");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Installment_Plan.ToList().OrderBy(o=>o.Scheme);
            return View(a);
        }
        public ActionResult Add_Plan()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Payment Plan");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Add_Plan(Installment_Plan model)
        {            
            dbm.Installment_Plan.Add(model);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Plan", "Payment_Plan");
        }
        public ActionResult ShowMember(int id)
        {

            var data = dbm.Installment_Plan.Where(O => O.ID == id).SelectMany(o => o.Plots).ToList();
            return View(data);
        }
        public ActionResult Delete(int id)
        {
            var a = dbm.Installment_Plan.FirstOrDefault(o=>o.ID==id);
            dbm.Installment_Plan.Remove(a);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Plan", "Payment_Plan");
        }
    }
}