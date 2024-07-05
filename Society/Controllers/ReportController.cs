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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Society.Controllers
{
    public class ReportController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        
        public ActionResult single_defaulter()
        {
            return View();
        }
        public ActionResult defaulter(string search1)
        {
            
                var a = dbm.Payment_Plan.Where(o => o.Plot.Reg_No == search1).Select(o => new
                {
                    o.Plot.Reg_No,
                    o.Plot.Member.Applicant_Name,
                    Net = o.Net_Price_Plot,
                    Cell_No = o.Plot.Member.Cell_No,
                    Paid_amount = o.Installments.Sum(q => q.Monthly_Paid),
                    due = o.Installments.Sum(q => q.Amount_Paid) - o.Installments.Sum(m => m.Monthly_Paid),
                    defaulter = o.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(q => q.Amount_Paid) - o.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid),
                    last_paid =  o.Plot.Member.Parties.FirstOrDefault(r => r.Member_ID == o.Plot.Member_ID).Party_Detail.Where(t => t.Party_ID == o.Plot.Member.Parties.FirstOrDefault(p=>p.Member_ID==o.Plot.Member_ID).ID).OrderByDescending(y => y.Pay_Date).FirstOrDefault().Current_Amount - o.Plot.Member.Parties.FirstOrDefault(r => r.Member_ID == o.Plot.Member_ID).Party_Detail.Where(t => t.Party_ID == o.Plot.Member.Parties.FirstOrDefault(p => p.Member_ID == o.Plot.Member_ID).ID).OrderByDescending(y => y.Pay_Date).FirstOrDefault().Previous_Amount,
                    last_paid_date = o.Plot.Member.Parties.FirstOrDefault(r => r.Member_ID == o.Plot.Member_ID).Party_Detail.Where(t => t.Party_ID == o.Plot.Member.Parties.FirstOrDefault(p => p.Member_ID == o.Plot.Member_ID).ID).OrderByDescending(y => y.Pay_Date).FirstOrDefault().Pay_Date,
                    o.Pocession_Payment
                    
                });
                var json = JsonConvert.SerializeObject(a);
                return Json(json, JsonRequestBehavior.AllowGet);

        

        }
        public ActionResult find_next(int next)
        {
            var a = dbm.Payment_Plan.Select(o => new
            {
                o.Plot.Reg_No,
                o.Plot.Member.Applicant_Name,
                Net = o.Net_Price_Plot,
                Cell_No = o.Plot.Member.Cell_No,
                Paid_amount = o.Installments.Sum(q => q.Monthly_Paid),
                due = o.Installments.Sum(z => z.Amount_Paid) - o.Installments.Sum(m => m.Monthly_Paid),
                defaulter = o.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(z => z.Amount_Paid) - o.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid)
            }).Skip(next).Take(10).OrderBy(o=>o.Reg_No);
            var json = JsonConvert.SerializeObject(a);
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public ActionResult find_previous(int previous)
        {
            var a = dbm.Payment_Plan.Select(o => new
            {
                o.Plot.Reg_No,
                o.Plot.Member.Applicant_Name,
                Net = o.Net_Price_Plot,
                Cell_No = o.Plot.Member.Cell_No,
                Paid_amount = o.Installments.Sum(q => q.Monthly_Paid),
                due = o.Installments.Sum(z => z.Amount_Paid) - o.Installments.Sum(m => m.Monthly_Paid),
                defaulter = o.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(z => z.Amount_Paid) - o.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid)
            }).Skip(previous).Take(10);
            var json = JsonConvert.SerializeObject(a);
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        // GET: Report
        public ActionResult Defaulter_Report()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Payment_Plan.ToList().Where(o=>o.Plot.Member.Status==true);
            
            return View(a); }
        public async Task<ActionResult> send_message()
        {
            var aa =await dbm.Payment_Plan.Where(o => o.Plot.Member.Status == true).ToListAsync();
            foreach (var x in aa)

            {
                var find_last_amount1 = x.Installments.Where(o => o.Monthly_Paid > 0).ToList();
                if (find_last_amount1.Count > 0)
                {


                    var find_last_amount = x.Installments.Where(o => o.Monthly_Paid > 0).Last();
                    if (find_last_amount != null)
                    {
                        var mem_ID = find_last_amount.Payment_Plan.Plot.Member.Member_ID;
                        var party_ID = find_last_amount.Payment_Plan.Plot.Member.Parties.First(o => o.Member_ID == mem_ID).ID;
                        var a = find_last_amount.Payment_Plan.Plot.Member.Parties.First(o => o.Member_ID == mem_ID).Party_Detail.Where(o => o.Party_ID == party_ID).OrderBy(o => o.ID).Last();
                        double amount = (double)a.Current_Amount - (double)a.Previous_Amount;

                        var first = x.Installments.Where(o => o.Payment_Month <= DateTime.Today).Sum(i => i.Amount_Paid);
                        var first1 = x.Installments.Sum(i => i.Amount_Paid);
                        double second = (double)x.Installments.Where(o => o.Payment_Month <= DateTime.Today).Sum(i => i.Monthly_Paid);
                        double paid_amount = (double)x.Installments.Sum(i => i.Monthly_Paid);
                        double third = (double)first - second;

                        double remaining = (double)first1 - paid_amount;
                        double net = (double)x.Net_Price_Plot;
                        if (third <= 0) { }
                        else
                        { string phone = x.Plot.Member.Cell_No;
                            string eb = x.Plot.Reg_No;
                            saa(phone,third, eb) ;

                        }
                    }
                }



            }

            return View();
        }
        public ActionResult saa(string Phone_no,double Display_amount,string EB_number)
        {
            var token = Session["Token"].ToString();
            try
            {
                HttpWebRequest myReq;
                HttpWebResponse myResp;
                StreamReader myReader;
                myReq = (HttpWebRequest)HttpWebRequest.Create("https://portal.zekli.com:9090/api/Dashboard/GetBalance");
                myReq.Method = "GET";
                myReq.ContentType = "application/json";
                myReq.Accept = "application/json";
                myReq.Headers.Add("Authorization", "Bearer " + token);
                myResp = (HttpWebResponse)myReq.GetResponse();
                myReader = new System.IO.StreamReader(myResp.GetResponseStream());
                var reader = myReader.ReadToEnd();
                var data = JObject.Parse(reader);
                var balance = Convert.ToInt32(data["data"]["balanceSMS"]);
                if (balance > 0)
                {
                    try
                    {
                        HttpWebRequest myReq1;
                        HttpWebResponse myResp1;
                        StreamReader myReader1;
                        myReq1 = (HttpWebRequest)WebRequest.Create("https://portal.zekli.com:9090/api/SMS");
                        myReq1.Method = "POST";
                        myReq1.ContentType = "application/json";
                        myReq1.Accept = "application/json";
                        string aa = "Dear Customer, a Total Amount of Rs. " + Display_amount.ToString("n0") + "is Due Against Your Plot, EB-" + EB_number + ". Kindly Clear the Pending Payment at Your Earlier, Thank You...";

                        string no = Phone_no;
                        string result = no.Substring(0, 3);
                        string netid = "";
                        if (result == "030" || result == "032")
                        {
                            netid = "14";
                        }
                        else if (result == "034")
                        {
                            netid = "15";
                        }
                        else if (result == "031")
                        {
                            netid = "16";
                        }
                        else if (result == "033")
                        {
                            netid = "17";
                        }

                        myReq1.Headers.Add("Authorization", "Bearer " + token);
                        string myData = "{\"deductionTypeId\":\"8\",\"maskId\":\"83\",\"lstSMS\":[{\"mobileNetworkId\":\"" + netid + "\",\"phoneNumber\":\"" + no + "\",\"message\":\"" + aa + "\"}]}";
                        myReq1.GetRequestStream().Write(System.Text.Encoding.UTF8.GetBytes(myData), 0, System.Text.Encoding.UTF8.GetBytes(myData).Count());
                        myResp1 = (HttpWebResponse)myReq1.GetResponse();
                        myReader1 = new StreamReader(myResp1.GetResponseStream());
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {

                        return View(ex);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(ex);
            }






        }

        public ActionResult Member_Report()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Payment_Plan.ToList();
            return View(a);
        }



        public ActionResult Member_DealerCommisionReport()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.dealer_data.ToList();
            return View(a);
        }
        public ActionResult Receipt_Register()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();

            var a = dbm.Party_Detail.ToList().Take(100);
            return View(a);
        }
        public ActionResult Installment_Report()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Payment_Plan.ToList().Take(100);
            return View(a);
        }
        public ActionResult Monthly_Paid()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Installments.Where(o=>o.Installment_ID==0).ToList();
            return View(a);
        }
        [HttpPost]
        public ActionResult Monthly_Paid(FormCollection form)
        {
            var month =Convert.ToDateTime( form["month"]);
        
            var chk = form["chk"];
            if (chk=="false") {

                var a = dbm.Installments.Where(o => o.Payment_Month.Month == month.Month && o.Payment_Month.Year == month.Year && o.IsPaid == false).Select(o=>new 
                {
                    Applicant_Name=  o.Payment_Plan.Plot.Member.Applicant_Name,
                    CNIC=  o.Payment_Plan.Plot.Member.CNIC,
                    Net_Price=o.Payment_Plan.Net_Price_Plot,
                    Month=o.Payment_Month,
                    Installment_No= o.Installment_No,
                    Amount=o.Monthly_Paid,
                    unpaid="unpaid"
                }
                ).ToList().Take(200);
             
               
                var json = JsonConvert.SerializeObject(a);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var a = dbm.Installments.Where(o => o.Payment_Month.Month == month.Month && o.Payment_Month.Year == month.Year && o.IsPaid == true).Select(o => new
                {
                    Applicant_Name = o.Payment_Plan.Plot.Member.Applicant_Name,
                    CNIC = o.Payment_Plan.Plot.Member.CNIC,
                    Net_Price = o.Payment_Plan.Net_Price_Plot,
                    Month = o.Payment_Month,
                    Installment_No = o.Installment_No,
                    Amount = o.Amount_Paid,
                    unpaid = "paid"
                }
                ).ToList().Take(200) ;
             
                var json = JsonConvert.SerializeObject(a);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
           
        }
        public ActionResult Trial_Balance(FormCollection form) {

            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();
            dy.ledger_list = dbm.Ledger_Account.ToList();
            dy.element_list = dbm.Element_Account.ToList();
            dy.control_list = dbm.Control_Account.ToList();
            return View(dy);
        
        
        }


        public ActionResult LedgerReport()
        {

            if (Session["emp_id"] != null)
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.Where(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
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
            ViewBag.ledgerAccount = dbm.Ledger_Account.ToList();
            return View();


        }
        [HttpPost]
        public ActionResult LedgerReport(string dateRange, int ledger)
        {

            DateTime from = Convert.ToDateTime(dateRange.Split('-')[0]);
            DateTime to = Convert.ToDateTime(dateRange.Split('-')[1]);

            if (Session["emp_id"] != null)
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.Where(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Reports");
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
            var record = dbm.reciept_voucher_payment.Where(o => o.Ledger_ID == ledger
            ).Where(o => o.reciept_vouchers.VoucherDate >= from && o.reciept_vouchers.VoucherDate <= to).ToList().OrderBy(o => o.reciept_vouchers.VoucherDate
            );
            ViewBag.record = record;
            ViewBag.openingBalance = record.FirstOrDefault()?.PreviousBalance;
            ViewBag.closingBalance = record.LastOrDefault()?.CurrentBalance;
            ViewBag.range = $"{from.ToString("yyyy-MM-dd")} - {to.ToString("yyyy-MM-dd")}";
            ViewBag.title = record.FirstOrDefault()?.Ledger_Account?.Ledger_Account_Title;
            ViewBag.ledgerAccount = dbm.Ledger_Account.ToList();
            return View();


        }

    }
}