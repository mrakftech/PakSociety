using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Net;
using System.Net.NetworkInformation;

namespace Society.Controllers
{
    public class ShiftsController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        //taskEntities dbm1 = new taskEntities();
        // GET: Shifts
        public ActionResult set_party(int id,int id1)
        {
            var a = dbm.Parties.Where(o => o.Balance>0 && o.ID>=id && o.ID<=id1).ToList();
            foreach (var s in a) {

                var chk = s.Party_Detail.ToList().OrderBy(o=>o.Pay_Date);
                var chk1 = s.Party_Detail.OrderBy(o=>o.Pay_Date).First();
                double current = 0;
                foreach (var z in chk) {
                  
                    if (chk1.ID == z.ID)
                    { var find_amount = z.Current_Amount - z.Previous_Amount;
                        current = (double)find_amount;
                        z.Previous_Amount = 0;
                        z.Current_Amount = find_amount;
                        dbm.Entry(z).State = EntityState.Modified;
                        dbm.SaveChanges();
                    }
                    else
                    {
                        var paid_amount = z.Current_Amount - z.Previous_Amount;
                      
                        z.Previous_Amount = current;
                        z.Current_Amount =z.Previous_Amount+ paid_amount;
                        current = (double)z.Current_Amount;
                        dbm.Entry(z).State = EntityState.Modified;
                        dbm.SaveChanges();

                    }
                
                
                }
            }
            return View();
        }
   
