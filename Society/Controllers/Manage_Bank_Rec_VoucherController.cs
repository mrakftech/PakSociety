using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Web.Script.Serialization;
namespace Society.Controllers
{
    public class Manage_Bank_Rec_VoucherController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        public ActionResult result(string search1)
        {
           
                var trips = dbm.reciept_vouchers.Where(o => (o.Manual_Voucher_No.StartsWith(search1) && o.Type=="BRV") ).ToList().Take(300).Select(o => new { o.ChequeDate,o.ChequeNumber,o.Comments,o.Drown_bank,o.IsApproved,o.Manual_Voucher_No,o.RV_ID,o.Type,o.VoucherDate,o.VoucherNum, o.reciept_voucher_payment.FirstOrDefault(k => k.RV_ID == o.RV_ID).Amount });
                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            
           



           

        }
        // GET: Manage_Bank_Rec_Voucher
        public ActionResult Manage_Bank_Rec_Voucher()
        {

            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Voucher");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }


            dynamic dy = new ExpandoObject();
            dy.Pending_Amount = dbm.reciept_vouchers.Where(o => o.Type == "BRV" && o.reciept_voucher_payment.Any()).OrderBy(o=>o.IsApproved==1).ToList().Take(200).Reverse();

            return View(dy);


        }
        public ActionResult Add_Bank_Rec_Voucher(string t)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Voucher");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();
            reciept_vouchers rec = new reciept_vouchers();
            int count1 = dbm.reciept_vouchers.Count();

            if (count1 > 0)
            {

                int id1 = dbm.reciept_vouchers.Max(a => a.VoucherNum);
                rec = dbm.reciept_vouchers.FirstOrDefault(x => x.VoucherNum == id1);

            }

            
            dy.getreciept_vouchersinfo = rec;
            dy.getledgeraccountinfo = dbm.Ledger_Account.ToList();
            if (t == "BRV")
            {
                dy.type = t;
                dy.heading = "Bank Reciept Voucher";
                dy.display = "";
                dy.getledgeraccountinfo1 = dbm.Ledger_Account.ToList().Where(o=>o.Control_Account.Control_Account_Title=="Bank");
            }
            else if (t == "BPV")
            {
                dy.type = t;
                dy.heading = "Bank Payment Voucher";
                dy.display = "";
                dy.getledgeraccountinfo1 = dbm.Ledger_Account.ToList().Where(o => o.Control_Account.Control_Account_Title == "Bank");
            }
            else if (t == "CRV")
            {
                dy.type = t;
                dy.heading = "Cash Reciept Voucher";
                dy.display = "style=display:none;";
                dy.getledgeraccountinfo1 = dbm.Ledger_Account.ToList().Where(o => o.Control_Account.Control_Account_Title == "Cash");
            }
            else if (t == "CPV")
            {
                dy.type = t;
                dy.heading = "Cash Payment Voucher";
                dy.display = "style=display:none;";
                dy.getledgeraccountinfo1 = dbm.Ledger_Account.ToList().Where(o => o.Control_Account.Control_Account_Title == "Cash");
            }
            else
            {
                dy.type = t;
                dy.heading = "Journal Voucher";
                dy.display = "style=display:none;";
                dy.getledgeraccountinfo1 = dbm.Ledger_Account.ToList();
            }

