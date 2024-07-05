using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace Society.Controllers
{
    public class LoanController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Loan
        public ActionResult Manage_Loan()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Loan");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var list = dbm.Loans.ToList();
            return View(list);
        }
        public ActionResult AddNew_Loan()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Loan");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var emp_list = dbm.Salary_With_Allowences.ToList();
            return View(emp_list);
        }
        public async Task<ActionResult> ResizeAsyncInstallment(int skip)
        {
            var data = await dbm.Installments.Where(o => o.Image != null).ToListAsync();
            foreach (var file in data)
            {

                if (file.Image != null && file.Image.Length > 0)
                {

                    var baseImage = ResizeImage(file.Image, 600, 600);
                    file.Image = baseImage;

                    dbm.Entry(file).State = EntityState.Modified;
                    dbm.SaveChanges();

                }
            }
            return View();
        }
        public async Task<ActionResult> ResizeAsync(int skip)
        {
            var data =await dbm.reciept_vouchers.Where(o=>o.Image!=null).ToListAsync();
            foreach(var file in data)
            {

                if (file.Image != null && file.Image.Length > 0)
                {

                    var baseImage = ResizeImage(file.Image,500,500);
                    file.Image =baseImage;
                   
                    dbm.Entry(file).State = EntityState.Modified;
                    dbm.SaveChanges();
                  
                }
            }return View();
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
        public ActionResult AddNew_Loan(Loan model)
        {

            var check = dbm.Loans.FirstOrDefault(o => o.Employee.Employee_ID == model.Emp_ID);
            if (check == null)
            {
                Loan l = new Loan();
                l.Emp_ID = model.Emp_ID;
                l.Loan_Amount = model.Loan_Amount;
                l.Loan_Installments = model.Loan_Installments;
                l.No_Of_Installments = model.No_Of_Installments;
                l.Single_Installment_Amount = model.Single_Installment_Amount;
                dbm.Loans.Add(l);
                if (dbm.SaveChanges() > 0)
                {
                    for (int i = 1; i <= l.No_Of_Installments; i++)
                    {
                        DateTime today = DateTime.Today;
                        Loan_Installments installment = new Loan_Installments();
                        installment.Loan_ID = l.Loan_ID;
                        installment.Installment_Number = i;
                        installment.IsPaid = "Not Paid";
                        installment.Payment_Month = today.AddMonths(i);
                        installment.Amount = l.Single_Installment_Amount;



                        dbm.Loan_Installments.Add(installment);
                        dbm.SaveChanges();

                    }
                    return RedirectToAction("Manage_Loan", "loan");
                }
            }
            else
            {
                return RedirectToAction("Manage_Loan", "loan");
            }
            return RedirectToAction("Manage_Loan", "loan");

        }
        public ActionResult Delete(int id)
        {
            var x = dbm.Loans.FirstOrDefault(o => o.Loan_ID == id);
            if (x != null)
            {
                var y = dbm.Loan_Installments.Where(o => o.Loan_ID == id).ToList();
                foreach (var i in y)
                {

                    dbm.Loan_Installments.Remove(i);
                }
                dbm.Loans.Remove(x);
                dbm.SaveChanges();

            }



            return RedirectToAction("Manage_Loan", "loan");
        }
        public ActionResult View(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "Manage Loan");
                ViewBag.Key = s.Target;
            }
            dynamic dy = new ExpandoObject();
            dy.data = dbm.Loans.FirstOrDefault(o => o.Loan_ID == id);
            dy.list = dbm.Loan_Installments.Where(p => p.Loan_ID == id).ToList();
            return View(dy);


        }

    }
}