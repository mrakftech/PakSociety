using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using DPFP.Capture;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using static Society.Controllers.ThumbScanController;
using System.Web.Razor.Parser.SyntaxTree;
using System.Runtime;
using System.Data.Entity.Migrations;

namespace Society.Controllers
{
    public class MemberController : Controller
    {

        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        double balance1;
        string App_name;
        double display_amount;
        double total_paid;
        string EB_number;
        string Phone_no;

        // GET: Member
        [HttpPost]
        public JsonResult Index(string Prefix)
        {
            //Note : you can bind same list from database

            //Searching records from list using LINQ query
            var Name1 = dbm.Members.Where(o => o.CNIC.StartsWith(Prefix)).ToList().Take(100);

            if (Name1 != null)
            {
                var Name = dbm.Members.Where(o => o.CNIC.StartsWith(Prefix)).Select(o => new { o.CNIC, o.Applicant_Name, o.Cell_No, o.Email, o.Father_Husband_Name, o.Nominee_Address, o.Nominee_CNIC, o.Nominee_Name, o.NomineeFather, o.Country, o.Nominee_Phone_No, o.Occupation, o.Office_No, o.Permenent_Postel_Address, o.Present_Postel_Address, o.Relation }).ToList().Take(100);
                return Json(Name, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult Index1(string Prefix)
        {
            //Note : you can bind same list from database

            //Searching records from list using LINQ query
            var Name1 = dbm.Dealers.Where(o => o.Company_Name.StartsWith(Prefix)).ToList().Take(10);

            if (Name1 != null)
            {
                var Name = dbm.Dealers.Where(o => o.Company_Name.StartsWith(Prefix)).Select(o => new { o.Dealer_ID, o.Company_Name, o.Percentage }).ToList().Take(10);
                return Json(Name, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult delete_development(int id)
        {
            var dlt = dbm.Development_Charges.FirstOrDefault(o => o.ID == id);
            var id1 = dlt.Payment_Plan.Payment_ID;
            var voucher_delete = dbm.reciept_vouchers.FirstOrDefault(o => o.Manual_Voucher_No == dlt.receipt_No);

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
            dbm.Development_Charges.Remove(dlt);
            dbm.SaveChanges();
            return RedirectToAction("list_development_charge", "Member", new { id = id1 });
        }
        public ActionResult list_development_charge(int id)
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
            var a = dbm.Development_Charges.Where(o => o.Payment_ID == id).ToList();
            return View(a);
        }
        public ActionResult intimationLetter(int id)
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
            var member = dbm.Payment_Plan.FirstOrDefault(o => o.Plot.Member.Member_ID == id);
            if (string.IsNullOrWhiteSpace(member.Plot.Member.ILNo))
            {
                member.Plot.Member.ILNo = Guid.NewGuid().ToString();
                dbm.Entry(member).State = EntityState.Modified;
                dbm.SaveChanges();
            }
            var barcode = member.Plot.Member.ILNo;
            var qr = GenerateQRCode(barcode);
            var barcodegenerate = GenerateBarCode(barcode);
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Party");
                ViewBag.Key = s.Target;
            }
            ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(barcodegenerate);
            ViewBag.QrcodeImage = "data:image/png;base64," + Convert.ToBase64String(qr);
            ViewBag.defaulterAmount = member.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(q => q.Amount_Paid) - member.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid);
            var paid_amount = member.Installments.Select(o => o.Monthly_Paid).Sum();
            var x = member.Installments.Sum(o => o.Amount_Paid);

            ViewBag.remaining_amount = x - paid_amount;
            return View(member);
        }
        private byte[] GenerateBarCode(string qrcodeText)
        {
            byte[] value;


            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.CODE_128;
            barcodeWriter.Options.PureBarcode = true;
            var result = barcodeWriter.Write(qrcodeText);

            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {

                barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                byte[] bytes = memory.ToArray();
                value = bytes;

            }
            return value;
        }
        private byte[] GenerateQRCode(string qrcodeText)
        {
            byte[] value;


            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {

                barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                byte[] bytes = memory.ToArray();
                value = bytes;

            }
            return value;
        }
        public async Task<ActionResult> Manage_Member(bool isAll = false)
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
            List<Payment_Plan> result = null;

            var task = Task.Run(() =>
            {
                if (isAll)
                {
                    result = dbm.Payment_Plan.ToList();
                }
                else
                {
                    result = dbm.Payment_Plan.ToList();
                }
            });


            await task;
            return View(result);
        }
        public ActionResult Attachment(int id)
        {


            var a = dbm.memberImages.Where(x => x.Member_Id == id).ToList();


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

            ViewBag.id = id;

            return View(a);


        }
        [HttpPost]
        public ActionResult Attachment(HttpPostedFileBase Profile_Image, int id)
        {
            memberImage memberImage = new memberImage();

            memberImage.Image = new byte[Profile_Image.ContentLength];
            Profile_Image.InputStream.Read(memberImage.Image, 0, Profile_Image.ContentLength);
            memberImage.Member_Id = id;
            dbm.memberImages.Add(memberImage);
            dbm.SaveChanges();
            return RedirectToAction("attachment", "Member", new { id = id });
        }
        public ActionResult DeleteAttachment(int id)
        {


            var a = dbm.memberImages.Where(x => x.id == id).FirstOrDefault();
            var memberId = a.Member_Id
                ;
            dbm.memberImages.Remove(a);
            dbm.SaveChanges();



            return RedirectToAction("attachment", "Member", new { id = memberId });


        }
        public class ImageModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Data { get; set; }
        }

        [HttpPost]
        public ActionResult Capture(ImageModel model)
        {
            // Convert image data to byte array

            // Save image to database


            return RedirectToAction("Index");
        }


