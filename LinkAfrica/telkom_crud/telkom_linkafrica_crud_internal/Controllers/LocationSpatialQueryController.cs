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
    public class LocationSpatialQueryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LocationSpatialQueryController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet]
        public IActionResult Get()
        {

            string Query = "Town SpatialQuery Controller Working";

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
        public IActionResult Post(LOCATION_ARR Param)
        {
            string Query = string.Empty;
            DataTable dt = new DataTable();
            string BufferValue = "0";

            if (Param.BufferValue != null)
            {
                double sBV = double.Parse(Param.BufferValue); //double.Parse(Param.BufferValue) / 111320;

                BufferValue = sBV.ToString();
            }
            string LocationQuery = string.Empty;
            try
            {

                switch (Param.Actions)
                {
                    case "RES_REDG":
                        LocationQuery =
                        " select count(*) as NUMCOUNT  from " +
                        " (select m.OGR_FID, SDO_CS.MAKE_2D(m.ORA_GEOMETRY, 4326) ORA_GEOMETRY from gisBoundary m inner join " +
                        " Location n on m.TAG_VALUE = n.caption where n.BOUNDARYID = " + Param.LocationID1 + ") a, (select m.OGR_FID, SDO_CS.MAKE_2D(m.ORA_GEOMETRY, 4326) ORA_GEOMETRY from gisBoundary m inner join Location n " +
                        " on m.TAG_VALUE = n.caption where n.BOUNDARYID = " + Param.LocationID2 + ") b where SDO_RELATE(SDO_CS.MAKE_2D(SDO_GEOM.SDO_BUFFER(b.ORA_GEOMETRY, " + BufferValue + ", 0.005), 4326), SDO_CS.MAKE_2D(SDO_GEOM.SDO_BUFFER(a.ORA_GEOMETRY, " + BufferValue + ", 0.005), 4326), 'mask=ANYINTERACT  querytype=WINDOW') = 'TRUE'";

                        break;
                    case "RES_RED_FARMG":
                        string whereVariable = "'0'";
                        string[] arrLPI = Param.LPIs.Split(',');

                        foreach (string str in arrLPI)
                        {
                            whereVariable = whereVariable + ", '" + str + "' ";
                        }
                        LocationQuery =
                        " select count(*) as NUMCOUNT  from  (select m.OGR_FID, SDO_CS.MAKE_2D(m.ORA_GEOMETRY, 4326) ORA_GEOMETRY from GISPARCEL m where ID in (" + whereVariable + ")) a, " +
                        "(select m.OGR_FID, SDO_CS.MAKE_2D(m.ORA_GEOMETRY, 4326) ORA_GEOMETRY from gisBoundary m inner join Location n " +
                        " on m.TAG_VALUE = n.caption where n.BOUNDARYID = " + Param.LocationID2 + ") b where SDO_RELATE(SDO_CS.MAKE_2D(SDO_GEOM.SDO_BUFFER(b.ORA_GEOMETRY, " + BufferValue + ", 0.005), 4326), SDO_CS.MAKE_2D(SDO_GEOM.SDO_BUFFER(a.ORA_GEOMETRY, " + BufferValue + ", 0.005), 4326), 'mask=ANYINTERACT  querytype=WINDOW') = 'TRUE'";



                        break;
                }


                if (!string.IsNullOrEmpty(LocationQuery))
                {
                    string OracDataSource = _configuration.GetConnectionString("OracleAppCon");
                    NpgsqlDataReader myReader;
                    using (NpgsqlConnection myCon = new NpgsqlConnection(OracDataSource))
                    {
                        myCon.Open();
                        using (NpgsqlCommand myCommand = new NpgsqlCommand(LocationQuery, myCon))
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
