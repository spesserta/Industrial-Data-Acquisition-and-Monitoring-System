using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Industrial.DAL
{
    public class DataAccess
    {
        string dbConfig = ConfigurationManager.ConnectionStrings["db_config"].ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adapter;
        SqlTransaction trans;


        private void Dispose()
        {
            if (adapter != null)
            {
                adapter.Dispose(); adapter = null;
            }
            if (comm != null)
            {
                comm.Dispose(); comm = null;
            }
            if (trans != null)
            {
                trans.Dispose(); trans = null;
            }
            if (conn != null)
            {
                conn.Close(); conn.Dispose(); conn = null;
            }
        }


        private DataTable GetDatas(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                conn = new SqlConnection(dbConfig);
                conn.Open();

                adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }

            return dt;
        }

        public DataTable GetStorageArea()
        {
            string strSql = "select * from storage_area";
            return this.GetDatas(strSql);
        }
        public DataTable GetDevices()
        {
            string strSql = "select * from devices";
            return this.GetDatas(strSql);
        }
        public DataTable GetMonitorValues()
        {
            string strSql = "select * from Monitor_values ORDER BY d_id, value_id";
            return this.GetDatas(strSql);
        }
    }
}
