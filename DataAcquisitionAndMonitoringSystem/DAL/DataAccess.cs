using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indus.Industrial.DAL
{
    /// <summary>
    /// 数据访问层，封装对 SQL Server 数据库的常用操作
    /// </summary>
    public class DataAccess
    {
        // 从配置文件中读取数据库连接字符串
        string dbConfig = ConfigurationManager.ConnectionStrings["db_config"].ToString();
        SqlConnection conn;        // 数据库连接对象
        SqlCommand comm;           // SQL 命令执行对象
        SqlDataAdapter adapter;    // 数据适配器，用于填充 DataSet/DataTable
        SqlTransaction trans;      // 数据库事务对象


        private void Dispose()
        {
            // 释放数据适配器
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

        private DataTable GetDatas(string sql)  //查询
        {
            DataTable dt = new DataTable();
            try
            {
                // 创建数据库连接并打开
                conn = new SqlConnection(dbConfig);
                conn.Open();

                // 使用适配器执行查询并填充 DataTable
                adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                // 抛出异常，交由上层处理
                throw ex;
            }
            finally
            {
                // 无论成功或失败，均释放资源
                this.Dispose();
            }

            return dt;
        }

 
        //获取所有存储区域信息
        public DataTable GetStorageArea()
        {
            string strSql = "select * from storage_area";
            return this.GetDatas(strSql);
        }

        //获取所有设备信息
        public DataTable GetDevices()
        {
            string strSql = "select * from devices";
            return this.GetDatas(strSql);
        }

        //获取所有监控点位值，按设备 ID 和点位 ID 排序
        public DataTable GetMonitorValues()
        {
            string strSql = "select * from Monitor_values ORDER BY d_id, value_id";
            return this.GetDatas(strSql);
        }

        //更新监控点位的报警阈值（四级报警：lolo、low、high、hihi）
        public bool UpdateMonitorValueAlarm(string valueId, double lolo, double low, double high, double hihi)
        {
            bool result = false;
            try
            {
                // 创建数据库连接并打开
                conn = new SqlConnection(dbConfig);
                conn.Open();

                // 构造 UPDATE 参数化 SQL，防止 SQL 注入
                string strSql = "UPDATE Monitor_values SET alarm_lolo=@lolo, alarm_low=@low, alarm_high=@high, alarm_hihi=@hihi WHERE value_id=@valueId";
                comm = new SqlCommand(strSql, conn);
                // 添加参数
                comm.Parameters.AddWithValue("@lolo", lolo);
                comm.Parameters.AddWithValue("@low", low);
                comm.Parameters.AddWithValue("@high", high);
                comm.Parameters.AddWithValue("@hihi", hihi);
                comm.Parameters.AddWithValue("@valueId", valueId);

                // 执行更新并判断是否影响到了行
                result = comm.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                // 抛出异常，交由上层处理
                throw ex;
            }
            finally
            {
                // 释放数据库资源
                this.Dispose();
            }
            return result;
        }
    }
}

