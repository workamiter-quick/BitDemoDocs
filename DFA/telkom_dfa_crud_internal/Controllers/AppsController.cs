using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Threading;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppsController : ControllerBase
    {

        private readonly DBHelper _dBHelper;
        private readonly IConfiguration _configuration;
        private readonly GoogleMapsProvider _googleMapsProvider;
        private readonly EmailService _emailService;

        public AppsController(DBHelper dBHelper, IConfiguration configuration, GoogleMapsProvider googleMapsProvider, EmailService emailService)
        {
            _dBHelper = dBHelper;
            _configuration = configuration;
            _googleMapsProvider = googleMapsProvider;
            _emailService = emailService;
        }

        [HttpGet]
        public JsonResult Get()
        {

            try
            {
            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception("amit");
            }
            finally { }
            return new JsonResult("dt");
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            string Query = @"SELECT id, company_name, requestor_name, email_address, api_key, api_key_expiry, submitted_at	FROM public.api_access_requests where email_address = '" + email + "'";
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
                DataRow row = dt.Rows[0];
                var result = new ApiAccessRequestDto
                {
                    CompanyName = row["company_name"].ToString(),
                    RequestorName = row["requestor_name"].ToString(),
                    EmailAddress = row["email_address"].ToString(),
                    ApiKey = row["api_key"].ToString()
                };
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }

        }


        [HttpPost("sendapprovalemailtouser")]
        public IActionResult SendApprovalEmailToUser(string email)
        {
            string result;
            try
            {
                string Query = @"SELECT id, company_name, requestor_name, email_address, api_key, api_key_expiry, submitted_at	FROM public.api_access_requests where email_address = '" + email + "'";
                DataTable dt = new DataTable();
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
                DataRow row = dt.Rows[0];
                string apiKey = row["api_key"].ToString();

                _emailService.SendEmailToUserAfterApproval(apiKey, email);

                return Ok("Done");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getfeasibility")]
        public IActionResult GetFeasibility(string key, double latitude_y, double longitude_x, string bandwidth, string term, string products = "NA", bool all_products = false, bool use_max_distance = false, bool include_pop = false)
        {
            List<SiteDetailsApi> lstFinalSiteDetails = new List<SiteDetailsApi>();
            try
            {
                // 1. Check if email already exists
                string checkQuery = "SELECT COUNT(1) FROM api_access_requests WHERE api_key = @api_key";
                var checkParams = new[] { new NpgsqlParameter("@api_key", key) };
                int count = _dBHelper.ExecuteScalar(checkQuery, checkParams);

                if (count == 0)
                {
                    return Unauthorized("API KEY is wrong.");
                }

                GoogleCoord oGoogleCoord = new GoogleCoord();

                oGoogleCoord.Longitude_X = longitude_x.ToString().Replace(",", ".");
                oGoogleCoord.Latitude_Y = latitude_y.ToString().Replace(",", ".");
                oGoogleCoord.Bandwidth = bandwidth.ToString();
                oGoogleCoord.ContractTerm = term.ToString();
                oGoogleCoord.UseMaxDistance = use_max_distance.ToString();


                List<SiteDetailsApi> lstSiteDetails = GetSiteDetail(oGoogleCoord);

                RoutInputCoord oRoutInputCoord = new RoutInputCoord();
                oRoutInputCoord.StartLng_X = lstSiteDetails[0].Longitude_X;
                oRoutInputCoord.StartLat_Y = lstSiteDetails[0].Latitude_Y;
                oRoutInputCoord.EndLng_X = longitude_x.ToString();
                oRoutInputCoord.EndLat_Y = latitude_y.ToString();

                RouteResult oRouteResult = GetRoute(oRoutInputCoord);
                List<SiteDetailsApi> lstFinalSiteDetails_l1 = new List<SiteDetailsApi>();
                foreach (SiteDetailsApi site in lstSiteDetails)
                {
                    var distance = oRouteResult.Distance.Split(' ')[0];
                    var unit = oRouteResult.Distance.Split(' ')[1];

                    if (unit == "m")
                    {
                        site.Distance_Meters = (float.Parse(distance, CultureInfo.InvariantCulture)) + " meters";
                        site.Distance_Meters_Int = (int)(float.Parse(distance, CultureInfo.InvariantCulture));
                    }
                    else if (unit == "km")
                    {
                        site.Distance_Meters = (float.Parse(distance, CultureInfo.InvariantCulture) * 1000) + " meters";
                        site.Distance_Meters_Int = (int)(float.Parse(distance, CultureInfo.InvariantCulture) * 1000);
                    }

                    if (use_max_distance)
                    {
                        lstFinalSiteDetails_l1.Add(site);
                    }
                    else
                    {
                        if (site.Distance_Meters_Int <= 100)
                            lstFinalSiteDetails_l1.Add(site);
                    }
                }

                if (products == "NA")
                {
                    lstFinalSiteDetails = lstFinalSiteDetails_l1;
                }
                else
                {
                    string products_upper = products.ToUpper();
                    foreach (SiteDetailsApi site in lstFinalSiteDetails_l1)
                    {
                        if (products_upper.Contains(site.Product))
                        {
                            lstFinalSiteDetails.Add(site);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally { }
            return new JsonResult(lstFinalSiteDetails);
        }

        [HttpPost]
        public IActionResult Post(ApiKeyInput BM)
        {
            string Result = string.Empty;
            try
            {
                // 1. Check if email already exists
                string checkQuery = "SELECT COUNT(1) FROM api_access_requests WHERE email_address = @Email";
                var checkParams = new[] { new NpgsqlParameter("@Email", BM.EmailAddress) };
                int count = _dBHelper.ExecuteScalar(checkQuery, checkParams);

                if (count > 0)
                {
                    return BadRequest(new { message = "Email address already exists." });
                }

                // 2. Insert if not exists
                DateTime dtExpiry = DateTime.Now.AddYears(10);
                string apiKey = Guid.NewGuid().ToString("N");

                string insertQuery = @"INSERT INTO api_access_requests
            (company_name, requestor_name, email_address, api_key, api_key_expiry)
            VALUES (@Company, @Requestor, @Email, @ApiKey, @Expiry)";

                var insertParams = new[]
                {
            new NpgsqlParameter("@Company", BM.CompanyName),
            new NpgsqlParameter("@Requestor", BM.RequestorName),
            new NpgsqlParameter("@Email", BM.EmailAddress),
            new NpgsqlParameter("@ApiKey", apiKey),
            new NpgsqlParameter("@Expiry", dtExpiry)
        };

                bool res = _dBHelper.ExecuteNonQuery(insertQuery, insertParams);

                Result = res ? "Saved" : "Failed";
                if (Result == "Saved")
                {
                    _emailService.SendEmailToAdmin(BM);
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
                return StatusCode(500, new { message = Result });
            }

            return new JsonResult(Result);
        }

        #region
        private List<SiteDetailsApi> GetSiteDetail(GoogleCoord _GoogleCoord)
        {
            DataTable dt = new DataTable();
            List<SiteDetailsApi> lstSiteDetails = new List<SiteDetailsApi>();

            try
            {
                string countPolygon = "SELECT count(*) as cnt FROM site_polygon_layer WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_CountPolygon = _dBHelper.ExecuteSQL(countPolygon);
                string polygonCount = Ds_CountPolygon.Tables[0].Rows[0][0].ToString();

                string countPolygonForPTCPT = "SELECT count(*) as cnt FROM cpt_pta WHERE ST_Contains(geom, ST_SetSRID(ST_MakePoint(" + _GoogleCoord.Longitude_X + ", " + _GoogleCoord.Latitude_Y + "), 4326));";

                DataSet Ds_PolygonForPTCPT = _dBHelper.ExecuteSQL(countPolygonForPTCPT);
                string PolygonForPTCPT_Count = Ds_PolygonForPTCPT.Tables[0].Rows[0][0].ToString();


                string productPriceQuery = "SELECT n0, sub_n0, xxx, product, sheet, contract_term_months, bandwidth_mbps, mrc_ex_vat, " +
                    "nrc_lt_1gbps_ex_vat, nrc_1_10gbps_ex_vat, nrc_ex_vat, product_features, comments " +
                    "FROM public.enterprise_products where contract_term_months in (" + _GoogleCoord.ContractTerm + ") and bandwidth_mbps in (" + _GoogleCoord.Bandwidth + ");";

                DataSet ProductPrice_Ds = _dBHelper.ExecuteSQL(productPriceQuery);

                foreach (DataRow dr in ProductPrice_Ds.Tables[0].Rows)
                {
                    SiteDetailsApi objSiteDetails = new SiteDetailsApi();
                    objSiteDetails.Product = dr["xxx"].ToString();
                    objSiteDetails.MRC = dr["mrc_ex_vat"].ToString();
                    objSiteDetails.NRC = dr["nrc_lt_1gbps_ex_vat"].ToString();
                    objSiteDetails.Term = dr["contract_term_months"].ToString();
                    objSiteDetails.Bandwidth = dr["bandwidth_mbps"].ToString();

                    lstSiteDetails.Add(objSiteDetails);
                }

                string Data_Query = string.Empty;

                string queryDistanceInMeters = "50000";

                if (polygonCount == "0")
                {

                    Data_Query = @"SELECT  " +
                   "gid, operator, network_type, link_residential, link_enterprise, link_business, link_backhual, link_connect, '' as pop_name, " +
                   "ST_X(ST_Centroid(geom)) AS center_x, " +
                   "ST_Y(ST_Centroid(geom)) AS center_y, " +
                   "ST_Distance( " +
                   "ST_Transform((ST_Centroid(geom)), 32735), " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X.Replace(",", ".") + " " + _GoogleCoord.Latitude_Y.Replace(",", ".") + ")'), 4326), 32735) " +
                   ") AS distance_meters " +
                   "FROM network_line_layer " +
                   "WHERE ST_DWithin( " +
                   "ST_Transform((ST_Centroid(geom)), 32735),  " +
                   "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X.Replace(",", ".") + " " + _GoogleCoord.Latitude_Y.Replace(",", ".") + ")'), 4326), 32735), " +
                   "" + queryDistanceInMeters + " " +
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
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X.Replace(",", ".") + " " + _GoogleCoord.Latitude_Y.Replace(",", ".") + ")'), 4326), 32735) " +
                       ") AS distance_meters " +
                       "FROM site_polygon_layer " +
                       "WHERE ST_DWithin( " +
                       "ST_Transform((ST_Centroid(geom)), 32735),  " +
                       "ST_Transform(ST_SetSRID(ST_GeomFromText('POINT(" + _GoogleCoord.Longitude_X.Replace(",", ".") + " " + _GoogleCoord.Latitude_Y.Replace(",", ".") + ")'), 4326), 32735), " +
                       "" + queryDistanceInMeters + " " +
                       ")  " +
                       "ORDER BY distance_meters " +
                       "LIMIT 1";
                }


                DataSet Splited_Ds = _dBHelper.ExecuteSQL(Data_Query);
                dt = Splited_Ds.Tables[0];

                if (dt.Rows[0]["link_residential"].ToString().ToUpper() == "TRUE")
                {
                    foreach (SiteDetailsApi objSiteDetails_link_residential in lstSiteDetails)
                    {
                        if (objSiteDetails_link_residential.Product == "link residential".ToUpper())
                        {
                            objSiteDetails_link_residential.GID = dt.Rows[0]["gid"].ToString();
                            objSiteDetails_link_residential.PoP = dt.Rows[0]["pop_name"].ToString();
                            objSiteDetails_link_residential.Longitude_X = dt.Rows[0]["center_x"].ToString();
                            objSiteDetails_link_residential.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                            objSiteDetails_link_residential.Distance_Meters = "--";
                            objSiteDetails_link_residential.PolygonCount = polygonCount;
                            objSiteDetails_link_residential.Operator = dt.Rows[0]["operator"].ToString();
                            objSiteDetails_link_residential.Network_Type = dt.Rows[0]["network_type"].ToString();
                            objSiteDetails_link_residential.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                        }
                    }
                }

                if (dt.Rows[0]["link_enterprise"].ToString().ToUpper() == "TRUE")
                {
                    foreach (SiteDetailsApi objSiteDetails_link_enterprise in lstSiteDetails)
                    {
                        if (objSiteDetails_link_enterprise.Product == "link enterprise".ToUpper())
                        {
                            objSiteDetails_link_enterprise.GID = dt.Rows[0]["gid"].ToString();
                            objSiteDetails_link_enterprise.PoP = dt.Rows[0]["pop_name"].ToString();
                            objSiteDetails_link_enterprise.Longitude_X = dt.Rows[0]["center_x"].ToString();
                            objSiteDetails_link_enterprise.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                            objSiteDetails_link_enterprise.Distance_Meters = "--";
                            objSiteDetails_link_enterprise.PolygonCount = polygonCount;
                            objSiteDetails_link_enterprise.Operator = dt.Rows[0]["operator"].ToString();
                            objSiteDetails_link_enterprise.Network_Type = dt.Rows[0]["network_type"].ToString();
                            objSiteDetails_link_enterprise.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                        }
                    }
                }

            

                if (dt.Rows[0]["link_business"].ToString().ToUpper() == "TRUE")
                {
                    foreach (SiteDetailsApi objSiteDetails_link_business in lstSiteDetails)
                    {
                        if (objSiteDetails_link_business.Product == "link business".ToUpper())
                        {
                            objSiteDetails_link_business.GID = dt.Rows[0]["gid"].ToString();
                            objSiteDetails_link_business.PoP = dt.Rows[0]["pop_name"].ToString();
                            objSiteDetails_link_business.Longitude_X = dt.Rows[0]["center_x"].ToString();
                            objSiteDetails_link_business.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                            objSiteDetails_link_business.Distance_Meters = "--";
                            objSiteDetails_link_business.PolygonCount = polygonCount;
                            objSiteDetails_link_business.Operator = dt.Rows[0]["operator"].ToString();
                            objSiteDetails_link_business.Network_Type = dt.Rows[0]["network_type"].ToString();
                            objSiteDetails_link_business.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                        }
                    }
                }

                if (dt.Rows[0]["link_backhual"].ToString().ToUpper() == "TRUE")
                {
                    foreach (SiteDetailsApi objSiteDetails_link_backhual in lstSiteDetails)
                    {
                        if (objSiteDetails_link_backhual.Product == "link backhual".ToUpper())
                        {
                            objSiteDetails_link_backhual.GID = dt.Rows[0]["gid"].ToString();
                            objSiteDetails_link_backhual.PoP = dt.Rows[0]["pop_name"].ToString();
                            objSiteDetails_link_backhual.Longitude_X = dt.Rows[0]["center_x"].ToString();
                            objSiteDetails_link_backhual.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                            objSiteDetails_link_backhual.Distance_Meters = "--";
                            objSiteDetails_link_backhual.PolygonCount = polygonCount;
                            objSiteDetails_link_backhual.Operator = dt.Rows[0]["operator"].ToString();
                            objSiteDetails_link_backhual.Network_Type = dt.Rows[0]["network_type"].ToString();
                            objSiteDetails_link_backhual.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                        }
                    }
                }

                if (dt.Rows[0]["link_connect"].ToString().ToUpper() == "TRUE")
                {
                    foreach (SiteDetailsApi objSiteDetails_link_connect in lstSiteDetails)
                    {
                        if (objSiteDetails_link_connect.Product == "link connect".ToUpper())
                        {
                            objSiteDetails_link_connect.GID = dt.Rows[0]["gid"].ToString();
                            objSiteDetails_link_connect.PoP = dt.Rows[0]["pop_name"].ToString();
                            objSiteDetails_link_connect.Longitude_X = dt.Rows[0]["center_x"].ToString();
                            objSiteDetails_link_connect.Latitude_Y = dt.Rows[0]["center_y"].ToString();
                            objSiteDetails_link_connect.Distance_Meters = "--";
                            objSiteDetails_link_connect.PolygonCount = polygonCount;
                            objSiteDetails_link_connect.Operator = dt.Rows[0]["operator"].ToString();
                            objSiteDetails_link_connect.Network_Type = dt.Rows[0]["network_type"].ToString();
                            objSiteDetails_link_connect.IsCPTPTA = PolygonForPTCPT_Count == "0" ? false : true;
                        }
                    }
                }


                //string productPriceQuery = "SELECT n0, sub_n0, xxx, product, sheet, contract_term_months, bandwidth_mbps, mrc_ex_vat, " +
                //    "nrc_lt_1gbps_ex_vat, nrc_1_10gbps_ex_vat, nrc_ex_vat, product_features, comments " +
                //    "FROM public.enterprise_products where contract_term_months in (" + _GoogleCoord.ContractTerm + ") and bandwidth_mbps in (" + _GoogleCoord.Bandwidth + ");";

                //DataSet ProductPrice_Ds = _dBHelper.ExecuteSQL(productPriceQuery);

                //foreach (SiteDetails objSiteDetails in lstSiteDetails)
                //{
                //    foreach (DataRow drr in ProductPrice_Ds.Tables[0].Rows)
                //    {
                //        if (objSiteDetails.Product == drr["xxx"].ToString())
                //        {
                //            objSiteDetails.MRC = drr["mrc_ex_vat"].ToString();
                //            objSiteDetails.NRC = drr["nrc_lt_1gbps_ex_vat"].ToString();
                //        }
                //    }
                //}


                if (polygonCount != "0")
                {
                    string ThirdPartyQuery = "SELECT gid, n0, bandwidth, term, mrc, nrc, fcc_name, sheet_name FROM fcc where bandwidth in (" + _GoogleCoord.Bandwidth + ") and term in (" + _GoogleCoord.ContractTerm + ") and  fcc_name = '" + lstSiteDetails[0].Operator + "'";
                    DataSet ThirdParty_Ds = _dBHelper.ExecuteSQL(ThirdPartyQuery);
                    foreach (SiteDetailsApi objSiteDetails in lstSiteDetails)
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
            return lstSiteDetails;
        }

        private RouteResult GetRoute(RoutInputCoord coordinates)
        {
            RouteResult result1 = null;
            RouteResult result2 = null;
            double result1Distance = 0;
            double result2Distance = 0;


            result1 = _googleMapsProvider.GetRoute(coordinates.StartLat_Y.Replace(",", "."), coordinates.StartLng_X.Replace(",", "."), coordinates.EndLat_Y.Replace(",", "."), coordinates.EndLng_X.Replace(",", "."));
            result2 = _googleMapsProvider.GetRoute(coordinates.EndLat_Y.Replace(",", "."), coordinates.EndLng_X.Replace(",", "."), coordinates.StartLat_Y.Replace(",", "."), coordinates.StartLng_X.Replace(",", "."));


            if (result1.Distance.Split(' ')[1] == "km")
            {
                result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture) * 1000;
            }
            else if (result1.Distance.Split(' ')[1] == "m")
            {
                result1Distance = double.Parse(result1.Distance.Split(' ')[0], CultureInfo.InvariantCulture);
            }


            if (result2.Distance.Split(' ')[1] == "km")
            {
                result2Distance = double.Parse(result2.Distance.Split(' ')[0], CultureInfo.InvariantCulture) * 1000;
            }
            else if (result2.Distance.Split(' ')[1] == "m")
            {
                result2Distance = double.Parse(result2.Distance.Split(' ')[0], CultureInfo.InvariantCulture);
            }


            if (result1 == null && result2 == null)
                throw new Exception("Route could not be determined.");

            if (result1Distance < result2Distance)
                return result1;

            if (result2Distance < result1Distance)
                return result2;

            return result2;
        }
        #endregion
    }
}
