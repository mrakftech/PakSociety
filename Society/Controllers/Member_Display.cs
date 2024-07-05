using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Society.Data
{
    public class Member_Display
    {
        public string Reg_No { get; set; }
        public int Member_ID { get; set; }
        public System.DateTime Application_Date { get; set; }
        public string CNIC { get; set; }
        public string Applicant_Start_Text { get; set; }
        public string Applicant_Name { get; set; }
        public string Plan_Name { get; set; }
        public string Father_Husband_Name { get; set; }
        public string Occupation { get; set; }
        public string Present_Postel_Address { get; set; }
        public string Permenent_Postel_Address { get; set; }
        public string Office_No { get; set; }
        public string Cell_No { get; set; }
        public string Email { get; set; }
        public string Nominee_Name { get; set; }
        public string Nominee_CNIC { get; set; }
        public string Relation { get; set; }
        public string Nominee_Phone_No { get; set; }
        public string Nominee_Address { get; set; }
        public byte[] Profile_Img { get; set; }
        public Nullable<bool> Status { get; set; }
        public int percentage { get; set; }
    }
}