        public ActionResult result(string search1, string search2)
        {
            if (search2 == "Ref#")
            {
                var trips = dbm.Payment_Plan.Where(o => o.Plot.Reg_No.Contains(search1)).ToList()
                    .Select(o => new
                    {
                        o.Plot.Member.Employee.F_Name,
                        o.Plot.Member.Employee.L_Name,
                        o.Plot.Member.Refund_Check,
                        o.Plot.Reg_No,
                        o.Plot.Member.Applicant_Name,
                        o.Plot.Member.CNIC,
                        o.Plot.Installment_Plan.Plan_Name
                    ,
                        isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                        o.Plot.Date,
                        o.Plot.Member.createdDate,
                        o.Plot.Member.Status,
                        o.Plot.Member_ID,
                        o.Plot.Member.Dealer_Name,
                        first = o.Installments.Sum(n => n.Amount_Paid)
                    ,
                        second = o.Installments.Sum(u => u.Monthly_Paid)
                    }).Take(200);


                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            if (search2 == "APPLICATION#")
            {
                var trips = dbm.Payment_Plan.Where(o => o.Plot.Qm_No.Contains(search1)).ToList()
                    .Select(o => new
                    {
                        o.Plot.Member.Employee.F_Name,
                        o.Plot.Member.Employee.L_Name,
                        o.Plot.Member.Refund_Check,
                        o.Plot.Reg_No,
                        o.Plot.Member.Applicant_Name,
                        o.Plot.Member.CNIC,
                        o.Plot.Installment_Plan.Plan_Name
                    ,
                        isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                        o.Plot.Date,
                        o.Plot.Member.createdDate,
                        o.Plot.Member.Status,
                        o.Plot.Member_ID,
                        o.Plot.Member.Dealer_Name,
                        first = o.Installments.Sum(n => n.Amount_Paid)
                    ,
                        second = o.Installments.Sum(u => u.Monthly_Paid)
                    }).Take(200);


                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else if (search2 == "CNIC")
            {
                var trips = dbm.Payment_Plan.Where(o => o.Plot.Member.CNIC.Contains(search1)).ToList().Select(o => new
                {
                    o.Plot.Member.Employee.F_Name,
                    o.Plot.Member.Employee.L_Name,
                    o.Plot.Member.Refund_Check,
                    o.Plot.Reg_No,
                    o.Plot.Member.Applicant_Name,
                    o.Plot.Member.CNIC,
                    o.Plot.Installment_Plan.Plan_Name
                    ,
                    isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                    o.Plot.Date,
                    o.Plot.Member.createdDate,
                    o.Plot.Member.Status,
                    o.Plot.Member_ID,
                    o.Plot.Member.Dealer_Name,
                    first = o.Installments.Sum(n => n.Amount_Paid)
                    ,
                    second = o.Installments.Sum(u => u.Monthly_Paid)
                }).Take(200);
                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else if (search2 == "Member Name")
            {
                var trips = dbm.Payment_Plan.Where(o => o.Plot.Member.Applicant_Name.Contains(search1)).ToList().Select(o => new
                {
                    o.Plot.Member.Employee.F_Name,
                    o.Plot.Member.Employee.L_Name,
                    o.Plot.Member.Refund_Check,
                    o.Plot.Reg_No,
                    o.Plot.Member.Applicant_Name,
                    o.Plot.Member.CNIC,
                    o.Plot.Installment_Plan.Plan_Name
                    ,
                    isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                    o.Plot.Date,
                    o.Plot.Member.createdDate,
                    o.Plot.Member.Status,
                    o.Plot.Member_ID,
                    o.Plot.Member.Dealer_Name,
                    first = o.Installments.Sum(n => n.Amount_Paid)
                  ,
                    second = o.Installments.Sum(u => u.Monthly_Paid)
                }).Take(200);
                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else if (search2 == "Size")
            {
                var trips = dbm.Payment_Plan.Where(o => o.Plot.Installment_Plan.Plan_Name == search1).ToList().Select(o => new
                {
                    o.Plot.Member.Employee.F_Name,
                    o.Plot.Member.Employee.L_Name,
                    o.Plot.Member.Refund_Check,
                    o.Plot.Reg_No,
                    o.Plot.Member.Applicant_Name,
                    o.Plot.Member.CNIC,
                    o.Plot.Installment_Plan.Plan_Name
                    ,
                    isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                    o.Plot.Date,
                    o.Plot.Member.createdDate,
                    o.Plot.Member.Status,
                    o.Plot.Member_ID,
                    o.Plot.Member.Dealer_Name,
                    second = o.Installments.Sum(u => u.Monthly_Paid)
                }).Take(200);
                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }

            else if (search2 == "Plot No")
            {
                if (search1 != "" && search1 != null)
                {
                    var trips = dbm.Payment_Plan.Where(o => o.Plot.Plot_No.ToString() == search1).ToList().Select(o => new
                    {
                        o.Plot.Member.Employee.F_Name,
                        o.Plot.Member.Employee.L_Name,
                        o.Plot.Member.Refund_Check,
                        o.Plot.Reg_No,
                        o.Plot.Member.Applicant_Name,
                        o.Plot.Member.CNIC,
                        o.Plot.Installment_Plan.Plan_Name
                    ,
                        isBlock = (((DateTime.Now.Year - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month).LastOrDefault()?.Paid_Month.Value.Year) * 12 + DateTime.Now.Month - o.Installments.Where(x => x.Paid_Month != null).OrderBy(z => z.Paid_Month)?.LastOrDefault()?.Paid_Month.Value.Month) >= 6 && o.Installments.Any(xx => xx.IsPaid == false)) || o.Installments.All(x => x.Monthly_Paid == 0) && o.Installments.Any(v => v.Amount_Paid != 0),
                        o.Plot.Date,
                        o.Plot.Member.createdDate,
                        o.Plot.Member.Status,
                        o.Plot.Member_ID,
                        o.Plot.Member.Dealer_Name,
                        first = o.Installments.Sum(n => n.Amount_Paid)
                        ,
                        second = o.Installments.Sum(u => u.Monthly_Paid)
                    }).Take(200);
                    var json = JsonConvert.SerializeObject(trips);
                    return Json(json, JsonRequestBehavior.AllowGet);
                }

            }



            return Json(JsonRequestBehavior.AllowGet);

        }
        public ActionResult Update_Member(int id)
        {

            var a = dbm.Members.FirstOrDefault(o => o.Member_ID == id);
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
            ViewBag.plan = dbm.Installment_Plan.ToList();
            ViewBag.price_Of_Plot = dbm.Installments.Where(p => p.Payment_Plan.Plot.Member_ID == id).Sum(o => o.Amount_Paid);
            if (dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id) != null)
            {
                ViewBag.dealerid = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id).Dealer_ID;
            }
            if (dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id) != null)
            {

                ViewBag.percentage = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id).Percentage;
            }
            if (dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id) != null && dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id).Dealer_ID != null)
            {

                ViewBag.Company_Name = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id).Dealer.Company_Name;
            }
            ViewBag.plotList = dbm.newPlots.Where(o => o.isAssigned == false).ToList();
            ViewBag.priceWithoutPocession = dbm.Installments.Where(p => p.Payment_Plan.Plot.Member_ID == id).Where(o => !o.Comment.Contains("Po")).Sum(o => o.Amount_Paid);

            return View(a);


        }
        [HttpPost]
        public ActionResult Update_Member(Member model, Plot model1, string Profile_Image, string thumb_Image, FormCollection form, Dealer_Commision dealer_Commision, Dealer dealer, float Pocession_Payment)
        {
            var emp_ID = Convert.ToInt32(Session["emp_id"]);
            var Member_ID1 = Convert.ToInt32(form["Member_ID1"]);
            if (model1.Plot_No != null)
            {
                var previousAssigned = dbm.Plots.FirstOrDefault(o => o.Member_ID == Member_ID1);
                if (previousAssigned != null)
                {
                    var resetPlot = dbm.newPlots.FirstOrDefault(o => o.Id == previousAssigned.Plot_No);
                    if (resetPlot != null)
                    {
                        resetPlot.isAssigned = false;
                        dbm.Entry(resetPlot).State = EntityState.Modified;
                        dbm.SaveChanges();

                    }
                }
                var assignedplot = dbm.newPlots.FirstOrDefault(o => o.Id == model1.Plot_No);
                assignedplot.isAssigned = true;
                dbm.Entry(assignedplot).State = EntityState.Modified;
                dbm.SaveChanges();
                model1.Street = assignedplot.Street;
                model1.Block = assignedplot.Block;

            }
            var newdate = form["new_date"];
            var original_price = dbm.Installments.Where(p => p.Payment_Plan.Plot.Member_ID == Member_ID1).Sum(o => o.Amount_Paid);
            original_price = Convert.ToDouble(Math.Round((double)original_price, 0));
            if (form["Discount"] == "")
            {
                form["Discount"] = "0";
            }
            var Discount = Convert.ToDouble(form["Discount"]);
            if (form["Extra_Charge"] == "")
            {
                form["Extra_Charge"] = "0";
            }
            var extra_charge = Convert.ToDouble(form["Extra_Charge"]);
            var aaaa = dbm.Members.FirstOrDefault(o => o.Member_ID == Member_ID1);

            var plan1 = dbm.Installment_Plan.FirstOrDefault(o => o.ID == model1.Size);

            double price_of_Plot = (double)plan1.Total_Price - (double)Discount + extra_charge;


            if (!string.IsNullOrWhiteSpace(Profile_Image))
            {
                Profile_Image = Profile_Image.Split(',')[1];
                model.Profile_Img = Convert.FromBase64String(Profile_Image);

                aaaa.Profile_Img = model.Profile_Img;
            }
            if (!string.IsNullOrWhiteSpace(thumb_Image))
            {
                thumb_Image = thumb_Image.Split(',')[1];
                model.thumb_img = Convert.FromBase64String(thumb_Image);

                aaaa.thumb_img = model.thumb_img;

            }
            aaaa.Applicant_Name = model.Applicant_Name;
            aaaa.Applicant_Start_Text = model.Applicant_Start_Text;

            aaaa.Cell_No = model.Cell_No;
            aaaa.CNIC = model.CNIC;
            aaaa.Email = model.Email;
            aaaa.Father_Husband_Name = model.Father_Husband_Name;
            aaaa.Father_Husband_Start_Text = model.Father_Husband_Start_Text;
            aaaa.Nominee_Address = model.Nominee_Address;
            aaaa.Nominee_CNIC = model.Nominee_CNIC;
            aaaa.Nominee_Name = model.Nominee_Name;
            aaaa.Country = model.Country;
            aaaa.NomineeFather = model.NomineeFather;
            aaaa.Nominee_Phone_No = model.Nominee_Phone_No;
            aaaa.Occupation = model.Occupation;
            aaaa.Office_No = model.Office_No;
            aaaa.Permenent_Postel_Address = model.Permenent_Postel_Address;
            aaaa.Present_Postel_Address = model.Present_Postel_Address;
            aaaa.Relation = model.Relation;
            aaaa.notes = model.notes;
            aaaa.Employee_id = emp_ID;
            aaaa.Dealer_Name = dealer.Company_Name;
            //aaaa.MemberShipNumber = model.MemberShipNumber;


            dbm.Entry(aaaa).State = EntityState.Modified;
            dbm.SaveChanges();




            aaaa.Application_Date = model.Application_Date;
            dbm.Entry(aaaa).State = EntityState.Modified;
            dbm.SaveChanges();

            var plot = dbm.Plots.FirstOrDefault(o => o.Plot_ID == model1.Plot_ID);



            plot.Size = model1.Size;
            plot.Block = model1.Block;
            plot.Date = model1.Date;
            plot.Plot_No = model1.Plot_No;
            plot.Plot_Type = model1.Plot_Type;

            plot.Reg_No = model1.Reg_No;
            plot.Qm_No = model1.Qm_No;
            plot.Street = model1.Street;
            plot.Plot_No_Sub = model1.Plot_No_Sub;
            plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Extra_Charge = extra_charge;
            plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Discount = Discount;
            dbm.Entry(plot).State = EntityState.Modified;
            dbm.SaveChanges();
            var plan = dbm.Installment_Plan.FirstOrDefault(o => o.ID == model1.Size);
            var installment_plan = dbm.Installment_Plan.FirstOrDefault(o => o.ID == model1.Size);
            var model2 = dbm.Payment_Plan.FirstOrDefault(o => o.Plot_ID == model1.Plot_ID);
            var half_yearly_count = installment_plan.No_Of_Half_Yearly;
            var monthly_count = installment_plan.No_Of_Monthly;
            var total = monthly_count;
            var payment_month = dbm.Installments.First(o => o.Payment_ID == model2.Payment_ID).Payment_Month;
            if (newdate != null) { payment_month = Convert.ToDateTime(newdate); }
            var installment_update = dbm.Installments.Where(o => o.Payment_ID == model2.Payment_ID).ToList();
            foreach (var ii in installment_update)
            {


                var dlt = dbm.Sub_Installment.Where(o => o.Installment_ID == ii.Installment_ID).ToList();
                dbm.Sub_Installment.RemoveRange(dlt);
                dbm.SaveChanges();
            }
            dbm.Installments.RemoveRange(installment_update);
            dbm.SaveChanges();

            var actual_price = plan.Total_Price - price_of_Plot;
            Installment i = new Installment();
            i.Payment_ID = model2.Payment_ID;
            int count = (plan?.No_Of_Half_Yearly ?? 0) + (plan?.No_Of_Monthly ?? 0) + (plan?.NoOfQuaterly ?? 0) + (plan?.NoOfYearly ?? 0);
            var subs = actual_price / (count + 2);
            if (plan.Booking != 0)
            {
                i.IsPaid = false;
                i.Payment_Month = payment_month;
                i.Amount_Paid = plan.Booking - subs;
                i.Installment_No = 1;
                i.Comment = "Booking";
                i.Monthly_Paid = 0;
                i.Single_Monthly_Installment = plan.Monthly;
                i.Single_Half_Yearly_Installment = plan.Half_Yearly;
                i.no_of_half_yearly_installments = plan.No_Of_Half_Yearly;
                i.no_of_monthly_installments = plan.No_Of_Monthly;

                dbm.Installments.Add(i);
                dbm.SaveChanges();
            }
            if (plan.Allocation != 0)
            {
                i.Payment_ID = model2.Payment_ID;
                i.IsPaid = false;
                i.Payment_Month = payment_month.AddMonths(2);
                i.Amount_Paid = plan.Allocation - subs;
                i.Installment_No = 2;
                i.Comment = "1st Installments";
                i.Monthly_Paid = 0;
                i.Single_Monthly_Installment = plan.Monthly;
                i.Single_Half_Yearly_Installment = plan.Half_Yearly;
                i.no_of_half_yearly_installments = plan.No_Of_Half_Yearly;
                i.no_of_monthly_installments = plan.No_Of_Monthly;
                dbm.Installments.Add(i);
                dbm.SaveChanges();
            }
            //---------------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------------

            int installmentCount = 2;

            for (int j = 1; j <= count; j++)
            {

                i.Payment_Month = payment_month.Date.AddMonths(installmentCount + 3);
                i.Amount_Paid = plan.Quatlerly_installment - subs;
                i.Installment_No = j + 2;
                i.Comment = "Quarterly installment";
                i.Monthly_Paid = 0;
                dbm.Installments.Add(i);
                dbm.SaveChanges();

                installmentCount = installmentCount + 3;
            }
            if (plan.Pocession != 0)
            {
                i.Payment_Month = payment_month.Date.AddMonths((int)installmentCount + 1);
                i.Installment_No = count + 3;
                i.Amount_Paid = Pocession_Payment - actual_price;
                i.Comment = "Possession";
                i.Monthly_Paid = 0;
                dbm.Installments.Add(i);
                dbm.SaveChanges();
            }

            var payment_plan = dbm.Payment_Plan.FirstOrDefault(o => o.Plot_ID == model1.Plot_ID);

            payment_plan.Net_Price_Plot = dbm.Installments.Where(o => o.Payment_ID == payment_plan.Payment_ID).Sum(o => o.Amount_Paid);
            payment_plan.Gross_Price_Plot = payment_plan.Net_Price_Plot;
            payment_plan.Pocession_Payment = Pocession_Payment - actual_price;
            dbm.Entry(payment_plan).State = EntityState.Modified;
            dbm.SaveChanges();


            var party = dbm.Parties.FirstOrDefault(o => o.Member_ID == aaaa.Member_ID);
            var detail = dbm.Party_Detail.Where(o => o.Party_ID == party.ID).ToList();

            foreach (var adding_amount in detail)
            {
                var paid_amount = adding_amount.Current_Amount - adding_amount.Previous_Amount;
                double remaining_amount = 0;
                if (paid_amount != 0)
                {
                    var add = dbm.Installments.Where(o => o.Payment_ID == model2.Payment_ID).ToList().OrderBy(o => o.Installment_No).FirstOrDefault();


                    if (paid_amount == (add.Amount_Paid - add.Monthly_Paid))
                    {
                        add.IsPaid = true;
                        add.Monthly_Paid = add.Monthly_Paid + paid_amount;
                        add.Paid_Month = adding_amount.Pay_Date;


                        dbm.Entry(add).State = EntityState.Modified;
                        dbm.SaveChanges();
                        Sub_Installment sub = new Sub_Installment();
                        sub.Installment_ID = add.Installment_ID;
                        sub.Amount = paid_amount;
                        sub.Receipt_No = adding_amount.Receipt_No;
                        sub.Date = adding_amount.Pay_Date;
                        dbm.Sub_Installment.Add(sub);
                        dbm.SaveChanges();
                    }
                    else if (paid_amount < (add.Amount_Paid - add.Monthly_Paid))
                    {

                        add.Monthly_Paid = add.Monthly_Paid + paid_amount;

                        add.Paid_Month = adding_amount.Pay_Date;

                        dbm.Entry(add).State = EntityState.Modified;
                        dbm.SaveChanges();
                        Sub_Installment sub = new Sub_Installment();
                        sub.Installment_ID = add.Installment_ID;
                        sub.Amount = paid_amount;
                        sub.Receipt_No = adding_amount.Receipt_No;
                        sub.Date = adding_amount.Pay_Date;
                        dbm.Sub_Installment.Add(sub);
                        dbm.SaveChanges();

                    }
                    else if (paid_amount > (add.Amount_Paid - add.Monthly_Paid))
                    {
                        var iii = dbm.Installments.Where(o => o.Payment_ID == model2.Payment_ID && o.IsPaid == false).ToList().OrderBy(o => o.Installment_No);
                        var last = iii.Last();
                        foreach (var aaa in iii)
                        {
                            if (aaa.Monthly_Paid == null) { aaa.Monthly_Paid = 0; }
                            if (paid_amount > 0)
                            {
                                if (paid_amount >= (aaa.Amount_Paid - aaa.Monthly_Paid))
                                {
                                    var temp = aaa.Monthly_Paid;
                                    aaa.Monthly_Paid = aaa.Amount_Paid;
                                    aaa.IsPaid = true;
                                    remaining_amount = (double)aaa.Monthly_Paid - (double)temp;
                                    paid_amount = (double)paid_amount - ((double)aaa.Monthly_Paid - (double)temp);
                                }
                                else if (paid_amount < (aaa.Amount_Paid - aaa.Monthly_Paid))
                                {
                                    aaa.Monthly_Paid = aaa.Monthly_Paid + paid_amount;
                                    remaining_amount = (double)paid_amount;
                                    paid_amount = (double)paid_amount - (double)aaa.Monthly_Paid;
                                }





                                aaa.Paid_Month = adding_amount.Pay_Date;
                                dbm.Entry(aaa).State = EntityState.Modified;
                                if (dbm.SaveChanges() > 0)
                                {

                                    Sub_Installment sub = new Sub_Installment();
                                    sub.Installment_ID = aaa.Installment_ID;
                                    sub.Amount = remaining_amount;
                                    sub.Receipt_No = adding_amount.Receipt_No;
                                    sub.Date = adding_amount.Pay_Date;
                                    dbm.Sub_Installment.Add(sub);
                                    dbm.SaveChanges();
                                }


                            }


                        }
                        if (paid_amount > 0)
                        {
                            var last1 = dbm.Installments.FirstOrDefault(o => o.Installment_ID == last.Installment_ID);
                            last1.Monthly_Paid = last1.Monthly_Paid + paid_amount;
                            dbm.Entry(last1).State = EntityState.Modified;
                            dbm.SaveChanges();
                        }


                    }
                }

            }



            var ledger = dbm.Ledger_Account.FirstOrDefault(o => o.Member_ID == plot.Member_ID);
            ledger.Ledger_Account_Title = plot.Reg_No;
            dbm.Entry(ledger).State = EntityState.Modified;
            dbm.SaveChanges();
            var updateDealer = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == plot.Member_ID);
            var data = dbm.Dealers.FirstOrDefault(o => o.Company_Name == dealer.Company_Name);
            if (updateDealer == null)
            {
                Dealer_Commision dealer_Commision1 = new Dealer_Commision();
                if (data == null)
                {
                    Dealer dealer1 = new Dealer();
                    dealer1.Company_Name = dealer.Company_Name;
                    dealer1.Percentage = dealer_Commision.Percentage;
                    dbm.Dealers.Add(dealer1);
                    dbm.SaveChanges();
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
                    l.Dealer_ID = dealer1.Dealer_ID;
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

                    l.Ledger_Account_Title = dealer1.Company_Name;

                    dbm.Ledger_Account.Add(l);
                    dbm.SaveChanges();
                    dealer_Commision1.Dealer_ID = dealer1.Dealer_ID;
                }
                if (dealer_Commision1.Dealer_ID == null)
                {

                    dealer_Commision1.Dealer_ID = data.Dealer_ID;
                }
                dealer_Commision1.Member_id = (int)plot.Member_ID;
                dealer_Commision1.Percentage = dealer_Commision.Percentage;

                dbm.Dealer_Commision.Add(dealer_Commision1);
                dbm.SaveChanges();
            }
            else
            {
                var dealerdata = dbm.Dealers.FirstOrDefault(o => o.Company_Name == dealer.Company_Name);
                if (dealer.Dealer_ID != 0 && dealer.Company_Name == null)
                {
                    updateDealer.Dealer_ID = null;
                    updateDealer.Percentage = null;
                    dbm.Entry(updateDealer).State = EntityState.Modified;
                    dbm.SaveChanges();
                }
                else if (dealerdata.Dealer1.Count() > 0)
                {
                    updateDealer.Dealer_ID = dealer.Dealer_ID;
                    updateDealer.Percentage = dealer_Commision.Percentage;
                    dbm.Entry(updateDealer).State = EntityState.Modified;
                    dbm.SaveChanges();

                }
                else if (dealer.Company_Name != null)
                {
                    Dealer dealer1 = new Dealer();
                    dealer1.Company_Name = dealer.Company_Name;
                    dealer1.Percentage = dealer_Commision.Percentage;
                    dbm.Dealers.Add(dealer1);
                    dbm.SaveChanges();
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
                    l.Dealer_ID = dealer1.Dealer_ID;
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

                    l.Ledger_Account_Title = dealer1.Company_Name;

                    dbm.Ledger_Account.Add(l);
                    dbm.SaveChanges();
                    updateDealer.Dealer_ID = dealer1.Dealer_ID;
                    updateDealer.Percentage = dealer_Commision.Percentage;
                    dbm.Entry(updateDealer).State = EntityState.Modified;
                    dbm.SaveChanges();

                }

            }
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
            }
            return RedirectToAction("view_installment", "Member", new { id = plot.Member_ID });

        }
        public JsonResult change_plan(FormCollection form)
        {
            var x = Convert.ToBoolean(form["x"]);

            if (x)
            {
                var a = dbm.Installment_Plan.Where(o => o.Plot_Type == "non NPF members").ToList().Select(o => new { o.ID, o.Allocation, o.Booking, o.Confirmation, o.Half_Yearly, o.Monthly, o.No_Of_Half_Yearly, o.No_Of_Monthly, o.Plan_Name, o.Pocession, o.Scheme, o.Total_Price, o.Plot_Type });
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            var aa = dbm.Installment_Plan.Where(o => o.Plot_Type == "NPF members").ToList().Select(o => new { o.ID, o.Allocation, o.Booking, o.Confirmation, o.Half_Yearly, o.Monthly, o.No_Of_Half_Yearly, o.No_Of_Monthly, o.Plan_Name, o.Pocession, o.Scheme, o.Total_Price, o.Plot_Type });
            return Json(aa, JsonRequestBehavior.AllowGet);

        }
        // DigitalPersona library namespace
        public ActionResult Add_Member()
        {

            dynamic dy = new ExpandoObject();
            var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Bank");

            dy.ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).ToList();


            dy.installment_plan = dbm.Installment_Plan.Where(o => o.Plot_Type == "NPF members").ToList();
            dy.plotList = dbm.newPlots.Where(o => o.isAssigned == false).ToList();
            dy.dealerList = dbm.Dealers.ToList();

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
            return View(dy);
        }


        [HttpGet]
        public JsonResult GetNewPlotDataFloorandType(int unitNOId)
        {
            var UnitDataFromNewPlotTable = dbm.newPlots
                .Where(x => x.Id == unitNOId)
                .Select(x => new
                {
                    Floor = x.Street,
                    Type = x.Block,
                    wide = x.wide

                })
                .FirstOrDefault(); // Ensure only one object is returned
            return Json(UnitDataFromNewPlotTable, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNewPlotDataFloorandUnitNO(string unitTypeId)
        {
            var UnitDataFromNewPlotTable = dbm.newPlots
                .Where(x => x.plotType == unitTypeId && x.isAssigned == false)
                .Select(x => new
                {
                    Floor = x.Street,
                    Type = x.Block,
                    wide = x.wide,
                    PlotNo = x.PlotNo

                })
                .ToList(); // Ensure only one object is returned
            return Json(UnitDataFromNewPlotTable, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNewPlotDataStreetAndSize(string unitTypeId)
        {
            var UnitDataFromNewPlotTable = dbm.newPlots
                .Where(x => x.PlotNo == unitTypeId) // Ensure correct filtering by plot number
                .Select(x => new
                {
                    Floor = x.Street,
                    Type = x.Block,
                    wide = x.wide,
                    PlotNo = x.PlotNo
                })
                .ToList();
            return Json(UnitDataFromNewPlotTable, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Add_Member(FormCollection form, Member model, Plot model1, Installment model2, Payment_Plan model3, Installment_Plan model4, string Profile_Image, string thumb_Image, HttpPostedFileBase txtPaymentModeImage, HttpPostedFileBase Image1,
          HttpPostedFileBase Image2,
          HttpPostedFileBase Image3,
          HttpPostedFileBase Image4,
          HttpPostedFileBase Image5,
          HttpPostedFileBase Image6,
          HttpPostedFileBase Image7,
          HttpPostedFileBase Image8
          )
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID1 = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID1);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            //var plan = dbm.Installment_Plan.FirstOrDefault(c => c.ID == size);
            var emp_ID = Convert.ToInt32(Session["emp_id"]);
            //var allocation = Convert.ToDouble(form["allocation_t"]);
            var dealer_id = form["dealer_id"];///////////////////////////////////////

            //var dealerId = Convert.ToInt32(form["dealer_name"]);

            int? dealerId = null; // Declare dealerId as a nullable integer

            // Check if the form contains the key and if its value can be converted to an integer
            if (!string.IsNullOrEmpty(form["dealer_name"]) && int.TryParse(form["dealer_name"], out int tempDealerId))
            {
                dealerId = tempDealerId;
            }



            var dealerName = dbm.Dealers.FirstOrDefault(x => x.Dealer_ID == dealerId);

            //var dealer_name = form["dealer_name"];
            var dealer_name = dealerName?.CEOName;
            var Dealer_percentage = form["Percentage"];

            var Booking = form["Booking"];
            var newPlotId = form["NewPlotId"];////////////////////////////////////////
            var Total_Price = model3.Gross_Price_Plot;


            var NewPlotId = (form["Plot_No"]);  /// in database the table newplot from the newPlot we  get the Plot_no
            var newPlotData = dbm.newPlots.FirstOrDefault(x => x.PlotNo == NewPlotId);
            //var confirmation = Convert.ToDouble(form["confirmation_t"]);
            if (model1.Plot_No != null)
            {

                var getPlotDetails = dbm.newPlots.FirstOrDefault(o => o.Id == newPlotData.Id);

                getPlotDetails.isAssigned = true;
                dbm.Entry(getPlotDetails).State = EntityState.Modified;
                dbm.SaveChanges();
                model1.Street = getPlotDetails.Street;
                model1.Block = getPlotDetails.Block;
            }
            ICollection<HttpPostedFileBase> httpPostedFileBases = new List<HttpPostedFileBase>();
            httpPostedFileBases.Add(Image1);
            httpPostedFileBases.Add(Image2);
            httpPostedFileBases.Add(Image3);
            httpPostedFileBases.Add(Image4);
            httpPostedFileBases.Add(Image5);
            httpPostedFileBases.Add(Image6);
            httpPostedFileBases.Add(Image7);
            httpPostedFileBases.Add(Image8);

            model.Application_Date = model1.Date;

            if (!string.IsNullOrWhiteSpace(Profile_Image))
            {
                Profile_Image = Profile_Image.Split(',')[1];
                model.Profile_Img = Convert.FromBase64String(Profile_Image);

            }
            if (!string.IsNullOrWhiteSpace(thumb_Image))
            {
                thumb_Image = thumb_Image.Split(',')[1];
                model.thumb_img = Convert.FromBase64String(thumb_Image);

            }
            model.ILNo = Guid.NewGuid().ToString();
            model.Status = true;
            model.Employee_id = emp_ID;
            dbm.Members.Add(model);
            if (dbm.SaveChanges() > 0)
            {
                foreach (var item in httpPostedFileBases)
                {
                    if (item != null)
                    {
                        memberImage memberImage = new memberImage();

                        memberImage.Image = new byte[item.ContentLength];
                        item.InputStream.Read(memberImage.Image, 0, item.ContentLength);
                        memberImage.Member_Id = model.Member_ID;
                        dbm.memberImages.Add(memberImage);
                        dbm.SaveChanges();
                    }

                }

                Installment_Plan instalPlan = new Installment_Plan();
                model4.Total_Price = Total_Price;
                model4.Plan_Name = newPlotData.PlotNo;

                model4.Plot_Type = newPlotData.Block;
                dbm.Installment_Plan.Add(model4);
                dbm.SaveChanges();


                Party p = new Party();
                p.Member_ID = model.Member_ID;
                p.Balance = 0;
                dbm.Parties.Add(p);
                dbm.SaveChanges();

                model1.Member_ID = model.Member_ID;
                model1.Installment_Plan = model4;
                model1.newPlot = newPlotData;
                dbm.Plots.Add(model1);
                if (dbm.SaveChanges() > 0)
                {

                    Plot_History plot = new Plot_History();
                    plot.Address = model.Permenent_Postel_Address;
                    plot.H_Date = model.Application_Date;
                    plot.Name = model.Applicant_Name;
                    plot.Phone_Number = model.Cell_No;
                    plot.Plot_ID = model1.Plot_ID;
                    plot.Status = true;
                    plot.CNIC = model.CNIC;
                    plot.History_No = 1;
                    dbm.Plot_History.Add(plot);
                    dbm.SaveChanges();

                    // for payment plan
                    model3.Plot_ID = model1.Plot_ID;
                    model3.Pocession_Payment = model4.Pocession;
                    model3.Outstanding = 0;

                    dbm.Payment_Plan.Add(model3);
                    if (dbm.SaveChanges() > 0)
                    {
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
                        l.Member_ID = model.Member_ID;
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

                        l.Ledger_Account_Title = model1.Reg_No;

                        dbm.Ledger_Account.Add(l);
                        dbm.SaveChanges();
                        if (model4.Booking != 0)
                        {

                            model2.Payment_ID = model3.Payment_ID;
                            model2.IsPaid = false;
                            model2.Payment_Month = model1.Date;
                            model2.Amount_Paid = model3.total_downpayment;
                            model2.Installment_No = 1;
                            model2.Comment = "Booking";
                            model2.Monthly_Paid = 0;
                            dbm.Installments.Add(model2);
                            dbm.SaveChanges();

                        }

                        int count = (model4?.NoOfQuaterly ?? 0);
                        int installmentCount = 1;  // Start from 1 because 1 installment is already added for the booking

                        for (int i = 1; i <= count; i++)
                        {
                            model2 = new Installment(); // Ensure a new instance is created for each installment
                            model2.Payment_ID = model3.Payment_ID;
                            model2.IsPaid = false;
                            model2.Payment_Month = model1.Date.AddMonths(installmentCount * 3); // Every 3 months
                            model2.Amount_Paid = model4.Quatlerly_installment;
                            model2.Installment_No = installmentCount + 1; // installmentCount + 1 to ensure the installment numbers are sequential
                            model2.Comment = "Quarterly installment";
                            model2.Monthly_Paid = 0;
                            dbm.Installments.Add(model2);
                            dbm.SaveChanges();

                            installmentCount++; // Increment the installment count for the next installment
                        }

                        if (model4.Pocession != 0)
                        {
                            model2.Payment_ID = model3.Payment_ID;
                            model2.IsPaid = false;
                            model2.Payment_Month = model1.Date.AddMonths(installmentCount * 3);
                            model2.Installment_No = installmentCount + 1;
                            //model2.Amount_Paid = model3.Pocession_Payment;
                            model2.Amount_Paid = model3.Pocession_Payment;
                            model2.Comment = "Possession Charges";
                            model2.Monthly_Paid = 0;
                            dbm.Installments.Add(model2);
                            dbm.SaveChanges();
                        }


                    }

                }


            }

            // FOR DEALER SECTION
            var existdealer = dbm.Dealers.FirstOrDefault(o => o.CEOName == dealer_name);
            var nameExist = existdealer?.CEOName;
            if (existdealer != null)
            {
                //Dealer dealer = new Dealer();
                //dealer.Company_Name = dealer_name;
                //dealer.Reg_Date = DateTime.Now;
                if (Dealer_percentage != "")
                {

                    //  dealer.Percentage = Convert.ToDouble(Dealer_percentage);
                    existdealer.Percentage = Convert.ToDouble(Dealer_percentage);
                    dbm.SaveChanges(); // Entity Framework will automatically detect and update the changes
                }
                //dbm.Dealers.Add(dealer);
                //dbm.SaveChanges();

                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Dealer Data");
                var ledger_count = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Max(x => x.Ledger_Account_Code);

                var dealerILedgerTable = dbm.Ledger_Account.Where(x => x.Ledger_Account_Title == nameExist).FirstOrDefault();
                if (dealerILedgerTable == null)
                {
                    Ledger_Account l = new Ledger_Account();
                    l.Balance = 0;
                    l.Element_ID = setting.Element_ID;
                    l.Control_ID = setting.Control_ID;
                    l.Control_Account_Code = setting.Control_Account.Control_Account_Code;
                    l.Control_Account_Title = setting.Control_Account.Control_Account_Title;
                    l.Element_Account_Code = setting.Element_Account.Element_Account_Code;
                    l.Element_Account_Title = setting.Element_Account.Account_Title;
                    l.Dealer_ID = existdealer.Dealer_ID;
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

                    //l.Ledger_Account_Title = dealer.Company_Name;
                    l.Ledger_Account_Title = existdealer.CEOName;
                    // l.Ledger_Account_Title = existdealer.Company_Name;

                    dbm.Ledger_Account.Add(l);
                    dbm.SaveChanges();

                }

                Dealer_Commision dealer_Commision = new Dealer_Commision();
                dealer_Commision.Dealer_ID = existdealer.Dealer_ID;
                dealer_Commision.Member_id = model.Member_ID;
                if (Dealer_percentage != "")
                {

                    dealer_Commision.Percentage = Convert.ToDouble(Dealer_percentage);
                }

                if (Dealer_percentage != null)
                {
                    dbm.Dealer_Commision.Add(dealer_Commision);
                    dbm.SaveChanges();

                }
            }
            else if (dealer_id == "" && dealer_name == "")
            {

            }
            else if (dealer_id != "" && dealer_name != "" && Dealer_percentage != "")
            {


                Dealer_Commision dealer_Commision = new Dealer_Commision();
                dealer_Commision.Dealer_ID = Convert.ToInt32(dealer_id);
                dealer_Commision.Member_id = model.Member_ID;
                dealer_Commision.Percentage = Convert.ToDouble(Dealer_percentage);
                if (dealer_Commision.Percentage != null && dealer_Commision.Percentage != 0)
                {
                    var delaer = dbm.Dealers.FirstOrDefault(o => o.Dealer_ID == dealer_Commision.Dealer_ID);
                    delaer.Percentage = dealer_Commision.Percentage;
                    dbm.Entry(delaer).State = EntityState.Modified;
                    dbm.SaveChanges();
                }
                dbm.Dealer_Commision.Add(dealer_Commision);
                dbm.SaveChanges();
            }
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
            }

            return RedirectToAction("Manage_Member", "Member");
        }
        public ActionResult delete(int id)
        {
            var a = dbm.Members.FirstOrDefault(o => o.Member_ID == id);
            var b = dbm.Plots.FirstOrDefault(o => o.Member_ID == a.Member_ID);
            var plot = dbm.Plot_History.Where(o => o.Plot_ID == b.Plot_ID).ToList();
            var c = dbm.Payment_Plan.FirstOrDefault(o => o.Plot_ID == b.Plot_ID);
            var d = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).ToList();
            var e = dbm.Parties.FirstOrDefault(o => o.Member_ID == a.Member_ID);
            var f = dbm.Party_Detail.Where(o => o.Party_ID == e.ID).ToList();
            var dealer = dbm.Dealer_Commision.FirstOrDefault(o => o.Member_id == id);
            if (a.Ledger_Account.FirstOrDefault(o => o.Member_ID == id).Balance == 0 && a.Ledger_Account.FirstOrDefault(o => o.Member_ID == a.Member_ID).reciept_voucher_payment.Count() == 0)
            {
                if (dealer != null)
                {
                    dbm.Dealer_Commision.Remove(dealer);
                    dbm.SaveChanges();
                }

                //if (dealer != null)
                //{
                //    var dealerDetails = dbm.Dealer_Commision_Detail.Where(o => o.Dealer_Commsion_ID == dealer.Dealer_ID).ToList();
                //    dbm.Dealer_Commision_Detail.RemoveRange(dealerDetails);
                //    dbm.SaveChanges();

                //    dbm.Dealer_Commision.Remove(dealer);
                //    dbm.SaveChanges();
                //}
                dbm.Party_Detail.RemoveRange(f);
                dbm.SaveChanges();
                dbm.Parties.Remove(e);
                dbm.SaveChanges();
                dbm.Plot_History.RemoveRange(plot);
                dbm.SaveChanges();
                dbm.Installments.RemoveRange(d);
                dbm.SaveChanges();

                dbm.Payment_Plan.Remove(c);
                dbm.SaveChanges();
                //var newPlot = dbm.Plots
                //.Where(x => x.Member_ID == id)
                //.FirstOrDefault();

                //var newPlotId = newPlot?.Plot_No;
                //var newPlotChangeStatus = dbm.newPlots.FirstOrDefault(x => x.Id == newPlotId);
                //newPlotChangeStatus.isAssigned = false;
                //dbm.Entry(newPlotChangeStatus).State = EntityState.Modified;

                dbm.Plots.Remove(b);
                dbm.SaveChanges();
                var g = dbm.Ledger_Account.FirstOrDefault(o => o.Member_ID == id);

                dbm.Ledger_Account.Remove(g);
                dbm.SaveChanges();
                dbm.Members.Remove(a);
                dbm.SaveChanges();
            }


            return RedirectToAction("Manage_Member", "Member");
        }
        public ActionResult view_installment(int id)
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

            dynamic dy = new ExpandoObject();

            dy.credit_account = dbm.Ledger_Account.FirstOrDefault(o => o.Member_ID == id);
            dy.ledger = dbm.Ledger_Account.Where(o => o.Control_Account_Title == "Bank").ToList();
            var member = dbm.Members.FirstOrDefault(o => o.Member_ID == id);
            var plot = dbm.Plots.FirstOrDefault(o => o.Member_ID == member.Member_ID);
            dy.size = plot.Installment_Plan.Plan_Name;
            var c = dbm.Payment_Plan.FirstOrDefault(o => o.Plot_ID == plot.Plot_ID);
            dy.party_id = dbm.Payment_Plan.FirstOrDefault(o => o.Plot_ID == plot.Plot_ID).Plot.Member.Parties.FirstOrDefault(o => o.Member_ID == member.Member_ID).ID;
            dy.c = c;
            dy.b = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).ToList();
            dy.paid = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID && o.IsPaid == true).Count();
            dy.paid_amount = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Select(o => o.Monthly_Paid).Sum();
            var x = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Sum(o => o.Amount_Paid);

            dy.remaining_amount = x - dy.paid_amount;
            dy.check = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID && o.IsPaid == false).Count();
            dy.cancled = dbm.Members.Where(o => o.Member_ID == id && o.Status == false).Count();
            dy.a = dbm.Installments.FirstOrDefault(o => o.Payment_ID == c.Payment_ID);
            dy.showIntimationLetter = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).OrderBy(q => q.Installment_No).Take(2).All(zz => zz.IsPaid == true);
            if (c.Plot.Installment_Plan.Scheme == true && (dy.paid_amount / x) * 100 >= 30)
            {
                dy.showIntimationLetter = true;
            }
            else if (c.Plot.Installment_Plan.Scheme == false && (dy.paid_amount / x) * 100 >= 20)
            {
                dy.showIntimationLetter = true;

            }
            else
            {
                dy.showIntimationLetter = false;

            }
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                ViewBag.Key = s.Target;
            }

            return View(dy);
        }
        public ActionResult print(int id)
        {
            dynamic dy = new ExpandoObject();
            var c = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == id);
            dy.data = c;
            dy.installment_detail = dbm.Installments.Where(o => o.Payment_ID == id);
            dy.paid_amount = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Select(o => o.Monthly_Paid).Sum();
            var x = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Sum(o => o.Amount_Paid);
            dy.defaulterAmount = c.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(q => q.Amount_Paid) - c.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid);

            dy.remaining_amount = x - dy.paid_amount;
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
            ViewBag.development = dbm.Development_Charges.Where(o => o.Payment_ID == id).ToList();
            return View(dy);

        }
        public ActionResult printOnLetterHead(int id)
        {
            //int? surchargeAmount = dbm.SurchargePercentages.Select(xx => xx.SurchargePercentAmount).FirstOrDefault();
            //ViewBag.Surcharge = surchargeAmount;
            dynamic dy = new ExpandoObject();
            var c = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == id);
            dy.data = c;
            dy.installment_detail = dbm.Installments.Where(o => o.Payment_ID == id);
            dy.paid_amount = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Select(o => o.Monthly_Paid).Sum();
            var x = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Sum(o => o.Amount_Paid);
            dy.defaulterAmount = c.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(q => q.Amount_Paid) - c.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid);

            //dy.Newplotdat = dbm.Payment_Plan
            //    .Where(o => o.Payment_ID == id)
            //    .Include(o => o.Plot.newPlot.specialType)
            //    .FirstOrDefault();

            var paymentPlan = dbm.Payment_Plan
      .Where(o => o.Payment_ID == id)
      .Include(o => o.Plot).Include(O => O.Plot.newPlot)  // Include the newPlot navigation property
      .FirstOrDefault();

            // dy.Newplotdat can be null if no matching record is found
            dy.Newplotdat = paymentPlan?.Plot?.newPlot?.specialType;

            dy.remaining_amount = x - dy.paid_amount;
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
            ViewBag.development = dbm.Development_Charges.Where(o => o.Payment_ID == id).ToList();
            return View(dy);

        }

        public ActionResult registrationForm(int id)
        {
            //int? surchargeAmount = dbm.SurchargePercentages.Select(xx => xx.SurchargePercentAmount).FirstOrDefault();
            //ViewBag.Surcharge = surchargeAmount;
            dynamic dy = new ExpandoObject();
            var c = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == id);
            dy.data = c;

            // to get member Id start from here
            var plotIDget = c.Plot_ID;
            var memberId = dbm.Plots.FirstOrDefault(v => v.Plot_ID == plotIDget).Member_ID;
            var partyId = dbm.Parties.FirstOrDefault(f => f.Member_ID == memberId);
            //dy.a = dbm.Party_Detail.Where(o => o.Party_ID == id).ToList().OrderBy(o => o.Pay_Date);

            //dy.partydetail = dbm.Party_Detail.Where(o => o.Party_ID == partyId.ID).FirstOrDefault();
            dy.partydetail = dbm.Party_Detail
                   .Where(o => o.Party_ID == partyId.ID)
                   .OrderBy(o => o.ID)  // Assuming there's an ID or a similar field to order by to get the first entry
                   .FirstOrDefault();

            //ViewBag.ledger = dbm.Ledger_Account.ToList();
            //var val = Convert.ToInt32(a.Head);
            //ViewBag.type = dbm.Ledger_Account.FirstOrDefault(o => o.ID == val).Control_Account_Title;
            //ViewBag.paymentType = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).FirstOrDefault().Installment.Comment;
            //ViewBag.installmentNo = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).FirstOrDefault().Installment.Installment_No;
            //ViewBag.sub = dbm.Sub_Installment.Where(o => o.Receipt_No == a.Receipt_No).ToList();

            //end here


            dy.installment_detail = dbm.Installments.Where(o => o.Payment_ID == id);
            dy.paid_amount = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Select(o => o.Monthly_Paid).Sum();

            dy.ForBookingTotal = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID && o.Comment== "Booking" && o.IsPaid==true && o.Installment_No==1).FirstOrDefault();



            var x = dbm.Installments.Where(o => o.Payment_ID == c.Payment_ID).Sum(o => o.Amount_Paid);
            dy.defaulterAmount = c.Installments.Where(w => w.Payment_Month <= DateTime.Today).Sum(q => q.Amount_Paid) - c.Installments.Where(e => e.Payment_Month <= DateTime.Today).Sum(m => m.Monthly_Paid);


          

            //dy.Newplotdat = dbm.Payment_Plan
            //    .Where(o => o.Payment_ID == id)
            //    .Include(o => o.Plot.newPlot.specialType)
            //    .FirstOrDefault();

            var paymentPlan = dbm.Payment_Plan
      .Where(o => o.Payment_ID == id)
      .Include(o => o.Plot).Include(O => O.Plot.newPlot)  // Include the newPlot navigation property
      .FirstOrDefault();

            // dy.Newplotdat can be null if no matching record is found
            dy.Newplotdat = paymentPlan?.Plot?.newPlot?.specialType;

            dy.remaining_amount = x - dy.paid_amount;
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
            ViewBag.development = dbm.Development_Charges.Where(o => o.Payment_ID == id).ToList();
            return View(dy);
        }





        [HttpPost]
        public ActionResult SurchargerPercent(int selectedValue)
        {
            if (selectedValue != 0) // Assuming 0 is not a valid value
            {
                // Check if the database has any entries with SurchargePercentAmount 0 or null
                var existingValue = dbm.SurchargePercentages.FirstOrDefault(v => v.SurchargePercentAmount == 0 || v.SurchargePercentAmount == null);

                if (existingValue != null)
                {
                    // If an entry with 0 or null is found, update it
                    existingValue.SurchargePercentAmount = selectedValue;
                    dbm.Entry(existingValue).State = EntityState.Modified;
                }
                else
                {
                    // Check if an entry with the same selectedValue already exists
                    existingValue = dbm.SurchargePercentages.FirstOrDefault(v => v.SurchargePercentAmount == selectedValue);
                    if (existingValue != null)
                    {
                        // If found, update the existing value
                        existingValue.SurchargePercentAmount = selectedValue;
                        dbm.Entry(existingValue).State = EntityState.Modified;
                    }
                    else
                    {
                        // Otherwise, add a new entry with the selected value
                        var newValue = new SurchargePercentage { SurchargePercentAmount = selectedValue };
                        dbm.SurchargePercentages.Add(newValue);
                    }
                }

                dbm.SaveChanges();
            }

            return RedirectToAction("Setting", "Account_Setting"); // Redirect to appropriate action
        }


        public static byte[] ResizeImage(byte[] originalImageBytes, int newWidth, int newHeight)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(originalImageBytes))
                {
                    Image originalImage = Image.FromStream(ms);

                    // Create a new bitmap with the specified width and height
                    Bitmap resizedImage = new Bitmap(newWidth, newHeight);

                    // Create a graphics object from the resized image
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        // Set the interpolation mode for better image quality
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // Draw the original image onto the resized image
                        graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                    }

                    // Convert the resized image to a byte array
                    using (MemoryStream resizedMs = new MemoryStream())
                    {
                        resizedImage.Save(resizedMs, ImageFormat.Jpeg); // Change ImageFormat as needed
                        return resizedMs.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        [HttpPost]
        public ActionResult view_installment(FormCollection form, HttpPostedFileBase txtPaymentModeImage)
        {
            int detail_id = 0;
            var checking = form["checking"];
            var date = form["checkDate"];
            var bankName = form["bankName"];
            //var manualvouchernumber = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1);
            int maxVoucherNum = dbm.reciept_vouchers.Any() ? dbm.reciept_vouchers.Max(o => o.VoucherNum) : 0;
            var manualvouchernumber = Convert.ToString(maxVoucherNum + 1);

            var payment_date = Convert.ToDateTime(form["txtPaymentDate"]);
            var firstledger = form["firstledger"];
            var secondledger = form["secondledger"];
            var firsttype = form["firsttype"];
            var secondtype = form["secondtype"];
            var comment = form["comment"];
            var Payment_mode = form["Payment_mode"];
            var paymentModeShow = Payment_mode;

            if (Payment_mode == "DISCOUNT") { paymentModeShow = "Disc"; }
            else if (Payment_mode == "DEALER ADJUSTMENT") { paymentModeShow = "D.Adj"; }
            else if (Payment_mode == "Adjustment") { paymentModeShow = "Adj"; }
            else if (Payment_mode == "CASHATBANK") { paymentModeShow = "BRV"; }
            else if (Payment_mode == "ONLINETRANSFER") { paymentModeShow = "BRV"; }
            var Payment_ID = Convert.ToInt32(form["Payment_ID"]);
            var checknumber = form["checknumber"];
            var Manual_Voucher_No = form["Manual_Voucher_No"];

            var amount = Convert.ToDouble(form["txtPaymentAmount"]);
            var amount1 = amount;
            var check = form["check"];
            var isMemberShipFee = form["isMemberShipFee"];
            var checkdate = DateTime.MinValue;
            if (Payment_mode == "CRV" || date == "")
            {

            }
            else
            {
                checkdate = Convert.ToDateTime(form["checkDate"]);
            }

            ///Update data into installment table.....................................................................................
            ///

            //insert data into recept voucher table...............................................................................
            if (check == "yes")
            {

                Development_Charges dev = new Development_Charges();
                dev.Payment_ID = Payment_ID;
                dev.Amount = amount;
                dev.Paid_Date = payment_date;
                dev.Narration = checknumber;
                secondledger = form["secondledger_development"];
                dev.Head = dbm.Ledger_Account.First(o => o.Ledger_Complete_Code == secondledger).ID;
                //dev.receipt_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

                int nextVoucherNum121 = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                dev.receipt_No = Convert.ToString(nextVoucherNum121) + "/" + DateTime.Today.Year + "/" + paymentModeShow;


                dbm.Development_Charges.Add(dev);
                dbm.SaveChanges();
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
                // rec.Manual_Voucher_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                int nextVoucherNumManu = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                rec.Manual_Voucher_No = Convert.ToString(nextVoucherNumManu) + "/" + DateTime.Today.Year + "/" + paymentModeShow;


                //var num = dbm.reciept_vouchers.Max(o => o.VoucherNum);
                //num = num + 1;
                //rec.VoucherNum = num;

                int num = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) : 0;
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
                        return RedirectToAction("Manage_Member", "Member");

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
                        TempData["Message"] = "/Party/development_charges?id=" + dev.ID + "";
                        var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);
                        return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });
                        //return RedirectToAction("Print", "Party", new { id = detail_id });

                    }

                }
            }
            else if (isMemberShipFee == "yes")
            {
                secondledger = form["secondledger_development"];

                var paymentPlan = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);
                paymentPlan.memberShipFee = Convert.ToDouble(paymentPlan.memberShipFee) + amount1;
                dbm.Entry(paymentPlan).State = EntityState.Modified;
                dbm.SaveChanges();
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
                rec.IsChecked = 0;
                rec.ChequeNumber = checknumber;
                rec.Comments = comment;
                rec.VoucherDate = payment_date;
                rec.ChequeDate = checkdate;
                //rec.Manual_Voucher_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                int nextVoucherNumManut = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                rec.Manual_Voucher_No = Convert.ToString(nextVoucherNumManut) + "/" + DateTime.Today.Year + "/" + paymentModeShow;


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
                        return RedirectToAction("Manage_Member", "Member");

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
                        //TempData["Message"] = "/Party/development_charges?id=" + dev.ID + "";
                        var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);
                        return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });
                        //return RedirectToAction("Print", "Party", new { id = detail_id });

                    }

                }
            }
            else
            {
                if (checking == null)
                {
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
                    rec.IsApproved = 0;
                    rec.IsChecked = 0;
                    rec.ChequeNumber = checknumber;
                    rec.Comments = comment;
                    rec.VoucherDate = payment_date;
                    rec.ChequeDate = checkdate;
                    var num = dbm.reciept_vouchers.Max(o => o.VoucherNum);
                    num = num + 1;
                    rec.VoucherNum = num;

                    //rec.Manual_Voucher_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                    int nextVoucherNumManuttt = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                    rec.Manual_Voucher_No = Convert.ToString(nextVoucherNumManuttt) + "/" + DateTime.Today.Year + "/" + paymentModeShow;


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
                            pay.CurrentBalance = pay.PreviousBalance;
                            dbm.reciept_voucher_payment.Add(pay);
                            dbm.SaveChanges();
                            // var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger);
                            ///  a.Balance = pay.CurrentBalance;
                            //  dbm.Entry(a).State = EntityState.Modified;
                            //  dbm.SaveChanges();
                        }
                        else
                        {//if type is D....................................................................
                            pay.TransType = firsttype;
                            pay.Comments = comment;
                            pay.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger).Balance;
                            pay.CurrentBalance = pay.PreviousBalance;
                            dbm.reciept_voucher_payment.Add(pay);
                            dbm.SaveChanges();
                            //  var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == firstledger);
                            //  a.Balance = pay.CurrentBalance;
                            //  dbm.Entry(a).State = EntityState.Modified;
                            //  dbm.SaveChanges();
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
                            pay1.CurrentBalance = pay1.PreviousBalance;
                            dbm.reciept_voucher_payment.Add(pay1);
                            dbm.SaveChanges();
                            // var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                            // a.Balance = pay1.CurrentBalance;
                            //  dbm.Entry(a).State = EntityState.Modified;
                            //  dbm.SaveChanges();
                            //  return RedirectToAction("print", "Party", new { id = party_detail } );


                        }
                        else
                        {//if type is D....................................................................
                            pay1.TransType = secondtype;
                            pay1.Comments = comment;
                            pay1.PreviousBalance = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger).Balance;
                            pay1.CurrentBalance = pay1.PreviousBalance;
                            dbm.reciept_voucher_payment.Add(pay1);
                            dbm.SaveChanges();
                            // var a = dbm.Ledger_Account.FirstOrDefault(o => o.Ledger_Complete_Code == secondledger);
                            //  a.Balance = pay1.CurrentBalance;
                            //  dbm.Entry(a).State = EntityState.Modified;
                            //  dbm.SaveChanges();
                            //  return RedirectToAction("print", "Party", new { id = party_detail });

                        }
                        Dummy dy = new Dummy();
                        dy.Amount = amount1;
                        //dy.Manual_Voucher_Number = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

                        int nextVoucherNumManutttrr = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                        dy.Manual_Voucher_Number = Convert.ToString(nextVoucherNumManutttrr) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

                        dy.Payment_Date = payment_date;
                        dy.Payment_ID = Payment_ID;
                        dy.RV_ID = rec.RV_ID;
                        dbm.Dummies.Add(dy);
                        dbm.SaveChanges();





                    }
                    if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
                    {
                        var emp_ID = Convert.ToInt32(Session["emp_id"]);
                        var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                        var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Member");
                        ViewBag.Key = s.Target;
                    }
                    if (Payment_mode == "BRV")
                    {
                        return RedirectToAction("Manage_Bank_Rec_Voucher", "Manage_Bank_Rec_Voucher");
                    }
                    else
                    {
                        return RedirectToAction("Manage_Cash_Rec_Voucher", "Manage_Cash_Rec_Voucher");

                    }
                }
                else
                {


                    double remaining_amount = 0;
                    var i = dbm.Installments.Where(o => o.Payment_ID == Payment_ID && o.IsPaid == false).OrderBy(o => o.Installment_No).FirstOrDefault();
                    if (i.Monthly_Paid == null)
                    {
                        i.Monthly_Paid = 0;
                    }
                    if (amount == (i.Amount_Paid - i.Monthly_Paid))
                    {
                        if (i.Comment == "Booking" || i.Comment == "Allocation Charges" || i.Comment == "Confirmation")
                        {

                            //var checking_id = i.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == i.Payment_Plan.Plot.Member.Member_ID);
                            //var percentage_dealer = i.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == i.Payment_Plan.Plot.Member.Member_ID)?.Percentage;

                            //if (percentage_dealer != null)
                            //{
                            //    Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail();
                            //    dealer_Commision_Detail.Dealer_Commsion_ID = checking_id.ID;
                            //    dealer_Commision_Detail.Due_date = DateTime.Now;
                            //    dealer_Commision_Detail.Status = false;
                            //    dealer_Commision_Detail.Percentage = checking_id.Percentage / 3;
                            //    dealer_Commision_Detail.Due_Amount = ((double)dealer_Commision_Detail.Percentage * (double)i.Payment_Plan.Net_Price_Plot) / 100;
                            //    dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                            //    dbm.SaveChanges();
                            //}
                            var checking_id = i.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == i.Payment_Plan.Plot.Member.Member_ID);
                            var percentage_dealer = i.Payment_Plan.Plot.Member.Dealer_Commision?.FirstOrDefault(o => o.Member_id == i.Payment_Plan.Plot.Member.Member_ID)?.Percentage;
                            var newplotdataWide = i.Payment_Plan.Plot.newPlot.wide;
                            if (percentage_dealer != null)
                            {
                                Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail();
                                dealer_Commision_Detail.Dealer_Commsion_ID = checking_id.ID;
                                dealer_Commision_Detail.Due_date = DateTime.Now;
                                dealer_Commision_Detail.Status = false;
                                double? newplotdataWideDouble = Convert.ToDouble(newplotdataWide);
                                double? NetPercentage = checking_id.Percentage * newplotdataWideDouble;

                                dealer_Commision_Detail.Percentage = NetPercentage;
                                dealer_Commision_Detail.Due_Amount = NetPercentage ?? 0;
                                dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                                dbm.SaveChanges();
                            }
                        }
                        i.IsPaid = true;
                        i.Monthly_Paid = i.Monthly_Paid + amount;
                        i.Paid_Month = payment_date;

                        if (txtPaymentModeImage == null)
                        {

                        }
                        else
                        {


                            i.Image = new byte[txtPaymentModeImage.ContentLength];
                            txtPaymentModeImage.InputStream.Read(i.Image, 0, txtPaymentModeImage.ContentLength);

                        }
                        dbm.Entry(i).State = EntityState.Modified;
                        dbm.SaveChanges();
                        Sub_Installment sub = new Sub_Installment();
                        sub.Installment_ID = i.Installment_ID;
                        sub.Amount = amount;

                        // sub.Receipt_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                        int maxVoucherNumtest = dbm.reciept_vouchers
                       .Select(o => (int?)o.VoucherNum)
                       .DefaultIfEmpty(0)
                       .Max() ?? 0;
                        sub.Receipt_No = Convert.ToString(maxVoucherNumtest + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

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
                        //  sub.Receipt_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                        int nextVoucherNumManutttrrSub = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                        sub.Receipt_No = Convert.ToString(nextVoucherNumManutttrrSub) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

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
                                        if (checking_id != null && checking_id.Percentage != null && checking_id.Percentage != 0)
                                        {
                                            Dealer_Commision_Detail dealer_Commision_Detail = new Dealer_Commision_Detail();
                                            dealer_Commision_Detail.Dealer_Commsion_ID = checking_id.ID;
                                            dealer_Commision_Detail.Status = false;
                                            dealer_Commision_Detail.Due_date = DateTime.Now;
                                            dealer_Commision_Detail.Percentage = checking_id.Percentage / 3;
                                            dealer_Commision_Detail.Due_Amount = ((double)dealer_Commision_Detail.Percentage * (double)a.Payment_Plan.Net_Price_Plot) / 100;
                                            dbm.Dealer_Commision_Detail.Add(dealer_Commision_Detail);
                                            dbm.SaveChanges();
                                        }
                                        if (a.Comment == "Booking")
                                        {
                                            int c = 0;
                                            foreach (var inst in iii.Where(o => o.Comment != "Booking").ToList())
                                            {
                                                if (inst.Comment != "Quarterly installment")
                                                {

                                                    c = c + 1;
                                                }
                                                inst.Payment_Month = payment_date.AddMonths(c);
                                                dbm.Entry(inst).State = EntityState.Modified;
                                                dbm.SaveChanges();
                                            }
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

                                    int nextVoucherNum = (dbm.reciept_vouchers.Any()) ? dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1 : 1;
                                    sub.Receipt_No = Convert.ToString(nextVoucherNum) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

                                    sub.Date = (DateTime)a.Paid_Month;

                                    dbm.Sub_Installment.Add(sub);
                                    dbm.SaveChanges();

                                }


                            }


                        }
                        if (amount > 0)
                        {
                            var last1 = dbm.Installments.FirstOrDefault(o => o.Installment_ID == last.Installment_ID);
                            last1.Monthly_Paid = last1.Monthly_Paid + amount;
                            dbm.Entry(last1).State = EntityState.Modified;
                            dbm.SaveChanges();
                        }

                    }

                    var memberid = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == Payment_ID);
                    Phone_no = memberid.Plot.Member.Cell_No;
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
                        detail.Pay_Date = payment_date;
                        detail.voucher_check = true;
                        detail.Check_No = checknumber;
                        detail.Manual_Voucher_No = Manual_Voucher_No;
                        detail.transferFromBank = bankName;
                        var head = dbm.Ledger_Account.First(o => o.Ledger_Complete_Code == secondledger).ID;
                        detail.Head = Convert.ToString(head);
                        detail.UserId = Convert.ToInt32(Session["emp_id"]);
                        // detail.Receipt_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                        int maxVoucherNumtest11 = dbm.reciept_vouchers
                       .Select(o => (int?)o.VoucherNum)
                       .DefaultIfEmpty(0)
                       .Max() ?? 0;

                        detail.Receipt_No = Convert.ToString(maxVoucherNumtest11 + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;


                        dbm.Party_Detail.Add(detail);
                        dbm.SaveChanges();
                        detail_id = detail.ID;
                    }
                    //insert data into recept voucher table...............................................................................
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
                    /// rec.Manual_Voucher_No = Convert.ToString(dbm.reciept_vouchers.Max(o => o.VoucherNum) + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;
                    int maxVoucherNumtest111122 = dbm.reciept_vouchers
                      .Select(o => (int?)o.VoucherNum)
                      .DefaultIfEmpty(0)
                      .Max() ?? 0;

                    rec.Manual_Voucher_No = Convert.ToString(maxVoucherNumtest111122 + 1) + "/" + DateTime.Today.Year + "/" + paymentModeShow;

                    // var num = dbm.reciept_vouchers.Max(o => o.VoucherNum);
                    int? maxVoucherNum12 = dbm.reciept_vouchers.Max(o => (int?)o.VoucherNum);

                    int num = maxVoucherNum12 ?? 0; // Use 0 if maxVoucherNum is null

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
                            return RedirectToAction("Manage_Member", "Member");

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
                            TempData["Message"] = "/Party/Print?id=" + detail_id + "";
                            var paid = dbm.Installments.Where(o => o.Payment_ID == Payment_ID).Select(o => o.Monthly_Paid).Sum();
                            total_paid = (double)paid;
                            var x = dbm.Installments.Where(o => o.Payment_ID == Payment_ID).Sum(o => o.Amount_Paid);

                            balance1 = (double)x - (double)paid;
                            App_name = i.Payment_Plan.Plot.Member.Applicant_Name;
                            display_amount = Convert.ToDouble(amount1.ToString("n0"));
                            EB_number = i.Payment_Plan.Plot.Reg_No;

                            return RedirectToAction("view_installment", "Member", new { id = memberid.Plot.Member_ID });
                            //return RedirectToAction("Print", "Party", new { id = detail_id });

                        }

                    }
                }
            }


            return RedirectToAction("Manage_Member", "Member");

        }
        public ActionResult reg_no_chk(string value)
        {
            var chk = dbm.Plots.Any(o => o.Reg_No == value);

            return Json(chk, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ScanThumb()
        {
        searchagain:

            var data = await dbm.ThumbScans.FirstOrDefaultAsync(o => o.isSaved == false);
            if (data != null)
            {
                data.isSaved = true;
                dbm.Entry(data).State = EntityState.Modified;
                dbm.SaveChanges();
                var img = $"data:image/jpeg;base64,{Convert.ToBase64String(data.url)}";
                return Json(img, JsonRequestBehavior.AllowGet);

            }
            Thread.Sleep(3000);

            goto searchagain;


            //   return Json(null, JsonRequestBehavior.AllowGet);

        }

        public ActionResult saa()
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
                        string aa = "Dear Customer, Thank You...!  For Paying Rs. " + display_amount.ToString("n0") + " Against EB-" + EB_number + " Total Paid Rs." + total_paid.ToString("n0") + " And Your Remaining Balance Is Rs. " + balance1.ToString("n0") + "";

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





        public JsonResult Change(FormCollection form)
        {
            //var settings = new List<string> { "Bank", "Cash", "DISCOUNT", "Adjustment", "ONLINETRANSFER", "DEALER ADJUSTMENT", "CASHATBANK" };

            var type = form["type"];
            if (type == "BRV")
            {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Bank");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "BPV")
            {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Bank");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }


            else if (type == "CRV")
            {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Cash");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "DISCOUNT")
            {

                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "DISCOUNT");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "DEALER ADJUSTMENT")
            {

                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "DEALER ADJUSTMENT");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "CASHATBANK")
            {

                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "CASHATBANK");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "ONLINETRANSFER")
            {

                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "ONLINETRANSFER");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else if (type == "CPV")
            {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Cash");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var setting = dbm.Account_Setting.FirstOrDefault(o => o.Setting_Name == "Adjustment");
                var ledger = dbm.Ledger_Account.Where(o => o.Element_ID == setting.Element_ID && o.Control_ID == setting.Control_ID).Select(o => new
                {
                    Ledger_Complete_Code = o.Ledger_Complete_Code,
                    Ledger_Account_Title = o.Ledger_Account_Title
                }


                ).ToList();
                return Json(ledger, JsonRequestBehavior.AllowGet);

                //var setting = dbm.Account_Setting.Where(o => settings.Contains(o.Setting_Name)).ToList();
                //var element = setting.Select(oc => oc.Element_ID).ToList();
                //var control = setting.Select(oc => oc.Control_ID).ToList();
                //var ledger = dbm.Ledger_Account.Where(o => element.Contains(o.Element_ID) && control.Contains(o.Control_ID)).Select(o => new
                //{
                //    Ledger_Complete_Code = o.Ledger_Complete_Code,
                //    Ledger_Account_Title = o.Ledger_Account_Title
                //}


                //).ToList();
                //return Json(ledger, JsonRequestBehavior.AllowGet);
            }


        }

    }


}
