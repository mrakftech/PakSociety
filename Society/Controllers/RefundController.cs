using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Society.Controllers
{
    public class RefundController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Refund
        public ActionResult manage_refund(FormCollection form)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Refunds.ToList();
            return View(a);
        }
            public ActionResult refund(FormCollection form)
        {

            var manualvouchernumber = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1);
            var payment_date = Convert.ToDateTime(form["txtPaymentDate"]);
            var firstledger = form["firstledger"];
            var secondledger = form["secondledger_refund"];
            var firsttype = form["firsttype"];
            var secondtype = form["secondtype"];

            var Payment_mode = form["Payment_mode"];
            var Payment_ID = Convert.ToInt32(form["Payment_ID"]);
            var checknumber = form["checknumber"];

            var amount = Convert.ToDouble(form["txtPaymentAmount"]);
            var amount1 = amount;



            Refund re = new Refund();
            re.Amount = amount;
            re.Date = payment_date;
            var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);
            re.Member_Name = memberid.Plot.Member.Applicant_Name;
            re.Reg_No = memberid.Plot.Reg_No;
            re.Narration = checknumber;
            re.Receipt_no= manualvouchernumber + "/" + DateTime.Today.Year + "/" + Payment_mode;
            re.Head = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Ledger_Account_Title;
            re.CNIC = memberid.Plot.Member.CNIC;
            re.father_name = memberid.Plot.Member.Father_Husband_Name;
            re.Ledger_Type = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Control_Account_Title;
            re.Plot_type = memberid.Plot.Plot_Type;
            re.Size = memberid.Plot.Installment_Plan.Plan_Name;
            dbm.Refunds.Add(re);
            dbm.SaveChanges();
            memberid.Plot.Member.Status = false;
            memberid.Plot.Member.Refund_Check = true;
            dbm.Entry(memberid).State = EntityState.Modified;
            dbm.SaveChanges();
            reciept_vouchers rec = new reciept_vouchers();
            rec.Type = Payment_mode;

            rec.IsApproved = 1;
            rec.IsChecked = 1;
            rec.ChequeNumber = checknumber;
            rec.Comments = "Amount Refunded";
            rec.VoucherDate = payment_date;

            rec.Manual_Voucher_No = manualvouchernumber + "/" + DateTime.Today.Year + "/" + Payment_mode;


            var num = dbm.reciept_vouchers.Max(o => o.VoucherNum);
            num = num + 1;
            rec.VoucherNum = num;
            dbm.reciept_vouchers.Add(rec);
            if (dbm.SaveChanges() > 0)
            {
                //inssert data into recept voucher table ................................................
                reciept_voucher_payment pay = new reciept_voucher_payment();
                pay.RV_ID = rec.RV_ID;
                pay.Amount = (double)amount1;
                string[] arrey = firstledger.Split('.');
                pay.Element_Account_Code = Convert.ToInt32(arrey[0]);
                pay.Control_Account_Code = Convert.ToInt32(arrey[1]);
                pay.Ledger_Account_Code = Convert.ToInt32(arrey[2]);
                pay.Ledger_ID = dbm.Ledger_Account.FirstOrDefault(o => o.Element_Account_Code == pay.Element_Account_Code && o.Control_Account_Code == pay.Control_Account_Code && o.Ledger_Account_Code == pay.Ledger_Account_Code).ID;
                //if type is C....................................................................
                if (firsttype == "C")
                {
                    pay.TransType = firsttype;
                    pay.Comments = "Amount Refunded";
                    pay.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger).Balance;
                    pay.CurrentBalance = pay.PreviousBalance - amount1;
                    dbm.reciept_voucher_payment.Add(pay);
                    dbm.SaveChanges();
                    var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger);
                    a.Balance = pay.CurrentBalance;
                    dbm.Entry(a).State = EntityState.Modified;
                    dbm.SaveChanges();
                }
                else
                {//if type is D....................................................................
                    pay.TransType = firsttype;
                    pay.Comments = "Amount Refunded";
                    pay.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger).Balance;
                    pay.CurrentBalance = pay.PreviousBalance + amount1;
                    dbm.reciept_voucher_payment.Add(pay);
                    dbm.SaveChanges();
                    var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger);
                    a.Balance = pay.CurrentBalance;
                    dbm.Entry(a).State = EntityState.Modified;
                    dbm.SaveChanges();
                }

                //inssert data into recept voucher table ................................................
                reciept_voucher_payment pay1 = new reciept_voucher_payment();
                pay1.RV_ID = rec.RV_ID;
                pay1.Amount = (double)amount1;
                string[] arrey1 = secondledger.Split('.');
                pay1.Element_Account_Code = Convert.ToInt32(arrey1[0]);
                pay1.Control_Account_Code = Convert.ToInt32(arrey1[1]);
                pay1.Ledger_Account_Code = Convert.ToInt32(arrey1[2]);
                pay1.Ledger_ID = dbm.Ledger_Account.FirstOrDefault(o => o.Element_Account_Code == pay1.Element_Account_Code && o.Control_Account_Code == pay1.Control_Account_Code && o.Ledger_Account_Code == pay1.Ledger_Account_Code).ID;
                //if type is C....................................................................
                if (secondtype == "C")
                {
                    pay1.TransType = secondtype;
                    pay1.Comments = "Amount Refunded";
                    pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                    pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                    dbm.reciept_voucher_payment.Add(pay1);
                    dbm.SaveChanges();
                    var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                    a.Balance = pay1.CurrentBalance;
                    dbm.Entry(a).State = EntityState.Modified;
                    dbm.SaveChanges();
                    return RedirectToAction("print", "refund", new { id = re.ID});

                }
                else
                {//if type is D....................................................................
                    pay1.TransType = secondtype;
                    pay1.Comments = "Amount Refunded";
                    pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                    pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                    dbm.reciept_voucher_payment.Add(pay1);
                    dbm.SaveChanges();
                    var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                    a.Balance = pay1.CurrentBalance;
                    dbm.Entry(a).State = EntityState.Modified;
                    dbm.SaveChanges();
                   

                    return RedirectToAction("print", "refund", new { id = memberid.Plot.Member_ID });
                    //return RedirectToAction("Print", "Party", new { id = detail_id });

                }





            }
            return View();
        }
        public ActionResult print(int id)
        {
            var a = dbm.Refunds.First(o => o.ID == id);
       
            return View(a);
        }
    }
}
        