using Crud.Models;
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
    public class SearchController : Controller
    {
        private readonly IConfiguration _configuration;
        private string topFeatureCount = "200";
        public SearchController(IConfiguration _conf)
        {
            _configuration = _conf;
            topFeatureCount = _configuration["TopFeatureCount"].ToString();
        }

        [HttpGet]
        public IActionResult Get()
        {

            string Query = "Working";


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
        public IActionResult Post(SearchInput SI)
        {
            string Query = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                if ((SI.SearchType == "MN" && SI.DMCode == "-1") || (SI.SearchType == "AR" && SI.ARCode == "-1"))
                {
                    Query = SearchByProvince(SI);
                }
                if (SI.SearchType == "AR" && SI.ARCode != "-1")
                {
                    Query = SearchByAdminRegion(SI);
                }

                if (SI.SearchType == "MN" && SI.LMCode != "-1")
                {
                    Query = SearchByLM(SI);
                }
                if (SI.SearchType == "MN" && SI.LMCode == "-1" && SI.DMCode != "-1")
                {
                    Query = SearchByDM_Metro(SI);
                }
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


        private string SearchByProvince(SearchInput SI)
        {
            string DataQuery = string.Empty;
            try
            {
                switch (SI.SearchCriteria)
                {
                    case "Farm_Name":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.FARMNAME) Like lower('" + SI.FarmName + "%') and a.PROVINCEID = " + SI.ProvinceCode + " and rownum <= " + topFeatureCount;
                        break;
                    case "Town_Name":
                        DataQuery = "select b.OGR_FID, b.TAG_VALUE, b.COMMENTS, b.TAG_JUST, a.MDBCODE, SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(b.ORA_GEOMETRY, 4326)) as wkt  from Location a inner join GISBoundary b on lower(a.CAPTION) = lower(b.TAG_VALUE) where a.CATEGORY = 'T' and a.PARENTBOUNDARYID = '" + SI.ProvinceCode + "' and b.TAG_VALUE like '%" + SI.TownByName + "%'";
                        break;
                    case "SG_Number":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE a.SG_NUMBER = '" + SI.SGNumber + "' and a.PROVINCEID = " + SI.ProvinceCode + " and rownum <= " + topFeatureCount;
                        break;
                    case "LPI":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.ID) Like lower('" + SI.LPI + "%') and a.PROVINCEID = " + SI.ProvinceCode + " and rownum <= " + topFeatureCount;
                        break;
                    case "RPP":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.REGION) Like lower('" + SI.Region + "%') and lower(a.PARCEL) Like lower('%" + SI.Parcel + "%') and lower(a.PORTION) Like lower('%" + SI.Portion + "') and a.PROVINCEID = '" + SI.ProvinceCode + "' and rownum <= " + topFeatureCount;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return DataQuery;
        }

        private string SearchByDM_Metro(SearchInput SI)
        {
            string DataQuery = string.Empty;
            try
            {
                switch (SI.SearchCriteria)
                {
                    case "Farm_Name":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.FARMNAME) Like lower('" + SI.FarmName + "%') and a.DMID = '" + SI.DMCode + "' and rownum <= " + topFeatureCount;
                        break;
                    case "Town_Name":
                        DataQuery = "select x.OGR_FID,x.TAG_VALUE,x.TAG_X,x.TAG_Y,x.TAG_JUST,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select * from gisBoundary where Boundary_Type_ID = 73 and lower(TAG_VALUE) like lower('" + SI.TownByName + "%')) x,  ( " +
                        "select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                        "where a.Boundary_Type_ID in (57, 61) and b.Boundaryid = " + SI.DMCode + " " +
                        ") c where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "SG_Number":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.TOWN_NAME, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE a.SG_NUMBER = '" + SI.SGNumber + "' and a.DMID = '" + SI.DMCode + "' and rownum <= " + topFeatureCount;
                        break;
                    case "LPI":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.ID) Like lower('" + SI.LPI + "%') and a.DMID = '" + SI.DMCode + "' and rownum <= " + topFeatureCount;
                        break;
                    case "RPP":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a WHERE lower(a.REGION) Like lower('" + SI.Region + "%') and lower(a.PARCEL) Like lower('%" + SI.Parcel + "%') and lower(a.PORTION) Like lower('%" + SI.Portion + "') and a.DMID = '" + SI.DMCode + "' and rownum <= " + topFeatureCount;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return DataQuery;
        }

        private string SearchByLM(SearchInput SI)
        {
            string DataQuery = string.Empty;
            try
            {
                switch (SI.SearchCriteria)
                {

                    case "Farm_Name":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE lower(a.FARMNAME) Like lower('" + txtFarmByName.Text.Trim() + "%') and a.DMID = '" + dm_metrocode + "' and rownum <= " + topFeatureCount;

                        DataQuery = "select x.OGR_FID, x.FARMNAME,x.LSTATUS,x.WSTATUS,x.TAG_VALUE, x.ID as LPI, x.SG_NUMBER, x.REGION,x.PARCEL,x.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select * from GISPARCEL where lower(FARMNAME) like lower('" + SI.FarmName + "%')) x,  " +
                        "(select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                        "where a.Boundary_Type_ID = 59  and b.Boundaryid = " + SI.LMCode + " ) c " +
                        "where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";

                        break;
                    case "Town_Name":
                        DataQuery = "select x.OGR_FID,x.TAG_VALUE,x.TAG_X,x.TAG_Y,x.TAG_JUST,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select * from gisBoundary where Boundary_Type_ID = 73 and lower(TAG_VALUE) like lower('" + SI.TownByName + "%')) x,  ( " +
                        "select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                        "where a.Boundary_Type_ID = 59 and b.Boundaryid = " + SI.LMCode + " " +
                        ") c where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "SG_Number":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.TOWN_NAME, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE a.SG_NUMBER = '" + txtSGNumber.Text.Trim() + "' and a.DMID = '" + dm_metrocode + "' and rownum <= " + topFeatureCount;

                        DataQuery = "select x.OGR_FID, x.FARMNAME,x.LSTATUS,x.WSTATUS,x.TAG_VALUE, x.ID as LPI, x.SG_NUMBER, x.REGION,x.PARCEL,x.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select * from GISPARCEL where lower(SG_NUMBER) = lower('" + SI.SGNumber + "')) x,  " +
                        "(select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                        "where a.Boundary_Type_ID = 59  and b.Boundaryid = " + SI.LMCode + " ) c " +
                        "where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "LPI":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE lower(a.ID) Like lower('" + txtLPI.Text.Trim() + "%') and a.DMID = '" + dm_metrocode + "' and rownum <= " + topFeatureCount;

                        DataQuery = "select x.OGR_FID, x.FARMNAME,x.LSTATUS,x.WSTATUS,x.TAG_VALUE, x.ID as LPI, x.SG_NUMBER, x.REGION,x.PARCEL,x.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select * from GISPARCEL where lower(ID) Like lower('" + SI.LPI + "%')) x,  " +
                        "(select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                        "where a.Boundary_Type_ID = 59  and b.Boundaryid = " + SI.LMCode + " ) c " +
                        "where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "RPP":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE lower(a.REGION) Like lower('" + ddlRegion.SelectedValue.ToString() + "%') and lower(a.PARCEL) Like lower('%" + txtParcel.Text.Trim() + "%') and lower(a.PORTION) Like lower('%" + txtPortion.Text.Trim() + "') and a.DMID = '" + dm_metrocode + "' and rownum <= " + topFeatureCount;

                        DataQuery = "select x.OGR_FID, x.FARMNAME,x.LSTATUS,x.WSTATUS,x.TAG_VALUE, x.ID as LPI, x.SG_NUMBER, x.REGION,x.PARCEL,x.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(x.ORA_GEOMETRY, 4326)) as wkt from " +
                       "(select * from GISPARCEL where lower(REGION) Like lower('" + SI.Region + "%') and lower(PARCEL) Like lower('%" + SI.Parcel + "%') and lower(PORTION) Like lower('%" + SI.Portion + "')) x,  " +
                       "(select b.Boundaryid, a.ORA_GEOMETRY, b.CAPTION from gisBoundary a inner join Location b on a.TAG_VALUE = b.MDBCODE " +
                       "where a.Boundary_Type_ID = 59  and b.Boundaryid = " + SI.LMCode + " ) c " +
                       "where sdo_anyinteract(x.ORA_GEOMETRY, c.ORA_GEOMETRY) = 'TRUE'";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return DataQuery;
        }

        private string SearchByAdminRegion(SearchInput SI)
        {
            string DataQuery = string.Empty;
            try
            {
                switch (SI.SearchCriteria)
                {
                    case "Farm_Name":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = '" + SI.ARCode + "') b WHERE lower(a.FARMNAME) Like lower('" + SI.FarmName + "%') and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "Town_Name":

                        DataQuery = "select a.OGR_FID,a.TAG_VALUE,a.TOWN_CODE,a.TAG_X,a.TAG_Y,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt from " +
                        "(select x.OGR_FID, x.TAG_VALUE, y.MDBCODE as TOWN_CODE, x.TAG_X, x.TAG_Y, x.ORA_GEOMETRY from GISBOUNDARY x " +
                        "inner join LOCATION y on x.TAG_VALUE = y.Caption where x.BOUNDARY_TYPE_ID = 73 and y.Category = 'T') a, " +
                        "(select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = '" + SI.ARCode + "') b " +
                        "where  sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE' and lower(a.TAG_VALUE) Like lower('" + SI.TownByName + "%') ";

                        break;
                    case "SG_Number":
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = '" + SI.ARCode + "') b WHERE a.SG_NUMBER = '" + SI.SGNumber + "' and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        break;
                    case "LPI":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.MIN_REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE lower(a.ID) Like lower('" + txtLPI.Text.Trim() + "%')  and a.ADCODE = '" + ARcode + "' and rownum <= " + topFeatureCount;
                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = '" + SI.ARCode + "') b WHERE lower(a.ID) Like lower('" + SI.LPI + "%') and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        if (SI.ProvinceCode == "9" || SI.ProvinceCode == "7")
                        {
                            DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE,a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = (select MDBCODE from Location where Category = 'AR' and parentboundaryid = " + SI.ProvinceCode + " and Caption = '" + SI.ARCode + "')) b WHERE lower(a.ID) Like lower('" + SI.LPI + "%') and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        }
                        break;
                    case "RPP":
                        //DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.MIN_REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(a.ORA_GEOMETRY) as wkt  FROM GISPARCEL a WHERE lower(a.TOWN_CODE) Like lower('" + ddlRegion.SelectedValue.ToString() + "%') and lower(a.PARCEL) Like lower('%" + txtParcel.Text.Trim() + "%')  and lower(a.PORTION) Like lower('%" + txtPortion.Text.Trim() + "') and a.ADCODE = '" + ARcode + "' and rownum <= " + topFeatureCount;


                        DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = '" + SI.ARCode + "') b WHERE lower(a.REGION) Like lower('" + SI.Region + "%') and lower(a.PARCEL) Like lower('%" + SI.Parcel + "%')  and lower(a.PORTION) Like lower('%" + SI.Portion + "') and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        if (SI.ProvinceCode == "9" || SI.ProvinceCode == "7")
                        {

                            DataQuery = "SELECT a.OGR_FID, a.FARMNAME,a.LSTATUS,a.WSTATUS,a.TAG_VALUE, a.ID as LPI, a.SG_NUMBER, a.TOWN_CODE, a.REGION,a.PARCEL,a.PORTION,SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as wkt  FROM GISPARCEL a, (select ORA_GEOMETRY from GISBOUNDARY where BOUNDARY_TYPE_ID = 52 and TAG_VALUE = (select MDBCODE from Location where Category = 'AR' and parentboundaryid = " + SI.ProvinceCode + " and Caption = '" + SI.ARCode + "')) b WHERE lower(a.REGION) Like lower('" + SI.Region + "%') and lower(a.PARCEL) Like lower('%" + SI.Parcel + "%')  and lower(a.PORTION) Like lower('%" + SI.Portion + "') and sdo_anyinteract(a.ORA_GEOMETRY, b.ORA_GEOMETRY) = 'TRUE'";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return DataQuery;
        }

    }
}
