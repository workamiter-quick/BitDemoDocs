using Crud.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpatialQueryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SpatialQueryController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet]
        public IActionResult Get()
        {

            string Query = "SpatialQuery Controller Working";

            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(Query);



        }

        [HttpPost]
        public IActionResult Post(LPIS_ARR LPIs)
        {
            string Query = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                string whereVariable = "'0'";
                foreach (string str in LPIs.LPIs)
                {
                    whereVariable = whereVariable + ", '" + str + "' ";
                }
                //Query = "select a.ID as SOURCE_LPI,b.ID as TARGET_LPI, " +
                //"SDO_GEOM.RELATE(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326), 'determine', SDO_CS.MAKE_2D(b.ORA_GEOMETRY, 4326), 0.005) RELATIONSHIP " +
                //"from GISPARCEL a, GISPARCEL b " +
                //"where a.ID in (" + whereVariable + ") and " +
                //"b.ID in ("+ whereVariable + ")";

                Query = "select SOURCE_LPI, count(SOURCE_LPI) as CNT_DISJOINT from(select * from " +
                "(select a.ID as SOURCE_LPI, " +
                "SDO_GEOM.RELATE(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326), 'determine', SDO_CS.MAKE_2D(b.ORA_GEOMETRY, 4326), 0.005) RELATIONSHIP " +
                "from GISPARCEL a, GISPARCEL b " +
                "where a.ID in (" + whereVariable + ") and " +
                "b.ID in (" + whereVariable + ")) " +
                "where RELATIONSHIP = 'DISJOINT') group by SOURCE_LPI order by 2 desc";

                if (!string.IsNullOrEmpty(Query))
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
                else
                {
                    throw new Exception("Query Issue");
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