        public ActionResult Manage_Shifts()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Shifts");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Shifts.ToList();
            return View(a);
        }
        public ActionResult set()
        {

            var aa = dbm.reciept_voucher_payment.Where(o => o.Ledger_ID == 2).ToList();
            foreach (var sa in aa)
            {
                sa.reciept_vouchers.Type = "CRV";
                dbm.Entry(sa).State = EntityState.Modified;
                dbm.SaveChanges();
            }
            return View();
        }

        /*  public ActionResult calculate2()
          {
              var a = dbm.singe_record.Where(o=>o.Booking_Date.Contains("-")).ToList();
              foreach (var first in a) {
                  string[] arrey1 = first.Booking_Date.Split('-');
                 var days = Convert.ToInt32(arrey1[0]);
                  var month = Convert.ToInt32(arrey1[1]);
                  var years= Convert.ToInt32(arrey1[2]);
                  var cal = years + "-" + month + "-" + days;

                  var update = dbm.singe_record.FirstOrDefault(o=>o.Reg_No==first.Reg_No);
                  update.Booking_Date = cal;
                  dbm.Entry(update).State = EntityState.Modified;
                  dbm.SaveChanges();
              }
              return View(); }*/
          public ActionResult calculate1()
              {

                  var a = dbm.C_Members_Record__.ToList();
                  foreach (var data in a ) {






                            
                              var find = dbm.Installment_Plan.FirstOrDefault(o => o.Plan_Name == data.Size );
                              if (find!=null)
                              {
                                  if (data.Reg_No == data.Reg_No && data.Reg_No == data.Reg_No)
                                  {
                                      Member m = new Member();
                                      m.Applicant_Name = data.Member_Name;
                                      m.Applicant_Start_Text = "Mr.";
                                      m.CNIC = data.CNIC;
                                      m.Application_Date =(DateTime) data.Booking_Date;
                                    
                                      m.Father_Husband_Name = data.SO_DO_WO;
                                      m.Father_Husband_Start_Text = "Mr.";
                                      m.Cell_No = data.Mobile;
                                      m.Permenent_Postel_Address = data.Address;
                                      m.Present_Postel_Address = data.Address;
                                  m.Status = true;


                                      dbm.Members.Add(m);

                                      if (dbm.SaveChanges() > 0)
                                      {
                                      Party prty = new Party();
                                      prty.Member_ID = m.Member_ID;
                                      prty.Balance = 0;
                                      dbm.Parties.Add(prty);
                                      dbm.SaveChanges();
                                          Plot p = new Plot();
                                          p.Member_ID = m.Member_ID;
                                          p.Block = data.Block;
                                          p.Street = data.Street;
                                          p.Plot_Type = data.Category;
                                      string s =Convert.ToString( data.Reg_No);
                                      p.Reg_No = s;

                                          p.Size = find.ID;
                                          p.Date = m.Application_Date;
                                          dbm.Plots.Add(p);
                                          if (dbm.SaveChanges() > 0)
                                          {

                                              Plot_History plot = new Plot_History();
                                              plot.Address = m.Permenent_Postel_Address;
                                              plot.H_Date = m.Application_Date;
                                              plot.Name = m.Applicant_Name;
                                              plot.Phone_Number = m.Cell_No;
                                              plot.Plot_ID = p.Plot_ID;
                                              plot.Status = true;
                                              plot.CNIC = m.CNIC;
                                              plot.History_No = 1;
                                              dbm.Plot_History.Add(plot);
                                              dbm.SaveChanges();
                                              Payment_Plan plan = new Payment_Plan();
                                              if (find.Total_Price==data.Plot_Price) {
                                                  plan.Gross_Price_Plot = find.Total_Price;
                                                  plan.Net_Price_Plot = find.Total_Price;
                                                  plan.Pocession_Payment = find.Pocession;
                                                  plan.Plot_ID = plot.Plot_ID;
                                                  plan.Outstanding = 0;
                                                  dbm.Payment_Plan.Add(plan);
                                                  dbm.SaveChanges();
                                              }
                                              else
                                              {
                                                  plan.Gross_Price_Plot = data.Plot_Price;
                                                  plan.Net_Price_Plot = data.Plot_Price;
                                                  plan.Pocession_Payment = find.Pocession;
                                                  plan.Plot_ID = plot.Plot_ID;
                                                  plan.Outstanding = 0;
                                                  dbm.Payment_Plan.Add(plan);
                                                  dbm.SaveChanges();

                                              }
                                              var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Registration");
                                              var ledger_count = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Max(x => x.Ledger_Account_Code);
                                              Ledger_Account l = new Ledger_Account();
                                              l.Balance = 0;
                                              l.Element_ID = setting.Element_ID;
                                              l.Control_ID = setting.Control_ID;
                                              l.Control_Account_Code = setting.Control_Account.Control_Account_Code;
                                              l.Control_Account_Title = setting.Control_Account.Control_Account_Title;
                                              l.Element_Account_Code = setting.Element_Account.Element_Account_Code;
                                              l.Element_Account_Title = setting.Element_Account.Account_Title;
                                              l.Member_ID = m.Member_ID;
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

                                              l.Ledger_Account_Title = p.Reg_No;

                                              dbm.Ledger_Account.Add(l);
                                              dbm.SaveChanges();

                                              Installment i = new Installment();
                                              i.Payment_ID = plan.Payment_ID;
                                              i.IsPaid = false;
                                              i.Installment_No = 1;
                                              i.Amount_Paid = find.Booking;
                                              i.Comment="Booking";
                                              i.Monthly_Paid = 0;
                                              i.no_of_half_yearly_installments = find.No_Of_Half_Yearly;
                                              i.no_of_monthly_installments = find.No_Of_Monthly;
                                              i.Payment_Month =m.Application_Date;
                                              dbm.Installments.Add(i);
                                              dbm.SaveChanges();

                                        

                                              int count = 0;
                                              count =(int) find.No_Of_Half_Yearly +(int) find.No_Of_Monthly;
                                              if (find.Total_Price==data.Plot_Price) {
                                                  for (int ii = 3; ii <= count + 2; ii++)
                                                  {
                                                      if (ii % 7 != 0)
                                                      {
                                                          i.Payment_Month = i.Payment_Month.AddMonths(1);
                                                          i.Amount_Paid = i.Single_Monthly_Installment;
                                                          i.Installment_No = ii - 1;
                                                          i.Comment = "Monthly installment";
                                                          i.Monthly_Paid = 0;
                                                          dbm.Installments.Add(i);
                                                          dbm.SaveChanges();
                                                      }
                                                      else
                                                      {
                                                          if (i.Single_Half_Yearly_Installment == 0) { i.Single_Half_Yearly_Installment = i.Single_Monthly_Installment; }
                                                          i.Payment_Month = i.Payment_Month.AddMonths(1);
                                                          i.Amount_Paid = i.Single_Half_Yearly_Installment;
                                                          i.Installment_No = ii - 1;
                                                          i.Comment = "Half Yearly installment";
                                                          i.Monthly_Paid = 0;
                                                          dbm.Installments.Add(i);
                                                          dbm.SaveChanges();
                                                      }

                                                  }
                                              }
                                              else
                                              {
                                                  for (int ii = 3; ii <= count + 2; ii++)
                                                  {
                                                      double value = (double)data.Plot_Price - ( (double)find.Booking + (double)find.Pocession);
                                                      double total_remaining = value;
                                                      double percent =(double) total_remaining / (double)(find.No_Of_Half_Yearly+find.No_Of_Monthly);
                                                      if (ii % 7 != 0)
                                                      {
                                                          i.Payment_Month = i.Payment_Month.AddMonths(1);
                                                          i.Amount_Paid = percent;
                                                          i.Installment_No = ii - 1;
                                                          i.Comment = "Monthly installment";
                                                          i.Monthly_Paid = 0;
                                                          dbm.Installments.Add(i);
                                                          dbm.SaveChanges();
                                                      }
                                                      else
                                                      {
                                                          if (i.Single_Half_Yearly_Installment == 0) { i.Single_Half_Yearly_Installment = i.Single_Monthly_Installment; }
                                                          i.Payment_Month = i.Payment_Month.AddMonths(1);
                                                          i.Amount_Paid = percent;
                                                          i.Installment_No = ii - 1;
                                                          i.Comment = "Half Yearly installment";
                                                          i.Monthly_Paid = 0;
                                                          dbm.Installments.Add(i);
                                                          dbm.SaveChanges();
                                                      }
                                                  }
                                                  }
                                              i.Payment_Month = i.Payment_Month.AddMonths(1);
                                              i.Installment_No = count + 1;
                                              i.Amount_Paid = find.Pocession;
                                              i.Comment = "Pocession";
                                              i.Monthly_Paid = 0;
                                              dbm.Installments.Add(i);
                                              dbm.SaveChanges();


                                          }


                                      }
                                  }
                              }




                  }
                  return View();
              }
        public ActionResult calculate2() {
            var a = dbm.update_record.ToList();
            foreach (var z in a) {
                var regno = Convert.ToString(z.old_Reg_No);
                var update = dbm.Plots.FirstOrDefault(o=>o.Reg_No==regno);

                update.Member.Applicant_Name = z.Member_Name;
                update.Member.Cell_No = z.Mobile;
                update.Member.Office_No = z.Phone;
                update.Member.Permenent_Postel_Address = z.Address;
                update.Member.Present_Postel_Address = z.Address;
                update.Member.Father_Husband_Name = z.SO_DO_WO;
                update.Block =Convert.ToString( z.Block);
                update.Street =Convert.ToString( z.Street);
                update.Member.CNIC ="0";
                dbm.Entry(update).State = EntityState.Modified;
                dbm.SaveChanges();

            }
            
            return View(); }
          public ActionResult calculate(int id , int id1)
      {
            
          var aaa = dbm.Payment_Plan.Where(o=>o.Payment_ID>=id && o.Payment_ID<=id1).ToList();
          foreach(var database in aaa)
          {
              int iiii = Convert.ToInt32(database.Plot.Reg_No);
              var b = dbm.Sheet1_.Where(o=>o.Reg_No==iiii).ToList();
              foreach (var table in b) {
                  string t = Convert.ToString(table.Reg_No);
                  if (database.Plot.Reg_No==t) {

                      if (table.Ledger_Account == "Faysal Bank Limited")
                      {

                          double remaining_amount = 0;

                          var manualvouchernumber = table.Old_V_Number;
                         var payment_date = table.Date;
                         var firstledger =database.Plot.Member.Ledger_Account.FirstOrDefault(o=>o.Member_ID==database.Plot.Member_ID).Ledger_Complete_Code;

                          var secondledger = dbm.Ledger_Account.FirstOrDefault(o=>o.Ledger_Account_Title== "Faysal Bank Limited").Ledger_Complete_Code;
                         var firsttype = "C";
                          var secondtype = "D";
                          var Payment_mode = "BRV";
                          var Payment_ID = database.Payment_ID;
                          var checknumber = "";
                          var we = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).ID;
                          var amount = table.Amount;
                          var amount1 = amount;
                          var checkdate = DateTime.MinValue;
                          var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                          if (i.Monthly_Paid == null)
                          {
                              i.Monthly_Paid = 0;
                          }
                          if (amount == (i.Amount_Paid - i.Monthly_Paid))
                          {
                              i.IsPaid = true;
                              i.Monthly_Paid = i.Monthly_Paid + amount;
                              i.Paid_Month = payment_date;


                              dbm.Entry(i).State = EntityState.Modified;
                              dbm.SaveChanges();
                              Sub_Installment sub = new Sub_Installment();
                              sub.Installment_ID = i.Installment_ID;
                              sub.Amount = amount;
                              sub.Receipt_No =Convert.ToString( table.Old_V_Number);
                              sub.Date = (DateTime)i.Paid_Month;
                              dbm.Sub_Installment.Add(sub);
                              dbm.SaveChanges();
                          }
                          else if (amount < (i.Amount_Paid - i.Monthly_Paid))
                          {

                              i.Monthly_Paid = i.Monthly_Paid + amount;

                              i.Paid_Month = payment_date;

                              dbm.Entry(i).State = EntityState.Modified;
                              dbm.SaveChanges();
                              Sub_Installment sub = new Sub_Installment();
                              sub.Installment_ID = i.Installment_ID;
                              sub.Amount = amount;
                              sub.Receipt_No = Convert.ToString(table.Old_V_Number);
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
                                          var temp = a.Monthly_Paid;
                                          a.Monthly_Paid = a.Amount_Paid;
                                          a.IsPaid = true;
                                          remaining_amount = (double)a.Monthly_Paid - (double)temp;
                                          amount = (double)amount - ((double)a.Monthly_Paid - (double)temp);
                                      }
                                      else if (amount < (a.Amount_Paid - a.Monthly_Paid))
                                      {
                                          a.Monthly_Paid = a.Monthly_Paid + amount;
                                          remaining_amount =(double) amount;
                                          amount = (double)amount - (double)a.Monthly_Paid;
                                      }





                                      a.Paid_Month = payment_date;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      if (dbm.SaveChanges() > 0)
                                      {

                                          Sub_Installment sub = new Sub_Installment();
                                          sub.Installment_ID = a.Installment_ID;
                                          sub.Amount = remaining_amount;
                                          sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                          sub.Date = (DateTime)a.Paid_Month;
                                          dbm.Sub_Installment.Add(sub);
                                          dbm.SaveChanges();
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
                              detail.Pay_Date =(DateTime) table.Date;
                                detail.voucher_check = false;
                              detail.Head = we.ToString();
        detail.Receipt_No=manualvouchernumber;
                              dbm.Party_Detail.Add(detail);
                              dbm.SaveChanges();
                          }
                          //insert data into recept voucher table...............................................................................
                          reciept_vouchers rec = new reciept_vouchers();
                          rec.Type = Payment_mode;

                          rec.IsApproved = 1;
                          rec.IsChecked = 1;
                          rec.ChequeNumber = checknumber;
                          rec.Comments = table.Narration;
                          rec.VoucherDate = (DateTime)table.Date;
                          rec.ChequeDate = checkdate;
                          rec.Manual_Voucher_No = Convert.ToString(table.Old_V_Number);


                          rec.VoucherNum = dbm.reciept_vouchers.Max(o=>o.VoucherNum) + 1;
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
                                  pay.Comments = "Installment Paid";
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
                                  pay.Comments = "Installment Paid";
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
                                  pay1.Comments = "Installment Paid";
                                  pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                  pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                                  dbm.reciept_voucher_payment.Add(pay1);
                                  dbm.SaveChanges();
                                  var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                  a.Balance = pay1.CurrentBalance;
                                  dbm.Entry(a).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                 // return RedirectToAction("Manage_Member", "Member");

                              }
                              else
                              {//if type is D....................................................................
                                  pay1.TransType = secondtype;
                                  pay1.Comments = "Installment Paid";
                                  pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                  pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                                  dbm.reciept_voucher_payment.Add(pay1);
                                  dbm.SaveChanges();
                                  var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                  a.Balance = pay1.CurrentBalance;
                                  dbm.Entry(a).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                 // return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });

                              }






                          }
                      }
                    else if (table.Ledger_Account == "Cash")
                      {
                          {

                              double remaining_amount = 0;

                              var manualvouchernumber = table.Old_V_Number;
                              var payment_date = table.Date;
                              var firstledger = database.Plot.Member.Ledger_Account.FirstOrDefault(o => o.Member_ID == database.Plot.Member_ID).Ledger_Complete_Code;

                              var secondledger = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Account_Title == "Cash").Ledger_Complete_Code;
                              var firsttype = "C";
                              var secondtype = "D";
                              var Payment_mode = "CRV";
                              var Payment_ID = database.Payment_ID;
                              var checknumber = "";
                              var we = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).ID;
                              var amount = table.Amount;
                              var amount1 = amount;
                              var checkdate = DateTime.MinValue;
                              var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                              if (i.Monthly_Paid == null)
                              {
                                  i.Monthly_Paid = 0;
                              }
                              if (amount == (i.Amount_Paid - i.Monthly_Paid))
                              {
                                  i.IsPaid = true;
                                  i.Monthly_Paid = i.Monthly_Paid + amount;
                                  i.Paid_Month = payment_date;


                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                  sub.Date = (DateTime)i.Paid_Month;
                                  dbm.Sub_Installment.Add(sub);
                                  dbm.SaveChanges();
                              }
                              else if (amount < (i.Amount_Paid - i.Monthly_Paid))
                              {

                                  i.Monthly_Paid = i.Monthly_Paid + amount;

                                  i.Paid_Month = payment_date;

                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
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
                                              sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                              sub.Date = (DateTime)a.Paid_Month;
                                              dbm.Sub_Installment.Add(sub);
                                              dbm.SaveChanges();
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
                                  detail.Pay_Date = (DateTime)table.Date;
                                  detail.Head = we.ToString(); detail.voucher_check = false;
                                    detail.Receipt_No = manualvouchernumber;
                                    dbm.Party_Detail.Add(detail);
                                  dbm.SaveChanges();
                              }
                              //insert data into recept voucher table...............................................................................
                              reciept_vouchers rec = new reciept_vouchers();
                              rec.Type = Payment_mode;

                              rec.IsApproved = 1;
                              rec.IsChecked = 1;
                              rec.ChequeNumber = checknumber;
                                rec.Comments = table.Narration;
                                rec.VoucherDate = (DateTime)table.Date;
                              rec.ChequeDate = checkdate;
                              rec.Manual_Voucher_No = Convert.ToString(table.Old_V_Number);


                                rec.VoucherNum = dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1;
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
                                      pay.Comments = "Installment Paid";
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
                                      pay.Comments = "Installment Paid";
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
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("Manage_Member", "Member");

                                  }
                                  else
                                  {//if type is D....................................................................
                                      pay1.TransType = secondtype;
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });

                                  }






                              }
                          }
                      }
                      else if (table.Ledger_Account == "Askari Bank Limited")
                      {
                          {

                              double remaining_amount = 0;

                              var manualvouchernumber = table.Old_V_Number;
                              var payment_date = table.Date;
                              var firstledger = database.Plot.Member.Ledger_Account.FirstOrDefault(o => o.Member_ID == database.Plot.Member_ID).Ledger_Complete_Code;

                              var secondledger = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Account_Title == "Askari Bank Limited").Ledger_Complete_Code;
                              var firsttype = "C";
                              var secondtype = "D";
                              var Payment_mode = "BRV";
                              var Payment_ID = database.Payment_ID;
                              var checknumber = "";
                              var we = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).ID;
                              var amount = table.Amount;
                              var amount1 = amount;
                              var checkdate = DateTime.MinValue;
                              var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                              if (i.Monthly_Paid == null)
                              {
                                  i.Monthly_Paid = 0;
                              }
                              if (amount == (i.Amount_Paid - i.Monthly_Paid))
                              {
                                  i.IsPaid = true;
                                  i.Monthly_Paid = i.Monthly_Paid + amount;
                                  i.Paid_Month = payment_date;


                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                  sub.Date = (DateTime)i.Paid_Month;
                                  dbm.Sub_Installment.Add(sub);
                                  dbm.SaveChanges();
                              }
                              else if (amount < (i.Amount_Paid - i.Monthly_Paid))
                              {

                                  i.Monthly_Paid = i.Monthly_Paid + amount;

                                  i.Paid_Month = payment_date;

                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
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
                                              sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                              sub.Date = (DateTime)a.Paid_Month;
                                              dbm.Sub_Installment.Add(sub);
                                              dbm.SaveChanges();
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
                                  detail.Pay_Date = (DateTime)table.Date;
                                  detail.Head = we.ToString(); detail.voucher_check = false;
                                    detail.Receipt_No = manualvouchernumber;
                                    dbm.Party_Detail.Add(detail);
                                  dbm.SaveChanges();
                              }
                              //insert data into recept voucher table...............................................................................
                              reciept_vouchers rec = new reciept_vouchers();
                              rec.Type = Payment_mode;

                              rec.IsApproved = 1;
                              rec.IsChecked = 1;
                              rec.ChequeNumber = checknumber;
                                rec.Comments = table.Narration;
                                rec.VoucherDate = (DateTime)table.Date;
                              rec.ChequeDate = checkdate;
                              rec.Manual_Voucher_No = Convert.ToString(table.Old_V_Number);


                                rec.VoucherNum = dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1;
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
                                      pay.Comments = "Installment Paid";
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
                                      pay.Comments = "Installment Paid";
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
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("Manage_Member", "Member");

                                  }
                                  else
                                  {//if type is D....................................................................
                                      pay1.TransType = secondtype;
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });

                                  }






                              }
                          }
                      }
                      else if (table.Ledger_Account == "Pay Order")
                      {
                          {

                              double remaining_amount = 0;

                              var manualvouchernumber = table.Old_V_Number;
                              var payment_date = table.Date;
                              var firstledger = database.Plot.Member.Ledger_Account.FirstOrDefault(o => o.Member_ID == database.Plot.Member_ID).Ledger_Complete_Code;

                              var secondledger = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Account_Title == "Pay Order").Ledger_Complete_Code;
                              var firsttype = "C";
                              var secondtype = "D";
                              var Payment_mode = "BRV";
                              var Payment_ID = database.Payment_ID;
                              var checknumber = "";
                              var we = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).ID;
                              var amount = table.Amount;
                              var amount1 = amount;
                              var checkdate = DateTime.MinValue;
                              var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                              if (i.Monthly_Paid == null)
                              {
                                  i.Monthly_Paid = 0;
                              }
                              if (amount == (i.Amount_Paid - i.Monthly_Paid))
                              {
                                  i.IsPaid = true;
                                  i.Monthly_Paid = i.Monthly_Paid + amount;
                                  i.Paid_Month = payment_date;


                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                  sub.Date = (DateTime)i.Paid_Month;
                                  dbm.Sub_Installment.Add(sub);
                                  dbm.SaveChanges();
                              }
                              else if (amount < (i.Amount_Paid - i.Monthly_Paid))
                              {

                                  i.Monthly_Paid = i.Monthly_Paid + amount;

                                  i.Paid_Month = payment_date;

                                  dbm.Entry(i).State = EntityState.Modified;
                                  dbm.SaveChanges();
                                  Sub_Installment sub = new Sub_Installment();
                                  sub.Installment_ID = i.Installment_ID;
                                  sub.Amount = amount;
                                  sub.Receipt_No = Convert.ToString(table.Old_V_Number);
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
                                              sub.Receipt_No = Convert.ToString(table.Old_V_Number);
                                              sub.Date = (DateTime)a.Paid_Month;
                                              dbm.Sub_Installment.Add(sub);
                                              dbm.SaveChanges();
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
                                  detail.Pay_Date = (DateTime)table.Date;
                                  detail.Head = we.ToString(); detail.voucher_check = false;
                                    detail.Receipt_No = manualvouchernumber;
                                    dbm.Party_Detail.Add(detail);
                                  dbm.SaveChanges();
                              }
                              //insert data into recept voucher table...............................................................................
                              reciept_vouchers rec = new reciept_vouchers();
                              rec.Type = Payment_mode;

                              rec.IsApproved = 1;
                              rec.IsChecked = 1;
                              rec.ChequeNumber = checknumber;
                                rec.Comments = table.Narration;
                                rec.VoucherDate = (DateTime)table.Date;
                              rec.ChequeDate = checkdate;
                              rec.Manual_Voucher_No = Convert.ToString(table.Old_V_Number);


                                rec.VoucherNum = dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1;
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
                                      pay.Comments = "Installment Paid";
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
                                      pay.Comments = "Installment Paid";
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
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("Manage_Member", "Member");

                                  }
                                  else
                                  {//if type is D....................................................................
                                      pay1.TransType = secondtype;
                                      pay1.Comments = "Installment Paid";
                                      pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                                      pay1.CurrentBalance = pay1.PreviousBalance + amount1;
                                      dbm.reciept_voucher_payment.Add(pay1);
                                      dbm.SaveChanges();
                                      var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                                      a.Balance = pay1.CurrentBalance;
                                      dbm.Entry(a).State = EntityState.Modified;
                                      dbm.SaveChanges();
                                      // return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });

                                  }






                              }
                          }
                      }
                  }
                    dbm.Sheet1_.Remove(table);
                    dbm.SaveChanges();
                }
          }


          return View();
      }
        public ActionResult run()
        {
            var a = dbm.Members.ToList();
            var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Registration");
           
            foreach (var aa in a) {
                var ledger_count = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Max(x => x.Ledger_Account_Code);
                Ledger_Account l = new Ledger_Account();
                l.Balance = 0;
                l.Element_ID = setting.Element_ID;
                l.Control_ID = setting.Control_ID;
                l.Control_Account_Code = setting.Control_Account.Control_Account_Code;
                l.Control_Account_Title = setting.Control_Account.Control_Account_Title;
                l.Element_Account_Code = setting.Element_Account.Element_Account_Code;
                l.Element_Account_Title = setting.Element_Account.Account_Title;
                l.Member_ID = aa.Member_ID;
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

                l.Ledger_Account_Title = aa.Plots.FirstOrDefault(o=>o.Member_ID==aa.Member_ID).Reg_No;

                dbm.Ledger_Account.Add(l);
                dbm.SaveChanges();
            }
            return RedirectToAction("Manage_Shifts", "Shifts");


        }
        public ActionResult Add_Shift()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Shifts");
                ViewBag.Key = s.Target;
            }

            var ss = dbm.Shifts.FirstOrDefault(x => x.Shift_ID == 0);
            return View(ss);
        }
        [HttpPost]
        public ActionResult Add_Shift(FormCollection form)
        {

            Shift e = new Shift();
            e.Shift_Name = form["Shift_Name"];
            e.StartTime = TimeSpan.Parse(form["StartTime"]);
            e.EndTime = TimeSpan.Parse(form["EndTime"]);

            TimeSpan TS = TimeSpan.Parse(form["EndTime"]) - TimeSpan.Parse(form["StartTime"]);
            int hour = TS.Hours;
            e.WorkHours = hour;
            dbm.Shifts.Add(e);
            dbm.SaveChanges();
            return RedirectToAction("Manage_Shifts", "Shifts");

        }
        public ActionResult Update_Shift(int ID = 0)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Shifts");
                ViewBag.Key = s.Target;
            }
            Shift ss = new Shift();

            ss = dbm.Shifts.FirstOrDefault(x => x.Shift_ID == ID);
            return View(ss);

        }

        [HttpPost]
        public ActionResult Update_Shift(FormCollection form)
        {
            int id = Convert.ToInt32(form["Shift_ID"]);
            var s = dbm.Shifts.FirstOrDefault(o => o.Shift_ID == id);
            s.Shift_Name = form["Shift_Name"];

            s.StartTime = TimeSpan.Parse(form["StartTime"]);
            s.EndTime = TimeSpan.Parse(form["EndTime"]);
            TimeSpan TS = TimeSpan.Parse(form["EndTime"]) - TimeSpan.Parse(form["StartTime"]);
            int hour = TS.Hours;
            s.WorkHours = hour;
            dbm.Entry(s).State = EntityState.Modified;
            dbm.SaveChanges();
            //Update code

            return RedirectToAction("Manage_Shifts", "Shifts");
        }
        public ActionResult delete(int id)
        {
            var a = dbm.Shifts.FirstOrDefault(o => o.Shift_ID == id);
            dbm.Shifts.Remove(a);
            dbm.SaveChanges();

            return RedirectToAction("Manage_Shifts", "Shifts");
        }
    }
}