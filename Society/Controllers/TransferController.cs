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
    public class TransferController : Controller
    {

        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Transfer
        public ActionResult Transfer()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage File Transfer");
                ViewBag.Key = s.Target;
            }
            var a = dbm.Payment_Plan.Where(o=>o.Plot.Member.Status==true).ToList();
            return View(a);
        }
        class Item
        {
            public string second { get; set; }
            public double amount { get; set; }
        }
        

        [HttpPost]
        public ActionResult Transfer(string first, string[] references, double[] amounts)
        {
            List<string> referenceList = new List<string>();
            referenceList.Add(first);
            referenceList.AddRange(references);
            var allExist = referenceList.All(o => dbm.Plots.Select(l => l.Reg_No).Contains(o));
            if (!allExist)
            {
                ViewBag.Error = "Ref not found or entered amount not same!!!";
                return View();

            }
            var isAmountSame = dbm.Installments.Where(o=>o.Payment_Plan.Plot.Reg_No == first).Sum(o=>o.Monthly_Paid).Value.Equals(amounts.Sum());
            if (!allExist || !isAmountSame) 
            {
                ViewBag.Error = "Ref not found or entered amount not same!!!";
                return View();
            }
            double remaining_amount = 0;
            var zippedList = references.Zip(amounts, (reference, amount) => new Item{ second = reference, amount = amount });
            foreach (var item in zippedList)
            {
                var from = dbm.Payment_Plan.FirstOrDefault(o => o.Plot.Reg_No == first);

                var member = dbm.Members.FirstOrDefault(o => o.Member_ID == from.Plot.Member_ID);
                member.Status = false;

                dbm.Entry(member).State = EntityState.Modified;
                dbm.SaveChanges();
                var find = dbm.Parties.First(o => o.Member_ID == member.Member_ID);
                var find1 = dbm.Party_Detail.Where(o => o.Party_ID == find.ID).ToList();

                var to = dbm.Payment_Plan.FirstOrDefault(o => o.Plot.Reg_No == item.second);
                
                var amount1 = item.amount;
                var transfer1 = amount1;
                var update_ledger = dbm.Ledger_Account.FirstOrDefault(o => o.Member_ID == from.Plot.Member_ID);
                var update_ledger1 = dbm.Ledger_Account.FirstOrDefault(o => o.Member_ID == to.Plot.Member_ID);
                update_ledger1.Balance = update_ledger1.Balance + (item.amount * -1);
                dbm.Entry(update_ledger1).State = EntityState.Modified;
                dbm.SaveChanges();
                update_ledger.Balance =update_ledger.Balance + item.amount;
                var we1 = to.Plot.Member.Ledger_Account.FirstOrDefault(a => a.Member_ID == to.Plot.Member_ID).Ledger_Complete_Code;
                var we = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == we1).ID;
                dbm.Entry(update_ledger).State = EntityState.Modified;
                dbm.SaveChanges();
                reciept_vouchers rec = new reciept_vouchers();
                var num = dbm.reciept_vouchers.Max(o => o.VoucherNum);
                num = num + 1;
                rec.VoucherNum = num;
                rec.Manual_Voucher_No = num + "/" + DateTime.Today.Year + "/" + "MERG";
                rec.Comments = "Payment Transfer";
                rec.IsApproved = 1;
                rec.IsChecked = 1;
                rec.Type = "JV";
                rec.VoucherDate = DateTime.Today;
                dbm.reciept_vouchers.Add(rec);
                dbm.SaveChanges();
                reciept_voucher_payment pay = new reciept_voucher_payment();
                pay.RV_ID = rec.RV_ID;
                pay.Amount = (double)amount1;
                string[] arrey = update_ledger.Ledger_Complete_Code.Split('.');
                pay.Element_Account_Code = Convert.ToInt32(arrey[0]);
                pay.Control_Account_Code = Convert.ToInt32(arrey[1]);
                pay.Ledger_Account_Code = Convert.ToInt32(arrey[2]);
                pay.Ledger_ID = dbm.Ledger_Account.FirstOrDefault(o => o.Element_Account_Code == pay.Element_Account_Code && o.Control_Account_Code == pay.Control_Account_Code && o.Ledger_Account_Code == pay.Ledger_Account_Code).ID;
                //if type is C....................................................................

                pay.TransType = "C";
                pay.Comments = "Payment Transfer";
                pay.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == update_ledger.Ledger_Complete_Code).Balance - item.amount;
                pay.CurrentBalance =pay.PreviousBalance + item.amount;
                dbm.reciept_voucher_payment.Add(pay);
                dbm.SaveChanges();
                // var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger);
                ///  a.Balance = pay.CurrentBalance;
                //  dbm.Entry(a).State = EntityState.Modified;
                //  dbm.SaveChanges();
                reciept_voucher_payment pay1 = new reciept_voucher_payment();
                pay1.RV_ID = rec.RV_ID;
                pay1.Amount = (double)amount1;
                string[] arrey1 = update_ledger1.Ledger_Complete_Code.Split('.');
                pay1.Element_Account_Code = Convert.ToInt32(arrey1[0]);
                pay1.Control_Account_Code = Convert.ToInt32(arrey1[1]);
                pay1.Ledger_Account_Code = Convert.ToInt32(arrey1[2]);
                pay1.Ledger_ID = dbm.Ledger_Account.FirstOrDefault(o => o.Element_Account_Code == pay1.Element_Account_Code && o.Control_Account_Code == pay1.Control_Account_Code && o.Ledger_Account_Code == pay1.Ledger_Account_Code).ID;
                //if type is C....................................................................

                pay1.TransType = "D";
                pay1.Comments = "Payment Transfer";
                pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == update_ledger1.Ledger_Complete_Code).Balance + amount1;
                pay1.CurrentBalance = pay1.PreviousBalance - amount1;
                dbm.reciept_voucher_payment.Add(pay1);
                dbm.SaveChanges();
                var i = dbm.Installments.Where(o => o.Payment_ID == to.Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                if (i.Monthly_Paid == null)
                {
                    i.Monthly_Paid = 0;
                }
                var memberid1 = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == to.Payment_ID);

                var party1 = dbm.Parties.FirstOrDefault(o => o.Member_ID == memberid1.Plot.Member_ID);
                var temp_balance1 = party1.Balance;



 
                    amount1 = (double)(item.amount);
                    if (item.amount == (i.Amount_Paid - i.Monthly_Paid))
                    {
                        i.IsPaid = true;
                        i.Monthly_Paid = i.Monthly_Paid + item.amount;
                        i.Paid_Month = DateTime.Now;


                        dbm.Entry(i).State = EntityState.Modified;
                        dbm.SaveChanges();
                        Sub_Installment sub = new Sub_Installment();
                        sub.Installment_ID = i.Installment_ID;
                        sub.Amount = item.amount;
                        sub.Receipt_No = $"Transfer from {from.Plot.Reg_No}";
                        sub.Date = (DateTime)i.Paid_Month;
                        dbm.Sub_Installment.Add(sub);
                        dbm.SaveChanges();




                    }
                    else if (item.amount < (i.Amount_Paid - i.Monthly_Paid))
                    {

                        i.Monthly_Paid = i.Monthly_Paid + item.amount;

                        i.Paid_Month = DateTime.Now;


                        dbm.Entry(i).State = EntityState.Modified;
                        dbm.SaveChanges();
                        Sub_Installment sub = new Sub_Installment();
                        sub.Installment_ID = i.Installment_ID;
                        sub.Amount = item.amount;
                        sub.Receipt_No = $"Transfer from {from.Plot.Reg_No}";
                        sub.Date = (DateTime)i.Paid_Month;
                        dbm.Sub_Installment.Add(sub);
                        dbm.SaveChanges();

                    }
                    else if (item.amount > (i.Amount_Paid - i.Monthly_Paid))
                    {
                        var iii = dbm.Installments.Where(o => o.Payment_ID == to.Payment_ID).ToList().OrderBy(o => o.Installment_No);
                        var last = iii.Last();
                        foreach (var a in iii)
                        {
                            if (a.Monthly_Paid == null) { a.Monthly_Paid = 0; }
                            if (item.amount > 0)
                            {
                                if (item.amount >= (a.Amount_Paid - a.Monthly_Paid))
                                {
                                    var temp = a.Monthly_Paid;
                                    a.Monthly_Paid = a.Amount_Paid;
                                    a.IsPaid = true;
                                    remaining_amount = (double)a.Monthly_Paid - (double)temp;
                                    item.amount = (double)item.amount - ((double)a.Monthly_Paid - (double)temp);
                                }
                                else if (item.amount < (a.Amount_Paid - a.Monthly_Paid))
                                {
                                    a.Monthly_Paid = a.Monthly_Paid + item.amount;
                                    remaining_amount = (double)item.amount;
                                    item.amount = (double)item.amount - (double)a.Monthly_Paid;
                                }





                                a.Paid_Month = DateTime.Now;
                                dbm.Entry(a).State = EntityState.Modified;
                                if (dbm.SaveChanges() > 0)
                                {

                                    Sub_Installment sub = new Sub_Installment();
                                    sub.Installment_ID = a.Installment_ID;
                                    sub.Amount = remaining_amount;
                                    sub.Receipt_No = $"Transfer from {from.Plot.Reg_No}";
                                    sub.Date = (DateTime)a.Paid_Month;
                                    dbm.Sub_Installment.Add(sub);
                                    dbm.SaveChanges();
                                }


                            }


                        }
                        if (item.amount > 0)
                        {
                            var last1 = dbm.Installments.FirstOrDefault(o => o.Installment_ID == last.Installment_ID);
                            last1.Monthly_Paid = last1.Monthly_Paid + item.amount;
                            dbm.Entry(last1).State = EntityState.Modified;
                            dbm.SaveChanges();
                        }

                    }



                    var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == to.Payment_ID);

                    var party = dbm.Parties.FirstOrDefault(o => o.Member_ID == memberid.Plot.Member_ID);

                    var temp_balance = party.Balance;

                    party.Balance = party.Balance + amount1;
                    dbm.Entry(party).State = EntityState.Modified;
                    if (dbm.SaveChanges() > 0)
                    {



                    }







                    item.amount = 0;
                
                Party_Detail detail = new Party_Detail();
                detail.UserId = Convert.ToInt32(Session["emp_id"]);
                detail.Party_ID = party1.ID;
                detail.Previous_Amount = temp_balance1;
                detail.Current_Amount = detail.Previous_Amount + transfer1;
                detail.Pay_Date = DateTime.Today;
                detail.Receipt_No = num + "/" + DateTime.Today.Year + "/" + "MERG";
                detail.Check_No = "TRANSFER FROM EB-" + first + " TO EB-" + item.second + "";
                detail.voucher_check = false;
                detail.Head = Convert.ToString(member.Ledger_Account.First(o => o.Member_ID == member.Member_ID).ID);
                dbm.Party_Detail.Add(detail);
                dbm.SaveChanges();
                Payment_Transfer p = new Payment_Transfer();
                p.Amount = transfer1;
                p.From_EB = first;
                p.To_EB = item.second;
                p.Date = DateTime.Now;
                dbm.Payment_Transfer.Add(p);
                dbm.SaveChanges();

            }
            return RedirectToAction("Transfer", "Transfer");
        }
        public JsonResult sending(FormCollection form) {
            var reg_no = form["reg_no"];

            var chk = dbm.Payment_Plan.Where(o => o.Plot.Reg_No == reg_no && o.Plot.Member.Status == true).FirstOrDefault();
            if (
                chk==null)
            {
                var check = "false";
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var a = dbm.Payment_Plan.Where(o => o.Plot.Reg_No == reg_no && o.Plot.Member.Status == true).Select(o => new
                {
                    name = o.Plot.Member.Applicant_Name,
                    Net_Price_Plot = o.Net_Price_Plot,
                    Paid_amount = o.Installments.Sum(i => i.Monthly_Paid),
                    Remaining_amount = o.Installments.Sum(u => u.Amount_Paid) - o.Installments.Sum(j => j.Monthly_Paid)
                });


                var json = JsonConvert.SerializeObject(a);
               return Json(json, JsonRequestBehavior.AllowGet); 
                
            }
           

            }


        public JsonResult receiving1(FormCollection form)
        {
            var reg_no1 = form["reg_no1"];
            var a = dbm.Payment_Plan.Where(o => o.Plot.Reg_No == reg_no1 && o.Plot.Member.Status == true).Select(o => new
            {
                name = o.Plot.Member.Applicant_Name,
                Net_Price_Plot = o.Net_Price_Plot,
                Paid_amount = o.Installments.Sum(i => i.Monthly_Paid),
                Remaining_amount = o.Installments.Sum(u => u.Amount_Paid) - o.Installments.Sum(j => j.Monthly_Paid)
            });
            var json = JsonConvert.SerializeObject(a);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
     
    }
}