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
    public class EmployeeController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Employee
        public ActionResult Manage_Employee()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Employee");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Employees.ToList();
            return View(a);
        }
       
        public ActionResult Add_Employee()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Employee");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            ViewBag.roles = dbm.roles.ToList();
            var ss = dbm.Employees.FirstOrDefault(x => x.Employee_ID == 0);
            return View(ss);
        }
        [HttpPost]
        public ActionResult Add_Employee(FormCollection form, HttpPostedFileBase Profile_Picture)
        {

            Employee e = new Employee();
            e.F_Name = form["F_Name"];
            e.L_Name = form["L_Name"];
            e.Address = form["Address"];
            e.CNIC = form["CNIC"];
            e.Password = form["Password"];
            
            e.Designation = form["Designation"];
            e.DOB = Convert.ToDateTime(form["DOB"]);
            e.Email = form["Email"];
            e.Gender = form["gender"];
            e.Joining_Date = Convert.ToDateTime(form["Joining_Date"]);
            e.Martial_Status = form["Martial_Status"];
            e.Phone_Number = form["Phone_Number"];
            e.Role_ID =Convert.ToInt32( form["Roles_ID"]);
            e.Status = Convert.ToBoolean(form["Status"]);
            e.Type = form["Type"];
            if (Profile_Picture!=null)
            {
                e.Profile_Picture = new byte[Profile_Picture.ContentLength];
                Profile_Picture.InputStream.Read(e.Profile_Picture, 0, Profile_Picture.ContentLength);
            }
         
            dbm.Employees.Add(e);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Employee", "Employee");

        }
        public ActionResult Update_Employee(int ID = 0)
        {


            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Employee");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            ViewBag.roles = dbm.roles.ToList();
            var ss = dbm.Employees.FirstOrDefault(x => x.Employee_ID == ID);
            return View(ss);

        }

        [HttpPost]
        public ActionResult Update_Employee(Employee model, HttpPostedFileBase Profile_Picturee)
        {
            if (model.Employee_ID > 0) { }
            var s = dbm.Employees.FirstOrDefault(o => o.Employee_ID == model.Employee_ID);
            s.Address = model.Address;
            s.Designation = model.CNIC;
            s.DOB = model.DOB;
            s.Email = model.Email;
            s.F_Name = model.F_Name;
            s.L_Name = model.L_Name;
           
            s.Password = model.Password;
            s.Martial_Status = model.Martial_Status;
            s.Phone_Number = model.Phone_Number;
            if (Profile_Picturee!=null) {
                s.Profile_Picture = new byte[Profile_Picturee.ContentLength];
                Profile_Picturee.InputStream.Read(s.Profile_Picture, 0, Profile_Picturee.ContentLength);
            }
           
            s.CNIC = model.CNIC;
            s.Status = model.Status;
            s.Joining_Date = model.Joining_Date;
            s.Gender = model.Gender;
            s.Type = model.Type;
            s.Role_ID = model.Role_ID;
            dbm.Entry(s).State = EntityState.Modified;
            dbm.SaveChanges();


            return RedirectToAction("Manage_Employee", "Employee");
        }
        public ActionResult delete(int id)
        {
            var a = dbm.Employees.FirstOrDefault(o => o.Employee_ID == id);
            dbm.Employees.Remove(a);
            dbm.SaveChanges();

            return RedirectToAction("Manage_Employee", "Employee");
        }
    }
}