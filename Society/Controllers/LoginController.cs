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
using Newtonsoft.Json.Linq;
using System.IO;

namespace Society.Controllers
{
    public class LoginController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: Login
        public ActionResult Login()
        {
            Session["emp_id"] = null;

            return View();
        }public ActionResult UnAuthorized()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(Employee model)
        {
            var a = dbm.Employees.FirstOrDefault(o => o.CNIC == model.CNIC && o.Password == model.Password && o.Type == model.Type);
            if (a != null)
            {
                if (a.Type == "Employee")
                {
                    Session["CNIC"] = a.CNIC;
                    Session["type"] = a.Type;
                    Session["emp_id"] = a.Employee_ID;
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else if (a.Type == "Admin")
                {
                    Session["CNIC"] = a.CNIC;
                    Session["type"] = a.Type;
                    Session["emp_id"] = a.Employee_ID;
                   
                    return RedirectToAction("Dashboard", "Dashboard");
                }
               
            }
            return View();
        }
        public void Create()
        {
            try
            {
                HttpWebRequest myReq;
                HttpWebResponse myResp;
                StreamReader myReader;
                myReq = (HttpWebRequest)WebRequest.Create("https://portal.zekli.com:9090/api/Auth/Login");
                myReq.Method = "POST";
                myReq.ContentType = "application/json";
                myReq.Accept = "application/json";
                string myData = "{\"userName\":\"C3225127422\",\"password\":\"Q%$mT$4387\"}";
                myReq.GetRequestStream().Write(System.Text.Encoding.UTF8.GetBytes(myData), 0, System.Text.Encoding.UTF8.GetBytes(myData).Count());
                myResp = (HttpWebResponse)myReq.GetResponse();
                myReader = new System.IO.StreamReader(myResp.GetResponseStream());
                var reader = myReader.ReadToEnd();
                var data = JObject.Parse(reader);
                Session["Token"] = Convert.ToString(data["data"]["userInfo"]["token"]);
            }
            catch (WebException ex)
            {
                
            }
        }
        
    }
}