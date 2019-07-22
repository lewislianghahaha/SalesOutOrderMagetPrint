using System;
using System.Data;

namespace SalesOutOrderMagetPrint.DB
{
    public class DtList
    {
        public DataTable Get_FinalEmptydt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 25; i++)
            {
                var dc = new DataColumn();

                switch (i)
                {
                    case 0:
                        dc.ColumnName = "单据日期";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                    case 1:
                        dc.ColumnName = "单据编号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 2:
                        dc.ColumnName = "购货单位";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 3:
                        dc.ColumnName = "一级客户标签";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "二级客户名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 5:
                        dc.ColumnName = "二级客户标签";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 6:
                        dc.ColumnName = "仓库备注";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "摘要";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 8:
                        dc.ColumnName = "客服员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 9:
                        dc.ColumnName = "销售部门";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 10:
                        dc.ColumnName = "销售业务员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 11:
                        dc.ColumnName = "仓管员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 12:
                        dc.ColumnName = "产品名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 13:
                        dc.ColumnName = "规格型号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 14:
                        dc.ColumnName = "批号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 15:
                        dc.ColumnName = "实发数量";
                        dc.DataType = Type.GetType("System.Decimal"); 
                        break;
                    case 16:
                        dc.ColumnName = "定制";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 17:
                        dc.ColumnName = "备注";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 18:
                        dc.ColumnName = "包装件数";
                        dc.DataType = Type.GetType("System.Decimal");
                        break;
                    case 19:
                        dc.ColumnName = "Code1";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 20:
                        dc.ColumnName = "Code2";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 21:
                        dc.ColumnName = "Code3";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 22:
                        dc.ColumnName = "Code4";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 23:
                        dc.ColumnName = "Code5";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 24:
                        dc.ColumnName = "Code6";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 查询窗体临时表
        /// </summary>
        /// <returns></returns>
        public DataTable Get_Searchdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 11; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "单据编号";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 1:
                        dc.ColumnName = "单据日期";
                        dc.DataType = Type.GetType("System.Datetime");
                        break;
                    case 2:
                        dc.ColumnName = "一级客户编码";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    case 3:
                        dc.ColumnName = "一级客户名称";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 4:
                        dc.ColumnName = "二级客户编码";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 5:
                        dc.ColumnName = "客服员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 6:
                        dc.ColumnName = "销售部门";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 7:
                        dc.ColumnName = "销售业务员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 8:
                        dc.ColumnName = "仓管员";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 9:
                        dc.ColumnName = "仓库备注";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    case 10:
                        dc.ColumnName = "摘要";
                        dc.DataType = Type.GetType("System.String");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }



    }
}
