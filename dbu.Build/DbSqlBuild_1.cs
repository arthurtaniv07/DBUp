using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;

namespace dbu.Build
{
    public class DbSqlBuild_1
    {

        /// <summary>
        /// 组建页面底部的页码
        /// </summary>
        /// <param name="pageIndex">当前页的页码  从1开始</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="sumCount">总条数</param>
        /// <param name="pageCount">页码个数</param>
        /// <returns></returns>
        public static string BuildPageStr(int pageIndex, int pageSize, int sumCount, out int pageCount)
        {

            pageCount = sumCount % pageSize == 0 ? sumCount / pageSize : sumCount / pageSize + 1;


            //分页控件开始

            string temp = "<li class='{0}' onclick='javascript:{1};'><a href='javascript:;'>{2}</a></li>";
            StringBuilder pageStr = new StringBuilder();

            int start = 1;
            int end = 1;

            int showCount_left = 2;
            int left = 0;//左边的页码个数
            int right = 0;//右边的页码个数

            bool isLeft = false;//是否显示左边的页码个数  显示左边的...
            bool isRight = false;//是否显示右边的页码个数 显示右边的...
            if (pageCount > showCount_left * 2 + 1 + left + right)
            {
                if (pageIndex > left + showCount_left)
                {
                    isLeft = true;
                }
                if (pageIndex < pageCount - right - showCount_left)
                {
                    isRight = true;
                }
                start = pageIndex - showCount_left;
                end = pageIndex + showCount_left;
                if (pageIndex >= pageCount - showCount_left)
                {
                    start = pageCount - showCount_left * 2;
                    isRight = false;
                }
                if (pageIndex <= showCount_left)
                {
                    end = 1 + 2 * showCount_left;
                    isLeft = false;
                }

                if (end > pageCount) { end = pageCount; }
                if (start < 1) { start = 1; }
            }
            else
            {
                end = pageCount;
                start = 1;
            }




            pageStr.Append("<li class='prev " + (pageIndex == 1 ? "disabled" : "") + "' onclick='" + (pageIndex == 1 ? "" : "javascript:ToPrev();") + "'><a href='javascript:;'>上一页</a></li>");

            if (isLeft && left > 0)
            {
                for (int i = 1; i < left + 1; i++)
                {
                    pageStr.AppendFormat(temp, pageIndex == i ? "active" : "", string.Format("ToPageIndex({0})", i), i);
                }
                pageStr.Append("<li class='more '><a href='#'>...</a></li>");
            }

            for (int i = start; i < end + 1; i++)
            {
                pageStr.AppendFormat(temp, pageIndex == i ? "active" : "", string.Format("ToPageIndex({0})", i), i);
            }

            if (isRight && right > 0)
            {
                pageStr.Append("<li class='more '><a href='#'>...</a></li>");

                for (int i = pageCount - right + 1; i < pageCount + 1; i++)
                {
                    pageStr.AppendFormat(temp, pageIndex == i ? "active" : "", string.Format("ToPageIndex({0})", i), i);
                }
            }
            pageStr.Append("<li class='next " + (pageIndex == pageCount ? "disabled" : "") + "' onclick='" + (pageIndex == pageCount ? "" : "javascript:ToNext();") + "'><a href='javascript:;'>下一页</a></li>");
            return pageStr.ToString();
        }
        
        static StringBuilder sql = null;
        /// <summary>
        /// 获取分页sql  分页类型为maxId或者rownumber(效率：rowNumber3大于maxID大于notIn) 这依据primaryName而定  默认使用notin
        /// </summary>
        /// <param name="tabName">表名</param>
        /// <param name="queryStrings">查询的列名</param>
        /// <param name="primaryName">此项可为空  唯一、主键或索引列  如果此项为空则使用maxid方式进行分页</param>
        /// <param name="where">where条件  前后不加and和where关键字</param>
        /// <param name="pageSize">每页的数量</param>
        /// <param name="pageIndex">页码数</param>
        /// <param name="orderString">orderBy语句 不加order by关键字</param>
        /// <param name="tableSpecialColumn">单独查询的列 前面不需要加,</param>
        /// <param name="padType">分页方式 不区分大小写 rowNumber、maxID、notIn  如果为空 则以maxid进行分页 此项暂时未启用</param>
        /// <returns></returns>
        public static string GetPading(string tabName, string queryStrings, string primaryName, string where, int pageSize, int pageIndex, string orderString, string tableSpecialColumn = "", string padType = "notin")
        {
            sql = new StringBuilder();
            string padingType = "notin";

            //if (padType != null && padType.Trim() != "") { padingType = padType.Trim().ToLower();}
            if (!string.IsNullOrWhiteSpace(orderString)) { orderString = "order by " + orderString; }
            if (!string.IsNullOrWhiteSpace(where)) { where = "where " + where; }
            if (!string.IsNullOrWhiteSpace(tableSpecialColumn)) { tableSpecialColumn = "," + tableSpecialColumn; }
            int skip = pageSize * (pageIndex - 1);
            switch (padingType)
            {
                case "maxid":

                    //建议优化的时候，加上主键和索引，查询效率会提高。
                    //要分页的表显示的条数
                    sql.AppendFormat("select {0} {1}{2} from {3} ", pageSize > 0 ? "top " + pageSize : "", queryStrings, tableSpecialColumn, tabName);
                    sql.AppendFormat("{0} ", where);
                    if (skip != 0 && pageIndex > 0 && pageSize > 0)
                    {
                        sql.AppendFormat(string.IsNullOrWhiteSpace(where) ? " where " : " and ");
                        sql.AppendFormat("{0}>(select isnull(max({0}),0) from (select top {1} {0} from {2} {3} {4}) a) ", primaryName, skip, tabName, where, orderString);
                    }
                    sql.AppendFormat(orderString);

                    break;
                case "notin":

                    //建议优化的时候，加上主键和索引，查询效率会提高。
                    //要分页的表显示的条数
                    sql.AppendFormat("select {0} {1}{2} from {3} ", pageSize > 0 ? "top " + pageSize : "", queryStrings, tableSpecialColumn, tabName);
                    sql.AppendFormat("{0} ", where);
                    if (skip != 0 && pageIndex > 0 && pageSize > 0)
                    {
                        sql.AppendFormat(string.IsNullOrWhiteSpace(where) ? " where " : " and ");
                        sql.AppendFormat("{0} not in(select top {1} {0} from {2} {3} {4}) ", primaryName, skip, tabName, where, orderString);
                    }
                    sql.AppendFormat(orderString);

                    break;
                default:
                    break;
            }
            return sql.ToString();
        }


    }
}