using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportCSVController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public ExportCSVController(IConfiguration _conf)
        {
            _configuration = _conf;
        }



        [HttpGet]
        public IActionResult Get()
        {

            string Query = "select b.BOUNDARYID, b.CAPTION from gisBoundary a inner join " +
                "Location b on a.TAG_VALUE = b.caption where a.Boundary_Type_ID = 73 and b.Category = 'T'";

            DataTable dt = new DataTable();
            try
            {
                string OracDataSource = _configuration.GetConnectionString("OracleAppCon");
                NpgsqlDataReader myReader;
                using (NpgsqlConnection myCon = new NpgsqlConnection(OracDataSource))
                {
                    myCon.Open();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(Query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dt.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }

                string csvContent = DataTableToCsv(dt);

                // Save CSV content to a file
                System.IO.File.WriteAllText("output.csv", csvContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        static string DataTableToCsv(DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();

            // Write the header row
            foreach (DataColumn column in dataTable.Columns)
            {
                sb.Append($"{EscapeCsvField(column.ColumnName)},");
            }
            sb.AppendLine();

            // Write the data rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    sb.Append($"{EscapeCsvField(item.ToString())},");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        static string EscapeCsvField(string field)
        {
            // If the field contains a comma, double quote, or newline, enclose it in double quotes
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }




    }
}
