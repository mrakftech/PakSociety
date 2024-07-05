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
    public class Salary_With_AllowencesController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Salary_With_Allowences
        public ActionResult Manage_Salary()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Salary");
                ViewBag.Key = s.Target;
            }
            var a = dbm.Salary_With_Allowences.ToList();
            return View(a);
        }
        public ActionResult AddNew_Salary()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Salary");
                ViewBag.Key = s.Target;
            }

            var ss = dbm.Employees.ToList();
            return View(ss);
        }
        [HttpPost]
        public JsonResult AddNew_Salary(Salary_With_Allowences model)
        {
            string a;
            var check = dbm.Salary_With_Allowences.FirstOrDefault(o => o.Emp_ID == model.Emp_ID);
            if (check == null)
            {
                Salary_With_Allowences s = new Salary_With_Allowences();

                s = model;

                dbm.Salary_With_Allowences.Add(s);
                dbm.SaveChanges();



                a = "true";
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            else
            {
                a = "false";
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Update_Salary(int ID = 0)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Salary");
                ViewBag.Key = s.Target;
            }

            var p = dbm.Salary_With_Allowences.FirstOrDefault(x => x.Salary_ID == ID);

            return View(p);

        }

        [HttpPost]
        public ActionResult Update_Salary(Salary_With_Allowences model)
        {

            var s = dbm.Salary_With_Allowences.FirstOrDefault(o => o.Salary_ID == model.Salary_ID);
            s.Convance_Allownce = model.Convance_Allownce;
            s.Basic_Salary = model.Basic_Salary;
            s.House_Rent_Allowence = model.House_Rent_Allowence;
            s.Rate_Per_Hour = model.Rate_Per_Hour;
            s.Medical_Allowence = model.Medical_Allowence;
            s.Gross_Salary = model.Gross_Salary;


            dbm.Entry(s).State = EntityState.Modified;
            dbm.SaveChanges();
            //Update code

            return RedirectToAction("Manage_Salary", "Salary_With_Allowences");
        }
        public ActionResult Delete(int id)
        {
            var x = dbm.Salary_With_Allowences.FirstOrDefault(o => o.Salary_ID == id);
            if (x != null)
            {
                dbm.Salary_With_Allowences.Remove(x);
                dbm.SaveChanges();
            }
            return RedirectToAction("Manage_Salary", "Salary_With_Allowences");
        }
    }
}