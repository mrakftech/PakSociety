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
    public class AdvanceController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Advance
        public ActionResult Manage_Advance()
        {
            dynamic dy = new ExpandoObject();

            dy.employee_list = dbm.Employees.ToList();
            dy.advance_list = dbm.Advances.ToList();
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Advance");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View(dy);
        }
        [HttpPost]
        public ActionResult Manage_Advance(Advance model)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Advance");
                ViewBag.Key = s.Target;
            }
            var check = dbm.Advances.FirstOrDefault(o => o.Emp_ID == model.Emp_ID && o.Month == DateTime.Today.Month && o.Year == DateTime.Today.Year);
            if (check == null)
            {
                Advance a = new Advance();
                a.Advance_Amount = model.Advance_Amount;
                a.Advance_Period = DateTime.Today;
                a.Emp_ID = model.Emp_ID;
                a.Month = DateTime.Today.Month + 1;
                a.Year = DateTime.Today.Year;
                a.Status = "Un Paid";
                dbm.Advances.Add(a);
                dbm.SaveChanges();



                return RedirectToAction("Manage_Advance", "Advance");
            }
            else
            {
                ViewBag.Message = "record already exist";
                return RedirectToAction("Manage_Advance", "Advance", ViewBag.Message);
            }


        }
        public ActionResult Delete(int id)
        {
            var x = dbm.Advances.FirstOrDefault(o => o.Advance_ID == id);
            if (x != null)
            {
                dbm.Advances.Remove(x);
                dbm.SaveChanges();
            }
            return RedirectToAction("Manage_Advance", "Advance");
        }
    }
}