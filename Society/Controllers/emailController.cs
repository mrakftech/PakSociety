using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Twilio.Clients;
using Twilio;
using Society.Data;
using System.Data.Entity;

namespace Society.Controllers
{
    public class emailController : Controller
    {
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();
        // GET: email
        public ActionResult SendEmail()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string message)
        {
            try
            {
               

                    if (ModelState.IsValid)
                {
                    
                        var senderEmail = new MailAddress("testingemailforpc@gmail.com", "Jamil");
                        var receiverEmail = new MailAddress(receiver, "Receiver");
                        var password = "03430680010aA";
                        var sub = subject;
                        var body = message;
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };

                    
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = subject,
                            Body = body 
                        })
                        {
                     
                            smtp.Send(mess); 
                        
                        
                        
                         
                            
                        


                    }
                    
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }
        public ActionResult update(int id ,int id1) {
           
                var get_installment = dbm.Installments.Where(o=>o.Installment_ID>=id && o.Installment_ID<=id1).ToList();
                foreach (var get_installemnt_detail in get_installment) {

                   var w= get_installemnt_detail.Sub_Installment.Where(o=>o.Installment_ID==get_installemnt_detail.Installment_ID).ToList();
                    double sum = 0;
                    foreach (var get_sub_installment in w) {
                        sum = sum +(double) get_sub_installment.Amount;
                    
                    
                    }
                    get_installemnt_detail.Monthly_Paid = sum;
                    if (sum == get_installemnt_detail.Amount_Paid)
                    {
                        get_installemnt_detail.IsPaid = true;
                    }
                    else
                    {
                        get_installemnt_detail.IsPaid = false;
                    }
                    dbm.Entry(get_installemnt_detail).State = EntityState.Modified;
                    dbm.SaveChanges();

                }


               

            
           
            return View();
        }
    }
}