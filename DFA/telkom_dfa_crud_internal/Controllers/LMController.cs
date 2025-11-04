using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Data;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LMController : ControllerBase
    {

        private readonly DBHelper _dBHelper;
        public LMController(DBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        [HttpGet]
        public JsonResult Get()
        {

            string Query = @"select GL.GROUP_NAME as ""GROUP_NAME"", WC.objectid as ""OBJECTID"", WC.layer_alias_name as ""LAYER_ALIAS_NAME"", WC.layer_wms_url as ""LAYER_WMS_URL"", WC.layer_name as ""LAYER_NAME"", WC.format as ""FOMART"", WC.transparent as ""TRANSPARENT"", WC.tiled as ""TILED"", WC.buffer as ""BUFFER"", WC.display_outside_max_extent as ""DISPLAY_OUTSIDE_MAX_EXTENT"", WC.baselayer as ""BASELAYER"", WC.display_in_layerswitcher as ""DISPLAY_IN_LAYERSWITCHER"", WC.visibility as ""VISIBILITY"", WC.isdeleted as ""ISDELETED"", WC.groupid as ""GROUPID"", WC.userid as ""USERID"", WC.layer_index as ""LAYER_INDEX"" from GROUP_LAYERS GL right outer join  WMS_CONFIG WC on GL.OBJECTID = WC.GROUPID where WC.GROUPID is not null and WC.ISDELETED = 0 order by WC.GROUPID, WC.layer_index";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception(ex.StackTrace);
            }
            finally { }
            return new JsonResult(dt);



        }



        [HttpGet("[action]/{IDs}")]
        public JsonResult GetGroup(string IDs)
        {

            string Query = @"SELECT objectid as ""OBJECTID"", group_name as ""GROUPNAME"" FROM group_layers where isdeleted = 0 order by group_name";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception(ex.StackTrace);
            }
            finally { }
            return new JsonResult(dt);
        }

        [HttpGet("[action]/{ID}")]
        public JsonResult GetAllWMS(string ID)
        {

            string Query = @"select GL.GROUP_NAME as ""GROUP_NAME"", WC.objectid as ""OBJECTID"", WC.layer_alias_name as ""LAYER_ALIAS_NAME"", WC.layer_wms_url as ""LAYER_WMS_URL"", WC.layer_name as ""LAYER_NAME"", WC.fomart as ""FOMART"", WC.transparent as ""TRANSPARENT"", WC.tiled as ""TILED"", WC.buffer as ""BUFFER"", WC.display_outside_max_extent as ""DISPLAY_OUTSIDE_MAX_EXTENT"", WC.baselayer as ""BASELAYER"", WC.display_in_layerswitcher as ""DISPLAY_IN_LAYERSWITCHER"", WC.visibility as ""VISIBILITY"", WC.isdeleted as ""ISDELETED"", WC.groupid as ""GROUPID"", WC.userid as ""USERID"", WC.layer_index as ""LAYER_INDEX"" from GROUP_LAYERS GL right outer join  WMS_CONFIG WC on GL.OBJECTID = WC.GROUPID where WC.GROUPID is not null order by WC.GROUPID, WC.layer_index";

            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception(ex.StackTrace);
            }
            finally { }
            return new JsonResult(dt);
        }

        [HttpPost]
        public IActionResult Post(GroupBeans GB)
        {
            string Result = string.Empty;
            try
            {
                string DB_Query = "INSERT INTO group_layers(objectid, group_name, isdeleted, userid) VALUES ((select max(objectid) + 1 from group_layers), '" + GB.GroupName + "', 0, " + GB.UserID + ")";

                bool res = _dBHelper.ExecuteNonQuery(DB_Query);
                if (res)
                {
                    Result = "Saved";
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            finally
            {

            }
            return new JsonResult(Result);
        }

    }
}
