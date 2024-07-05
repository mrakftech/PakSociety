using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace Society.Controllers
{
    public class File_TransferController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: File_Transfer
        public async Task<ActionResult> fromScanThumb()
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

        public async Task<ActionResult> toScanThumb()
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

        public ActionResult Manage_File_Transfer()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage File Transfer");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            dynamic dy = new ExpandoObject();

            dy.a = dbm.Payment_Plan.Where(o => o.Plot.Member.Status == true).ToList().Take(100);

            return View(dy);
        }
        public ActionResult filter(string search1)
        {
            if (search1 != "")
            {

                var trips = dbm.Payment_Plan.Where(o => o.Plot.Reg_No == search1 && o.Plot.Member.Status == true).ToList()
                    .Select(o => new
                    {
                        o.Plot.Reg_No,
                        o.Plot.Member.Applicant_Name,
                        o.Plot.Member.CNIC,
                        o.Plot.Member.Permenent_Postel_Address,

                        o.Plot.Date,

                        o.Payment_ID


                    }).Take(200);
                var json = JsonConvert.SerializeObject(trips);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else if (search1 == "")
            {
                var trips1 = dbm.Payment_Plan.Where(o => o.Plot.Member.Status == true).ToList()
                   .Select(o => new
                   {
                       o.Plot.Reg_No,
                       o.Plot.Member.Applicant_Name,
                       o.Plot.Member.CNIC,
                       o.Plot.Member.Permenent_Postel_Address,

                       o.Plot.Date,

                       o.Payment_ID


                   }).Take(200);
                var json = JsonConvert.SerializeObject(trips1);
                return Json(json, JsonRequestBehavior.AllowGet);

            }

            return Json(JsonRequestBehavior.AllowGet);


        }

        public ActionResult File_Transfer(int ID)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage File Transfer");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var a = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == ID);
            ViewBag.plotsize = dbm.Installment_Plan.ToList();
            ViewBag.amount = dbm.Installments.Where(o => o.Payment_ID == a.Payment_ID).Sum(o => o.Amount_Paid);
            return View(a);
        }
        [HttpPost]
        public ActionResult File_Transfer(Payment_Plan model1, Member model, FormCollection form,string fromImage,string toImage,string fromThumbData, string toThumbData, string groupPhoto)
        {
            double final = 0;
            var oldcnic = form["oldcnic"];
            var oldname = form["oldname"];
            var Plot_Size = form["Plot_Size"];
            var new_date = form["new_date"];
            double amount2 = Convert.ToDouble(form["amount"]);
            var new_date2 = DateTime.Now;
            var reg = dbm.Plots.FirstOrDefault(o => o.Member_ID == model.Member_ID);
            var old_size = reg.Installment_Plan.Plan_Name;
            var new_size = reg.Installment_Plan.Plan_Name;

            var Plot_Size11 = Convert.ToInt32(Plot_Size);
            var plan = dbm.Installment_Plan.FirstOrDefault(o => o.ID == Plot_Size11);
            if (Plot_Size != null) { new_size = plan.Plan_Name; }
            if (amount2 < plan.Total_Price)
            {
                model1.Extra_Charge = 0;
                model1.Discount = plan.Total_Price - amount2;

            }
            else if (amount2 == plan.Total_Price)
            {
                model1.Extra_Charge = 0;
                model1.Discount = 0;
            }
            else
            {
                model1.Extra_Charge = amount2 - plan.Total_Price;
                model1.Discount = 0;

            }


            /*  if (plan1.Total_Price < price_of_Plot)
              {
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Extra_Charge = price_of_Plot - plan1.Total_Price;
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Discount = 0;
              }
              else if (plan1.Total_Price == price_of_Plot)
              {
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Extra_Charge = 0;
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Discount = 0;
              }
              else
              {
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Extra_Charge = 0;
                  plot.Payment_Plan.First(o => o.Plot_ID == model1.Plot_ID).Discount = plan1.Total_Price - price_of_Plot;

              }*/
            var find = reg.Reg_No;
            if (new_date != null)
            {
                var new_date1 = Convert.ToDateTime(new_date);
                var installment_update = dbm.Installments.Where(o => o.Payment_ID == model1.Payment_ID && o.IsPaid == false).ToList().OrderBy(o => o.Installment_No);
                int count = 0;
                foreach (var ii in installment_update)
                {
                    ii.Payment_Month = new_date1.AddMonths(count);
                    dbm.Entry(ii).State = EntityState.Modified;
                    dbm.SaveChanges();
                    count = count + 1;
                }
            }
            if (Plot_Size != null)
            {
                var Plot_Size1 = Convert.ToInt32(Plot_Size);


                var update_plot_size = dbm.Plots.FirstOrDefault(o => o.Member_ID == model.Member_ID);
                update_plot_size.Size = Plot_Size1;
                dbm.Entry(update_plot_size).State = EntityState.Modified;
                dbm.SaveChanges();

                var update_payment_plan = dbm.Payment_Plan.FirstOrDefault(o => o.Payment_ID == model1.Payment_ID);
                update_payment_plan.Net_Price_Plot = amount2;
                dbm.Entry(update_payment_plan).State = EntityState.Modified;
                dbm.SaveChanges();


                var paid_amount = dbm.Installments.Where(o => o.Payment_ID == model1.Payment_ID).Sum(o => o.Monthly_Paid);
                var findx = dbm.Parties.First(o => o.Member_ID == model.Member_ID);
                var find1 = dbm.Party_Detail.Where(o => o.Party_ID == findx.ID).ToList();

                var payment_month = dbm.Installments.First(o => o.Payment_ID == model1.Payment_ID).Payment_Month;
                if (new_date != null)
                {
                    payment_month = Convert.ToDateTime(new_date);
                }

                var installment_update = dbm.Installments.Where(o => o.Payment_ID == model1.Payment_ID).ToList();
                foreach (var ii in installment_update)
                {


                    var dlt = dbm.Sub_Installment.Where(o => o.Installment_ID == ii.Installment_ID).ToList();
                    dbm.Sub_Installment.RemoveRange(dlt);
                    dbm.SaveChanges();
                }
                var possessionValue = installment_update.FirstOrDefault(o=>o.Comment.Contains("Po")).Amount_Paid;
                dbm.Installments.RemoveRange(installment_update);
                dbm.SaveChanges();

                var actual_price = plan.Total_Price - amount2;
                var count = (plan?.No_Of_Half_Yearly ?? 0) + (plan?.No_Of_Monthly ?? 0) + (plan?.NoOfQuaterly ?? 0) + (plan?.NoOfYearly ?? 0);

                //--------------------------------------------------------------------------------------
                Installment i = new Installment();
                i.Payment_ID = model1.Payment_ID;


                i.IsPaid = false;
                i.Payment_Month = payment_month;
                i.Amount_Paid = plan.Booking;
                i.Installment_No = 1;
                i.Comment = "Booking";
                i.Monthly_Paid = 0;
                i.Single_Monthly_Installment = plan.Monthly;
                i.Single_Half_Yearly_Installment = plan.Half_Yearly;
                i.no_of_half_yearly_installments = plan.No_Of_Half_Yearly;
                i.no_of_monthly_installments = plan.No_Of_Monthly;

                dbm.Installments.Add(i);
                dbm.SaveChanges();
                //---------------------------------------------------------------------------------------
                i.Payment_ID = model1.Payment_ID;
                i.IsPaid = false;
                i.Payment_Month = payment_month.AddMonths(2);
                i.Amount_Paid = plan.Allocation;
                i.Installment_No = 2;
                i.Comment = "1st Installments";
                i.Monthly_Paid = 0;
                i.Single_Monthly_Installment = plan.Monthly;
                i.Single_Half_Yearly_Installment = plan.Half_Yearly;
                i.no_of_half_yearly_installments = plan.No_Of_Half_Yearly;
                i.no_of_monthly_installments = plan.No_Of_Monthly;
                dbm.Installments.Add(i);
                dbm.SaveChanges();
                //---------------------------------------------------------------------------------------

                int installmentCount = 2;

                for (int j = 1; j <= count; j++)
                {
                   

                        i.Payment_Month = payment_month.Date.AddMonths(installmentCount+3);
                        i.Amount_Paid = plan.Quatlerly_installment;
                        i.Installment_No = j+2;
                        i.Comment = "Quarterly installment";
                        i.Monthly_Paid = 0;
                        dbm.Installments.Add(i);
                        dbm.SaveChanges();



                    installmentCount = installmentCount + 3;
                }
                i.Payment_Month = payment_month.Date.AddMonths((int)installmentCount + 1);
                i.Installment_No = count + 3;
                i.Amount_Paid = possessionValue - actual_price;
                i.Comment = "Possession";
                i.Monthly_Paid = 0;
                dbm.Installments.Add(i);
                dbm.SaveChanges();
                double remaining_amount = 0;
                if (paid_amount != 0)
                {
                    var add = dbm.Installments.Where(o => o.Payment_ID == model1.Payment_ID).ToList().OrderBy(o => o.Installment_No).FirstOrDefault();


                    foreach (var c in find1)
                    {

                        double amount = 0;


                        amount = (double)c.Current_Amount - (double)c.Previous_Amount;
                        var amount1 = amount;
                        if (amount == (add.Amount_Paid - add.Monthly_Paid))
                        {
                            add.IsPaid = true;
                            add.Monthly_Paid = add.Monthly_Paid + amount;
                            add.Paid_Month = c.Pay_Date;


                            dbm.Entry(add).State = EntityState.Modified;
                            dbm.SaveChanges();
                            Sub_Installment sub = new Sub_Installment();
                            sub.Installment_ID = add.Installment_ID;
                            sub.Amount = amount;
                            sub.Receipt_No = c.Receipt_No;
                            sub.Date = (DateTime)add.Paid_Month;
                            dbm.Sub_Installment.Add(sub);
                            dbm.SaveChanges();




                        }
                        else if (amount < (add.Amount_Paid - add.Monthly_Paid))
                        {

                            add.Monthly_Paid = add.Monthly_Paid + amount;

                            add.Paid_Month = c.Pay_Date;


                            dbm.Entry(add).State = EntityState.Modified;
                            dbm.SaveChanges();
                            Sub_Installment sub = new Sub_Installment();
                            sub.Installment_ID = add.Installment_ID;
                            sub.Amount = amount;
                            sub.Receipt_No = c.Receipt_No;
                            sub.Date = (DateTime)add.Paid_Month;
                            dbm.Sub_Installment.Add(sub);
                            dbm.SaveChanges();

                        }
                        else if (amount > (add.Amount_Paid - add.Monthly_Paid))
                        {
                            var iii = dbm.Installments.Where(o => o.Payment_ID == model1.Payment_ID && o.IsPaid == false).ToList().OrderBy(o => o.Installment_No);
                            var last = iii.LastOrDefault();
                            foreach (var aa in iii)
                            {
                                if (aa.Monthly_Paid == null) { aa.Monthly_Paid = 0; }
                                if (amount > 0)
                                {
                                    if (amount >= (aa.Amount_Paid - aa.Monthly_Paid))
                                    {
                                        var temp = aa.Monthly_Paid;
                                        aa.Monthly_Paid = aa.Amount_Paid;
                                        aa.IsPaid = true;
                                        remaining_amount = (double)aa.Monthly_Paid - (double)temp;
                                        amount = (double)amount - ((double)aa.Monthly_Paid - (double)temp);
                                    }
                                    else if (amount < (aa.Amount_Paid - aa.Monthly_Paid))
                                    {
                                        aa.Monthly_Paid = aa.Monthly_Paid + amount;
                                        remaining_amount = (double)amount;
                                        amount = (double)amount - (double)aa.Monthly_Paid;
                                    }





                                    aa.Paid_Month = c.Pay_Date;
                                    dbm.Entry(aa).State = EntityState.Modified;
                                    if (dbm.SaveChanges() > 0)
                                    {

                                        Sub_Installment sub = new Sub_Installment();
                                        sub.Installment_ID = aa.Installment_ID;
                                        sub.Amount = remaining_amount;
                                        sub.Receipt_No = c.Receipt_No;
                                        sub.Date = (DateTime)aa.Paid_Month;
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

                    }
                }


            }

            var a = dbm.Members.FirstOrDefault(o => o.Member_ID == model.Member_ID);

            a.Applicant_Name = model.Applicant_Name;
            a.Applicant_Start_Text = model.Applicant_Start_Text;

            a.Cell_No = model.Cell_No;
            a.CNIC = model.CNIC;
            a.Email = model.Email;
            a.Father_Husband_Name = model.Father_Husband_Name;
            a.Father_Husband_Start_Text = model.Father_Husband_Start_Text;
            a.Nominee_Address = model.Nominee_Address;
            a.Nominee_CNIC = model.Nominee_CNIC;
            a.Nominee_Name = model.Nominee_Name;
            a.Country = model.Country;
            a.NomineeFather = model.NomineeFather;
            a.Nominee_Phone_No = model.Nominee_Phone_No;
            a.Occupation = model.Occupation;
            a.Office_No = model.Office_No;
            a.Permenent_Postel_Address = model.Permenent_Postel_Address;
            a.Present_Postel_Address = model.Present_Postel_Address;
            a.Relation = model.Relation;
            if (!string.IsNullOrWhiteSpace(toImage))
            {
                toImage = toImage.Split(',')[1];
                a.Profile_Img = Convert.FromBase64String(toImage);

            }
            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            var mm = dbm.Plots.FirstOrDefault(o => o.Member_ID == model.Member_ID);
            var plot = dbm.Plot_History.FirstOrDefault(o => o.Plot_ID == mm.Plot_ID && o.Status == true);
            plot.Status = false;

            dbm.Entry(plot).State = EntityState.Modified;
            dbm.SaveChanges();
            Plot_History update_plot = new Plot_History();
            update_plot.Status = true;
            update_plot.Plot_ID = mm.Plot_ID;
            update_plot.Address = a.Permenent_Postel_Address;
            update_plot.H_Date = DateTime.Now;
            update_plot.Name = a.Applicant_Name;
            update_plot.Phone_Number = a.Cell_No;
            update_plot.CNIC = a.CNIC;
            update_plot.History_No = plot.History_No + 1;
            dbm.Plot_History.Add(update_plot);
            dbm.SaveChanges();
            var Plot_Size2 = Convert.ToInt32(Plot_Size);
            var plan1 = dbm.Installment_Plan.FirstOrDefault(o => o.ID == Plot_Size2);

            var up = dbm.Payment_Plan.First(o => o.Plot_ID == mm.Plot_ID);
            up.Gross_Price_Plot = amount2;
            up.Net_Price_Plot = amount2;
            up.Discount = model1.Discount;
            up.Extra_Charge = model1.Extra_Charge;
            up.Pocession_Payment = plan.Pocession - final;

            dbm.Entry(up).State = EntityState.Modified;
            dbm.SaveChanges();
            
            
            File_Transfer f = new File_Transfer();
            f.EB_No = find;
            f.From_CNIC = oldcnic + "(" + old_size + ")";
            f.From_Name = oldname;
            f.To_CNIC = model.CNIC + "(" + new_size + ")"; ;
            f.To_Name = model.Applicant_Name;
            if (!string.IsNullOrWhiteSpace(fromImage))
            {
                fromImage = fromImage.Split(',')[1];
                f.fromImage = Convert.FromBase64String(fromImage);

            }
            if (!string.IsNullOrWhiteSpace(groupPhoto))
            {
                groupPhoto = groupPhoto.Split(',')[1];
                f.groupPhoto = Convert.FromBase64String(groupPhoto);

            }
            if (!string.IsNullOrWhiteSpace(fromThumbData))
            {
                fromThumbData = fromThumbData.Split(',')[1];
                f.fromThumb = Convert.FromBase64String(fromThumbData);

            }
            if (!string.IsNullOrWhiteSpace(toThumbData))
            {
                toThumbData = toThumbData.Split(',')[1];
                f.toThumb = Convert.FromBase64String(toThumbData);

            }
            f.toImage = a.Profile_Img;
            f.memberId = a.Member_ID;
            var transfer_date = Convert.ToDateTime(form["transfer_date"]);
            f.Date = transfer_date;
            dbm.File_Transfer.Add(f);
            dbm.SaveChanges();

            return RedirectToAction("Manage_File_Transfer", "File_Transfer");
        }
    }
}