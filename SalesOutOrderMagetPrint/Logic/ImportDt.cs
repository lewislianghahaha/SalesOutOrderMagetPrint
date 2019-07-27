using System;
using System.Data.SqlClient;
using SalesOutOrderMagetPrint.DB;

namespace SalesOutOrderMagetPrint.Logic
{
    public class ImportDt
    {
        SqlList sqlList=new SqlList();
        SearchDt searchDt=new SearchDt();

        public bool UpdateRecord(string fidlist)
        {
            var result = true;

            try
            {
                var sqlscript = sqlList.Updatedt(fidlist);
                var sqlconn = searchDt.GetConn();
                sqlconn.Open();
                var sqlCommand = new SqlCommand(sqlscript, sqlconn);
                sqlCommand.ExecuteNonQuery();
                sqlconn.Close();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
