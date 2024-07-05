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
    public class RolesController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Roles
        public ActionResult Manage_Roles()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Roles Setting");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.roles.ToList();
            return View(a);
        }
        public JsonResult addnewrole(FormCollection f)
        {

            string b = "false";
            string role = f["ROLE"];
            var a = dbm.roles.Where(o => o.Title == role).Select(c => c.Title).FirstOrDefault();
            if (a != null)
            {
                b = "exist";
                return Json(b, JsonRequestBehavior.AllowGet);
            }
            else
            {
                role r = new role();
                r.Title = role;
                dbm.roles.Add(r);

                if (dbm.SaveChanges() > 0) {
                    Role_Permission p = new Role_Permission();
                    p.Roles_ID = r.Roles_ID;
                    p.Module_Key = "Setting";
                    p.Target = false;
                    dbm.Role_Permission.Add(p);
                    dbm.SaveChanges();

                    Role_Permission p1 = new Role_Permission();
                    p1.Roles_ID = r.Roles_ID;
                    p1.Module_Key = "Manage Advance";
                    p1.Target = false;
                    dbm.Role_Permission.Add(p1);
                    dbm.SaveChanges();

                   

                    Role_Permission p3 = new Role_Permission();
                    p3.Roles_ID = r.Roles_ID;
                    p3.Module_Key = "Manage Dealer";
                    p3.Target = false;
                    dbm.Role_Permission.Add(p3);
                    dbm.SaveChanges();

                   

                    Role_Permission p6 = new Role_Permission();
                    p6.Roles_ID = r.Roles_ID;
                    p6.Module_Key = "Add Accounts";
                    p6.Target = false;
                    dbm.Role_Permission.Add(p6);
                    dbm.SaveChanges();

                   

                    Role_Permission p8 = new Role_Permission();
                    p8.Roles_ID = r.Roles_ID;
                    p8.Module_Key = "Manage Employee";
                    p8.Target = false;
                    dbm.Role_Permission.Add(p8);
                    dbm.SaveChanges();

                    

                  

                    Role_Permission p11 = new Role_Permission();
                    p11.Roles_ID = r.Roles_ID;
                    p11.Module_Key = "Manage File Transfer";
                    p11.Target = false;
                    dbm.Role_Permission.Add(p11);
                    dbm.SaveChanges();

                   

                   

                    Role_Permission p14 = new Role_Permission();
                    p14.Roles_ID = r.Roles_ID;
                    p14.Module_Key = "Manage Loan";
                    p14.Target = false;
                    dbm.Role_Permission.Add(p14);
                    dbm.SaveChanges();

                  
                    Role_Permission p16 = new Role_Permission();
                    p16.Roles_ID = r.Roles_ID;
                    p16.Module_Key = "Manage Voucher";
                    p16.Target = false;
                    dbm.Role_Permission.Add(p16);
                    dbm.SaveChanges();

                   

                    Role_Permission p17 = new Role_Permission();
                    p17.Roles_ID = r.Roles_ID;
                    p17.Module_Key = "Manage Member";
                    p17.Target = false;
                    dbm.Role_Permission.Add(p17);
                    dbm.SaveChanges();

                    Role_Permission p18 = new Role_Permission();
                    p18.Roles_ID = r.Roles_ID;
                    p18.Module_Key = "OverTime";
                    p18.Target = false;
                    dbm.Role_Permission.Add(p18);
                    dbm.SaveChanges();

                    Role_Permission p19 = new Role_Permission();
                    p19.Roles_ID = r.Roles_ID;
                    p19.Module_Key = "Manage Party";
                    p19.Target = false;
                    dbm.Role_Permission.Add(p19);
                    dbm.SaveChanges();

                    Role_Permission p20 = new Role_Permission();
                    p20.Roles_ID = r.Roles_ID;
                    p20.Module_Key = "Payment Plan";
                    p20.Target = false;
                    dbm.Role_Permission.Add(p20);
                    dbm.SaveChanges();

                    Role_Permission p21 = new Role_Permission();
                    p21.Roles_ID = r.Roles_ID;
                    p21.Module_Key = "Reports";
                    p21.Target = false;
                    dbm.Role_Permission.Add(p21);
                    dbm.SaveChanges();

                    Role_Permission p22 = new Role_Permission();
                    p22.Roles_ID = r.Roles_ID;
                    p22.Module_Key = "Roles Setting";
                    p22.Target = false;
                    dbm.Role_Permission.Add(p22);
                    dbm.SaveChanges();

                    Role_Permission p23 = new Role_Permission();
                    p23.Roles_ID = r.Roles_ID;
                    p23.Module_Key = "Salary";
                    p23.Target = false;
                    dbm.Role_Permission.Add(p23);
                    dbm.SaveChanges();

                    Role_Permission p24 = new Role_Permission();
                    p24.Roles_ID = r.Roles_ID;
                    p24.Module_Key = "Shifts";
                    p24.Target = false;
                    dbm.Role_Permission.Add(p24);
                    dbm.SaveChanges();

                    Role_Permission p25 = new Role_Permission();
                    p25.Roles_ID = r.Roles_ID;
                    p25.Module_Key = "OverSeas Form";
                    p25.Target = false;
                    dbm.Role_Permission.Add(p25);
                    dbm.SaveChanges();

                    Role_Permission p26 = new Role_Permission();
                    p26.Roles_ID = r.Roles_ID;
                    p26.Module_Key = "Open Files";
                    p26.Target = false;
                    dbm.Role_Permission.Add(p26);
                    dbm.SaveChanges();

                    b = "true"; }

                return Json(b, JsonRequestBehavior.AllowGet);
            }

          
        }
        [HttpPost]
        public JsonResult deleteUserRole(FormCollection f)
        {

            string b = "false";
            int id = Convert.ToInt32(f["ID"]);
            var a = dbm.roles.FirstOrDefault(o => o.Roles_ID == id);
         
          

                var dlt = dbm.Role_Permission.Where(o=>o.Roles_ID==a.Roles_ID).ToList();

                dbm.Role_Permission.RemoveRange(dlt);
                dbm.SaveChanges();
            dbm.roles.Remove(a);
            dbm.SaveChanges();

            b = "true"; 

            return Json(b, JsonRequestBehavior.AllowGet);
        }
        public ActionResult manage_permissions()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Roles Setting");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.roles.ToList();

            return View(a);
        }
        [HttpPost]
        public JsonResult getRolePermissionDataByRoleID(FormCollection form)
        {

            int id = Convert.ToInt32(form["ID"]);
            var a = dbm.Role_Permission.Where(o=>o.Roles_ID==id).Select(o=> new { o.Roles_Permission_ID,o.Target,o.Module_Key }).ToArray();

            var json = JsonConvert.SerializeObject(a);




            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult update_roles(int ID)
        {
            var a = dbm.Role_Permission.FirstOrDefault(o=>o.Roles_Permission_ID==ID);
            if (a.Target==false) {
                a.Target = true;
            }
            else
            {
                a.Target = false;
            }
            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            return Json( JsonRequestBehavior.AllowGet);
        }
            
    }
}