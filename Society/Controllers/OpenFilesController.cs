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
    public class OpenFilesController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        public class openFile1{
            public int Id { get; set; }
            public string IssueDate { get; set; }
            public double PBNo { get; set; }
            public string Dealer { get; set; }
            public double Balance { get; set; }
            public double RefNo { get; set; }
        }
        // GET: Loan
        public ActionResult Manage_OpenFiles()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Open Files");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var list = dbm.OpenFiles.ToList();
            return View(list);
        }
        public ActionResult AddNew_OpenFile()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Open Files");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddNew_OpenFile(OpenFile model)
        {
            dbm.OpenFiles.Add(model);
            dbm.SaveChanges();
            return RedirectToAction("Manage_OpenFiles", "OpenFiles");

        }
        public ActionResult Delete(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Open Files");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var x = dbm.OpenFiles.FirstOrDefault(o => o.Id == id);
            if (x != null)
            {
                
                dbm.OpenFiles.Remove(x);
                dbm.SaveChanges();

            }
            return RedirectToAction("Manage_OpenFiles", "OpenFiles");

        }
        public ActionResult Update(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Open Files");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var x = dbm.OpenFiles.FirstOrDefault(o => o.Id == id);
            
            return View(x);

        }
        [HttpPost]
        public ActionResult Update(OpenFile openFile)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Open Files");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var x = dbm.OpenFiles.FirstOrDefault(o => o.Id ==openFile.Id);
            x.Balance = openFile.Balance;
            x.Closer = openFile.Closer;
            x.Dealer = openFile.Dealer;
            x.IssueDate = openFile.IssueDate;
            x.PBNo = openFile.PBNo;
            x.RateMatured = openFile.RateMatured;
            x.RateofFile = openFile.RateofFile;
            x.RefNo = openFile.RefNo;
            x.ReIssued = openFile.ReIssued;
            x.Size = openFile.Size;
            x.Sr = dbm.OpenFiles.Count() + 1;
            x.Status = openFile.Status;
            x.Type = openFile.Type;
            dbm.Entry(x).State = EntityState.Modified;
            dbm.SaveChanges();

            return RedirectToAction("Manage_OpenFiles", "OpenFiles");

        }
    }
}