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
    public class PropertySelectorController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PropertySelectorController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet("{XY}")]
        public JsonResult Get(string XY)
        {    
            string[] Coords = XY.Split('|');


            string Query = @"select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, a.ORA_GEOMETRY.Get_WKT() as WKT from GISPARCEL a where st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + Coords[0] + " " + Coords[1] + " 0.0)',1000026974)) = 'TRUE' order by PARCEL_TYPE_DESC";
            //string Query = @"select GID, PRCL_KEY, ID as LPI, PARCEL_TYPE_DESC, a.ORA_GEOMETRY.Get_WKT() as WKT from GISPARCEL a where st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + Coords[0] + " " + Coords[1] + " 0.0)',1000026974)) = 'TRUE' order by PARCEL_TYPE_DESC";
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
                //throw ex;
                throw new Exception("amit");
            }
            finally { }
            return new JsonResult(dt);



        }

    }
}
