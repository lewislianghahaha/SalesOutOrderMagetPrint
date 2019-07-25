using System;
using System.Data;
using SalesOutOrderMagetPrint.DB;

namespace SalesOutOrderMagetPrint.Logic
{
    //运算
    public class GenerateDt
    {
        DtList dtList=new DtList();
        SearchDt serDt=new SearchDt();

        /// <summary>
        /// 根据指定条件运算数据并生成DT(作打印之用)
        /// </summary>
        /// <param name="fidlist"></param>
        /// <param name="ordlist"></param>
        /// <returns></returns>
        public DataTable GenerDttoExport(string fidlist,string ordlist)
        {
            var resultdt=new DataTable();
            var ordersplit = ordlist.Split(',');

            try
            {
                //获取模板
                resultdt = dtList.Get_FinalEmptydt();
                //根据条件获取所需明细内容
                var dt = serDt.SearchEntrydt(fidlist);
                //根据获取的DT,选择出0(单据日期) 6(仓库备注) 7(摘要)的值(注:将相同的值进行合并),并用“|”分隔
                var strsplit = MarkNewValue(dt).Split('|');

                //循环将明细记录插入至临时表内
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var rowdtl = resultdt.NewRow();
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        //当列ID分别为0(单据日期) 1(单据编号) 6(仓库备注) 7(摘要)作特别处理;其它按正常赋值操作

                        switch (j)
                        {
                            case 0:
                                rowdtl[j] = strsplit[0];
                                break;
                            case 1:
                                rowdtl[j] = Convert.ToString(ordlist);
                                break;
                            case 6:
                                rowdtl[j] = strsplit[1];
                                break;
                            case 7:
                                rowdtl[j] = strsplit[2];
                                break;
                            default:
                                rowdtl[j] = dt.Rows[i][j];
                                break;
                        }
                    }

                    //每行对DT赋值完成后,最后对6个存放条码字段的列进行循环赋值(注:从列ID 19开始，截止至24)
                    for (var x = 0; x < ordersplit.Length; x++)
                    {
                        rowdtl[18 + x + 1] = ordersplit[x];
                    }
                    resultdt.Rows.Add(rowdtl);
                }
            }
            catch (Exception)
            {
                resultdt.Columns.Clear();
                resultdt.Rows.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 合并同类值(注:ID=0 单据日期 1 仓库备注 2摘要) 输出时用“|”进行合并输出
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string MarkNewValue(DataTable dt)
        {
            var result = string.Empty;
            var temp = string.Empty;
            var tempid=0;

            //外部循环（0:单据日期 1:仓库备注 2:摘要）
            for (var i = 0; i < 1; i++)
            {
                switch (i)
                {
                    case 0:
                        tempid = 0;
                        break;
                    case 1:
                        tempid = 6;
                        break;
                    case 2:
                        tempid = 7;
                        break;
                }

                foreach (DataRow row in dt.Rows)
                {
                    //单据日期相同合并;用“;”分隔
                    if (temp == "" && Convert.ToString(row[tempid]).Replace(" ","") !="")
                    {
                        temp = Convert.ToString(row[tempid])+";";
                    }
                    else
                    {
                        //若从DT中获取的值为空,就不能继续
                        if (Convert.ToString(row[tempid]).Replace(" ","") != "")
                        {
                            //将获取的值进行去空格及大写控制
                            var value = Convert.ToString(row[tempid]).Replace(" ", "").ToLower() + ";";
                            //判断是否存在
                            if (temp.Replace(" ", "").ToLower().Contains(value))
                            {
                                continue;
                            }
                            else
                            {
                                temp += Convert.ToString(row[tempid]) + ";";
                            }
                        }
                    }
                }
                result += temp + "|";
                //将相关中转变量清空
                temp = "";
            }
            return result;
        }
    }
}
