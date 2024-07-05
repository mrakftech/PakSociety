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
    public class DealerController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Dealer
        public ActionResult Manage_Dealer()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Dealer");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Dealers.ToList();
            return View(a);
        }
        public ActionResult dealer()
        {


            var a = dbm.dealer_data.ToList().Where(o => o.Reg_No != 211);
            foreach (var s in a)
            {
                var reg = Convert.ToString(s.Reg_No);
                var plot = dbm.Plots.FirstOrDefault(o => o.Reg_No == reg);
                if (plot != null)
                {
                    plot.Member.Dealer_Name = s.Dealer_Name;
                    dbm.Entry(plot).State = EntityState.Modified;
                    dbm.SaveChanges();

                    dbm.dealer_data.Remove(s);
                    dbm.SaveChanges();
                }


            }


            return View();
        }
        public ActionResult Add_Dealer()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Dealer");
                ViewBag.Key = s.Target;
            }
            ViewBag.dealer = dbm.Dealers.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Add_Dealer(Dealer model, string dealerType)
        {
            if (dealerType == "1")
            {
                model.ParentDealerId = null;
               
            }
            var dealerAttach = dbm.Dealers.FirstOrDefault(o=>o.Dealer_ID == model.ParentDealerId);
            model.Dealer1.Add(dealerAttach);
            dbm.Dealers.Add(model);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Dealer", "Dealer");
        }
        public ActionResult Delete(int id)
        {
            var a = dbm.Dealers.FirstOrDefault(o => o.Dealer_ID == id);
            dbm.Dealers.Remove(a);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Dealer", "Dealer");
        }
        public ActionResult Update_Dealer(int id)
        {
            ViewBag.dealer = dbm.Dealers.ToList();
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Dealer");
                ViewBag.Key = s.Target;
            }
            var a = dbm.Dealers.FirstOrDefault(o => o.Dealer_ID == id);

            return View(a);
        }
        [HttpPost]
        public ActionResult Update_Dealer(Dealer model,string dealerType)
        {
            if (dealerType == "1")
            {
                model.ParentDealerId = null;

            }
            var dealerAttach = dbm.Dealers.FirstOrDefault(o => o.Dealer_ID == model.ParentDealerId);
            var a = dbm.Dealers.FirstOrDefault(o => o.Dealer_ID == model.Dealer_ID);
           var removDealer = a.Dealer1.ToList();
            a.Company_Address = model.Company_Address;
            a.Company_Name = model.Company_Name;
            a.Company_Phone = model.Company_Phone;
            a.Contact_Person_Name = model.Contact_Person_Name;
            a.Percentage = model.Percentage;
            a.Person_CNIC = model.Person_CNIC;
            a.Person_Email = model.Person_Email;
            a.Person_Phone = model.Person_Phone;
            a.Reg_Date = model.Reg_Date;
            a.Reg_No = model.Reg_No;
            a.Status = model.Status;
            a.ParentDealerId = model.ParentDealerId;
            a.DealerAppartmentPrice = model.DealerAppartmentPrice;
            a.DealerOfficePrice = model.DealerOfficePrice;
            a.DealerShopPrice = model.DealerShopPrice;
            a.DealerPentHousePrice = model.DealerPentHousePrice;
            foreach (var item in removDealer)
            {

            a.Dealer1.Remove(item);
            }
            a.Dealer1.Add(dealerAttach);



            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            return RedirectToAction("Manage_Dealer", "Dealer");
        }
    }
}