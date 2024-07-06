using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using Society.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;


namespace Society.Controllers
{
    public class PartyController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Party

        public ActionResult Plots()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                ViewBag.printedName = aaaaa.F_Name;
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.newPlots.ToList();


            return View(a);
        }
        public ActionResult Party_Detail()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Parties.ToList();
            return View(a);
        }
        public ActionResult View( int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();
            dy.a = dbm.Party_Detail.Where(o=>o.Party_ID==id).ToList().OrderBy(o=>o.Pay_Date);
            dy.ledger = dbm.Ledger_Account.ToList();
            return View(dy);
        }
        public ActionResult delete(int id)
        {
            var prty_id = 0;
           var party_delete = dbm.Party_Detail.FirstOrDefault(o=>o.ID==id);
            if (party_delete.voucher_check == true)
            {
                var prty = dbm.Parties.Where(o => o.ID == party_delete.Party_ID).OrderBy(o=>o.ID).First();
                prty_id = prty.ID;
                prty.Balance = prty.Balance - (party_delete.Current_Amount - party_delete.Previous_Amount);
                dbm.Entry(prty).State = EntityState.Modified;
                dbm.SaveChanges();
                var voucher_No = party_delete.Receipt_No;
                var voucher_delete = dbm.reciept_vouchers.FirstOrDefault(o => o.Manual_Voucher_No == party_delete.Receipt_No);

                var nxt_voucher = dbm.reciept_voucher_payment.Where(o => o.RV_ID == voucher_delete.RV_ID).ToList();
                foreach (var s in nxt_voucher)
                {
                    if (s.TransType.Equals("C"))
                    {
                        var ledger = dbm.Ledger_Account.First(o => o.ID == s.Ledger_ID);
                        ledger.Balance = ledger.Balance + s.Amount;
                        dbm.Entry(ledger).State = EntityState.Modified;
                        dbm.SaveChanges();
                        dbm.reciept_voucher_payment.Remove(s);
                        dbm.SaveChanges();
                    }
                    else
                    {
                        var ledger = dbm.Ledger_Account.First(o => o.ID == s.Ledger_ID);
                        ledger.Balance = ledger.Balance - s.Amount;
                        dbm.Entry(ledger).State = EntityState.Modified;
                        dbm.SaveChanges();
                        dbm.reciept_voucher_payment.Remove(s);
                        dbm.SaveChanges();
                    }
                }

                dbm.reciept_vouchers.Remove(voucher_delete);
                dbm.SaveChanges();


                var previous_of_dlt = party_delete.Previous_Amount;

                var prty_dlt_id = party_delete.Party_ID;
                var simple_id = party_delete.ID;


                dbm.Party_Detail.Remove(party_delete);
                dbm.SaveChanges();

                var total = dbm.Party_Detail.Where(o => o.Party_ID == prty_dlt_id && o.ID > simple_id).ToList().OrderBy(o => o.ID);

                foreach (var sc in total)
                {

                    var for_getting_paid_amount = sc.Current_Amount - sc.Previous_Amount;
                    sc.Previous_Amount = previous_of_dlt;
                    sc.Current_Amount = sc.Previous_Amount + for_getting_paid_amount;
                    previous_of_dlt = sc.Current_Amount;
                    dbm.Entry(sc).State = EntityState.Modified;
                    dbm.SaveChanges();

                }
                var sub_installment = dbm.Sub_Installment.Where(o => o.Receipt_No == voucher_No).ToList();

                foreach (var ss in sub_installment)
                {
                    var installment = dbm.Installments.First(o => o.Installment_ID == ss.Installment_ID);
                    installment.Monthly_Paid = installment.Monthly_Paid - ss.Amount;
                    installment.IsPaid = false;
                    dbm.Entry(installment).State = EntityState.Modified;
                    dbm.SaveChanges();



                    dbm.Sub_Installment.Remove(ss);
                    dbm.SaveChanges();
                }
            }
           
               
            

            return RedirectToAction("View", "Party", new { id = prty_id});
        }
        [HttpPost]
        public ActionResult UpdatePlot(newPlot plot)
        {
           
                dbm.Entry(plot).State = EntityState.Modified;
                dbm.SaveChanges();
                return Json(new { success = true });
            
        }


        [HttpPost]
        public ActionResult DeletePlot(int id)
        {
            try
            {
                var plot = dbm.newPlots.Find(id);
                if (plot != null)
                {
                    dbm.newPlots.Remove(plot);
                    dbm.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                if (innerException is SqlException sqlException && sqlException.Number == 547) // Check for foreign key constraint violation error number
                {
                    // Handle the specific case where the delete operation violates a foreign key constraint
                    // For example, you can return a JSON response indicating the failure
                    return Json(new { success = false, message = "Cannot delete plot due to existing references." });
                }
                // Handle other types of DbUpdateException or rethrow if needed
                return Json(new { success = false, message = "Cannot delete plot due to existing references." });
            }
        }
        public ActionResult print(int id) {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                ViewBag.printedName = aaaaa.F_Name;
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Party_Detail.FirstOrDefault(o => o.ID == id);

            ViewBag.ledger = dbm.Ledger_Account.ToList();
            var val = Convert.ToInt32(a.Head);
            ViewBag.type = dbm.Ledger_Account.FirstOrDefault(o => o.ID == val).Control_Account_Title;
            ViewBag.paymentType = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).FirstOrDefault().Installment.Comment;
            ViewBag.installmentNo = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).FirstOrDefault().Installment.Installment_No;
            ViewBag.sub = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).ToList();
           
            return View(a);
        }
          [HttpPost]
        public ActionResult Plots(newPlot newPlot)
        {
            if (Session["emp_id"] != null)
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.Where(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Any(o => o.Target == true);
                if (s.Any(o => o.Target == true) == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            else
            {
                return RedirectToAction("UnAuthorized", "Login");
            }
            if (Session["emp_id"] != null)
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.Where(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Any(o => o.Target == true);
            }
            newPlot.isAssigned = false;
            if (!dbm.newPlots.Any(o => o.PlotNo == newPlot.PlotNo && o.Block == newPlot.Block))
            {
                var a = dbm.newPlots.Add(newPlot);
                dbm.SaveChanges();
            }

            var aa = dbm.newPlots.ToList();
            return View(aa);
        }

        public ActionResult printNew(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                ViewBag.printedName = aaaaa.F_Name;
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Party_Detail.FirstOrDefault(o => o.ID == id);

            var val = Convert.ToInt32(a.Head);
            ViewBag.type = dbm.Ledger_Account.FirstOrDefault(o => o.ID == val).Control_Account_Title;
            ViewBag.sub = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).ToList();

            return View(a);
        }
        public ActionResult printReceipt(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var data = dbm.paymentVouchers.FirstOrDefault(o=>o.Id==id);
            return View(data);
        }
        public ActionResult AddReceipt(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddReceipt(paymentVoucher paymentVoucher)
        {
           var balance = dbm.Payment_Plan.FirstOrDefault(o=>o.Payment_ID == paymentVoucher.paymentId).Plot.Member.Parties.FirstOrDefault().Balance;
            paymentVoucher.totalbalance = (double)balance;
            paymentVoucher.date = DateTime.Now;
            dbm.paymentVouchers.Add(paymentVoucher);
            dbm.SaveChanges();
            return RedirectToAction("printReceipt", "Party", new { id = paymentVoucher.Id });
        }
        public ActionResult ManageReceipt(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            ViewBag.id = id;
            var data = dbm.paymentVouchers.Where(o => o.paymentId == id).ToList();
            return View(data);
        }
        public ActionResult development_charges(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Development_Charges.FirstOrDefault(o => o.ID == id);
            var total_paid = dbm.Development_Charges.Where(o => o.Payment_ID == a.Payment_ID && o.ID<=id).Sum(o => o.Amount);
            var size = dbm.Plots.Where(o => o.Payment_Plan.Any(z => z.Payment_ID == a.Payment_ID)).FirstOrDefault().Installment_Plan.Plan_Name;
            string[] arrey1 = size.Split('x');
            var first = Convert.ToInt32(arrey1[0]);
            var second = Convert.ToInt32(arrey1[1]);
            var total_amount = ((first * second) / 250)*a.Payment_Plan.development_charges_per_marla;
          ViewBag.amount =  total_amount - total_paid;
            var val = Convert.ToInt32(a.Head);
            ViewBag.type = dbm.Ledger_Account.FirstOrDefault(o => o.ID == val).Control_Account_Title;
           

            return View(a);
        }
    }
}