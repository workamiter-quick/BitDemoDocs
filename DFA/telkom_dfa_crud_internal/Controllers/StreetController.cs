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
    public class StreetController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public StreetController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet("{StreetObjectID}")]
        public JsonResult Get(string StreetObjectID)
        {
            string Query = @"select OBJECTID, a.SHAPE.Get_WKT() as WKT, STREET, REPLACE(regexp_substr(a.SHAPE.Get_WKT(),'[^,]+', 1), 'LINESTRING (', '') as FirstPoint from STREETCENTERLINE a where OBJECTID = " + StreetObjectID;
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
                throw new Exception("Street Data Exception" + ex.StackTrace);
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpPost]
        public IActionResult Post(StreetLineCenterInput SB)
        {
            List<DataTable> lDT = new List<DataTable>();
            try
            {
                lDT = GetAttributeData(SB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(lDT);
        }

        private List<DataTable> GetAttributeData(StreetLineCenterInput sB)
        {
            List<DataTable> lstInfoDataSet = new List<DataTable>();
            List<DataRow> lstInfoDataRow = new List<DataRow>();
            string Parcel_Query = string.Empty;
            try
            {

                try
                {
                    Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                            "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'ERF(' || TAG_VALUE || ')' as LYR  from GISPARCEL a where a.PRCL_TYPE = 'E' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + sB.XCoord + " " + sB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                            "order by PARCEL_TYPE_DESC";

                    if (!string.IsNullOrEmpty(Parcel_Query))
                    {
                        DataTable dt = new DataTable();
                        dt.TableName = "Street";
                        string OracDataSource = _configuration.GetConnectionString("OracleAppCon");
                        NpgsqlDataReader myReader;
                        using (NpgsqlConnection myCon = new NpgsqlConnection(OracDataSource))
                        {
                            myCon.Open();
                            using (NpgsqlCommand myCommand = new NpgsqlCommand(Parcel_Query, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                dt.Load(myReader);
                                myReader.Close();
                                myCon.Close();
                                if (dt.Rows.Count > 0)
                                    lstInfoDataSet.Add(dt);

                            }
                        }
                    }
                }
                catch (Exception ex) { }
                finally { }

            }
            catch (Exception ex) { }
            finally { }

            return lstInfoDataSet;
        }
    }
}
