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
    public class PayroleController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Payrole
        public ActionResult Manage_Payrole()
        {
            var a = dbm.PayRoles.Where(o => o.Month == DateTime.Now.Month && o.Year ==DateTime.Now.Year).ToList();
           
            return View(a);
        }
        public ActionResult Payrole_list()
        {
            var a = dbm.PayRoles.Where(o => o.PayRole_ID==0).ToList();

            return View(a);
        }
        [HttpPost]
        public ActionResult Payrole_list(DateTime month)
        {
            var a = dbm.PayRoles.Where(o => o.Period.Year == month.Year && o.Period.Month==month.Month).ToList();

            return View(a);
        }
        [HttpPost]

        public ActionResult Manage_Payrole(DateTime payrole_date)
        {

            var check = dbm.PayRoles.FirstOrDefault(o => o.Period.Month == payrole_date.Month);

            if (check == null) { 
            double deduct_Installment = 0;
            double deduct_Advance = 0;
            double add_overtime = 0;
            double hours = 0;
            var employee = dbm.Salary_With_Allowences.ToList();
            foreach (var e in employee)
            {
                PayRole run = new PayRole();
                run.Emp_ID = e.Emp_ID;
                run.Year = payrole_date.Year;
                run.Month = payrole_date.Month;
                run.Period = payrole_date;
                run.Gross_Salary = e.Gross_Salary;
                var installment = dbm.Loan_Installments.FirstOrDefault(o => o.Loan.Emp_ID == e.Emp_ID && o.Payment_Month.Month == payrole_date.Month && o.Payment_Month.Year == payrole_date.Year && o.IsPaid == "Not Paid");
                if (installment != null)
                {
                    installment.IsPaid = "Paid";
                    dbm.Entry(installment).State = EntityState.Modified;
                    dbm.SaveChanges();

                    deduct_Installment = (double)installment.Amount;
                }

                var advance = dbm.Advances.FirstOrDefault(o => o.Emp_ID == e.Emp_ID && o.Month == payrole_date.Month && o.Year == payrole_date.Year && o.Status == "Un Paid");
                if (advance != null)
                {
                    advance.Status = "Paid";
                    dbm.Entry(advance).State = EntityState.Modified;
                    dbm.SaveChanges();

                    deduct_Advance = (double)advance.Advance_Amount;
                }

                var overtime = dbm.Attendences.Where(o => o.Employee_ID == e.Emp_ID && o.Attendence_date.Month == payrole_date.Month && o.Attendence_date.Year == payrole_date.Year).ToList();
                foreach (var i in overtime)
                {
                    hours = hours + (double)i.Over_Time;









                }
                if (hours > 0)
                {
                    add_overtime = (double)hours * (double)e.Rate_Per_Hour;
                    if (add_overtime > 0)
                    {
                        run.Net_Salary = (e.Gross_Salary - deduct_Advance - deduct_Installment) + add_overtime;
                        OverTime o = new OverTime();
                        o.Emp_ID = e.Emp_ID;
                        o.Month = payrole_date;
                        o.No_Of_Hours = hours;
                        o.Rate_Per_Hour = e.Rate_Per_Hour;
                        o.Total_Amount = add_overtime;

                        dbm.OverTimes.Add(o);
                        dbm.SaveChanges();
                    }
                    else { run.Net_Salary = e.Gross_Salary - deduct_Advance - deduct_Installment; }
                }


                run.Net_Salary = e.Gross_Salary - deduct_Advance - deduct_Installment;
                run.Salary_ID = e.Salary_ID;

                dbm.PayRoles.Add(run);
                dbm.SaveChanges();
                deduct_Advance = 0;
                deduct_Installment = 0;
                add_overtime = 0;
                hours = 0;
            }

            return RedirectToAction("Manage_Payrole", "Payrole");
        }else{
                ViewBag.error = "Alere";

                var a = dbm.PayRoles.Where(o => o.Month == DateTime.Now.Month && o.Year == DateTime.Now.Year).ToList();
            
                return View(a);
            }
        }
        public ActionResult Salary_Slip(int id,DateTime month)
        {
            dynamic dy = new ExpandoObject();
           
            var a = dbm.PayRoles.FirstOrDefault(o => o.PayRole_ID == id);
            dy.Payroles = a;
            var advance = dbm.Advances.Where(o => o.Emp_ID == a.Emp_ID && o.Month == month.Month && o.Year == month.Year).Select(o => o.Advance_Amount).DefaultIfEmpty().First();
            if (advance == null)
            {
                dy.Advances = 0;
            }
            else
            {
                dy.Advances = advance;
            }

            var loan = dbm.Loan_Installments.Where(o => o.Loan.Emp_ID == a.Emp_ID && o.IsPaid == "Paid" && o.Payment_Month.Month == month.Month && o.Payment_Month.Year == month.Year).Select(o => o.Amount).DefaultIfEmpty().First();
            if (loan == null)
            {
                dy.Loans = 0;
            }
            else
            {
                dy.Loans = loan;

            }
            var overtime = dbm.OverTimes.Where(o => o.Emp_ID == a.Emp_ID && o.Month.Month == month.Month && o.Month.Year == month.Year).Select(o => o.Total_Amount).DefaultIfEmpty().First();
            if (overtime == null)
            {
                dy.OverTimes = 0;

            }
            else
            {
                dy.OverTimes = overtime;
            }



            dy.Salary_With_Allowences = dbm.Salary_With_Allowences.FirstOrDefault(o => o.Emp_ID == a.Emp_ID);
            return View(dy);
        }
        

    }
}