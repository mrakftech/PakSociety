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
    public class AttendenceController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Attendence
        public ActionResult Mark_Attendence()
        {
           
            if (Session["CNIC"] == null|| Session["type"]==null) {

                RedirectToAction("Login", "Login");
            }
            else {
                var CNIC = Session["CNIC"].ToString();
                var id = Convert.ToInt32(Session["emp_id"].ToString());
                if (Session["type"].ToString() == "Admin")
                {
                    dynamic dy = new ExpandoObject();

                    var emp_List = dbm.Employees.ToList();

                    dy.emp_List = emp_List;
                    dy.shift = dbm.Shifts.ToList();
                    var today_atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today).ToList();
                    dy.today_atndc = today_atndc;
                    return View(dy);
                }
                else
                {
                    dynamic dy = new ExpandoObject();

                    var emp_List = dbm.Employees.Where(o => o.CNIC == CNIC).ToList();

                    dy.emp_List = emp_List;
                    dy.shift = dbm.Shifts.ToList();
                    var today_atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today && o.Employee_ID == id).ToList();
                    dy.today_atndc = today_atndc;
                    return View(dy);
                }
            }
            return View();
           
        }
        public JsonResult Mark_absent_json(FormCollection form)
        {
            var id = Convert.ToInt32(form["employe_id"]);
            var exist = dbm.Attendences.FirstOrDefault(o => o.Attendence_date == DateTime.Today && o.Employee_ID == id);
            if (exist == null)
            {

                var detail = dbm.Employees.FirstOrDefault(o => o.Employee_ID == id);
                Attendence a = new Attendence();
                a.Employee_ID = id;
                a.Employee_Name = detail.F_Name + "" + detail.L_Name;
                a.Attendence_Status = false;
                a.Attendence_date = DateTime.Today;




                dbm.Attendences.Add(a);
                dbm.SaveChanges();
                var atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today).Select(o=> new {
                    ID = o.ID,
                    Employee_ID = o.Employee_ID,
                    Employee_Name = o.Employee_Name,
                    IN_Time = o.IN_Time,
                    OUT_Time = o.OUT_Time,
                    Attendence_date = o.Attendence_date,
                    Over_Time = o.Over_Time,
                    Attendence_Status = o.Attendence_Status,
                    Shift = o.Shift

                }).ToList();
                var json = JsonConvert.SerializeObject(atndc);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string e = "exist";
                return Json(e, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Mark_attendence_json(FormCollection form)
        {
            var id = Convert.ToInt32(form["employe_id"]);
            var exist = dbm.Attendences.FirstOrDefault(o => o.Attendence_date == DateTime.Today && o.Employee_ID == id);
            if (exist == null)
            {
                TimeSpan intime = TimeSpan.Parse(form["intime"]);
                var overtime = form["overtime"];
                var shift = form["shift"];
                int hours;
                if (overtime == "")
                {
                    hours = 0;
                }
                else
                {
                    hours = Convert.ToInt32(overtime);
                }
                var detail = dbm.Employees.FirstOrDefault(o => o.Employee_ID == id);
                Attendence a = new Attendence();
                a.Employee_ID = id;
                a.Employee_Name = detail.F_Name + "" + detail.L_Name;
                a.Attendence_Status = true;
                a.Attendence_date = DateTime.Today;
                a.IN_Time = intime;
                a.OUT_Time = null;
                a.Over_Time = hours;
                a.Shift = shift;
                dbm.Attendences.Add(a);
                dbm.SaveChanges();
                var atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today).Select(o => new {
                    ID = o.ID,
                    Employee_ID = o.Employee_ID,
                    Employee_Name = o.Employee_Name,
                    IN_Time = o.IN_Time,
                    OUT_Time = o.OUT_Time,
                    Attendence_date = o.Attendence_date,
                    Over_Time = o.Over_Time,
                    Attendence_Status = o.Attendence_Status,
                    Shift = o.Shift

                }).ToList();
                var json = JsonConvert.SerializeObject(atndc);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string e = "exist";
                return Json(e, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Update_attendence_json(FormCollection form)
        {
            var id = Convert.ToInt32(form["id"]);
            var in_time = TimeSpan.Parse(form["in_time"]);
            var out_time = TimeSpan.Parse(form["out_time"]);

            TimeSpan TS = TimeSpan.Parse(form["out_time"]) - TimeSpan.Parse(form["in_time"]);
            float over_time_hour = TS.Hours;
            float over_Time_minute = TS.Minutes;
            float complete_over_time = (over_time_hour * 60) + over_Time_minute;
            float time = complete_over_time / 60;

            var shift = form["shift"];
            var value = dbm.Shifts.FirstOrDefault(o => o.Shift_Name == shift);
            double difference = Math.Round(time - (float)value.WorkHours, 2);



            var a = dbm.Attendences.FirstOrDefault(o => o.ID == id);

            a.IN_Time = in_time;
            a.OUT_Time = out_time;
            a.Over_Time = difference;
            a.Shift = shift;
            a.Attendence_Status = true;
            dbm.Entry(a).State = EntityState.Modified;
            dbm.SaveChanges();
            var atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today).Select(o => new {
                ID = o.ID,
                Employee_ID = o.Employee_ID,
                Employee_Name = o.Employee_Name,
                IN_Time = o.IN_Time,
                OUT_Time = o.OUT_Time,
                Attendence_date = o.Attendence_date,
                Over_Time = o.Over_Time,
                Attendence_Status = o.Attendence_Status,
                Shift = o.Shift

            }).ToList();
            var json = JsonConvert.SerializeObject(atndc);

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult delete(FormCollection form)
        {
            var id = Convert.ToInt32(form["id"]);
            var a = dbm.Attendences.FirstOrDefault(o => o.ID == id);
            dbm.Attendences.Remove(a);
            dbm.SaveChanges();
            var atndc = dbm.Attendences.Where(o => o.Attendence_date == DateTime.Today).Select(o => new {
                ID = o.ID,
                Employee_ID = o.Employee_ID,
                Employee_Name = o.Employee_Name,
                IN_Time = o.IN_Time,
                OUT_Time = o.OUT_Time,
                Attendence_date = o.Attendence_date,
                Over_Time = o.Over_Time,
                Attendence_Status = o.Attendence_Status,
                Shift = o.Shift

            }).ToList();
            if (atndc.Count == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);


            }
            else
            {
                var json = JsonConvert.SerializeObject(atndc);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
           


        }
        public ActionResult Monthly_Attendence_Report()
        {
            dynamic dy = new ExpandoObject();

            var emp_List = dbm.Employees.ToList();

            dy.emp_List = emp_List;
            dy.attendence = dbm.Attendences.ToList();
            return View(dy);

        }
        public JsonResult Monthly_Attendence_Report_json(FormCollection form)
        {
            var id = Convert.ToInt32(form["emp_id"]);
            var fromdate = Convert.ToDateTime(form["fromdate"]);
            var todate = Convert.ToDateTime(form["todate"]);
           

            var atndc = dbm.Attendences.Where(o => o.Employee_ID == id && o.Attendence_date >= fromdate && o.Attendence_date <= todate).Select(o => new {
                ID = o.ID,
                Employee_ID = o.Employee_ID,
                Employee_Name = o.Employee_Name,
                IN_Time = o.IN_Time,
                OUT_Time = o.OUT_Time,
                Attendence_date = o.Attendence_date,
                Over_Time = o.Over_Time,
                Attendence_Status = o.Attendence_Status,
                Shift = o.Shift

            }).ToList();
            if (atndc.Count == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);


            }
            else
            {
                var json = JsonConvert.SerializeObject(atndc);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
       



    }
}