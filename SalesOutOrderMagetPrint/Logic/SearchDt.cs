using System;
using System.Data;
using System.Data.SqlClient;
using SalesOutOrderMagetPrint.DB;
using UTools;

namespace SalesOutOrderMagetPrint.Logic
{
    public class SearchDt
    {
        DtList dtList=new DtList();
        SqlList sqlList=new SqlList();

        public DataTable Searchdt(string fcustname,string scustname,DateTime sdt,DateTime edt)
        {
            var resultdt=new DataTable();
            var dt=new DataTable();

            try
            {
                //获取临时表记录
                var tempdt = dtList.Get_Searchdt();
                var sqlscript = sqlList.Get_Search(fcustname, scustname, sdt, edt);
                var sqlDataAdapter=new SqlDataAdapter(sqlscript,GetConn());
                sqlDataAdapter.Fill(dt);
                resultdt = dt.Rows.Count > 0 ? dt : tempdt;
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConn()
        {
            var conn = new Conn();
            var sqlcon = new SqlConnection(conn.GetConnectionString());
            return sqlcon;
        }

        /// <summary>
        /// 根据条件查询明细记录
        /// </summary>
        /// <param name="fidlist"></param>
        /// <returns></returns>
        public DataTable SearchEntrydt(string fidlist)
        {
            var dt=new DataTable();

            try
            {
                var sqlscript = sqlList.Get_SalOut(fidlist);
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, GetConn());
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception)
            {
                dt.Columns.Clear();
                dt.Rows.Clear();
            }
            return dt;
        }
    }
}
