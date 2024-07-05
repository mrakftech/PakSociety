using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Society.Data;
using System.Dynamic;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Society.Controllers
{
    public class FormController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Advance
        // GET: Account_Setting


        public ActionResult Verification()
        {
            return View();
        }
        public ActionResult VerificationUpdate(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "OverSeas Form");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View(dbm.Overseas.FirstOrDefault(o =>o.id ==id));
        }
        public ActionResult VerificationApprove(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "OverSeas Form");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var data = dbm.Overseas.FirstOrDefault(o => o.id == id);
            if(data == null)
            {
                TempData["NoDataFound"] = "No Record Found!!!";
                return RedirectToAction("ManageForm", "Form");
            }
            var preiousMember = dbm.Plots.FirstOrDefault(o =>o.Reg_No == data.ReferenceNo);
            if (preiousMember == null)
            {
                TempData["NoDataFound"] = $"No record found for Reference No {data.ReferenceNo}";
                return RedirectToAction("ManageForm", "Form");
            }
            preiousMember.Member.Applicant_Name = data.Name;
            preiousMember.Member.Father_Husband_Name = data.SoDoWo;
            preiousMember.Member.CNIC = data.CNICOrPassport;
            preiousMember.Member.Cell_No = data.Contact;
            preiousMember.Member.Office_No = data.SecondaryContact;
            preiousMember.Member.Email = data.Email;
            preiousMember.Member.Profile_Img = data.profileImage;
            preiousMember.Member.Permenent_Postel_Address = data.pakistanAddress;
            preiousMember.Member.Present_Postel_Address = data.OverseasAddress;
            preiousMember.Member.Nominee_Name = data.KinName;
            preiousMember.Member.Country = data.Country;
            preiousMember.Member.NomineeFather = data.KinSoDoWo;
            preiousMember.Member.Nominee_CNIC = data.kinCNIC;
            preiousMember.Member.Nominee_Phone_No = data.KinContact;
            dbm.Entry(preiousMember).State = EntityState.Modified;
            dbm.SaveChanges();
            TempData["Success"] = "Approved and record updated successfully...";
            return RedirectToAction("ManageForm", "Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerificationUpdate(Oversea oversea, string Profile_Image)
        {
            var exist = await dbm.Overseas.FirstOrDefaultAsync(o =>o.id !=oversea.id && o.CNICOrPassport.Contains(oversea.CNICOrPassport));
            if (exist != null)
            {
                ViewBag.Exist = "CNIC already exist...";
                return View();
            }

            if (!string.IsNullOrWhiteSpace(Profile_Image))
            {
                Profile_Image = Profile_Image.Split(',')[1];
                oversea.profileImage = Convert.FromBase64String(Profile_Image);

            }
            dbm.Entry(oversea).State = EntityState.Modified;
            dbm.SaveChanges();

            return RedirectToAction("ManageForm", "Form");
        }
        public async Task<ActionResult> delete(int id)
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "OverSeas Form");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            var data =await dbm.Overseas.FirstOrDefaultAsync(o =>o.id ==  id);
            dbm.Overseas.Remove(data);
            await dbm.SaveChangesAsync();
            return RedirectToAction("ManageForm", "Form");
        }
        public async Task<ActionResult> ManageForm()
        {
            if (Session["emp_id"] == null) return RedirectToAction("UnAuthorized", "Login");
            {
                var emp_ID = Convert.ToInt32(Session["emp_id"]);
                var aaaaa = dbm.Employees.FirstOrDefault(o => o.Employee_ID == emp_ID);
                var s = aaaaa.role.Role_Permission.FirstOrDefault(o => o.Roles_ID == aaaaa.Role_ID && o.Module_Key == "OverSeas Form");
                ViewBag.Key = s.Target;
                if (s.Target == false)
                {
                    return RedirectToAction("UnAuthorized", "Login");
                }
            }
            return View(await dbm.Overseas.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Verification(Oversea oversea, string Profile_Image)
        {
            var exist =await dbm.Overseas.FirstOrDefaultAsync(o => o.CNICOrPassport.Contains(oversea.CNICOrPassport));
            if (exist != null)
            {
                ViewBag.Exist = "CNIC already exist...";
                return View();
            }

            if (!string.IsNullOrWhiteSpace(Profile_Image))
            {
                Profile_Image = Profile_Image.Split(',')[1];
                oversea.profileImage = Convert.FromBase64String(Profile_Image);

            }
            dbm.Overseas.Add(oversea);
            await dbm.SaveChangesAsync();
            ViewBag.Success = "Application Submitted Successfully...";
            return View();
        }
    }
}