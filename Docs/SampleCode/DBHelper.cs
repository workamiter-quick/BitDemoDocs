using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;


namespace Crud.Controllers
{
    public class DBHelper
    {

        private readonly IConfiguration _configuration;
        public DBHelper(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        public DataSet ExecuteSQL(string SQL)
        {

            DataSet gpDataSet = new DataSet();
            NpgsqlConnection conn = null;
            NpgsqlDataAdapter Oda = null;
            try
            {

                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                conn = new NpgsqlConnection(oradb);
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;

                Oda = new NpgsqlDataAdapter(cmd);
                gpDataSet = new DataSet();
                Oda.Fill(gpDataSet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }

            return gpDataSet;

        }

        public bool ExecuteNonQuery(string ExecuteQuery)
        {
            NpgsqlConnection conn = null;
            bool isExecute = false;
            try
            {
                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                using (conn = new NpgsqlConnection(oradb))
                using (NpgsqlCommand cmd = new NpgsqlCommand(ExecuteQuery, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    isExecute = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return isExecute;
        }

        public int ExecuteScalar(string ExecuteQuery)
        {
            NpgsqlConnection conn = null;
            int generatedId = 0;
            try
            {
                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                using (conn = new NpgsqlConnection(oradb))
                using (NpgsqlCommand cmd = new NpgsqlCommand(ExecuteQuery, conn))
                {
                    conn.Open();                   
                    generatedId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return generatedId;
        }
    }
}
