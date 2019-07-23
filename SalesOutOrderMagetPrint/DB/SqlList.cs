using System;

namespace SalesOutOrderMagetPrint.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        /// <summary>
        /// 查询生成报表记录
        /// </summary>
        /// <returns></returns>
        public string Get_SalOut(string fidlist)
        {
            _result = $@"SELECT a.FDATE 单据日期,a.FBILLNO 单据编号,
	                            x1.FNAME 购货单位,x.f_ytc_text25 一级客户标签,
	                            y1.FNAME 二级客户名称,y.F_YTC_TEXT25 二级客户标签,
	                            a.F_YTC_REMARKS2 仓库备注,
	                            a.F_YTC_TEXT1 摘要,

	                            z1.FNAME 客服员,
	                            h1.FNAME 销售部门,
	                            h4.FNAME 销售业务员,
	                            z4.FNAME 仓管员,

	                            d.FNAME 产品名称,
	                            d.FSPECIFICATION 规格型号,
	                            z.FNUMBER 批号,
	                            Convert(decimal(18,0),b.FREALQTY) 实发数量,
	                            f.FDATAVALUE 定制,
	                            b.FNOTE 备注,
	                            CONVERT(DECIMAL(18,2),b.F_YTC_DECIMAL) 包装件数

                        FROM dbo.T_SAL_OUTSTOCK a
                        INNER JOIN dbo.T_SAL_OUTSTOCKENTRY b ON a.FID=b.FID

                        INNER JOIN dbo.T_BD_MATERIAL c ON b.FMATERIALID=c.FMATERIALID
                        INNER JOIN dbo.T_BD_MATERIAL_L d ON c.FMATERIALID=d.FMATERIALID
                        INNER JOIN T_BAS_ASSISTANTDATAENTRY e ON c.F_YTC_ASSISTANT1111=e.FENTRYID
                        INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L f ON e.FENTRYID=f.FENTRYID

                        INNER JOIN dbo.T_BD_CUSTOMER x ON a.FCUSTOMERID=x.FCUSTID
                        INNER JOIN dbo.T_BD_CUSTOMER_L x1 ON x.FCUSTID=x1.FCUSTID

                        LEFT JOIN dbo.T_BD_CUSTOMER y ON a.F_YTC_BASE=y.FCUSTID
                        LEFT JOIN dbo.T_BD_CUSTOMER_L y1 ON y.FCUSTID=y1.FCUSTID

                        --批号
                        LEFT JOIN T_BD_LOTMASTER z ON b.FLOT=z.FLOTID

                        --销售部门
                        LEFT JOIN dbo.T_BD_DEPARTMENT h ON a.FSALEDEPTID=h.FDEPTID
                        LEFT JOIN dbo.T_BD_DEPARTMENT_L h1 ON h.FDEPTID=h1.FDEPTID

                        --销售业务员
                        LEFT JOIN V_BD_SALESMAN h2 ON a.FSALESMANID=h2.fid
                        LEFT JOIN dbo.T_BD_STAFF h3 ON h2.FSTAFFID=h3.FSTAFFID
                        LEFT JOIN dbo.T_BD_STAFF_L h4 ON h3.FSTAFFID=h4.FSTAFFID

                        --客服员
                        LEFT JOIN dbo.V_USERS z1 ON a.F_YTC_USERID=z1.FUSERID

                        --仓管员
                        LEFT JOIN V_BD_WAREHOUSEWORKERS z2 ON a.FSTOCKERID=z2.fid
                        LEFT JOIN dbo.T_BD_STAFF z3 ON z2.FSTAFFID=z3.FSTAFFID
                        LEFT JOIN dbo.T_BD_STAFF_L z4 ON z3.FSTAFFID=z4.FSTAFFID

                        WHERE a.fid in('{fidlist}') --'XSCKD082458'--'XSCKD082471'
                        and (LEN(d.FNAME)>0)
                        ORDER BY b.FMATERIALID";

            return _result;
        }

        /// <summary>
        /// 查询页面SQL
        /// </summary>
        /// <param name="firstcustomerName"></param>
        /// <param name="seconCustomerName"></param>
        /// <param name="sdt"></param>
        /// <param name="edt"></param>
        /// <returns></returns>
        public string Get_Search(string firstcustomerName, string seconCustomerName,DateTime sdt,DateTime edt)
        {
            _result = $@"
                            SELECT a.FID,a.FBILLNO 单据编号,a.FDATE 单据日期,
	                               x.FNUMBER 一级客户编码,x1.FNAME 一级客户名称,
	                               y.FNUMBER 二级客户编码,y1.FNAME 二级客户名称,
	                               z1.FNAME 客服员,h1.FNAME 销售部门,h4.FNAME 销售业务员,z4.FNAME 仓管员,
	                               a.F_YTC_REMARKS2 仓库备注,a.F_YTC_TEXT1 摘要

                            FROM dbo.T_SAL_OUTSTOCK a
                            INNER JOIN dbo.T_BD_CUSTOMER x ON a.FCUSTOMERID=x.FCUSTID
                            INNER JOIN dbo.T_BD_CUSTOMER_L x1 ON x.FCUSTID=x1.FCUSTID

                            LEFT JOIN dbo.T_BD_CUSTOMER y ON a.F_YTC_BASE=y.FCUSTID
                            LEFT JOIN dbo.T_BD_CUSTOMER_L y1 ON y.FCUSTID=y1.FCUSTID

                            --客服员
                            LEFT JOIN dbo.V_USERS z1 ON a.F_YTC_USERID=z1.FUSERID

                            --销售部门
                            LEFT JOIN dbo.T_BD_DEPARTMENT h ON a.FSALEDEPTID=h.FDEPTID
                            LEFT JOIN dbo.T_BD_DEPARTMENT_L h1 ON h.FDEPTID=h1.FDEPTID

                            --销售业务员
                            LEFT JOIN V_BD_SALESMAN h2 ON a.FSALESMANID=h2.fid
                            LEFT JOIN dbo.T_BD_STAFF h3 ON h2.FSTAFFID=h3.FSTAFFID
                            LEFT JOIN dbo.T_BD_STAFF_L h4 ON h3.FSTAFFID=h4.FSTAFFID

                            --仓管员
                            LEFT JOIN V_BD_WAREHOUSEWORKERS z2 ON a.FSTOCKERID=z2.fid
                            LEFT JOIN dbo.T_BD_STAFF z3 ON z2.FSTAFFID=z3.FSTAFFID
                            LEFT JOIN dbo.T_BD_STAFF_L z4 ON z3.FSTAFFID=z4.FSTAFFID

                            WHERE (a.FDATE>='{sdt}')   --起始单据日期
                            AND (a.FDATE<='{edt}')     --结束单据日期
                            AND (x1.FNAME LIKE '%{firstcustomerName}%' or '{firstcustomerName}' is null)      --一级客户名称
                            AND (y1.FNAME LIKE '%'{seconCustomerName}'%' or '{seconCustomerName}' is null)      --二级客户名称
                            AND a.FDOCUMENTSTATUS='C'		--已审核
                            --a.FBILLNO IN('XSCKD082471','XSCKD082469')--'XSCKD082458'
                            ORDER BY x1.FNAME,y1.FNAME,a.FDATE
                        ";
            return _result;
        }


    }
}
