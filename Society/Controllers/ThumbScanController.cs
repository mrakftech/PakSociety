using Society.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Society.Controllers
{
    public class ThumbScanController : ApiController
    {
        public class Model
        {
            public string ImageData { get; set; }
            
        }
        lakeViewNewDbEntities3 dbm = new lakeViewNewDbEntities3();

        [HttpPost]
        public IHttpActionResult  Post([FromBody] Model model)
        {
            if (string.IsNullOrWhiteSpace(model.ImageData)) return InternalServerError();
            
                model.ImageData = model.ImageData.Split(',')[1];
            var data = Convert.FromBase64String(model.ImageData);
            if (data == null) return InternalServerError();

            ThumbScan thumbScan = new ThumbScan { isSaved = false, url =  data};
            dbm.ThumbScans.Add(thumbScan);
            dbm.SaveChanges();
            
            return Ok("Record Saved Successfully");
        }
        [HttpGet]
        public IHttpActionResult Get()
        {
            
            return Ok("Api Working");
        }

    }
}
