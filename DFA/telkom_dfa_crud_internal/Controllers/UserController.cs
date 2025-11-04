using Crud.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class UserController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public UserController(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string Query = @"SELECT ID,Name,Description FROM tblUser";
            DataTable dt = new DataTable();
            try
            {
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(Query, myCon))
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
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            string Query = @"SELECT ID,Name,Description FROM tblUser where ID = " + id.ToString();
            DataTable dt = new DataTable();
            try
            {
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(Query, myCon))
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
                throw ex;
            }
            finally { }
            return new JsonResult(dt);



        }

        [HttpPost]
        public JsonResult Post(User use)
        {
            string Result = string.Empty;
            Result = "Added Successfully";
            try {
                //string query = @"INSERT INTO tblUser (Name, Description) VALUES('" + use.Name + "','" + use.Description + "')";

                string query = @"
                           insert into dbo.tblUser
                           (Name, Description)
                    values (@Name, @Description)
                            ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", use.Name);
                        myCommand.Parameters.AddWithValue("@Description", use.Description);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                        Result = "Added Successfully";
                    }
                }
            }
            catch (Exception ex) {
                Result = ex.Message;
            }
            finally { }

            return new JsonResult(Result);
        }

        [HttpPut]
        public JsonResult Put(User use)
        {
            string Result = string.Empty;
            Result = "Updated Successfully";
            try
            {
              
                string query = @"
                           update dbo.tblUser
                           set Name= @Name,
                            Description=@Description
                            where ID=@ID
                            ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@ID", use.ID);
                        myCommand.Parameters.AddWithValue("@Name", use.Name);
                        myCommand.Parameters.AddWithValue("@Description", use.Description);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                        Result = "Updated Successfully";
                    }
                }
            }
            catch (Exception ex) {
                Result = ex.Message;
            }
            finally { }

            return new JsonResult(Result);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string Query = @"delete FROM tblUser where ID = " + id.ToString();
            DataTable dt = new DataTable();
            string Result = string.Empty;
            Result = "Deleted Successfully";
            try
            {
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(Query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dt.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                        Result = "Deleted Successfully";
                    }
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
