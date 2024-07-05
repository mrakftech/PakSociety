using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace Society.Controllers
{
    public class Dealer_CommisionController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Dealer_Commision
        public ActionResult test() {
            var a = dbm.Dealers.ToList();
            foreach (var dealer in a) {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Dealer Data");
                var ledger_count = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Max(x => x.Ledger_Account_Code);

                Ledger_Account l = new Ledger_Account();
                l.Balance = 0;
                l.Element_ID = setting.Element_ID;
                l.Control_ID = setting.Control_ID;
                l.Control_Account_Code = setting.Control_Account.Control_Account_Code;
                l.Control_Account_Title = setting.Control_Account.Control_Account_Title;
                l.Element_Account_Code = setting.Element_Account.Element_Account_Code;
                l.Element_Account_Title = setting.Element_Account.Account_Title;
                l.Dealer_ID = dealer.Dealer_ID;
                if (ledger_count == null)
                {
                    l.Ledger_Account_Code = 1;
                    l.Ledger_Complete_Code = setting.Element_Account.Element_Account_Code + "." + setting.Control_Account.Control_Account_Code + "." + "1";
                }
                else
                {
                    l.Ledger_Account_Code = ledger_count + 1;
                    l.Ledger_Complete_Code = setting.Element_Account.Element_Account_Code + "." + setting.Control_Account.Control_Account_Code + "." + (ledger_count + 1);
                }

                l.Ledger_Account_Title = dealer.Company_Name;

                dbm.Ledger_Account.Add(l);
                dbm.SaveChanges();

            }
            return View();
        }
        public ActionResult manage_Dealer_Commision(int id)
        { dynamic dy = new ExpandoObject();
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
            dy.dealer_data = dbm.Dealer_Commision_Detail.Where(o => o.Dealer_Commision.Member_id == id).ToList();
            var find_dealer = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id)?.Dealer_ID;
            dy.credit_account = dbm.Ledger_Account.FirstOrDefault(o => o.Dealer_ID == find_dealer);
            dy.ledger = dbm.Ledger_Account.Where(o => o.Control_Account_Title == "Bank").ToList();
            dy.Dealer_Commsion_ID = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id)?.ID;
            return View(dy);
        }
        [HttpPost]
        public ActionResult manage_Dealer_Commision(FormCollection form, HttpPostedFileBase txtPaymentModeImage)
        {
           
     
            var date = form["checkDate"];
            //var manualvouchernumber = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1);
            var maxVoucherNum = dbm.reciept_vouchers.Max(o => (int?)o.VoucherNum) ?? 0;
            var manualvouchernumber = Convert.ToString(maxVoucherNum + 1);

            var payment_date = Convert.ToDateTime(form["txtPaymentDate"]);
            var firstledger = form["firstledger"];
            var secondledger = form["secondledger"];
            var firsttype = form["firsttype"];
            var secondtype = form["secondtype"];
            var comment = form["comment"];
            var Payment_mode = form["Payment_mode"];
            var ID = Convert.ToInt32(form["ID"]);
            var Dealer_Commsion_ID = Convert.ToInt32(form["Dealer_Commsion_ID"]);
            var checknumber = form["checknumber"];

            var amount = Convert.ToDouble(form["txtPaymentAmount"]);
            var amount1 = amount;
            var check = form["check"];
            var checkdate = DateTime.MinValue;
            if (Payment_mode == "CRV" || date == "")
            {

            }
            else
            {
                checkdate = Convert.ToDateTime(form["checkDate"]);
            }
            Dealer_Commision_Detail find_id = null;
            if (ID == 0)
            {
                Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail
                {
                    Due_Amount = amount,
                    Dealer_Commsion_ID = Dealer_Commsion_ID,
                    Due_date = payment_date,
                    Name = "",
                    Pay_date = payment_date,
                    Percentage = 0,
                    Status = false,
                    
                };
                dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                dbm.SaveChanges();
                find_id = dealer_Commision_Detail;
            }
            else
            {
                find_id = dbm.Dealer_Commision_Detail.FirstOrDefault(o => o.ID == ID);
            }
         
            if (find_id.Status == false) 
            { 
            find_id.Status = true;
            find_id.Pay_date = payment_date;
            dbm.Entry(find_id).State = EntityState.Modified;
            dbm.SaveChanges();
                // var memberId = dbm.Dealer_Commision.FirstOrDefault(o => o.ID == Dealer_Commsion_ID).Member_id;
                var memberId = dbm.Dealer_Commision.FirstOrDefault(o => o.ID == Dealer_Commsion_ID)?.Member_id;
                if (memberId == null)
                {
                    // Handle the case where Member_id is null
                    // For example, you might want to log this or throw an exception
                    throw new InvalidOperationException("Member_id not found for the given Dealer_Commsion_ID");
                }

                // dev.receipt_No = manualvouchernumber + "/" + DateTime.Today.Year + "/" + Payment_mode;


                // change by amjad for leader data

                if (memberId != null)
                {
                    var memberName = dbm.Members.FirstOrDefault(x => x.Member_ID == memberId);
                    var dealerName = dbm.Dealer_Commision.FirstOrDefault(x => x.ID == Dealer_Commsion_ID)?.Dealer.CEOName;
                    var UnitType = dbm.Plots.FirstOrDefault(x => x.Member_ID == memberId)?.Block;
                    //var UnitNo = dbm.Plots.Where(x => x.Member_ID == memberId).Include(x => x.newPlot.PlotNo);
                    //var UnitNo = dbm.Plots.FirstOrDefault(x => x.Member_ID == memberId)?.newPlot?.PlotNo;
                    var UnitNoString = dbm.Plots.FirstOrDefault(x => x.Member_ID == memberId)?.newPlot?.PlotNo;

                    double? UnitNo = null;
                    if (double.TryParse(UnitNoString, out double result))
                    {
                        UnitNo = result;
                    }
                    if (memberName != null && dealerName != null && UnitType != null)
                    {
                        var dealer_Data = new dealer_data
                        {
                            Sale_Price = amount1,
                            Member_Name = memberName.Applicant_Name,
                            Dealer_Name = dealerName,
                            Type = UnitType,
                            Unit__= UnitNo
                        };

                        dbm.dealer_data.Add(dealer_Data);
                        dbm.SaveChanges();
                    }
                    else
                    {
                        // Handle case where data is not found
                    }
                }



                reciept_vouchers rec = new reciept_vouchers();
            rec.Type = Payment_mode;
            if (txtPaymentModeImage == null)
            {

            }
            else
            {
                rec.Image = new byte[txtPaymentModeImage.ContentLength];
                txtPaymentModeImage.InputStream.Read(rec.Image, 0, txtPaymentModeImage.ContentLength);
            }
            rec.IsApproved = 1;
            rec.IsChecked = 1;
            rec.ChequeNumber = checknumber;
            rec.Comments = comment;
            rec.VoucherDate = payment_date;
            rec.ChequeDate = checkdate;
                // rec.Manual_Voucher_No = manualvouchernumber + "/" + DateTime.Today.Year + "/" + Payment_mode;
                int nextVoucherNumManuPrev = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                rec.Manual_Voucher_No = Convert.ToString(nextVoucherNumManuPrev) + "/" + DateTime.Today.Year + "/" + Payment_mode;


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
                        pay.Comments = comment;
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
                        pay.Comments = comment;
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
                        pay1.Comments = comment;
                        pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                        pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                        dbm.reciept_voucher_payment.Add(pay1);
                        dbm.SaveChanges();
                        var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                        a.Balance = pay1.CurrentBalance;
                        dbm.Entry(a).State = EntityState.Modified;
                        dbm.SaveChanges();
                        return RedirectToAction("manage_Dealer_Commision", "Dealer_Commision", new { id = memberId });

                    }
                    else
                    {//if type is D....................................................................
                        pay1.TransType = secondtype;
                        pay1.Comments = comment;
                        pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                        pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                        dbm.reciept_voucher_payment.Add(pay1);
                        dbm.SaveChanges();
                        var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                        a.Balance = pay1.CurrentBalance;
                        dbm.Entry(a).State = EntityState.Modified;
                        dbm.SaveChanges();

                        return RedirectToAction("manage_Dealer_Commision", "Dealer_Commision", new { id = memberId });
                        //return RedirectToAction("Print", "Party", new { id = detail_id });

                    }
                }

            }
            return View();
        }
    }
}