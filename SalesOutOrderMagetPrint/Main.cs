﻿using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using RD.UI;
using SalesOutOrderMagetPrint.Logic;
using Stimulsoft.Report;

namespace SalesOutOrderMagetPrint
{
    public partial class Main : Form
    {
        TaskLogic task=new TaskLogic();
        Load load=new Load();

        //记录需合并打印的Fid列表(如:'183711','183872')
        private string _fidlist;
        //记录需合并打印的单据名称列表
        private string _ordlist;

        //保存查询出来的GridView记录
        private DataTable _dtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;

        public Main()
        {
            InitializeComponent();
            OnRegisterEvents();
        }

        private void OnRegisterEvents()
        {
            btnsearch.Click += Btnsearch_Click;
            tmMerge.Click += TmMerge_Click;

            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.TextChanged += BnPositionItem_TextChanged;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel2.Visible = false;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                OnSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 合并打印单据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmMerge_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.Rows.Count==0) throw new Exception("没有内容,不能操作");
                //判断所选行是否超过6
                if(gvdtl.SelectedRows.Count>6) throw new Exception("所合并的单据行不能超过6行,请重新选择再进行合并打印");
                //获取并判断所选择的单据行中的“一级客户编号” 以及 “二级客户编号”是否一致
                if (!CheckSameCustomer(gvdtl.SelectedRows))throw new Exception("所选择的单据行中,‘一级客户编号’或‘二级客户编号’不一致,请重新选择再继续.");

                //开始执行运算
                task.TaskId = 1;
                task.Fidlist = _fidlist;
                task.Ordlist = _ordlist;

                new Thread(Start).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (task.ResultTable.Rows.Count == 0) throw new Exception("运算异常,请联系管理员");
                else
                {
                    var resultdt = task.ResultTable;

                    //调用STI模板并执行导出代码
                    //加载STI模板 SalesOutReport.mrt
                    var filepath = Application.StartupPath + "/Report/SalesOutReport.mrt";
                    var stireport = new StiReport();
                    stireport.Load(filepath);
                    //加载DATASET 或 DATATABLE
                    stireport.RegData("Order", resultdt);
                    stireport.Compile();
                    stireport.Show();   //调用预览功能
                    //执行对所选择进行打印的单据更新“打印日期”及“打印次数”
                    OnUpdate(_fidlist);
                    //完成后刷新“查询”按钮
                    OnSearch();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)
        /// </summary>
        private void Start()
        {
            task.StartTask();

            //当完成后将Form2子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrent = 1;
                bnMoveFirstItem.Enabled = false;
                bnMovePreviousItem.Enabled = false;

                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrent--;
                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrent++;
                bnMoveFirstItem.Enabled = true;
                bnMovePreviousItem.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrent = _totalpagecount;
                bnMoveNextItem.Enabled = false;
                bnMoveLastItem.Enabled = false;

                bnMovePreviousItem.Enabled = true;
                bnMoveFirstItem.Enabled = true;

                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItem_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItem.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItem.Text) > _totalpagecount) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItem.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrent = Convert.ToInt32(bnPositionItem.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;

                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;

                    bnMovePreviousItem.Enabled = true;
                    bnMoveFirstItem.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItem.Enabled = true;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrows_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChange = true;
                _pageCurrent = 1;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能
        /// </summary>
        private void GridViewPageChange()
        {
            try
            {
                //获取查询的总行数
                var dtltotalrows = _dtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrows.SelectedItem);
                //计算出总页数
                _totalpagecount = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItem.Text = $"/ {_totalpagecount} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChange)
                {
                    bnPositionItem.Text = Convert.ToString(1);                       //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
                    if (_totalpagecount == 1)
                    {
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnMoveNextItem.Enabled = false;
                        bnMoveLastItem.Enabled = false;
                        bnPositionItem.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItem.Enabled = true;
                        bnMoveLastItem.Enabled = true;
                        bnPositionItem.Enabled = true;                             //跳转页文本框
                    }
                    _pageChange = false;
                }

                //显示_dtl的查询总行数
                tstotalrow.Text = $"共 {_dtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvdtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 作用:1)判断所选择的“一级客户编号”及“二级客户编号”是否一致 2)循环获取FID值,用,分隔
        /// </summary>
        /// <returns></returns>
        private bool CheckSameCustomer(DataGridViewSelectedRowCollection rows)
        {
            var result = true;

            //一级客户编号
            var fcustcode = string.Empty;
            //二级客户编号
            var scustcode = string.Empty;

            try
            {
                //循环检查所选的单据行中‘一级客户编号’及‘二级客户编号’是否一致(注:行与行之间的比较)
                foreach (DataGridViewRow row in rows)
                {
                    //第一行(初始化)时,将第一行的相关值赋给对应的变量内
                    if (fcustcode == "")
                    {
                        fcustcode= Convert.ToString(row.Cells[3].Value);
                        scustcode = Convert.ToString(row.Cells[5].Value);
                        _fidlist = "'"+ Convert.ToString(row.Cells[0].Value)+ "'";
                        _ordlist = Convert.ToString(row.Cells[1].Value);
                    }
                    //从第二行开始判断是否值一致
                    else
                    {
                        if (fcustcode == Convert.ToString(row.Cells[3].Value) && scustcode == Convert.ToString(row.Cells[5].Value))
                        {
                            fcustcode = Convert.ToString(row.Cells[3].Value);
                            scustcode = Convert.ToString(row.Cells[5].Value);
                            _fidlist += ','+ "'"+ Convert.ToString(row.Cells[0].Value)+ "'";
                            _ordlist +=','+ Convert.ToString(row.Cells[1].Value);
                        }
                        //若循环发现不一致,即跳出循环并将相关变量值清空
                        else
                        {
                            result = false;
                            _fidlist = "";
                            _ordlist = "";
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            gvdtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 查询
        /// </summary>
        void OnSearch()
        {
            task.TaskId = 0;
            task.Fcustname = txtfcustname.Text;
            task.Scustname = txtscustname.Text;
            task.Sdt = dps.Value.Date;
            task.Edt = dpe.Value.Date;

            new Thread(Start).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.ResultTable.Rows.Count > 0)
            {
                _dtl = task.ResultTable;
                panel2.Visible = true;
                //初始化下拉框所选择的默认值
                tmshowrows.SelectedItem = Convert.ToInt32(tmshowrows.SelectedItem) == 0
                    ? (object)"10"
                    : Convert.ToInt32(tmshowrows.SelectedItem);
                //定义初始化标记
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvdtl.DataSource = task.ResultTable;
                panel2.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlGridViewisShow();
        }

        /// <summary>
        /// 更新
        /// </summary>
        bool OnUpdate(string fidlist)
        {
            task.TaskId = 2;
            task.Fidlist = fidlist;
            task.StartTask();
            return task.ResultMark;
        }

    }
}
