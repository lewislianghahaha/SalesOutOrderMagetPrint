using System;
using System.Data;

namespace SalesOutOrderMagetPrint.Logic
{
    public class TaskLogic
    {
        SearchDt searchDt=new SearchDt();
        GenerateDt generateDt=new GenerateDt();

        #region 变量定义

        private int _taskid;             //记录中转ID
        private string _fcustname;       //获取一级客户名称
        private string _scustname;       //获取二级客户名称
        private DateTime _sdt;           //获取起始单据日期
        private DateTime _edt;           //获取结束单据日期
        private string _fidlist;         //获取所选的FID列表
        private string _ordlist;         //获取所选的单据名称列表
        
        private DataTable _resultTable;  //返回DT类型
        private bool _resultMark;        //返回是否成功标记

        #endregion

        #region Set

            /// <summary>
            /// 中转ID
            /// </summary>
            public int TaskId { set { _taskid = value; } }

            /// <summary>
            /// 获取一级客户名称
            /// </summary>
            public string Fcustname { set { _fcustname = value; } }

            /// <summary>
            /// 获取二级客户名称
            /// </summary>
            public string Scustname { set { _scustname = value; } }

            /// <summary>
            /// 获取起始单据日期
            /// </summary>
            public DateTime Sdt { set { _sdt = value; } }

            /// <summary>
            /// 获取结束单据日期
            /// </summary>
            public DateTime Edt { set { _edt = value; } }

            /// <summary>
            /// 获取所选的FID列表
            /// </summary>
            public string Fidlist { set { _fidlist = value; } }

            /// <summary>
            /// 获取所选的单据名称列表
            /// </summary>
            public string Ordlist { set { _ordlist = value; } }

        #endregion

        #region Get

            /// <summary>
            ///返回DataTable至主窗体
            /// </summary>
            public DataTable ResultTable => _resultTable;

            /// <summary>
            /// 返回结果标记
            /// </summary>
            public bool ResultMark => _resultMark;

        #endregion

        public void StartTask()
        {
            switch (_taskid)
            {
                //查询
                case 0:
                    Searchdt(_fcustname,_scustname,_sdt,_edt);
                    break;
                //合并打印
                case 1:
                    Generatedt(_fidlist,_ordlist);
                    break;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="fcustname"></param>
        /// <param name="scustname"></param>
        /// <param name="sdt"></param>
        /// <param name="edt"></param>
        private void Searchdt(string fcustname, string scustname, DateTime sdt, DateTime edt)
        {
            _resultTable = searchDt.Searchdt(fcustname, scustname, sdt, edt);
        }

        /// <summary>
        /// 运算及返回供打印使用的DT
        /// </summary>
        /// <param name="fidlist">FID列表</param>
        /// <param name="ordlist">单据名称列表</param>
        private void Generatedt(string fidlist,string ordlist)
        {
            _resultTable = generateDt.GenerDttoExport(fidlist,ordlist);
        }

    }
}
