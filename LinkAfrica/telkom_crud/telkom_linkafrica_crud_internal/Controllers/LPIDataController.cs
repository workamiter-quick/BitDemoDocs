using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LPIDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LPIDataController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet("{LPI}")]
        public IActionResult Get(string LPI)
        {
            //string Query = @"SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE a.ID = '" + LPI + "'";
            string LPIAll = "'0'";
            string[] allLPIs = LPI.Split('|');

            foreach (string s in allLPIs)
            {
                LPIAll = LPIAll + ", '" + s + "'";
            }

            LPIAll = LPIAll + ", '0'";
            string Query = @"SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, SDO_GEOM.SDO_CENTROID(a.ORA_GEOMETRY, 0.846049894).sdo_point.x as Center_X, SDO_GEOM.SDO_CENTROID(a.ORA_GEOMETRY, 0.846049894).sdo_point.y as Center_Y  FROM GISPARCEL a WHERE a.ID in (" + LPIAll + ")";
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }
    }
}
