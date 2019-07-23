using System;
using System.Data;

namespace SalesOutOrderMagetPrint.Logic
{
    //运算
    public class GenerateDt
    {
        /// <summary>
        /// 根据指定条件运算数据并生成DT(作打印之用)
        /// </summary>
        /// <returns></returns>
        public DataTable GenerDttoExport()
        {
            var resultdt=new DataTable();
            try
            {

            }
            catch (Exception)
            {
                resultdt.Columns.Clear();
                resultdt.Rows.Clear();
            }
            return resultdt;
        }
    }
}
