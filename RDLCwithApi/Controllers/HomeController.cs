using AspNetCore.Reporting;

using FIK.DAL.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RDLCwithApi.Models;
using System.Collections.Generic;
using System.Data;

namespace RDLCwithApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        SQL _sqlDal = null;
        string msg = "";

        public HomeController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            _sqlDal = new SQL(configuration.GetConnectionString("DefaultConnection"));
        }
        [HttpGet]
        public IActionResult Get()
        {
            var dt = new DataTable();
            dt = GetUserSql();

            //dt = GetUserList();


            string mimtype = "";
            int extension = 1;
            var path = $"{this.webHostEnvironment.WebRootPath}\\Reports\\MyTest1.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("prm", "RDLC Report");

            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("dsUsers", dt);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);


            return File(result.MainStream, "application/pdf");

        }

        public DataTable GetUserList()
        {
            var dt = new DataTable();
            dt.Columns.Add("UserId");
            dt.Columns.Add("Name");
            dt.Columns.Add("Email");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Password");

            DataRow row;

            for (int i = 101; i <= 1120; i++)
            {
                row = dt.NewRow();
                row["UserId"] = i;
                row["Name"] = "Mr. " + i;
                row["Email"] = "mr " + i + " @gmail.com";
                row["Phone"] = "0154545" + i;
                row["Password"] = "HAsH" + i;

                dt.Rows.Add(row);
            }
            return dt;
        }

        public DataTable GetUserSql()
        {
            DataTable dt = new DataTable();
            string q = string.Format(@"select * from Users ");
            var data = _sqlDal.Select<User>(q, ref msg);
            dt.Columns.Add("UserId");
            dt.Columns.Add("Name");
            dt.Columns.Add("Email");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Password");
            DataRow row;
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["UserId"] = item.UserId;
                row["Name"] = item.Name;
                row["Email"] = item.Email;
                row["Phone"] = item.Phone;
                row["Password"] = item.Password;

                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
