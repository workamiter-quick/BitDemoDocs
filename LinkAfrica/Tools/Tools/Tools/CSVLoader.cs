using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    internal static class CSVLoader
    {

        public static void LoadCsv()
        {
            DataSet ds = ExecuteSQL();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                string insertQuery = "INSERT INTO enterprise_products(n0, sub_n0, xxx, product, sheet, contract_term_months, " +
                     "bandwidth_mbps, mrc_ex_vat, nrc_lt_1gbps_ex_vat, nrc_1_10gbps_ex_vat, nrc_ex_vat, product_features, comments)" +
                     "VALUES (" + dr[0].ToString().Replace(",", ".") + ", " + dr[1].ToString().Replace(",", ".") + ", '" + dr[2].ToString() + "', " +
                     "'" + dr[3].ToString() + "', '" + dr[4].ToString() + "', " + dr[5].ToString().Replace(",", ".") + ", " + dr[6].ToString().Replace(",", ".") + ", " + dr[7].ToString().Replace(",", ".") + ", " + dr[8].ToString().Replace(",", ".") + ", " + dr[9].ToString().Replace(",", ".") + ", '" + dr[10].ToString() + "', '" + dr[11].ToString() + "', '" + dr[12].ToString() + "');";

                ExecuteInsert(insertQuery);
            }
        }
        public static void ExecuteInsert(string InsertExecuteQuery)
        {
            NpgsqlConnection conn = null;
            bool isExecute = false;
            try
            {
                string oradb = "Server=localhost;Port=5432;Database=LASptialData;User Id=postgres;Password=java;CommandTimeout=200;";
                using (conn = new NpgsqlConnection(oradb))
                using (NpgsqlCommand cmd = new NpgsqlCommand(InsertExecuteQuery, conn))
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
        }



        private static DataSet ExecuteSQL()
        {
            string strQuery = "select * from PriceData";
            DataSet objDataSet = new DataSet();
            SqlConnection objConn = null;
            try
            {
                string strConnString = "Data Source=QUICKSYSTEM;Initial Catalog=LA_Data;Persist Security Info=True;User ID=sa;Password=java;Max Pool Size=200;";
                objConn = new SqlConnection(strConnString);
                objConn.Open();
                SqlDataAdapter objDataAdapter = new SqlDataAdapter(strQuery, objConn);
                objDataAdapter.Fill(objDataSet);
            }
            catch (Exception ex)
            {
                throw new Exception("Portal database not found or some column not exist. \n" + ex.Message);
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Close();
                }
            }
            return objDataSet;
        }
    }
}

