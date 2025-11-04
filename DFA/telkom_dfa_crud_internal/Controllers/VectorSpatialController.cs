using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Emit;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VectorSpatialController : ControllerBase
    {

        private readonly DBHelper _dBHelper;
        private readonly IConfiguration _configuration;
        public VectorSpatialController(DBHelper dBHelper, IConfiguration configuration)
        {
            _dBHelper = dBHelper;
            _configuration = configuration;
        }
        #region Public


        //[HttpGet]
        //public IActionResult Get()
        //{
        //    string LMCode = "WC048";

        //    List<object> lstDT = new List<object>();
        //    try
        //    {
        //        DataSet Ward_Ds = _dBHelper.ExecuteSQL(WardQuery);
        //        int NumberOfWard = int.Parse(Ward_Ds.Tables[0].Rows[0][0].ToString());

        //        DataSet WardSize_ds = _dBHelper.ExecuteSQL(WardSizeQuery);
        //        int WardSize = int.Parse(WardSize_ds.Tables[0].Rows[0][0].ToString());

        //        lstDT.Add("NumberOfWard:" + NumberOfWard.ToString());
        //        lstDT.Add("WardSize:" + WardSize.ToString());
        //        //string WardGeometry = "select a.wardid, a.normvalue, ST_AsText(geom) as wkt  from " +
        //        //        "(select wardid, sum(regpop) as normvalue  from votingdistricts  where cat_b = '" + LMCode + "' group by wardid) a " +
        //        //        "inner join wards_2021 b on a.wardid = b.wardid";

        //        string WardGeometry = "select wardid, sum(regpop) as normvalue  from votingdistricts  where cat_b = '" + LMCode + "' group by wardid ";


        //        DataSet WardGeometry_Ds = _dBHelper.ExecuteSQL(WardGeometry);
        //        lstDT.Add(WardGeometry_Ds.Tables[0]);

        //        string WardSizeValueQuery = @"select id, wards2019, norm, norm_min, norm_max, variance_15par FROM wc_wardsize_2019 where cat_b = '" + LMCode + "'";

        //        DataSet WardSizeValueQuery_Ds = _dBHelper.ExecuteSQL(WardSizeValueQuery);
        //        lstDT.Add(WardSizeValueQuery_Ds.Tables[0]);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return new JsonResult(lstDT);
        //}



        [HttpPost("GetSiteDetail")]
        public IActionResult GetSiteDetail(GoogleCoord _GoogleCoord)
        {
            DataTable dt = new DataTable();
            List<SiteDetails> lstSiteDetails = new List<SiteDetails>();

            try
            {
                string countPolygon = "SELECT count(*) as cnt FROM site_polygon_layer WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_CountPolygon = _dBHelper.ExecuteSQL(countPolygon);
                string polygonCount = Ds_CountPolygon.Tables[0].Rows[0][0].ToString();

                string countPolygonForPTCPT = "SELECT count(*) as cnt FROM cpt_pta WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_PolygonForPTCPT = _dBHelper.ExecuteSQL(countPolygonForPTCPT);
                string PolygonForPTCPT_Count = Ds_PolygonForPTCPT.Tables[0].Rows[0][0].ToString();

                string Data_Query = string.Empty;
                string queryDistanceInMeters = "120";
                if(_GoogleCoord.UseMaxDistance == "true")
                    queryDistanceInMeters = "50000";

                if (polygonCount == "0")
                {

                    Data_Query = @"SELECT  " +
                   "gid, operator, network_type, link_residential, link_enterprise, link_business, link_backhual, link_connect, '' as pop_name, " +
                   "ST_X(ST_Centroid(geom)) AS center_x, " +
                   "ST_Y(ST_Centroid(geom)) AS center_y, " +
                   "ST_Distance( " +
                   "ST_Transform((ST_Centroid(geom)), 32735), " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735) " +
                   ") AS distance_meters " +
                   "FROM network_line_layer " +
                   "WHERE ST_DWithin( " +
                   "ST_Transform((ST_Centroid(geom)), 32735),  " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735), " +
                   "" + queryDistanceInMeters + " " +
                   ")  " +
                   "ORDER BY distance_meters " +
                   "LIMIT 1";
                }
                else
                {
                    Data_Query = @"SELECT  " +
                       "gid, operator, network_type, link_residential, link_enterprise, link_business, link_backhual, link_connect, '' as pop_name, " +
                       //"ST_X(ST_Centroid(geom)) AS center_x, " +
                       //"ST_Y(ST_Centroid(geom)) AS center_y, " +
                       _GoogleCoord.Longitude_X + " AS center_x, " +
                       _GoogleCoord.Latitude_Y + " AS center_y, " +
                       "ST_Distance( " +
                       "ST_Transform((ST_Centroid(geom)), 32735), " +
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735) " +
                       ") AS distance_meters " +
                       "FROM site_polygon_layer " +
                       "WHERE ST_DWithin( " +
                       "ST_Transform((ST_Centroid(geom)), 32735),  " +
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735), " +
                       " 50000 " +
                       ")  " +
                       "ORDER BY distance_meters " +
                       "LIMIT 1";
                }


                DataSet Splited_Ds = _dBHelper.ExecuteSQL(Data_Query);
                dt = Splited_Ds.Tables[0];

                if (dt.Rows[0]["link_residential"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_residential = new SiteDetails();
                    objSiteDetails_link_residential.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_residential.Product = "link residential".ToUpper();
                    objSiteDetails_link_residential.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_residential.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_residential.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_residential.Distance_Meters = "--";
                    objSiteDetails_link_residential.PolygonCount = polygonCount;
                    objSiteDetails_link_residential.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_residential.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_residential.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_residential);
                }

                if (dt.Rows[0]["link_enterprise"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_enterprise = new SiteDetails();
                    objSiteDetails_link_enterprise.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_enterprise.Product = "link enterprise".ToUpper();
                    objSiteDetails_link_enterprise.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_enterprise.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_enterprise.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_enterprise.Distance_Meters = "--";
                    objSiteDetails_link_enterprise.PolygonCount = polygonCount;
                    objSiteDetails_link_enterprise.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_enterprise.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_enterprise.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_enterprise);
                }

                if (dt.Rows[0]["link_residential"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_residential = new SiteDetails();
                    objSiteDetails_link_residential.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_residential.Product = "link residential".ToUpper();
                    objSiteDetails_link_residential.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_residential.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_residential.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_residential.Distance_Meters = "--";
                    objSiteDetails_link_residential.PolygonCount = polygonCount;
                    objSiteDetails_link_residential.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_residential.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_residential.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_residential);
                }

                if (dt.Rows[0]["link_business"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_business = new SiteDetails();
                    objSiteDetails_link_business.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_business.Product = "link business".ToUpper();
                    objSiteDetails_link_business.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_business.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_business.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_business.Distance_Meters = "--";
                    objSiteDetails_link_business.PolygonCount = polygonCount;
                    objSiteDetails_link_business.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_business.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_business.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_business);
                }

                if (dt.Rows[0]["link_backhual"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_backhual = new SiteDetails();
                    objSiteDetails_link_backhual.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_backhual.Product = "link backhual".ToUpper();
                    objSiteDetails_link_backhual.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_backhual.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_backhual.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_backhual.Distance_Meters = "--";
                    objSiteDetails_link_backhual.PolygonCount = polygonCount;
                    objSiteDetails_link_backhual.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_backhual.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_backhual.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_backhual);
                }

                if (dt.Rows[0]["link_connect"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_connect = new SiteDetails();
                    objSiteDetails_link_connect.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_connect.Product = "link connect".ToUpper();
                    objSiteDetails_link_connect.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_connect.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_connect.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_connect.Distance_Meters = "--";
                    objSiteDetails_link_connect.PolygonCount = polygonCount;
                    objSiteDetails_link_connect.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_connect.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_connect.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_connect);
                }


                string productPriceQuery = "SELECT n0, sub_n0, xxx, product, sheet, contract_term_months, bandwidth_mbps, mrc_ex_vat, " +
                    "nrc_lt_1gbps_ex_vat, nrc_1_10gbps_ex_vat, nrc_ex_vat, product_features, comments " +
                    "FROM public.enterprise_products where contract_term_months = " + _GoogleCoord.ContractTerm + " and bandwidth_mbps = " + _GoogleCoord.Bandwidth + ";";

                DataSet ProductPrice_Ds = _dBHelper.ExecuteSQL(productPriceQuery);

                foreach (SiteDetails objSiteDetails in lstSiteDetails)
                {
                    foreach (DataRow drr in ProductPrice_Ds.Tables[0].Rows)
                    {
                        if (objSiteDetails.Product == drr["xxx"].ToString())
                        {
                            objSiteDetails.MRC = drr["mrc_ex_vat"].ToString();
                            if (drr["nrc_ex_vat"].ToString() == "NA") {
                                objSiteDetails.NRC = drr["nrc_lt_1gbps_ex_vat"].ToString();
                            }
                            else {
                                objSiteDetails.NRC = drr["nrc_ex_vat"].ToString();
                            }                                
                        }
                    }
                }


                if (polygonCount != "0")
                {
                    string ThirdPartyQuery = "SELECT gid, n0, bandwidth, term, mrc, nrc, fcc_name, sheet_name FROM fcc where bandwidth = " + _GoogleCoord.Bandwidth + " and term = " + _GoogleCoord.ContractTerm + " and  fcc_name = '" + lstSiteDetails[0].Operator + "'";
                    DataSet ThirdParty_Ds = _dBHelper.ExecuteSQL(ThirdPartyQuery);
                    foreach (SiteDetails objSiteDetails in lstSiteDetails)
                    {
                        foreach (DataRow drr in ThirdParty_Ds.Tables[0].Rows)
                        {
                            objSiteDetails.Party_Provider_3rd = drr["fcc_name"].ToString();
                            objSiteDetails.Party_MRC_3rd = drr["mrc"].ToString();
                            objSiteDetails.Party_NRC_3rd = drr["nrc"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
            }
            finally
            {

            }
            return new JsonResult(lstSiteDetails);
        }


        [HttpPost("GetSiteDetailGreen")]
        public IActionResult GetSiteDetailGreen(GoogleCoord _GoogleCoord)
        {
            DataTable dt = new DataTable();
            List<SiteDetails> lstSiteDetails = new List<SiteDetails>();

            try
            {
                string countPolygon = "SELECT count(*) as cnt FROM site_polygon_layer WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_CountPolygon = _dBHelper.ExecuteSQL(countPolygon);
                string polygonCount = Ds_CountPolygon.Tables[0].Rows[0][0].ToString();

                string countPolygonForPTCPT = "SELECT count(*) as cnt FROM cpt_pta WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_PolygonForPTCPT = _dBHelper.ExecuteSQL(countPolygonForPTCPT);
                string PolygonForPTCPT_Count = Ds_PolygonForPTCPT.Tables[0].Rows[0][0].ToString();

                string Data_Query = string.Empty;

                if (polygonCount == "0")
                {

                    Data_Query = @"SELECT  " +
                   "gid, operator, network_type, link_residential, link_enterprise, link_business, link_backhual, link_connect, '' as pop_name, " +
                   "" + _GoogleCoord.Longitude_X_Green + " AS center_x, " +
                   "" + _GoogleCoord.Latitude_Y_Green + " AS center_y, " +
                   "ST_Distance( " +
                   "ST_Transform((ST_Centroid(geom)), 32735), " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735) " +
                   ") AS distance_meters " +
                   "FROM network_line_layer " +
                   "WHERE ST_DWithin( " +
                   "ST_Transform((ST_Centroid(geom)), 32735),  " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X_Green + " " + _GoogleCoord.Latitude_Y_Green + ")'), 4326), 32735), " +
                   "100 " +
                   ")  " +
                   "ORDER BY distance_meters " +
                   "LIMIT 1";
                }
                else
                {
                    Data_Query = @"SELECT  " +
                       "gid, operator, network_type, link_residential, link_enterprise, link_business, link_backhual, link_connect, '' as pop_name, " +
                       "ST_X(ST_Centroid(geom)) AS center_x, " +
                       "ST_Y(ST_Centroid(geom)) AS center_y, " +
                       "ST_Distance( " +
                       "ST_Transform((ST_Centroid(geom)), 32735), " +
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735) " +
                       ") AS distance_meters " +
                       "FROM site_polygon_layer " +
                       "WHERE ST_DWithin( " +
                       "ST_Transform((ST_Centroid(geom)), 32735),  " +
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X + " " + _GoogleCoord.Latitude_Y + ")'), 4326), 32735), " +
                       "50000 " +
                       ")  " +
                       "ORDER BY distance_meters " +
                       "LIMIT 1";
                }


                DataSet Splited_Ds = _dBHelper.ExecuteSQL(Data_Query);
                dt = Splited_Ds.Tables[0];

                if (dt.Rows[0]["link_residential"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_residential = new SiteDetails();
                    objSiteDetails_link_residential.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_residential.Product = "link residential".ToUpper();
                    objSiteDetails_link_residential.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_residential.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_residential.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_residential.Distance_Meters = "--";
                    objSiteDetails_link_residential.PolygonCount = polygonCount;
                    objSiteDetails_link_residential.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_residential.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_residential.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_residential);
                }

                if (dt.Rows[0]["link_enterprise"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_enterprise = new SiteDetails();
                    objSiteDetails_link_enterprise.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_enterprise.Product = "link enterprise".ToUpper();
                    objSiteDetails_link_enterprise.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_enterprise.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_enterprise.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_enterprise.Distance_Meters = "--";
                    objSiteDetails_link_enterprise.PolygonCount = polygonCount;
                    objSiteDetails_link_enterprise.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_enterprise.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_enterprise.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_enterprise);
                }

                if (dt.Rows[0]["link_residential"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_residential = new SiteDetails();
                    objSiteDetails_link_residential.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_residential.Product = "link residential".ToUpper();
                    objSiteDetails_link_residential.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_residential.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_residential.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_residential.Distance_Meters = "--";
                    objSiteDetails_link_residential.PolygonCount = polygonCount;
                    objSiteDetails_link_residential.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_residential.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_residential.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_residential);
                }

                if (dt.Rows[0]["link_business"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_business = new SiteDetails();
                    objSiteDetails_link_business.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_business.Product = "link business".ToUpper();
                    objSiteDetails_link_business.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_business.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_business.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_business.Distance_Meters = "--";
                    objSiteDetails_link_business.PolygonCount = polygonCount;
                    objSiteDetails_link_business.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_business.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_business.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_business);
                }

                if (dt.Rows[0]["link_backhual"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_backhual = new SiteDetails();
                    objSiteDetails_link_backhual.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_backhual.Product = "link backhual".ToUpper();
                    objSiteDetails_link_backhual.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_backhual.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_backhual.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_backhual.Distance_Meters = "--";
                    objSiteDetails_link_backhual.PolygonCount = polygonCount;
                    objSiteDetails_link_backhual.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_backhual.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_backhual.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_backhual);
                }

                if (dt.Rows[0]["link_connect"].ToString().ToUpper() == "TRUE")
                {
                    SiteDetails objSiteDetails_link_connect = new SiteDetails();
                    objSiteDetails_link_connect.GID = dt.Rows[0]["gid"].ToString();
                    objSiteDetails_link_connect.Product = "link connect".ToUpper();
                    objSiteDetails_link_connect.PoP = dt.Rows[0]["pop_name"].ToString();
                    objSiteDetails_link_connect.Longitude_X = dt.Rows[0]["center_x"].ToString();
                    objSiteDetails_link_connect.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                    objSiteDetails_link_connect.Distance_Meters = "--";
                    objSiteDetails_link_connect.PolygonCount = polygonCount;
                    objSiteDetails_link_connect.Operator = dt.Rows[0]["operator"].ToString();
                    objSiteDetails_link_connect.Network_Type = dt.Rows[0]["network_type"].ToString();
                    objSiteDetails_link_connect.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                    lstSiteDetails.Add(objSiteDetails_link_connect);
                }


                string productPriceQuery = "SELECT n0, sub_n0, xxx, product, sheet, contract_term_months, bandwidth_mbps, mrc_ex_vat, " +
                    "nrc_lt_1gbps_ex_vat, nrc_1_10gbps_ex_vat, nrc_ex_vat, product_features, comments " +
                    "FROM public.enterprise_products where contract_term_months = " + _GoogleCoord.ContractTerm + " and bandwidth_mbps = " + _GoogleCoord.Bandwidth + ";";

                DataSet ProductPrice_Ds = _dBHelper.ExecuteSQL(productPriceQuery);

                foreach (SiteDetails objSiteDetails in lstSiteDetails)
                {
                    foreach (DataRow drr in ProductPrice_Ds.Tables[0].Rows)
                    {
                        if (objSiteDetails.Product == drr["xxx"].ToString())
                        {
                            objSiteDetails.MRC = drr["mrc_ex_vat"].ToString();
                            objSiteDetails.NRC = drr["nrc_lt_1gbps_ex_vat"].ToString();
                        }
                    }
                }


                if (polygonCount != "0")
                {
                    string ThirdPartyQuery = "SELECT gid, n0, bandwidth, term, mrc, nrc, fcc_name, sheet_name FROM fcc where bandwidth = " + _GoogleCoord.Bandwidth + " and term = " + _GoogleCoord.ContractTerm + " and  fcc_name = '" + lstSiteDetails[0].Operator + "'";
                    DataSet ThirdParty_Ds = _dBHelper.ExecuteSQL(ThirdPartyQuery);
                    foreach (SiteDetails objSiteDetails in lstSiteDetails)
                    {
                        foreach (DataRow drr in ThirdParty_Ds.Tables[0].Rows)
                        {
                            objSiteDetails.Party_Provider_3rd = drr["fcc_name"].ToString();
                            objSiteDetails.Party_MRC_3rd = drr["mrc"].ToString();
                            objSiteDetails.Party_NRC_3rd = drr["nrc"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
            finally
            {

            }
            return new JsonResult(lstSiteDetails);
        }


        [HttpPost("GetAddressData")]
        public IActionResult GetAddressData(AddressSearch _Address)
        {
            DataTable dt = new DataTable();
            List<string> listAddress = new List<string>();
            try
            {
                string full_address = "select full_address from AddressData where full_address like '" + _Address.Address + "%' limit 5;";

                DataSet Ds_full_address = _dBHelper.ExecuteSQL(full_address);
                foreach (DataRow row in Ds_full_address.Tables[0].Rows)
                {
                    string value = row[0].ToString(); // first column
                    listAddress.Add(value);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
            finally
            {

            }
            return new JsonResult(listAddress);
        }

        [HttpPost("GetPostGressTool")]
        public IActionResult GetPostGressTool()
        {


            try
            {


                string jsonPath = @"C:\Temp\sheet1_data.json";
                string jsonData = System.IO.File.ReadAllText(jsonPath);

                // Parse the JSON array
                JArray items = JArray.Parse(jsonData);

                foreach (JObject item in items)
                {
                    string DB_Query = $@"
                        INSERT INTO enterprise_products (
                        N0, Sub_N0, xxx, Product, Sheet, Contract_Term_months, Bandwidth_mbps,
                        MRC_ex_VAT, NRC_lt_1gbps_ex_VAT, NRC_1_10gbps_ex_VAT, NRC_ex_VAT,
                        Product_features, Comments
                        ) VALUES (
                        {item["N0"]?.ToString() ?? "NULL"},
                        {item["Sub N0"]?.ToString() ?? "NULL"},
                        '{EscapeString((string?)item["xxx"])}',
                        '{EscapeString((string?)item["Product"])}',
                        '{EscapeString((string?)item["Sheet"])}',
                        {EscapeNan((string?)item["Contract Term (months)"])},
                        {EscapeNan((string?)item["Bandwidth (mbps) "])},
                        {EscapeNan((string?)item["MRC (ex VAT)"])},
                        {EscapeNan((string?)item["NRC <1gbps ex VAT"])},
                        {EscapeNan((string?)item["NRC 1-10gbps ex VAT"])},
                        {EscapeNan((string?)item["MRC (ex VAT)"])},
                        '{EscapeString((string?)item["Product features"])}',
                        '{EscapeString((string?)item["Comments"])}'
                        );";

                    bool res = _dBHelper.ExecuteNonQuery(DB_Query);
                }



            }
            catch (Exception ex)
            {
                // throw new Exception(ex.StackTrace);
            }
            finally
            {

            }
            return new JsonResult(true);
        }



        private static string EscapeString(string value)
        {
            return value?.Replace("'", "''") ?? "";
        }

        private static string EscapeNan(string value)
        {
            return value?.Replace("NaN", "0") ?? "0";
        }

        #endregion



        /*
         * 
         SELECT 
  gid, link_residential, link_enterprise, link_business, link_backhual, link_connect, pop_name,
  ST_X(ST_Centroid(geom)) AS center_x,
  ST_Y(ST_Centroid(geom)) AS center_y,
  ST_Distance(
    ST_Transform(geom, 32735),
    ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(28.053352801283754 -26.100700817195666)'), 4326), 32735)
  ) AS distance_meters
FROM site_polygon_layer
WHERE ST_DWithin(
  ST_Transform(geom, 32735),
  ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(28.053352801283754 -26.100700817195666)'), 4326), 32735),
  15000
) 
ORDER BY distance_meters
LIMIT 1
        */


    }



}
