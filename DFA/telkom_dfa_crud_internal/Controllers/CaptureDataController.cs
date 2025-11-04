using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CaptureDataController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public CaptureDataController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpPost]
        public IActionResult Post(IdentifyInput IIB)
        {
            List<DataTable> lDT = new List<DataTable>();
            try
            {
                lDT = GetAttributeData(IIB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return new JsonResult(lDT);



        }

        private List<DataTable> GetAttributeData(IdentifyInput IIB)
        {
            List<DataTable> lstInfoDataSet = new List<DataTable>();
            List<DataRow> lstInfoDataRow = new List<DataRow>();
            string Parcel_Query = string.Empty;
            try
            {
                foreach (string Layer in IIB.lstVisibleLayer)
                {
                    try
                    {
                        switch (Layer)
                        {
                            case "GISPARCEL_ERF":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'ERF(' || TAG_VALUE || ')' as LYR  from GISPARCEL a where a.PRCL_TYPE = 'E' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_FARMPORTION":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'FARM PORTION(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'FP' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_PARENTFARM":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'PARENT FARM(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'F' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_PUBLICPLACE": // PK
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                   "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'PUBLIC PLACE(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'PK' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                   "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_SECTIONALTITLE":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                   "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'SECTIONAL TITLE(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'SC' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                   "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_UNALIENATEDSTATELAND":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                   "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'UNALIENATED STATE LAND(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'USL' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                   "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_STREET":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                   "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'STREET(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'STR' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                   "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_UNALIENATEDRIVERBED":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                  "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'UNALIENATED RIVER BED(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'URB' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                  "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_SURVEYEDINFORMALERF":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                   "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'SURVEYED INFORMAL ERF(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'IE' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                   "order by PARCEL_TYPE_DESC";
                                break;
                            case "GISPARCEL_HOLDING":
                                Parcel_Query = "select GID, PRCL_KEY, LSTATUS,WSTATUS, TAG_VALUE, PRCL_TYPE, ID as LPI, PARCEL_TYPE_DESC, FARMNAME, TOWN_CODE, TOWN_NAME, REGION, PARCEL, PORTION, " +
                                "SDO_UTIL.TO_WKTGEOMETRY(SDO_CS.MAKE_2D(a.ORA_GEOMETRY, 4326)) as WKT, 'HOLDING(' || TAG_VALUE || ')' as LYR from GISPARCEL a where a.PRCL_TYPE = 'H' and st_intersects(a.ORA_GEOMETRY, sdo_geometry('POINT (" + IIB.XCoord + " " + IIB.YCoord + " 0.0)',1000026974)) = 'TRUE' " +
                                "order by PARCEL_TYPE_DESC";
                                break;
                        }

                        if (!string.IsNullOrEmpty(Parcel_Query))
                        {
                            DataTable dt = new DataTable();
                            dt.TableName = Layer;
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
            }
            catch (Exception ex) { }
            finally { }

            return lstInfoDataSet;
        }

        #region Query





     


  




        #endregion
    }
}
