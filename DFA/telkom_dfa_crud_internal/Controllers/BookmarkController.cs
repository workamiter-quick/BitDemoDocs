using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookmarkController : ControllerBase
    {

        private readonly DBHelper _dBHelper;
        
        public BookmarkController(DBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string Query = @"select CAPTION, URL, BRK_ID from BOOKMARK order by CAPTION";
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = _dBHelper.ExecuteSQL(Query);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception("amit");
            }
            finally { }
            return new JsonResult(dt);
        }

        [HttpGet("{UID}")]
        public IActionResult Get(string UID)
        {
            string Query = @"select CAPTION, URL, BRK_ID from BOOKMARK where USERID = '" + UID + "' order by CAPTION";
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

        [HttpPost]
        public IActionResult Post(BookmarkInput BM)
        {
            string Result = string.Empty;
            try
            {
                string DB_Query = "insert into BOOKMARK(BRK_ID, CAPTION, URL, USERID, ISACTIVE) VALUES((select NVL(max(BRK_ID), 0) + 1  from BOOKMARK), '" + BM.Caption + "', '" + BM.URL + "', '" + BM.UserID + "', '1')";
                
                bool res = _dBHelper.ExecuteNonQuery(DB_Query);
                if (res) {
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


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string Query = "delete from BOOKMARK where BRK_ID = " + id.ToString();
            string Result = string.Empty;
            try
            {
                
                bool res = _dBHelper.ExecuteNonQuery(Query);
                if (res)
                {
                    Result = "Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            finally { }
            return new JsonResult(Result);



        }
    }
}
