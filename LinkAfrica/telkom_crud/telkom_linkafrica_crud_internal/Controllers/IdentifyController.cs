using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentifyController : ControllerBase
    {

        private readonly DBHelper _dBHelper;
        public IdentifyController(DBHelper dBHelper)
        {
            _dBHelper = dBHelper;
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
                            case "Housing_Point_Layer":
                                Parcel_Query = "SELECT gid, housing_id, operator, status, project_id, comment, captured_date, master_type, master_size, enclosure, route_stage, latitude, longitude, region, route_type, suburb, town, province, user_name, id2, ST_AsText(geom) AS WKT, link_residential, link_enterprise, link_business, link_backhual, link_connect, 'Housing_Point_Layer' as \"LYR\" FROM housing_point_layer where ST_Intersects(ST_Transform(geom, 3857), ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint(" + IIB.XCoord + ", " + IIB.YCoord + "), 4326), 3857), 5))";
                                break;
                            case "Site_Point_Layer":
                                Parcel_Query = "SELECT gid, id2, site_name, me_olt, backhaul_type, network_type, comments, latitude, longitude, suburb, town, province, status, user_name, circuit_d, client_name, region, captured_date, ST_AsText(geom) AS WKT, link_residential, link_enterprise, link_business, link_backhual, link_connect, 'site_point_layer' as \"LYR\" FROM site_point_layer where ST_Intersects(ST_Transform(geom, 3857), ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint(" + IIB.XCoord + ", " + IIB.YCoord + "), 4326), 3857), 5))";
                                break;
                            case "Network_Line_Layer":
                                Parcel_Query = "SELECT gid, id2, status, project_id, comments, civ_method, civ_dep_mm, captured_date, route_stage, route_type, operator, duct, duct_type, cable_type, re_ins_asp, re_ins_con, re_ins_pav, re_ins_nat, re_ins_gra, civ_len_m, region, network_type, user_name, suburb, town, province, circuit_id, distance, ST_AsText(geom) AS WKT, link_residential, link_enterprise, link_business, link_backhual, link_connect, 'network_line_layer' as \"LYR\" FROM network_line_layer where ST_Intersects(ST_Transform(geom, 3857), ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint(" + IIB.XCoord + ", " + IIB.YCoord + "), 4326), 3857), 5))";
                                break;
                            case "Site_Polygon_Layer":
                                Parcel_Query = "SELECT gid, id2, site_name, address, zone, block, type, network_type, backhaul_type, suburb, town, province, region, latitude, longitude, area, backhaul_d, status, comments, layer_type, user_name, operator, captured_date, pop_name, ST_AsText(geom) AS WKT, link_residential, link_enterprise, link_business, link_backhual, link_connect, 'site_polygon_layer' as \"LYR\" FROM site_polygon_layer where ST_Intersects(ST_Transform(geom, 3857), ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint(" + IIB.XCoord + ", " + IIB.YCoord + "), 4326), 3857), 5))";
                                break;
                            case "Managed_Backhaul": // PK
                                Parcel_Query = "SELECT gid, site_name, suburb, town, province, lat, lon, connection, network_type, site_type, region, rings, status, user_name, captured_date, operator, id2, ST_AsText(geom) AS WKT, link_residential, link_enterprise, link_business, link_backhual, link_connect, 'managed_backhaul' as \"LYR\" FROM managed_backhaul where ST_Intersects(ST_Transform(geom, 3857), ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint(" + IIB.XCoord + ", " + IIB.YCoord + "), 4326), 3857), 5))";
                                break;
                        }

                        if (!string.IsNullOrEmpty(Parcel_Query))
                        {
                            DataTable dt = new DataTable();
                            dt.TableName = Layer;

                            DataSet ds = _dBHelper.ExecuteSQL(Parcel_Query);
                            dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                dt.TableName = Layer;
                                lstInfoDataSet.Add(dt);
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




    }
}
