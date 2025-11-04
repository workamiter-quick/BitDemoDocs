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
    public class LocationController : ControllerBase
    {
        private readonly DBHelper _dBHelper;
        public LocationController(DBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {

            string Query = "select * from LOCATION";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("{ID}")]
        public IActionResult Get(string ID)
        
        {

            string Query = string.Empty;
            if (ID != "0")
            {
                Query = "select BOUNDARYID as Code, Caption as Name  from LOCATION where Category = 'P' and  BOUNDARYID = " + ID;
            }
            else
            {
                Query = "select BOUNDARYID as Code, Caption as Name  from LOCATION where Category = 'P'";
            }
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetDM(string ID)
        {
            string Query = string.Empty;
            if (ID != "0")
            {
                Query = "select a.BOUNDARYID as DISTRICT, a.Caption as DISTRICT_N from LOCATION a inner join LOCATION b on a.ParentboundaryID = b.boundaryid where a.Category in ('M', 'D') and b.BOUNDARYID = '" + ID + "' order by 2";
            }
            else
            {
                Query = "select a.BOUNDARYID as DISTRICT, a.Caption as DISTRICT_N from LOCATION a inner join LOCATION b on a.ParentboundaryID = b.boundaryid where a.Category in ('M', 'D')";
            }
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);
        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetLM(string ID)
        {
            string Query = string.Empty;
            if (ID != "0")
            {
                Query = "select BOUNDARYID, CAPTION from LOCATION WHERE PARENTBOUNDARYID = (select BOUNDARYID from LOCATION WHERE BOUNDARYID = '" + ID + "') and Category in ('L') order by 2";
            }
            else
            {
                Query = "select BOUNDARYID, CAPTION from LOCATION  where Category in ('L') order by 2";
            }
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetAR(string ID)
        {
            string Query = string.Empty;
            if (ID != "0")
            {
                Query = "select BOUNDARYID, CAPTION from LOCATION WHERE PARENTBOUNDARYID = (select BOUNDARYID from LOCATION WHERE BOUNDARYID = '" + ID + "') and Category = 'AR' order by 2";
            }
            else
            {
                Query = "select BOUNDARYID, CAPTION from LOCATION WHERE Category = 'AR' order by 2";
            }
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetRegionByAR(string ID)
        {
            string Query = string.Empty;
            if (ID != "0")
            {

                Query = "select MDBCODE, MDBCODE || '(' || Caption || ')' as Captions from Location where MDBCODE in ( " +
                    "select distinct a.REGION from GISPARCEL a, (select TAG_VALUE, ORA_GEOMETRY from GISBOUNDARY " +
                    "where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = (select distinct trim(MDBCODE) from Location where Category = 'AR' and Caption = '" + ID + "')) b " +
                    "where sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE' and a.REGION is not null)";
            }
            else
            {
                Query = "select MDBCODE, MDBCODE || '(' || Caption || ')' as Captions from Location where MDBCODE in ( " +
                "select distinct a.REGION from GISPARCEL a, (select TAG_VALUE, ORA_GEOMETRY from GISBOUNDARY " +
                "where BOUNDARY_TYPE_ID = 52) b " +
                "where sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE' and a.REGION is not null)";
            }
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetRegionByDMMetro(string ID)
        {
            string Query = string.Empty;

            Query = "select MDBCODE, MDBCODE || '(' || Caption || ')' as Captions from Location where MDBCODE in ( " +
            "select distinct a.REGION from GISPARCEL a, (select TAG_VALUE, ORA_GEOMETRY from GISBOUNDARY " +
            "where BOUNDARY_TYPE_ID in (61, 57) and TAG_VALUE = (select MDBCODE from Location where Category in ('D', 'M') and BOUNDARYID = '" + ID + "')) b " +
            "where sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE' and a.REGION is not null) order by Caption";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);
        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetRegionByLM(string ID)
        {
            string Query = string.Empty;

            Query = "select MDBCODE, MDBCODE || '(' || Caption || ')' as Captions from Location where MDBCODE in ( " +
            "select distinct a.REGION from GISPARCEL a, (select TAG_VALUE, ORA_GEOMETRY from GISBOUNDARY " +
            "where BOUNDARY_TYPE_ID in (59) and TAG_VALUE = (select MDBCODE from Location where Category in ('L') and BOUNDARYID = '" + ID + "')) b " +
            "where sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE' and a.REGION is not null) order by Caption";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);

        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetLocationByID(string ID)
        {
            DataTable dt = new DataTable();
            try
            {
                string Query = string.Empty;

                Query = "select distinct a.BOUNDARY_TYPE_ID from Location a  inner join M_BOUNDARY_TYPE b on a.BOUNDARY_TYPE_ID = b.ID where a.BOUNDARYID = " + ID;

                dt = GetData(Query);
                if (dt.Rows.Count > 0)
                {
                    string BOUNDARY_TYPE_ID = dt.Rows[0][0].ToString();
                    switch (BOUNDARY_TYPE_ID)
                    {
                        case "52":// Administrative Registration
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  52 and a.TAG_VALUE = (select MDBCODE from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 52) and rownum <= 1";
                            break;
                        case "57"://District Municipality
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  57 and a.TAG_VALUE = (select MDBCODE from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 57) and rownum <= 1";
                            break;
                        case "59"://Local Municipality
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  59 and a.TAG_VALUE = (select MDBCODE from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 59) and rownum <= 1";
                            break;
                        case "61"://Metropolitan Municipality
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  61 and a.TAG_VALUE = (select MDBCODE from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 61) and rownum <= 1";
                            break;
                        case "64"://Provincial
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  64 and a.TAG_VALUE = (select Caption from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 64) and rownum <= 1";
                            break;
                        case "67"://National  Boundary
                            break;
                        case "73"://Allotment Township
                            Query = "select SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt, a.TAG_X as CENTER_X, a.TAG_Y as CENTER_Y from GISBOUNDARY a where a.BOUNDARY_TYPE_ID =  73 and a.TAG_VALUE = (select Caption from Location where BOUNDARYID = " + ID + " and BOUNDARY_TYPE_ID = 73) and rownum <= 1";
                            break;
                    }
                    dt = GetData(Query);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("[action]/{ID}")]
        public IActionResult GetTownByID(string ID)
        {
            DataTable dt = new DataTable();
            try
            {
                string Query = string.Empty;

                Query = "select b.BOUNDARYID, b.CAPTION, SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt from gisBoundary a inner join Location b on a.TAG_VALUE = b.caption where a.Boundary_Type_ID =73 and b.Category = 'T' and b.BOUNDARYID = " + ID;

                dt = GetData(Query);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }


        [HttpGet("[action]/{IDs}")]
        public IActionResult GetLocationsID(string IDs)
        {
            string[] Param = IDs.Split('|');
            string whereClause = "(0";
            foreach (string ID in Param)
            {
                whereClause = whereClause + ", " + ID;
            }
            whereClause = whereClause + ")";

            DataTable dt = new DataTable();
            try
            {
                string Query = string.Empty;

                Query = "select b.BOUNDARYID, b.CAPTION, SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt from gisBoundary a inner join Location b on a.TAG_VALUE = b.caption where b.BOUNDARYID in " + whereClause;

                //Query = "select b.BOUNDARYID, b.CAPTION, SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(SDO_GEOM.SDO_BUFFER(a.ORA_GEOMETRY, 5000, 0.0000005), 4326)) as wkt from gisBoundary a inner join Location b on a.TAG_VALUE = b.caption where b.BOUNDARYID in " + whereClause;

                dt = GetData(Query);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }


        private DataTable GetData(string Query)
        {
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return dt;

        }
    }
}