            return View(dy);

        }
        [HttpPost]
        public ActionResult insert(FormCollection f)
        {
            var jss = new JavaScriptSerializer();
            var aa = jss.Deserialize<List<dynamic>>(f["transection"]);
            reciept_vouchers rec = new reciept_vouchers();
            rec.Type = f["rType"];
            rec.VoucherNum = Convert.ToInt32(f["voucherNum"]);
            rec.VoucherDate = Convert.ToDateTime(f["voucherDate"]);
            rec.ChequeNumber = f["chequeNum"];
            rec.ChequeDate = Convert.ToDateTime(f["chequeDate"]);
            rec.Drown_bank = f["chequeStatus"];
            rec.Manual_Voucher_No = f["chequeType"];
            rec.Comments = f["voucherComments"];
            rec.Manual_Voucher_No =Convert.ToString( rec.VoucherNum);
            rec.IsApproved = 0;
            rec.IsChecked = 0;
            double dummy_amount=0;
            dbm.reciept_vouchers.Add(rec);
            dbm.SaveChanges();
            double currentBalance;
            string code = "1";
            foreach (var a in aa)
            {


                string account_code = a[1] + "." + a[2] + "." + a[3];

                var xx = dbm.Ledger_Account.Where(o => o.Ledger_Complete_Code == account_code).Select(o => o.Balance).FirstOrDefault();
                var xx1 = dbm.Ledger_Account.Where(o => o.Ledger_Complete_Code == account_code).Select(o => o.ID).FirstOrDefault();
                double balance = Convert.ToDouble(a[5]);
                dummy_amount = balance;
                char type = Convert.ToChar(a[0]);
                if (type == 'C')
                {
                    currentBalance = Convert.ToDouble(xx) - balance;
                }
                else
                {
                    currentBalance = Convert.ToDouble(xx) + balance;
                }
                reciept_voucher_payment rec_pay = new reciept_voucher_payment();
               
                rec_pay.RV_ID = rec.RV_ID;
                rec_pay.TransType = Convert.ToString(a[0]);
                rec_pay.Element_Account_Code = Convert.ToInt32(a[1]);
                rec_pay.Control_Account_Code = Convert.ToInt32(a[2]);
                rec_pay.Ledger_Account_Code = Convert.ToInt32(a[3]);
                rec_pay.Comments = Convert.ToString(a[4]);
                rec_pay.Amount = Convert.ToDouble(a[5]);
                rec_pay.PreviousBalance = Convert.ToDouble(xx);
                rec_pay.CurrentBalance = currentBalance;
                rec_pay.Ledger_ID = xx1;
                dbm.reciept_voucher_payment.Add(rec_pay);
                dbm.SaveChanges();
               // var x = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == account_code);
               // x.Balance = (double?)currentBalance;
               // dbm.Entry(x).State = EntityState.Modified;

               // dbm.SaveChanges();









            }
            Dummy d = new Dummy();
            d.RV_ID = rec.RV_ID;
            d.Amount =dummy_amount;
            d.Manual_Voucher_Number= Convert.ToString(rec.VoucherNum)+"/"+ DateTime.Today.Year + "/" + rec.Type ;
            d.Payment_Date= Convert.ToDateTime(f["voucherDate"]);
            dbm.Dummies.Add(d);
            dbm.SaveChanges();
            return Json(code, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult checkVoucher(int ID, char type)
        {
            string code;
            if (type == 'C')
            {
                var x = dbm.reciept_vouchers.FirstOrDefault(o => o.VoucherNum == ID);
                x.IsChecked = 1;

                dbm.Entry(x).State = EntityState.Modified;

                if (dbm.SaveChanges() == 1)
                {
                    code = "200";
                    return Json(code, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {

                var x = dbm.reciept_vouchers.FirstOrDefault(o => o.VoucherNum == ID);
                x.IsChecked = 0;

                dbm.Entry(x).State = EntityState.Modified;

                if (dbm.SaveChanges() == 1)
                {
                    code = "200";
                    return Json(code, JsonRequestBehavior.AllowGet);

                }

            }
            code = "201";
            return Json(code, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult deleteVoucher(int ID)
        {
            string code;
            var x = dbm.reciept_vouchers.FirstOrDefault(o => o.VoucherNum == ID);
            var z = dbm.Dummies.FirstOrDefault(o=>o.RV_ID==x.RV_ID);
            dbm.Dummies.Remove(z);
            dbm.SaveChanges();
            var xx = dbm.reciept_voucher_payment.Where(o=>o.RV_ID==x.RV_ID).ToList();
            dbm.reciept_voucher_payment.RemoveRange(xx);
            dbm.SaveChanges();
            dbm.reciept_vouchers.Remove(x);
            

            if (dbm.SaveChanges() == 1)
            {
                code = "200";
                return Json(code, JsonRequestBehavior.AllowGet);

            }
            code = "201";
            return Json(code, JsonRequestBehavior.AllowGet);
        }
           
        [HttpPost]
        public ActionResult approvedVoucher(int ID, char type)
        {
            var party_detail = 0;
            int code=0;
        
            if (type == 'C')
            {




                var x = dbm.reciept_vouchers.FirstOrDefault(o => o.VoucherNum == ID);
                var dummy = dbm.Dummies.FirstOrDefault(o=>o.RV_ID==x.RV_ID);
                if (dummy!=null) {
                    if (dummy.Payment_ID==null) {


                        var found = dbm.reciept_voucher_payment.Where(o => o.RV_ID == x.RV_ID).ToList();
                        foreach (var s in found) {
                            if (s.TransType.Equals('C'))
                            {
                                var ledger = dbm.Ledger_Account.FirstOrDefault(o=>o.ID==s.Ledger_ID);
                                ledger.Balance = ledger.Balance - s.Amount;
                                dbm.Entry(ledger).State = EntityState.Modified;
                                dbm.SaveChanges();
                            }
                            else
                            {
                                var ledger = dbm.Ledger_Account.FirstOrDefault(o => o.ID == s.Ledger_ID);
                                ledger.Balance = ledger.Balance + s.Amount;
                                dbm.Entry(ledger).State = EntityState.Modified;
                                dbm.SaveChanges();
                            }
                        }
                        x.IsApproved = 1;
                        dbm.Entry(x).State = EntityState.Modified;

                        if (dbm.SaveChanges() == 1)
                        {
                           string c = "true";
                            return Json(c, JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        double remaining_amount = 0;

                        var manualvouchernumber = dummy.Manual_Voucher_Number;
                        var payment_date = dummy.Payment_Date;



                        var Payment_ID = dummy.Payment_ID;


                        var amount = dummy.Amount;
                        var amount1 = amount;
                        var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                        if (i.Monthly_Paid == null)
                        {
                            i.Monthly_Paid = 0;
                        }
                        if (amount == (i.Amount_Paid - i.Monthly_Paid))
                        {
                            if (i.Comment == "Booking" || i.Comment == "Confirmation" || i.Comment == "Confirmation")
                            {

                                var checking_id = i.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == i.Payment_Plan.Plot.Member.Member_ID);
                                if (checking_id != null)
                                {
                                    Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail();
                                    dealer_Commision_Detail.Dealer_Commsion_ID = checking_id.ID;
                                    dealer_Commision_Detail.Percentage = checking_id.Percentage / 3;
                                    dealer_Commision_Detail.Status = false;
                                    dealer_Commision_Detail.Due_Amount = ((double)dealer_Commision_Detail.Percentage * (double)i.Payment_Plan.Net_Price_Plot) / 100;
                                    dealer_Commision_Detail.Due_date = DateTime.Now;
                                    dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                                    dbm.SaveChanges();
                                }
                            }
                            i.IsPaid = true;
                            i.Monthly_Paid = i.Monthly_Paid + amount;
                            i.Paid_Month = payment_date;

                            if (x.Image == null)
                            {

                            }
                            else
                            {

                                i.Image = x.Image;
                            }
                            dbm.Entry(i).State = EntityState.Modified;
                            dbm.SaveChanges();
                            Sub_Installment sub = new Sub_Installment();
                            sub.Installment_ID = i.Installment_ID;
                            sub.Amount = amount;
                            sub.Receipt_No = manualvouchernumber;
                            sub.Date = (DateTime)i.Paid_Month;
                            dbm.Sub_Installment.Add(sub);
                            dbm.SaveChanges();
                        }
                        else if (amount < (i.Amount_Paid - i.Monthly_Paid))
                        {

                            i.Monthly_Paid = i.Monthly_Paid + amount;

                            i.Paid_Month = payment_date;

                            if (x.Image == null)
                            {

                            }
                            else
                            {
                                i.Image = x.Image;
                            }
                            dbm.Entry(i).State = EntityState.Modified;
                            dbm.SaveChanges();
                            Sub_Installment sub = new Sub_Installment();
                            sub.Installment_ID = i.Installment_ID;
                            sub.Amount = amount;
                            sub.Receipt_No = manualvouchernumber;
                            sub.Date = (DateTime)i.Paid_Month;
                            dbm.Sub_Installment.Add(sub);
                            dbm.SaveChanges();

                        }
                        else if (amount > (i.Amount_Paid - i.Monthly_Paid))
                        {
                            var iii = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).ToList().OrderBy(o => o.Installment_No);
                            var last = iii.Last();
                            foreach (var a in iii)
                            {
                                if (a.Monthly_Paid == null) { a.Monthly_Paid = 0; }
                                if (amount > 0)
                                {
                                    if (amount >= (a.Amount_Paid - a.Monthly_Paid))
                                    {
                                        if (a.Comment == "Booking" || a.Comment == "Confirmation" || a.Comment == "Confirmation")
                                        {

                                            var checking_id = a.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == a.Payment_Plan.Plot.Member.Member_ID);
                                            if (checking_id != null)
                                            {
                                                Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail();
                                                dealer_Commision_Detail.Dealer_Commsion_ID = checking_id.ID;
                                                dealer_Commision_Detail.Percentage = checking_id.Percentage / 3;
                                                dealer_Commision_Detail.Status = false;
                                                dealer_Commision_Detail.Due_Amount = ((double)dealer_Commision_Detail.Percentage * (double)a.Payment_Plan.Net_Price_Plot) / 100;
                                                dealer_Commision_Detail.Due_date = DateTime.Now;
                                                dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                                                dbm.SaveChanges();
                                            }
                                        }
                                        var temp = a.Monthly_Paid;
                                        a.Monthly_Paid = a.Amount_Paid;
                                        a.IsPaid = true;
                                        remaining_amount = (double)a.Monthly_Paid - (double)temp;
                                        amount = (double)amount - ((double)a.Monthly_Paid - (double)temp);
                                    }
                                    else if (amount < (a.Amount_Paid - a.Monthly_Paid))
                                    {
                                        a.Monthly_Paid = a.Monthly_Paid + amount;
                                        remaining_amount = (double)amount;
                                        amount = (double)amount - (double)a.Monthly_Paid;
                                    }





                                    a.Paid_Month = payment_date;
                                    dbm.Entry(a).State = EntityState.Modified;
                                    if (dbm.SaveChanges() > 0)
                                    {

                                        Sub_Installment sub = new Sub_Installment();
                                        sub.Installment_ID = a.Installment_ID;
                                        sub.Amount = remaining_amount;
                                        sub.Receipt_No = manualvouchernumber;
                                        sub.Date = (DateTime)a.Paid_Month;
                                        dbm.Sub_Installment.Add(sub);
                                        dbm.SaveChanges();
                                    }

                                    if (x.Image == null)
                                    {

                                    }
                                    else
                                    {
                                        a.Image = x.Image;
                                    }
                                }


                            }
                            var last1 = dbm.Installments.FirstOrDefault(o => o.Installment_ID == last.Installment_ID);
                            last1.Monthly_Paid = last1.Monthly_Paid + amount;
                            dbm.Entry(last1).State = EntityState.Modified;
                            dbm.SaveChanges();


                        }
                        var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);

                        var party = dbm.Parties.FirstOrDefault(o => o.Member_ID == memberid.Plot.Member_ID);
                        var temp_balance = party.Balance;
                        party.Balance = party.Balance + amount1;
                        dbm.Entry(party).State = EntityState.Modified;
                        if (dbm.SaveChanges() > 0)
                        {

                            Party_Detail detail = new Party_Detail();
                            detail.Party_ID = party.ID;
                            detail.Previous_Amount = temp_balance;
                            detail.Current_Amount = party.Balance;
                            detail.Pay_Date = (DateTime)payment_date;
                            detail.Receipt_No = manualvouchernumber;
                            detail.Check_No = dummy.reciept_vouchers.ChequeNumber;
                            detail.voucher_check = true;
                            detail.Head = Convert.ToString(x.reciept_voucher_payment.FirstOrDefault(o => o.RV_ID == x.RV_ID && o.TransType.Equals("D")).Ledger_ID);
                            detail.UserId = Convert.ToInt32(Session["emp_id"]);
                            dbm.Party_Detail.Add(detail);
                            dbm.SaveChanges();
                            party_detail = detail.ID;
                            var first = dbm.reciept_voucher_payment.FirstOrDefault(o => o.RV_ID == x.RV_ID && o.TransType.Equals("C"));
                            var second = dbm.reciept_voucher_payment.FirstOrDefault(o => o.RV_ID == x.RV_ID && o.TransType.Equals("D"));

                            var first_ledger = dbm.Ledger_Account.FirstOrDefault(o => o.ID == first.Ledger_ID);
                            var second_ledger = dbm.Ledger_Account.FirstOrDefault(o => o.ID == second.Ledger_ID);

                            first.CurrentBalance = first.CurrentBalance - first.Amount;
                            dbm.Entry(first).State = EntityState.Modified;
                            dbm.SaveChanges();

                            first_ledger.Balance = first_ledger.Balance - first.Amount;
                            dbm.Entry(first_ledger).State = EntityState.Modified;
                            dbm.SaveChanges();

                            second.CurrentBalance = second.CurrentBalance + second.Amount;
                            dbm.Entry(first).State = EntityState.Modified;
                            dbm.SaveChanges();

                            second_ledger.Balance = second_ledger.Balance + second.Amount;
                            dbm.Entry(second_ledger).State = EntityState.Modified;
                            dbm.SaveChanges();
                        }

                    }
                  
                }
             

                x.IsApproved = 1;
                dbm.Entry(x).State = EntityState.Modified;

                if (dbm.SaveChanges() == 1)
                {
                    code = party_detail;
                    return Json(code, JsonRequestBehavior.AllowGet);

                }

            }
           
            code = 201;
            return Json(code, JsonRequestBehavior.AllowGet);


        }
        public dynamic voucherpreview(int VoucherNum)
        {

            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Voucher");
                ViewBag.Key = s.Target;
            }




            string heading;
            dynamic dy = new ExpandoObject();
            reciept_vouchers reciept_Vouchers_details = new reciept_vouchers();

            reciept_Vouchers_details = dbm.reciept_vouchers.FirstOrDefault(o => o.VoucherNum == VoucherNum);

            var reciept_voucher_payment_details = dbm.reciept_voucher_payment.Where(o => o.reciept_vouchers.VoucherNum== VoucherNum).ToList();
          
            var count = reciept_voucher_payment_details.Count;
            if (reciept_Vouchers_details.Type == "BRV")
            {
                heading = "Bank Reciept Voucher";
            }
            else if (reciept_Vouchers_details.Type == "BPV")
            {
                heading = "Bank Payment Voucher";
            }
            else if (reciept_Vouchers_details.Type == "CRV")
            {
                heading = "Cash Reciept Voucher";
            }
            else if (reciept_Vouchers_details.Type == "CPV")
            {
                heading = "Cash Payment Voucher";
            }
            else
            {
                heading = "Journal Voucher";
            }


            dy.heading = heading;
            dy.reciept_Vouchers_details = reciept_Vouchers_details;
            dy.reciept_voucher_payment_details = reciept_voucher_payment_details;
         
            dy.count = count;
          
            return View(dy);
        }

    }
}