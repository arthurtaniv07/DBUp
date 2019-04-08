using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dbu.Model;
using System.IO;
using System.Text.RegularExpressions;
using St.TemplateEngine;

namespace dbu.Build
{
    /// <summary>
    /// 生成文件时需要
    /// </summary>
    public class DBUFileHelper
    {
        public static List<StObject> StObjects_1 = null;

        #region 其他方法

        /// <summary>
        /// 将字符串的首字符大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="firstUpper"></param>
        /// <returns></returns>
        public static string firstUpperOrLower(string str, bool firstUpper = true)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Trim().Length == 1) { return str.ToUpper(); }
                else
                {
                    if (firstUpper) return str.Substring(0, 1).ToUpper() + str.Substring(1);
                    else return str.Substring(0, 1).ToLower() + str.Substring(1);
                }
            }
            return "";
        }


        /// <summary>
        /// 根据文件夹路径、表名、模型类型生成文件路径 
        /// </summary>
        /// <param name="dicPath"></param>
        /// <param name="tabName"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static string GetCSFilePath(string dicPath, string tabName, string modelName,string houzui=".cs")
        {
            string path = dicPath.Replace('/', '\\') + "\\" + firstUpperOrLower(tabName) + modelName + houzui;
            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}
            return path;
        }
        #endregion

        #region 生成实体类文件

        /// <summary>
        /// 生成实体类文件
        /// </summary>
        /// <param name="dicPath">文件夹路径</param>
        /// <param name="spaceName">命名空间</param>
        /// <param name="fss">表名称,数据库实体集合</param>
        /// <param name="serverName">服务器名称</param>
        /// <returns></returns>
        public static List<string> CreateModels(string dicPath, string spaceName, Dictionary<string, List<StObjField>> fss,string serverName)
        {
            try
            {

                List<string> res = new List<string>();

                List<StObjField> fs = null;
                foreach (string tabName in fss.Keys)
                {
                    string className = firstUpperOrLower(tabName);
                    fs = fss[tabName];
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.Text;");
                    //sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("namespace " + spaceName);
                    sb.AppendLine("{");
                    sb.AppendLine("\t[Serializable]");
                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine("\t/// " + fs[0].ObjDescribe);
                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine("\tpublic class " + className);
                    sb.AppendLine("\t{");
                    string t = "";

                    sb.AppendLine("\t\t#region 属性");
                    sb.AppendLine();
                    foreach (StObjField o in fs)
                    {
                        
                        string s = TypeManager.DBTypeToFType(o.ColTypeName);
                        t = firstUpperOrLower(o.ColName);
                        if (string.IsNullOrWhiteSpace(o.DefaultValue))
                        {
                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine(string.Format("\t\t/// {0}"
                                , (o.Describe + "").Length == 0 ? t + " 的描述" : o.Describe));
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine(string.Format("\t\tpublic {0} {1} {{ get; set; }}",
                         o.IsNull && !TypeManager.GetTypeIsNull(s.ToLower()) ? s + "?" : s, t));

                        }
                        else
                        {
                            string _fieldName = "_" + firstUpperOrLower(o.ColName, false);
                            string _fieldType = o.IsNull && !TypeManager.GetTypeIsNull(s.ToLower()) ? s + "?" : s;

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine(string.Format("\t\t/// {0}{1}"
                                , (o.Describe + "").Length == 0 ? t + " 的描述" : o.Describe,
                                (o.DefaultValue + "").Length == 0 ? "" : (o.Describe + "").Length == 0 ? "" : "  --  " + "默认 " + o.DefaultValue));
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return {2}; }} set {{ {2} = value; }} }}", _fieldType, t, _fieldName));
                            string dv = GetDefaultValue(o.DefaultValue, o.ColTypeName, serverName, o.ObjName, o.ColName);
                            sb.AppendLine(string.Format("\t\tprivate {0} {1} = {2};", _fieldType, _fieldName, dv));
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine("\t\t#endregion 属性  结束");
                    sb.AppendLine();

                    sb.AppendLine("\t\t#region 函数");
                    sb.AppendLine();
                    string classNameLow = className.ToLower();
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 深度克隆 " + fs[0].ObjDescribe);
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <returns>克隆后的对象</returns>");
                    sb.AppendLine("\t\tpublic " + className + " Clone()");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\t" + className + " c = new " + className + "();");
                    foreach (StObjField o in fs)
                    {
                        t = firstUpperOrLower(o.ColName);
                        sb.AppendLine("\t\t\tc." + t + " = this." + t + ";");
                    }
                    sb.AppendLine("\t\t\treturn c;");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion 函数  结束");
                    sb.AppendLine();

                    sb.AppendLine("\t}");
                    sb.AppendLine("}");
                    sb.AppendLine();
                    string filePath = GetCSFilePath(dicPath, tabName, "");
                    res.Add(filePath);
                    File.AppendAllText(filePath, sb.ToString());
                }

                return res;

            }
            catch (Exception e)
            {
                St.IO.StFileDebug.Log(e, St.IO.FileDebugPath.CSLog_yyMMdd);
                throw;
            }
        }
        #endregion


        #region 生成数据访问层文件 - DAL

        /*
        #region CommandText
        private const string selectAllText = "select * from Categories";
        private const string selectByKeyText = "select * from Categories where Id=@Id";

        private const string insertText = "insert into Categories(Name) values(@Name)";
        private const string updateByKeyText = "update Categories set Name=@Name where Id=@Id";

        private const string deleteByKeyText = "delete from Categories where Id=@Id";
        private const string deleteByKeyCheckedText = "delete from Categories where Id in ({0})";
        #endregion

        #region FindAllToIList
        public static IList<Categories> FindAllToIList()
        {
            return Find(selectAllText, CommandType.Text, null).Values.ToArray();
        }
        #endregion

        #region FindAllToDictionary
        public static Dictionary<int, Categories> FindAllToDictionary()
        {
            return Find(selectAllText, CommandType.Text, null);
        }
        #endregion

        #region FindByKey
        public static Categories FindByKey(Categories categories)
        {
            return Find(selectByKeyText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                }
                ).Values.FirstOrDefault();
        }
        #endregion

        #region FindByKey
        public static Categories FindByKey(int id)
        {
            return Find(selectByKeyText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                }).Values.FirstOrDefault();
        }
        #endregion

        #region Insert
        public static int Insert(Categories categories)
        {

            return DBHelper.ExecuteNonQuery(insertText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name}                }
            );

        }
        #endregion

        #region UpdateByKey
        public static int UpdateByKey(Categories categories)
        {
            return DBHelper.ExecuteNonQuery(updateByKeyText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name},
                    new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                }
            );
        }
        #endregion

        #region DeleteByCheckedKey
        public static int DeleteByCheckedKey(string checkedKey)
        {
            return DBHelper.ExecuteNonQuery(string.Format(deleteByKeyCheckedText, checkedKey), CommandType.Text, null);
        }
        #endregion

        #region DeleteByKey
        public static int DeleteByKey(Categories categories)
        {
            return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                });
        }
        #endregion

        #region DeleteByKey
        public static int DeleteByKey(int id)
        {
            return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                });
        }
        #endregion

        #region Find
        private static Dictionary<int, Categories> Find(string text, CommandType type, SqlParameter[] pars)
        {
            SqlDataReader dr = DBHelper.ExecuteReader(text, type, pars);
            Dictionary<int, Categories> categoriess = new Dictionary<int, Categories>();
            Categories categories = null;
            while (dr.Read())
            {
                categories = new Categories();
                if (!(dr["Id"] is DBNull))
                    categories.Id=Convert.ToInt32(dr["Id"].ToString());
                if (!(dr["Name"] is DBNull))
                    categories.Name=dr["Name"].ToString();



                categoriess.Add(categories.Id, categories);
            }
            return categoriess;
        }
        #endregion
            */

        /// <summary>
        /// 生成数据访问层文件
        /// </summary>
        /// <param name="dicPath">文件夹路径</param>
        /// <param name="spaceName">命名空间</param>
        /// <param name="fss">表名称,数据库实体集合</param>
        /// <returns></returns>
        public static List<string> CreateDAL(string dicPath, string spaceName, string modelNameSpace, Dictionary<string, List<StObjField>> fss)
        {
            try
            {

                List<string> res = new List<string>();

                List<StObjField> fs = null;
                foreach (string tabName in fss.Keys)
                {
                    string tab_firstUp = firstUpperOrLower(tabName);
                    string tab_firstLow = firstUpperOrLower(tabName, false);
                    string tab_lower = tabName.ToLower();

                    fs = fss[tabName];
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using System.Linq;");
                    sb.AppendLine("using System.Text;");
                    sb.AppendLine();
                    sb.AppendLine("using System.Data;");
                    sb.AppendLine("using System.Data.SqlClient;");
                    sb.AppendLine();
                    sb.AppendFormat("using {0};", modelNameSpace);
                    sb.AppendLine();
                    sb.AppendLine("using St.DataBase;");
                    sb.AppendLine();
                    sb.AppendLine("namespace " + spaceName);
                    sb.AppendLine("{");
                    sb.AppendLine("\tpublic class " + tab_firstUp + "DAL");
                    sb.AppendLine("\t{");

                    sb.AppendLine("\t\t");

                    //以@name的方式存储
                    List<string> _AT = new List<string>();
                    //以name的方式存储
                    List<string> _notAt = new List<string>();
                    //以name=@name的方式存储
                    List<string> _notAtAnd_at = new List<string>();
                    //主键名称
                    StObjField _primaryKey = new StObjField();
                    bool _primaryKeyIsExists = false;

                    //以name=@name的方式存储  非自增键  用于update
                    List<string> _notAtAnd_at_NotIdentity = new List<string>();
                    //以name的方式存储  非自增键  用于insert 
                    List<string> _notAt_NotIdentity = new List<string>();
                    //以@name的方式存储 非自增键  用于insert 
                    List<string> _AT_NotIdentity = new List<string>();


                    //唯一键列名
                    string _uniqueKeyName = "";
                    string _uniqueKeyType = "";
                    StObjField _uniqueKey = null;
                    //自增长列名
                    string _identityKeyName = "";
                    string _identityKeyType = "";
                    StObjField _identityKey = null;


                    string _notAtAnd_atStr = "{0}=@{0}";


                    foreach (var f in fs)
                    {
                        _notAt.Add(f.ColName);
                        _AT.Add("@" + f.ColName);
                        _notAtAnd_at.Add(string.Format(_notAtAnd_atStr, f.ColName));

                        if (f.IsPrimaryKey)
                        {
                            _primaryKey = f;
                            _primaryKeyIsExists = true;
                        }
                        if (f.IsUnique)
                        {
                            _uniqueKeyName = f.ColName;
                            _uniqueKeyType = TypeManager.DBTypeToFType(f.ColTypeName);
                            _uniqueKey = f;
                        }
                        if (f.IsIdentity)
                        {
                            _identityKeyName = f.ColName;
                            _identityKeyType = TypeManager.DBTypeToFType(f.ColTypeName);
                            _identityKey = f;
                        }
                        else if (!f.IsComputed)
                        {
                            _notAt_NotIdentity.Add(f.ColName);
                            _AT_NotIdentity.Add("@" + firstUpperOrLower(f.ColName));
                            _notAtAnd_at_NotIdentity.Add(string.Format(_notAtAnd_atStr, firstUpperOrLower(f.ColName)));
                        }
                    }

                    string title = "";
                    if (!_primaryKeyIsExists)
                    {
                        _primaryKey = fs.FirstOrDefault();
                        title = "By" + firstUpperOrLower(_primaryKey.ColName);
                    }


                    string selectAllText = "selectAllText";
                    string selectByKeyText = "selectByKeyText";
                    string selectByWhereText = "selectByWhereText";

                    string insertText = "insertText";
                    string updateByKeyText = "updateByKeyText";

                    string deleteByKeyText = "deleteByKeyText";
                    string deleteByKeyCheckedText = "deleteByKeyInText";

                    /*
                        private const string selectAllText = "select * from Categories";
                        private const string selectByKeyText = "select * from Categories where Id=@Id";

                        private const string insertText = "insert into Categories(Name) values(@Name)";
                        private const string updateByKeyText = "update Categories set Name=@Name where Id=@Id";

                        private const string deleteByKeyText = "delete from Categories where Id=@Id";
                        private const string deleteByKeyCheckedText = "delete from Categories where Id in ({0})";
                    */
                    //拼接sql语句
                    sb.AppendLine("\t\tprivate static DbHelper db=new DbHelper();");
                    sb.AppendLine();
                    sb.AppendLine("\t\tprivate const string tabName = \"" + tabName + "\";");


                    sb.AppendLine("\t\t#region CommandText");
                    sb.AppendLine("\t\tprivate const string " + selectAllText + " = \"select " + string.Join(",", _notAt) + " from " + tabName + "\";");
                    sb.AppendLine("\t\tprivate const string " + selectByKeyText + " = \"select " + string.Join(",", _notAt) + " from " + tabName + " where " + _primaryKey.ColName + "=@" + _primaryKey.ColName + "\";");
                    sb.AppendLine("\t\tprivate const string " + selectByWhereText + " = \"select " + string.Join(",", _notAt) + " from " + tabName + " {0}\";");
                    sb.AppendLine("\t\t");
                    sb.AppendLine("\t\tprivate const string " + insertText + " = \"insert into " + tabName + "(" + string.Join(", ", _notAt_NotIdentity) + ") values(" + string.Join(", ", _AT_NotIdentity) + ");select @@IDENTITY\";");
                    sb.AppendLine("\t\tprivate const string " + updateByKeyText + " = \"update  " + tabName + " set " + string.Join(", ", _notAtAnd_at_NotIdentity) + " where " + _primaryKey.ColName + "=@" + firstUpperOrLower(_primaryKey.ColName) + "\";");
                    sb.AppendLine();
                    sb.AppendLine("\t\tprivate const string " + deleteByKeyText + " = \"delete from " + tabName + " where " + string.Format("{0}=@{0}", _primaryKey.ColName) + "\";");
                    sb.AppendLine("\t\tprivate const string " + deleteByKeyCheckedText + " = \"delete from " + tabName + " where " + _primaryKey.ColName + " in ({0})\";");
                    sb.AppendLine("\t\t#endregion");
                    //拼接sql语句END


                    sb.AppendLine("\t\t");

                    string tab_firstLower = "_" + firstUpperOrLower(tabName, false);


                    if (fs[0].IsObjSelect)
                    {
                        #region 查询

                        /*
                #region FindAllToIList
                public static IList<Categories> FindAllToIList()
                {
                    return Find(selectAllText, CommandType.Text, null).Values.ToArray();
                }
                #endregion
                */
                        sb.AppendLine("\t\t#region GetModelList");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 获取全部数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <returns>全部数据</returns>");
                        sb.AppendLine("\t\tpublic static List<" + tab_firstUp + "> GetAllList()");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn Find(" + selectAllText + ", CommandType.Text, null);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        sb.AppendLine("\t\t#region GetModelList");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 查询指定条件的数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"where\">查询条件 不加where关键字</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static List<" + tab_firstUp + "> GetModelList(string where)");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn Find(string.Format(" + selectByWhereText + ",string.IsNullOrWhiteSpace(where)?\"\":\"where \"+where), CommandType.Text, null);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        /*
                        #region FindByKey
                        public static Categories FindByKey(int id)
                        {
                            return Find(selectByKeyText, CommandType.Text,
                                new SqlParameter[]{
                                    new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                                }).Values.FirstOrDefault();
                        }
                        #endregion*/
                        sb.AppendLine("\t\t#region GetModel");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// ");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">命名空间</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static " + tab_firstUp + " GetModel" + title + "(" + TypeManager.DBTypeToFType(_primaryKey.ColTypeName) + " " + firstUpperOrLower(_primaryKey.ColName, false) + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn Find(" + selectByKeyText + ", CommandType.Text, ");
                        sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                        sb.AppendLine("\t\t\t\t\tnew SqlParameter(\"@" + firstUpperOrLower(_primaryKey.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(_primaryKey.ColTypeName) + "," + _primaryKey.OldLength + "){Value=" + firstUpperOrLower(_primaryKey.ColName, false) + "}");
                        sb.AppendLine("\t\t\t\t}).FirstOrDefault();");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        /*
                private const string tabName = "Area";
                /// <summary>
                /// 使用PaddingType.NotIn的方式提取数据库数据
                /// </summary>
                /// <param name="where">查询条件 前面不加where关键字</param>
                /// <param name="take">要提取的数量</param>
                /// <param name="skip">要跳过的数量</param>
                /// <param name="orderString">额外查询的列</param>
                /// <param name="total">获取该条件差的总记录数</param>
                /// <param name="tableSpecialColumn">额外查询的列</param>
                /// <param name=""></param>
                /// <returns></returns>
                public static DataTable GetPageData_NotIn(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = "")
                {
                    total = -1;
                    string sql = db.GetPading(tabName, querystring, "id", where, take, skip, orderString, tableSpecialColumn, PaddingType.NotIn);
                    string c = string.Format("select count(*) from {0}{1}", tabName, string.IsNullOrWhiteSpace(where) ? "" : " where " + where);

                    DataSet ds = db.ExecuteDataSet(sql + ";" + c, CommandType.Text, null);
                    int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);

                    return ds.Tables[0];
                }*/
                        if (_primaryKey.IsIdentity || _primaryKey.IsPrimaryKey || _primaryKey.IsUnique)
                        {
                            string k = "";
                            //有这三者之一的方可使用 MaxID NotIn分页
                            k = _primaryKey.ColName;

                            sb.AppendLine("\t\t#region 函数 GetPageData_NotIn 使用PaddingType.NotIn的方式提取数据库数据");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 使用 PaddingType.NotIn 的方式提取数据库数据");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                            sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                            sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                            sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                            sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic static DataTable GetPageData_NotIn(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\ttotal = -1;");
                            sb.AppendLine("\t\t\tstring sql = db.GetPading(tabName, querystring, \"" + k + "\", where, take, skip, orderString, tableSpecialColumn, PaddingType.NotIn);");
                            sb.AppendLine("\t\t\tstring c = string.Format(\"select count(*) from {0}{1}\", tabName, string.IsNullOrWhiteSpace(where) ? \"\" : \" where \" + where);");
                            sb.AppendLine("\t\t\tDataSet ds = db.ExecuteDataSet(sql + \";\" + c, CommandType.Text, null);");
                            sb.AppendLine("\t\t\tint.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);");
                            sb.AppendLine("\t\t\treturn ds.Tables[0];");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");

                            sb.AppendLine("\t\t#region 函数 GetPageData_MaxID 使用PaddingType.MaxID的方式提取数据库数据");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 使用 PaddingType.MaxID 的方式提取数据库数据");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                            sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                            sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                            sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                            sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic static DataTable GetPageData_MaxID(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\ttotal = -1;");
                            sb.AppendLine("\t\t\tstring sql = db.GetPading(tabName, querystring, \"" + k + "\", where, take, skip, orderString, tableSpecialColumn, PaddingType.MaxID);");
                            sb.AppendLine("\t\t\tstring c = string.Format(\"select count(*) from {0}{1}\", tabName, string.IsNullOrWhiteSpace(where) ? \"\" : \" where \" + where);");
                            sb.AppendLine("\t\t\tDataSet ds = db.ExecuteDataSet(sql + \";\" + c, CommandType.Text, null);");
                            sb.AppendLine("\t\t\tint.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);");
                            sb.AppendLine("\t\t\treturn ds.Tables[0];");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }
                        sb.AppendLine("\t\t#region 函数 GetPageData_RowNumber3 使用PaddingType.RowNumber3的方式提取数据库数据");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 使用 PaddingType.RowNumber3 的方式提取数据库数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                        sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                        sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                        sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                        sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                        sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                        sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static DataTable GetPageData_RowNumber3(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\ttotal = -1;");
                        //RowNumber3方式不用穿主键或者唯一约束列名
                        sb.AppendLine("\t\t\tstring sql = db.GetPading(tabName, querystring, \"\", where, take, skip, orderString, tableSpecialColumn, PaddingType.RowNumber3);");
                        sb.AppendLine("\t\t\tstring c = string.Format(\"select count(*) from {0}{1}\", tabName, string.IsNullOrWhiteSpace(where) ? \"\" : \" where \" + where);");
                        sb.AppendLine("\t\t\tDataSet ds = db.ExecuteDataSet(sql + \";\" + c, CommandType.Text, null);");
                        sb.AppendLine("\t\t\tint.TryParse(ds.Tables[1].Rows[0][0].ToString(), out total);");
                        sb.AppendLine("\t\t\treturn ds.Tables[0];");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        #endregion
                    }


                    int m_i = 0;
                    if (fs[0].IsObjInsert)
                    {
                        #region 新增

                        /*
                    #region Insert
                    public static int Insert(Categories categories)
                    {

                        return DBHelper.ExecuteNonQuery(insertText, CommandType.Text,
                            new SqlParameter[]{
                                new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name}                }
                        );

                    }
                    #endregion*/

                        if (_identityKey != null)
                        {
                            sb.AppendLine("\t\t#region Add");
                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 向数据库插入一条新数据 返回插入的自增长值");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要插入的对象</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic static " + _identityKeyType + " Add(" + tab_firstUp + " " + tab_firstLower + ")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\tobject obj = db.ExecuteScalar(" + insertText + ", CommandType.Text, ");
                            sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                            m_i = 0;
                            foreach (var item in fs)
                            {
                                if (!item.IsIdentity && !item.IsComputed)
                                {
                                    sb.AppendLine("\t\t\t\t\t" + (m_i != 0 ? "," : "") + "new SqlParameter(\"@" + firstUpperOrLower(item.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(item.ColTypeName) + "," + item.OldLength + "){Value=" + tab_firstLower + "." + firstUpperOrLower(item.ColName) + "}");
                                    m_i = 1;
                                }
                            }
                            sb.AppendLine("\t\t\t\t});");
                            sb.AppendLine("\t\t\treturn obj is DBNull ? 0 : " + TypeManager.DBTypeToConvert(_identityKey.ColTypeName).Replace("##", "obj") + ";");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }
                        else
                        {
                            sb.AppendLine("\t\t#region Add");
                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 向数据库插入一条新数据 返回受影响行数");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要插入的对象</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic static int Add2(" + tab_firstUp + " " + tab_firstLower + ")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\treturn db.ExecuteNonQuery(" + insertText + ", CommandType.Text, ");
                            sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                            m_i = 0;
                            foreach (var item in fs)
                            {
                                if (!item.IsIdentity && !item.IsComputed)
                                {
                                    sb.AppendLine("\t\t\t\t\t" + (m_i != 0 ? "," : "") + "new SqlParameter(\"@" + firstUpperOrLower(item.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(item.ColTypeName) + "," + item.OldLength + "){Value=" + tab_firstLower + "." + firstUpperOrLower(item.ColName) + "}");
                                    m_i = 1;
                                }
                            }
                            sb.AppendLine("\t\t\t\t});");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }
                        #endregion
                    }

                    if (fs[0].IsObjUpdate)
                    {
                        #region 更新


                        /*

                #region UpdateByKey
                public static int UpdateByKey(Categories categories)
                {
                    return DBHelper.ExecuteNonQuery(updateByKeyText, CommandType.Text,
                        new SqlParameter[]{
                            new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name},
                            new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                        }
                    );
                }
                #endregion
                */

                        sb.AppendLine("\t\t#region 函数 Update 更新数据库的一条记录");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 更新数据库的一条记录");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要更新的对象</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static int Update(" + tab_firstUp + " " + tab_firstLower + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn db.ExecuteNonQuery(" + updateByKeyText + ", CommandType.Text, ");
                        sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                        m_i = 0;
                        foreach (var item in fs)
                        {
                            if (!item.IsIdentity && !item.IsComputed)
                            {
                                sb.AppendLine("\t\t\t\t\t" + (m_i != 0 ? "," : "") + "new SqlParameter(\"@" + firstUpperOrLower(item.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(item.ColTypeName) + "," + item.OldLength + "){Value=" + tab_firstLower + "." + firstUpperOrLower(item.ColName) + "}");
                                m_i = 1;

                            }
                        }
                        if (m_i > 0)
                        {
                            sb.AppendLine("\t\t\t\t\t,new SqlParameter(\"@" + firstUpperOrLower(_primaryKey.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(_primaryKey.ColTypeName) + "," + _primaryKey.OldLength + "){Value=" + tab_firstLower + "." + firstUpperOrLower(_primaryKey.ColName) + "}");
                        }
                        sb.AppendLine("\t\t\t\t});");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");
                        #endregion
                    }

                    if (fs[0].IsObjDelete)
                    {
                        #region 删除
                        /*
                    #region Delete
                    public static int Delete(string checkedKey)
                    {
                        return DBHelper.ExecuteNonQuery(string.Format(deleteByKeyCheckedText, checkedKey), CommandType.Text, null);
                    }
                    #endregion
                    */
                        string temp = "";
                        if (_primaryKey.IsPrimaryKey) { temp = "主键"; }
                        else if (_primaryKey.IsUnique) { temp = "唯一键"; }
                        else if (_primaryKey.IsIdentity) { temp = "标识列"; }

                        sb.AppendLine("\t\t#region 函数 Delete删除指定" + temp + "的数据");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定" + temp + "的数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + firstUpperOrLower(_primaryKey.ColName, false) + "\">要删除的 " + temp + " 集合，中间用 , 隔开 </param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static int Delete" + title + "s(string " + firstUpperOrLower(_primaryKey.ColName, false) + "s)");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn db.ExecuteNonQuery(string.Format(" + deleteByKeyCheckedText + "," + firstUpperOrLower(_primaryKey.ColName, false) + "s), CommandType.Text, null);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");



                        /*
                                #region DeleteByKey
                                public static int DeleteByKey(Categories categories)
                                {
                                    return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                                        new SqlParameter[]{
                                            new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                                        });
                                }
                                #endregion
                            */
                        sb.AppendLine("\t\t#region 函数 Delete 删除指定数据");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">要删除的对象  （通过" + temp + "）</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static int Delete(" + tab_firstUp + " " + tab_firstLower + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn db.ExecuteNonQuery(" + deleteByKeyText + ", CommandType.Text, ");
                        sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                        sb.AppendLine("\t\t\t\t\tnew SqlParameter(\"@" + firstUpperOrLower(_primaryKey.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(_primaryKey.ColTypeName) + "," + _primaryKey.OldLength + "){Value=" + tab_firstLower + "." + firstUpperOrLower(_primaryKey.ColName) + "}");
                        sb.AppendLine("\t\t\t\t});");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        /*
                                #region DeleteByKey
                                public static int DeleteByKey(int id)
                                {
                                    return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                                        new SqlParameter[]{
                                            new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                                        });
                                }
                                #endregion*/
                        sb.AppendLine("\t\t#region  函数 Delete 删除指定数据");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">" + temp + "</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static int Delete" + title + "(" + TypeManager.DBTypeToFType(_primaryKey.ColTypeName) + " " + _primaryKey.ColName + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn db.ExecuteNonQuery(" + selectByKeyText + ", CommandType.Text, ");
                        sb.AppendLine("\t\t\t\tnew SqlParameter[]{");
                        sb.AppendLine("\t\t\t\t\tnew SqlParameter(\"@" + firstUpperOrLower(_primaryKey.ColName) + "\",SqlDbType." + TypeManager.DBTypeToSqlDbType(_primaryKey.ColTypeName) + "," + _primaryKey.OldLength + "){Value=" + _primaryKey.ColName + "}");
                        sb.AppendLine("\t\t\t\t});");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");

                        sb.AppendLine("\t\t");
                        #endregion
                    }

                    if (fs[0].IsObjSelect)
                    {
                        #region 查询

                        /*
                #region Find
                private static Dictionary<int, Categories> Find(string text, CommandType type, SqlParameter[] pars)
                {
                    SqlDataReader dr = DBHelper.ExecuteReader(text, type, pars);
                    Dictionary<int, Categories> categoriess = new Dictionary<int, Categories>();
                    Categories categories = null;
                    while (dr.Read())
                    {
                        categories = new Categories();
                        if (!(dr["Id"] is DBNull))
                            categories.Id=Convert.ToInt32(dr["Id"].ToString());
                        if (!(dr["Name"] is DBNull))
                            categories.Name=dr["Name"].ToString();



                        categoriess.Add(categories.Id, categories);
                    }
                    return categoriess;
                }
                #endregion*/

                        sb.AppendLine("\t\t#region Find");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 通过指定文本查询结果");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"text\">要执行的命令或文本</param>");
                        sb.AppendLine("\t\t/// <param name=\"type\">执行的方式</param>");
                        sb.AppendLine("\t\t/// <param name=\"pars\">执行参数</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic static List<" + tab_firstUp + "> Find(string text,CommandType type,params SqlParameter[] pars)");
                        sb.AppendLine("\t\t{");
                        sb.AppendFormat("\t\t\tList<{0}> {1}s = new List<{0}>(); " + Environment.NewLine, tab_firstUp, tab_lower);
                        sb.AppendLine("\t\t\tDataSet ds = db.ExecuteDataSet(text, type, pars); ");
                        sb.AppendLine("\t\t\tif(ds == null || ds.Tables.Count == 0)");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendLine("\t\t\t\treturn new List<" + tab_firstUp + ">();");
                        sb.AppendLine("\t\t\t}");
                        sb.AppendFormat("\t\t\t{0}s = DataTableToList(ds.Tables[0]);" + Environment.NewLine, tab_lower);
                        sb.AppendLine(string.Format("\t\t\treturn {0}s; ", tab_lower));
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");
                        #endregion
                    }

                    #region DataTableToList

                    sb.AppendLine("\t\t#region DataTableToList");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将表转换为对象集合");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"dt\">要转换的表</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic static List<" + tab_firstUp + "> DataTableToList(DataTable dt)");
                    sb.AppendLine("\t\t{");
                    sb.AppendFormat("\t\t\tList<{0}> {1}s = new List<{0}>();" + Environment.NewLine, tab_firstUp, tab_lower);
                    sb.AppendFormat("\t\t\t{0} _{1} = null;" + Environment.NewLine, firstUpperOrLower(tabName), tab_lower);
                    sb.AppendLine("\t\t\tobject _temp = null;");
                    sb.AppendLine("\t\t\tDataColumnCollection dcs = dt.Columns;");
                    sb.AppendLine("\t\t\tforeach (DataRow item in dt.Rows)");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendFormat("\t\t\t\t_{1} = new {0}();" + Environment.NewLine, tab_firstUp, tab_lower);
                    string fType = "";

                    string _cols = "";
                    foreach (StObjField obj_f in fs)
                    {
                        _cols += string.Format(",\"{0}\"", obj_f.ColName.ToLower());
                        fType = TypeManager.DBTypeToFType(obj_f.ColTypeName);
                        //if (obj_f.ColName.ToLower() == "late10_hnumber" && 
                        //    tab_lower == "tms_attendanceimport") {
                        //    int a = 0;
                        //}

                        sb.AppendLine("\t\t\t\tif(dcs.Contains(\"" + obj_f.ColName.ToLower() + "\"))");
                        sb.AppendLine("\t\t\t\t{");
                        if (obj_f.IsNull)
                        {
                            sb.AppendLine("\t\t\t\t\t_temp = item[\"" + firstUpperOrLower(obj_f.ColName) + "\"];");
                            sb.AppendLine("\t\t\t\t\tif(!(_temp is DBNull))");
                            sb.AppendLine("\t\t\t\t\t{");
                            //程序允许为null
                            sb.AppendFormat("\t\t\t\t\t\t_{1}.{2} = {0};" + Environment.NewLine, TypeManager.DBTypeToConvert(obj_f.ColTypeName).Replace("##", "_temp"), tab_lower, firstUpperOrLower(obj_f.ColName));
                            sb.AppendLine("\t\t\t\t\t}");
                        }
                        else
                        {
                            sb.AppendFormat("\t\t\t\t\t_{1}.{2} = {0};" + Environment.NewLine, TypeManager.DBTypeToConvert(obj_f.ColTypeName).Replace("##", "item[\"" + obj_f.ColName + "\"]"), tab_lower, firstUpperOrLower(obj_f.ColName));
                        }
                        sb.AppendLine("\t\t\t\t}");
                    }

                    sb.AppendFormat("\t\t\t\t{0}s.Add(_{0});" + Environment.NewLine, tab_lower);
                    sb.AppendLine("\t\t\t}");
                    sb.AppendFormat("\t\t\treturn {0}s;" + Environment.NewLine, tab_lower);

                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");
                    sb.AppendLine("\t\t");
                    #endregion

                    _cols = _cols.Remove(0, 1);

                    sb.AppendLine("");
                    sb.AppendLine("\t\tprivate static List<string> cols = new List<string>() { " + _cols + " };");
                    sb.AppendLine("");

                    #region DataRowToModel

                    sb.AppendLine("\t\t#region DataRowToModel");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将数据行转换为对象");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"dt\">要转换的数据行</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic static " + tab_firstUp + " DataRowToModel(DataRow dr)");
                    sb.AppendLine("\t\t{");
                    sb.AppendFormat("\t\t\t{0} _{1} = null;" + Environment.NewLine, firstUpperOrLower(tabName), tab_lower);
                    sb.AppendLine("\t\t\tobject _temp = null;");
                    sb.AppendLine("\t\t\tDataColumnCollection dcs = dr.Table.Columns;");
                    sb.AppendFormat("\t\t\t_{1} = new {0}();" + Environment.NewLine, tab_firstUp, tab_lower);
                    foreach (StObjField obj_f in fs)
                    {
                        fType = TypeManager.DBTypeToFType(obj_f.ColTypeName);
                        //if (obj_f.ColName.ToLower() == "late10_hnumber" && 
                        //    tab_lower == "tms_attendanceimport") {
                        //    int a = 0;
                        //}
                        sb.AppendLine("\t\t\tif(dcs.Contains(\"" + obj_f.ColName.ToLower() + "\"))");
                        sb.AppendLine("\t\t\t{");
                        if (obj_f.IsNull)
                        {
                            sb.AppendLine("\t\t\t\t_temp = dr[\"" + firstUpperOrLower(obj_f.ColName) + "\"];");
                            sb.AppendLine("\t\t\t\tif(!(_temp is DBNull))");
                            sb.AppendLine("\t\t\t\t{");
                            //程序允许为null
                            sb.AppendFormat("\t\t\t\t\t_{1}.{2} = {0};" + Environment.NewLine, TypeManager.DBTypeToConvert(obj_f.ColTypeName).Replace("##", "_temp"), tab_lower, firstUpperOrLower(obj_f.ColName));
                            sb.AppendLine("\t\t\t\t}");
                        }
                        else
                        {
                            sb.AppendFormat("\t\t\t\t_{1}.{2} = {0};" + Environment.NewLine, TypeManager.DBTypeToConvert(obj_f.ColTypeName).Replace("##", "dr[\"" + obj_f.ColName + "\"]"), tab_lower, firstUpperOrLower(obj_f.ColName));
                        }
                        sb.AppendLine("\t\t\t}");
                    }

                    sb.AppendFormat("\t\t\treturn _{0};" + Environment.NewLine, tab_lower);

                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");
                    sb.AppendLine("\t\t");
                    #endregion



                    #region ListToDataTable
                    //System.Data.DataColumn;
                    //System.Data.DataRow;
                    sb.AppendLine("\t\t#region ListToDataTable");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将对象集合转换为表");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"" + tab_lower + "s\">要转换的对象集合</param>");
                    sb.AppendLine("\t\t/// <param name=\"IsIdentity\">是否加入自增列 默认不添加</param>");
                    sb.AppendLine("\t\t/// <param name=\"IsComputed\">是否加入计算列 默认不添加</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic static DataTable ListToDataTable(List<" + tab_firstUp + "> " + tab_lower + "s, bool IsIdentity = false, bool IsComputed = false)");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\tDataTable dt = new DataTable();");
                    sb.AppendLine("\t\t\tList<DataColumn> _cols = new List<DataColumn>();");

                    sb.AppendLine("\t\t\t");
                    sb.AppendLine("\t\t\t//添加列");
                    int inx = -1;
                    foreach (StObjField obj_f in fs)
                    {
                        fType = TypeManager.DBTypeToFType(obj_f.ColTypeName);
                        if (obj_f.IsIdentity)
                        {
                            sb.AppendLine("\t\t\tif(IsIdentity)");
                            sb.AppendLine("\t\t\t\t_cols.Add(new DataColumn() { ColumnName = \"" + obj_f.ColName.ToLower() + "\", DataType = typeof(" + fType + ") });");
                        }
                        else if (obj_f.IsComputed)
                        {
                            sb.AppendLine("\t\t\tif(IsComputed)");
                            sb.AppendLine("\t\t\t\t_cols.Add(new DataColumn() { ColumnName = \"" + obj_f.ColName.ToLower() + "\", DataType = typeof(" + fType + ") });");
                        }
                        else
                        {
                            sb.AppendLine("\t\t\t_cols.Add(new DataColumn() { ColumnName = \"" + obj_f.ColName.ToLower() + "\", DataType = typeof(" + fType + ") });");
                        }
                    }
                    sb.AppendLine("\t\t\tdt.Columns.AddRange(_cols.ToArray());");
                    inx = -1;

                    sb.AppendLine("\t\t\t");
                    sb.AppendLine("\t\t\t//添加行");
                    sb.AppendLine("\t\t\tDataRow _row = null;");
                    sb.AppendLine("\t\t\tforeach (" + tab_firstUp + " item in " + tab_lower + "s)");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendLine("\t\t\t\t_row = dt.NewRow();");
                    foreach (StObjField obj_f in fs)
                    {
                        if (obj_f.IsIdentity)
                        {
                            sb.AppendLine("\t\t\t\tif(IsIdentity)");
                            sb.AppendLine("\t\t\t\t\t_row[\"" + obj_f.ColName.ToLower() + "\"] = item." + firstUpperOrLower(obj_f.ColName) + ";");
                        }
                        else if (obj_f.IsComputed)
                        {
                            sb.AppendLine("\t\t\t\tif(IsComputed)");
                            sb.AppendLine("\t\t\t\t\t_row[\"" + obj_f.ColName.ToLower() + "\"] = item." + firstUpperOrLower(obj_f.ColName) + ";");
                        }
                        else
                        {
                            sb.AppendLine("\t\t\t\t_row[\"" + obj_f.ColName.ToLower() + "\"] = item." + firstUpperOrLower(obj_f.ColName) + ";");
                        }
                    }
                    sb.AppendLine("\t\t\t\tdt.Rows.Add(_row);");
                    sb.AppendLine("\t\t\t}");


                    sb.AppendLine("\t\t\treturn dt;");

                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");
                    sb.AppendLine("\t\t");
                    #endregion



                    //string t = "";
                    //foreach (StObjField o in fs)
                    //{
                    //    t = firstUpper(o.ColName);
                    //}

                    sb.AppendLine("\t}");
                    sb.AppendLine("}");



                    sb.AppendLine();
                    string filePath = GetCSFilePath(dicPath, tabName, "DAL");
                    res.Add(filePath);
                    File.AppendAllText(filePath, sb.ToString());
                }

                return res;

            }
            catch (Exception e)
            {
                St.IO.StFileDebug.Log(e, St.IO.FileDebugPath.CSLog_yyMMdd);
                throw;
            }
        }
        #endregion


        static bool IsStatic = true;
        static string StaticText = IsStatic ? " static" : "";

        #region 生成数据访问层文件 - BLL

        /// <summary>
        /// 生成数据访问层文件
        /// </summary>
        /// <param name="dicPath">文件夹路径</param>
        /// <param name="spaceName">命名空间</param>
        /// <param name="fss">表名称,数据库实体集合</param>
        /// <returns></returns>
        public static List<string> CreateBLL(string dicPath, string spaceName, string modelNameSpace, string dalNameSpace, Dictionary<string, List<StObjField>> fss)
        {
            try
            {

                List<string> res = new List<string>();

                List<StObjField> fs = null;
                foreach (string tabName in fss.Keys)
                {
                    string tab_firstUp = firstUpperOrLower(tabName);
                    string tab_lower = tabName.ToLower();
                    fs = fss[tabName];
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using System.Linq;");
                    sb.AppendLine("using System.Text;");
                    sb.AppendLine();
                    sb.AppendLine("using System.Data;");
                    sb.AppendLine("using System.Data.SqlClient;");
                    sb.AppendLine();
                    sb.AppendFormat("using {0};", modelNameSpace);
                    sb.AppendLine();
                    sb.AppendFormat("using {0};", dalNameSpace);
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("namespace " + spaceName);
                    sb.AppendLine("{");
                    sb.AppendLine("\tpublic class " + tab_firstUp + "BLL");
                    sb.AppendLine("\t{");

                    sb.AppendLine("\t\t");

                    //以@name的方式存储
                    List<string> _AT = new List<string>();
                    //以name的方式存储
                    List<string> _notAt = new List<string>();
                    //以name=@name的方式存储
                    List<string> _notAtAnd_at = new List<string>();
                    //主键名称
                    StObjField _primaryKey = new StObjField();
                    bool _primaryKeyIsExists = false;

                    //以name=@name的方式存储  非自增键
                    List<string> _notAtAnd_at_NotIdentity = new List<string>();
                    //以name的方式存储  非自增键
                    List<string> _notAt_NotIdentity = new List<string>();
                    //以@name的方式存储 非自增键
                    List<string> _AT_NotIdentity = new List<string>();


                    //唯一键列名
                    string _uniqueKeyName = "";
                    string _uniqueKeyType = "";
                    StObjField _uniqueKey = null;
                    //自增长列名
                    string _identityKeyName = "";
                    string _identityKeyType = "";
                    StObjField _identityKey = null;


                    string _notAtAnd_atStr = "{0}=@{0}";


                    foreach (var f in fs)
                    {
                        _notAt.Add(f.ColName);
                        _AT.Add("@" + f.ColName);
                        _notAtAnd_at.Add(string.Format(_notAtAnd_atStr, f.ColName));

                        if (f.IsPrimaryKey)
                        {
                            _primaryKey = f;
                            _primaryKeyIsExists = true;
                        }
                        if (f.IsUnique)
                        {
                            _uniqueKeyName = f.ColName;
                            _uniqueKeyType = TypeManager.DBTypeToFType(f.ColTypeName);
                            _uniqueKey = f;
                        }
                        if (f.IsIdentity)
                        {
                            _identityKeyName = f.ColName;
                            _identityKeyType = TypeManager.DBTypeToFType(f.ColTypeName);
                            _identityKey = f;
                        }
                        else if (!f.IsComputed)
                        {
                            _notAt_NotIdentity.Add(f.ColName);
                            _AT_NotIdentity.Add("@" + firstUpperOrLower(f.ColName));
                            _notAtAnd_at_NotIdentity.Add(string.Format(_notAtAnd_atStr, firstUpperOrLower(f.ColName)));
                        }
                    }

                    string title = "";
                    if (!_primaryKeyIsExists)
                    {
                        _primaryKey = fs.FirstOrDefault();
                        title = "By" + firstUpperOrLower(_primaryKey.ColName);
                    }


                    string dalName = firstUpperOrLower(tabName) + "DAL";
                    //sb.AppendLine(string.Format("\t\tprivate static {0}DAL {1}=new {0}DAL();",firstUpperOrLower(tabName),dalName));
                    //拼接sql语句END


                    sb.AppendLine("\t\t");

                    if (fs[0].IsObjInsert)
                    {
                        #region 查询

                        /*
                #region FindAllToIList
                public static IList<Categories> FindAllToIList()
                {
                    return Find(selectAllText, CommandType.Text, null).Values.ToArray();
                }
                #endregion
                */
                        sb.AppendLine("\t\t#region GetModelList");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 获取全部数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <returns>全部数据</returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " List<" + tab_firstUp + "> GetAllList()");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".GetAllList();");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        sb.AppendLine("\t\t#region GetModelList");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 查询指定条件的数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"where\">查询条件 不加where关键字</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " List<" + tab_firstUp + "> GetModelList(string where)");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".GetModelList(where);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        /*
                        #region FindByKey
                        public static Categories FindByKey(int id)
                        {
                            return Find(selectByKeyText, CommandType.Text,
                                new SqlParameter[]{
                                    new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                                }).Values.FirstOrDefault();
                        }
                        #endregion*/
                        sb.AppendLine("\t\t#region GetModel");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// ");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">命名空间</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " " + tab_firstUp + " GetModel" + title + "(" + TypeManager.DBTypeToFType(_primaryKey.ColTypeName) + " " + firstUpperOrLower(_primaryKey.ColName, false) + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".GetModel" + title + "(" + firstUpperOrLower(_primaryKey.ColName, false) + ");");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");





                        if (_primaryKey.IsIdentity || _primaryKey.IsPrimaryKey || _primaryKey.IsUnique)
                        {
                            string k = "";
                            //有这三者之一的方可使用 MaxID NotIn分页
                            k = _primaryKey.ColName;

                            sb.AppendLine("\t\t#region 函数 GetPageData_NotIn 使用PaddingType.NotIn的方式提取数据库数据");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 使用 PaddingType.NotIn 的方式提取数据库数据");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                            sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                            sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                            sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                            sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic " + StaticText + " DataTable GetPageData_NotIn(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\ttotal = -1;");
                            sb.AppendLine("\t\t\treturn " + dalName + ".GetPageData_NotIn( querystring , where, take, skip, orderString, out total, tableSpecialColumn);");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");

                            sb.AppendLine("\t\t#region 函数 GetPageData_MaxID 使用PaddingType.MaxID的方式提取数据库数据");

                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 使用 PaddingType.MaxID 的方式提取数据库数据");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                            sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                            sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                            sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                            sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                            sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic " + StaticText + " DataTable GetPageData_MaxID(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\ttotal = -1;");
                            sb.AppendLine("\t\t\treturn " + dalName + ".GetPageData_MaxID( querystring , where, take, skip, orderString, out total, tableSpecialColumn);");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }
                        sb.AppendLine("\t\t#region 函数 GetPageData_RowNumber3 使用PaddingType.NotIn的方式提取数据库数据");

                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 使用 PaddingType.RowNumber3 的方式提取数据库数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"querystring\">要查询的列</param>");
                        sb.AppendLine("\t\t/// <param name=\"where\">查询条件 前面不加where关键字</param>");
                        sb.AppendLine("\t\t/// <param name=\"take\">要提取的数量</param>");
                        sb.AppendLine("\t\t/// <param name=\"skip\">要跳过的数量</param>");
                        sb.AppendLine("\t\t/// <param name=\"orderString\">排序字段  前面不加order by</param>");
                        sb.AppendLine("\t\t/// <param name=\"total\">获取该条件差的总记录数</param>");
                        sb.AppendLine("\t\t/// <param name=\"tableSpecialColumn\">额外查询的列</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " DataTable GetPageData_RowNumber3(string querystring , string where, int take, int skip, string orderString, out int total, string tableSpecialColumn = \"\")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\ttotal = -1;");
                        sb.AppendLine("\t\t\treturn " + dalName + ".GetPageData_RowNumber3( querystring , where, take, skip, orderString, out total, tableSpecialColumn);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        #endregion
                    }

                    int m_i = 0;
                    string tab_firstLower = "_" + firstUpperOrLower(tabName, false);
                    if (fs[0].IsObjInsert)
                    {
                        #region 新增
                        /*
                    #region Insert
                    public static int Insert(Categories categories)
                    {

                        return DBHelper.ExecuteNonQuery(insertText, CommandType.Text,
                            new SqlParameter[]{
                                new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name}                }
                        );

                    }
                    #endregion*/


                        if (_identityKey != null)
                        {
                            sb.AppendLine("\t\t#region Add");
                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 向数据库插入一条新数据 返回插入的自增长值");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要插入的对象</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic " + StaticText + " " + _identityKeyType + " Add(" + tab_firstUp + " " + tab_firstLower + ")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\treturn " + dalName + ".Add(" + tab_firstLower + ");");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }
                        else
                        {
                            sb.AppendLine("\t\t#region Add");
                            sb.AppendLine("\t\t/// <summary>");
                            sb.AppendLine("\t\t/// 向数据库插入一条新数据 返回受影响行数");
                            sb.AppendLine("\t\t/// </summary>");
                            sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要插入的对象</param>");
                            sb.AppendLine("\t\t/// <returns></returns>");
                            sb.AppendLine("\t\tpublic " + StaticText + " int Add2(" + tab_firstUp + " " + tab_firstLower + ")");
                            sb.AppendLine("\t\t{");
                            sb.AppendLine("\t\t\treturn " + dalName + ".Add2(" + tab_firstLower + ");");
                            sb.AppendLine("\t\t}");
                            sb.AppendLine("\t\t#endregion");
                            sb.AppendLine("\t\t");
                        }

                        #endregion
                    }

                    if (fs[0].IsObjUpdate)
                    {
                        #region 更新

                        /*

                #region UpdateByKey
                public static int UpdateByKey(Categories categories)
                {
                    return DBHelper.ExecuteNonQuery(updateByKeyText, CommandType.Text,
                        new SqlParameter[]{
                            new SqlParameter("@Name",SqlDbType.NVarChar,400){Value=categories.Name},
                            new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                        }
                    );
                }
                #endregion
                */

                        sb.AppendLine("\t\t#region Update");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 更新数据库的一条记录");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + tab_firstLower + "\">需要更新的对象</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " int Update(" + tab_firstUp + " " + tab_firstLower + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".Update(" + tab_firstLower + ");");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        #endregion
                    }

                    if (fs[0].IsObjDelete)
                    {
                        #region 删除

                        /*
                    #region Delete
                    public static int Delete(string checkedKey)
                    {
                        return DBHelper.ExecuteNonQuery(string.Format(deleteByKeyCheckedText, checkedKey), CommandType.Text, null);
                    }
                    #endregion
                    */
                        string temp = "";
                        if (_primaryKey.IsPrimaryKey) { temp = "主键"; }
                        else if (_primaryKey.IsUnique) { temp = "唯一键"; }
                        else if (_primaryKey.IsIdentity) { temp = "标识列"; }

                        sb.AppendLine("\t\t#region Delete");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定" + temp + "的数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + firstUpperOrLower(_primaryKey.ColName, false) + "\">要删除的 " + temp + " 集合，中间用 , 隔开 </param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " int Delete" + title + "s(string " + firstUpperOrLower(_primaryKey.ColName, false) + "s)");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".Delete" + title + "s(" + firstUpperOrLower(_primaryKey.ColName, false) + "s);");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");
                        /*
                                #region DeleteByKey
                                public static int DeleteByKey(Categories categories)
                                {
                                    return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                                        new SqlParameter[]{
                                            new SqlParameter("@Id",SqlDbType.Int,4){Value=categories.Id}
                                        });
                                }
                                #endregion
                            */
                        sb.AppendLine("\t\t#region Delete");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">要删除的对象  （通过" + temp + "）</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " int Delete(" + tab_firstUp + " " + tab_firstLower + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".Delete(" + tab_firstLower + ");");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");

                        /*
                                #region DeleteByKey
                                public static int DeleteByKey(int id)
                                {
                                    return DBHelper.ExecuteNonQuery(deleteByKeyText, CommandType.Text,
                                        new SqlParameter[]{
                                            new SqlParameter("@Id",SqlDbType.Int,4){Value=id}
                                        });
                                }
                                #endregion*/
                        sb.AppendLine("\t\t#region Delete");
                        sb.AppendLine("\t\t/// <summary>");
                        sb.AppendLine("\t\t/// 删除指定数据");
                        sb.AppendLine("\t\t/// </summary>");
                        sb.AppendLine("\t\t/// <param name=\"" + _primaryKey.ColName + "\">" + temp + "</param>");
                        sb.AppendLine("\t\t/// <returns></returns>");
                        sb.AppendLine("\t\tpublic " + StaticText + " int Delete" + title + "(" + TypeManager.DBTypeToFType(_primaryKey.ColTypeName) + " " + _primaryKey.ColName + ")");
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\treturn " + dalName + ".Delete" + title + "(" + _primaryKey.ColName + ");");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine("\t\t#endregion");
                        sb.AppendLine("\t\t");
                        #endregion
                    }

                    sb.AppendLine("\t\t#region DataTableToList");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将表转换为对象集合");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"dt\">要转换的表</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic " + StaticText + " List<" + tab_firstUp + "> DataTableToList(DataTable dt)");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\treturn " + dalName + ".DataTableToList(dt);");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");

                    sb.AppendLine("\t\t");
                    sb.AppendLine("\t\t#region DataTableToList");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将数据行转换为对象");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"dt\">要转换的数据行</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic " + StaticText + " " + tab_firstUp + " DataRowToModel(DataRow dr)");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\treturn " + dalName + ".DataRowToModel(dr);");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");
                    sb.AppendLine("\t\t");




                    #region ListToDataTable

                    sb.AppendLine("\t\t#region ListToDataTable");
                    sb.AppendLine("\t\t/// <summary>");
                    sb.AppendLine("\t\t/// 将对象集合转换为表");
                    sb.AppendLine("\t\t/// </summary>");
                    sb.AppendLine("\t\t/// <param name=\"" + tab_lower + "s\">要转换的对象集合</param>");
                    sb.AppendLine("\t\t/// <param name=\"IsIdentity\">是否加入自增列 默认不添加</param>");
                    sb.AppendLine("\t\t/// <param name=\"IsComputed\">是否加入计算列 默认不添加</param>");
                    sb.AppendLine("\t\t/// <returns></returns>");
                    sb.AppendLine("\t\tpublic " + StaticText + " DataTable ListToDataTable(List<" + tab_firstUp + "> " + tab_lower + "s, bool IsIdentity = false, bool IsComputed = false)");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\treturn " + dalName + ".ListToDataTable(" + tab_lower + "s,IsIdentity,IsComputed);");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\t#endregion");
                    sb.AppendLine("\t\t");
                    #endregion

                    sb.AppendLine("\t}");
                    sb.AppendLine("}");



                    sb.AppendLine();
                    string filePath = GetCSFilePath(dicPath, tabName, "BLL");
                    res.Add(filePath);
                    File.AppendAllText(filePath, sb.ToString());
                }

                return res;
            }
            catch (Exception e)
            {
                St.IO.StFileDebug.Log(e, St.IO.FileDebugPath.CSLog_yyMMdd);
                throw;
            }
        }

        #endregion


        #region 生成HTML文件
        /// <summary>
        /// 生成HTML文件
        /// </summary>
        /// <param name="dicPath"></param>
        /// <param name="fss"></param>
        public static void CreateHTML(string dicPath, Dictionary<string, List<StObjField>> fss)
        {

            StObjField f = new StObjField();
            string tmepStr = St.IO.StFile.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Templates/html_all.txt", FileMode.Open, Encoding.Default);
            TemplateManager tm = TemplateManager.FromString(tmepStr);
            tm.SetValue("tabs", fss);
            tm.Functions.Add("My_GetValue", new TemplateFunction(GetValue));

            string output = tm.Process();
            // 删除空行
            //output = Regex.Replace(output, @"\n\s+\n", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string filePath = DBUFileHelper.GetCSFilePath(dicPath, ".ALL", "", ".html");
            File.AppendAllText(filePath, output, Encoding.UTF8);

            string tmepStr2 = St.IO.StFile.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Templates/html_model.txt", FileMode.Open, Encoding.Default);
            TemplateManager tm2 = null;
            foreach (string tabName in fss.Keys)
            {
                tm2 = TemplateManager.FromString(tmepStr2);
                tm2.SetValue("title", tabName);
                tm2.SetValue("fields", fss[tabName]);

                tm2.Functions.Add("My_GetValue", new TemplateFunction(GetValue));

                string output2 = tm2.Process();
                // 删除空行

                string filePath2 = DBUFileHelper.GetCSFilePath(dicPath, tabName, "", ".html");
                File.AppendAllText(filePath2, output2, Encoding.UTF8);
            }

        }

        private static object GetValue(object[] args)
        {
            object col = args[0];
            if (col == null) { return ""; }
            Type t = col.GetType();
            if (t.IsPrimitive) { return t + ""; }

            return col + "";
        }

        #endregion


        #region 生成SQL文件
        /// <summary>
        /// 获取默认值SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetDefaultSQL(object[] args)
        {
            string tabName = args[0] + "";
            string rel = "";
            StObjField f = args[1] as StObjField;
            if (f == null) { return ""; }
            if (f.DefaultValue == null) { return ""; }

            rel += string.Format(@"ALTER TABLE [dbo].[{0}] ADD  CONSTRAINT [DF_{0}_{1}]  DEFAULT {2} FOR [{1}]
GO", tabName, f.ColName, f.DefaultValue);
            return rel == "" ? "" : "\r\n" + rel + "\r\n";
        }
        /// <summary>
        /// 获取说明SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetDescribeSQL(object[] args)
        {
            string tabName = args[0] + "";
            string rel = "";
            StObjField f = args[1] as StObjField;
            if (f == null) { return ""; }
            if (f.Describe == null) { return ""; }

            rel += string.Format(@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{2}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'{1}'
GO", tabName, f.ColName, f.Describe);
            return rel == "" ? "" : "\r\n" + rel + "\r\n";
        }
        /// <summary>
        /// 获取说明SQL
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetCHECKSQL(object[] args)
        {
            string tabName = args[0] + "";
            string rel = "";
            List<StObject> ches = StObjects_1.Where(i => i.ObjName == tabName).ToList();
            if (ches.Count == 0) { return ""; }
            foreach (StObject item in ches)
            {
                rel += string.Format(@"
alter table [{0}] add constraint [{1}] check{2}
go
", tabName, item.Name, item.CreateText);
            }
            return rel.Trim('\r', '\n');
        }
        /// <summary>
        /// 获取主键列
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string DB_GetPrimaryKeyFields(object[] args)
        {
            List<StObjField> f = args[0] as List<StObjField>;
            if (f == null) { return ""; }
            var primarys = f.Where(i => i.IsPrimaryKey).Select(i => i.ColName);
            if (primarys.Count() == 0) { return ""; }
            return string.Format("primary key({0})", string.Join(",", primarys));
        }
        private static string DB_GetFieldStr(object[] args)
        {
            //[{$field.ColName}] [{$field.ColTypeName}]{$iif(field.IsIdentity," IDENTITY(1,1) ","")} {$iif(field.IsNull,"NULL","NOT NULL")}（）
            StObjField f = args[0] as StObjField;
            if (f == null) { return ""; }
            StringBuilder rel = new StringBuilder();
            rel.AppendFormat("[{0}]", f.ColName);
            if (f.IsComputed)
            {
                rel.AppendFormat(" as {0}", f.ComputedText);
                return rel.ToString();
            }
            rel.AppendFormat(" [{0}]", f.ColTypeName);
            switch (f.ColTypeName.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "time":
                case "varbinary":
                case "datetime2":
                case "binary":
                case "datetimeoffset":
                    rel.AppendFormat("({0})", f.OldLength < 0 ? "MAX" : "" + f.OldLength);
                    break;
                case "numeric":
                case "decimal":
                    rel.AppendFormat("({0},{1})", f.OldLength, f.Scale);
                    break;
                default:
                    break;
            }
            if (f.IsIdentity)
                rel.AppendFormat(" IDENTITY({0},{1})", f.IDENTITY_SEED, f.IDENTITY_INCREASE);
            if (f.IsUnique)
            {
                rel.Append(" UNIQUE");
            }
            rel.Append(f.IsNull ? " NULL" : " NOT NULL");
            return rel.ToString();
        }
        /// <summary>
        /// 生成外键SQL  未完成
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static object DB_GetFOREIGNKEYSQL(object[] args)
        {
            StObjField f = args[1] as StObjField;
            if (f == null) { return ""; }
            if (string.IsNullOrWhiteSpace(f.PrientTableName)) { return ""; }
            string tabName = args[0] + "";
            string rel = "";

            Dictionary<string, List<StObjField>> fss = args[2] as Dictionary<string, List<StObjField>>;
            if (!fss.ContainsKey(f.PrientTableName)) { return string.Format("--表【{0}】没在生成的所有表中", f.PrientTableName); }
            //if (fss == null) { return ""; }
            StObjField f_pk = fss[f.PrientTableName].Where(i => f.PrientTableID == i.ObjID && f.ParentColID == i.ColID).FirstOrDefault();
            //if (f_pk == null) { return ""; }



            //            rel += string.Format(@"ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{2}] FOREIGN KEY([{1}])
            //REFERENCES [dbo].[{2}] ([ID])
            //GO
            //ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_{2}]
            //GO", tabName, f.ColName, f.PrientTableName, f_pk.ColName);
            rel += string.Format(@"ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{2}] FOREIGN KEY([{1}])
REFERENCES [dbo].[{2}] ([{3}])
GO", tabName, f.ColName, f.PrientTableName, f_pk.ColName);
            return rel == "" ? "" : "\r\n" + rel + "\r\n";
        }
        /// <summary>
        /// 生成SQL文件
        /// </summary>
        /// <param name="dicPath"></param>
        /// <param name="dbName"></param>
        /// <param name="fss"></param>
        public static void CreateSQL(string dicPath,string dbName, Dictionary<string, List<StObjField>> fss)
        {

            StObjField f = new StObjField();
            string tmepStr = St.IO.StFile.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Templates/sql_all.txt", FileMode.Open, Encoding.Default);
            TemplateManager tm = TemplateManager.FromString(tmepStr);

            tm.SetValue("dbName", dbName);
            tm.SetValue("tabs", fss);

            tm.Functions.Add("My_GetValue", new TemplateFunction(GetValue));
            tm.Functions.Add("DB_GetDefaultSQL", new TemplateFunction(DB_GetDefaultSQL));
            tm.Functions.Add("DB_GetDescribeSQL", new TemplateFunction(DB_GetDescribeSQL));
            tm.Functions.Add("DB_GetFOREIGNKEYSQL", new TemplateFunction(DB_GetFOREIGNKEYSQL));
            tm.Functions.Add("DB_GetPrimaryKeyFields", new TemplateFunction(DB_GetPrimaryKeyFields));
            tm.Functions.Add("DB_GetFieldStr", new TemplateFunction(DB_GetFieldStr));
            tm.Functions.Add("DB_GetCHECKSQL", new TemplateFunction(DB_GetCHECKSQL));

            string output = tm.Process();
            // 删除空行
            //output = Regex.Replace(output, @"\n\s+\n", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string filePath = DBUFileHelper.GetCSFilePath(dicPath, ".ALL", "", ".sql");
            File.AppendAllText(filePath, output, Encoding.UTF8);

            //string tmepStr2 = St.IO.StFile.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Templates/html_model.txt", FileMode.Open, Encoding.Default);
            //TemplateManager tm2 = null;
            //foreach (string tabName in fss.Keys)
            //{
            //    tm2 = TemplateManager.FromString(tmepStr2);
            //    tm2.SetValue("title", tabName);
            //    tm2.SetValue("fields", fss[tabName]);

            //    tm2.Functions.Add("My_GetValue", new TemplateFunction(GetValue));

            //    string output2 = tm2.Process();
            //    // 删除空行

            //    string filePath2 = DBUFileHelper.GetCSFilePath(dicPath, tabName, "", ".html");
            //    File.AppendAllText(filePath2, output2, Encoding.UTF8);
            //}

        }

        #endregion

        public static string GetDefaultValue(string defaultValue, string dbtype,string serverName=null, string tabName = null, string fieldName = null)
        {
            try
            {
                string fieldInfo = string.Format("[{0} -> {1} -> {2}]",serverName,tabName,fieldName);
                dbtype = dbtype.ToLower();
                string pattern;
                Regex regex;
                MatchCollection matches;


                string rel = null;
                switch (dbtype)
                {
                    case "nvarchar":
                    case "varchar":
                    case "text":
                    case "ntext":
                    case "nchar":
                    case "char":
                    case "uniqueidentifier":
                    case "time":
                    case "sysname":
                        pattern = @"\(\'(.*?)\'\)";
                        regex = new Regex(pattern);
                        matches = regex.Matches(defaultValue);
                        if (matches.Count > 0)
                        {
                            //('')
                            rel = "\"" + (matches[0].Groups[1].Value) + "\"";
                        }
                        else
                        {
                            //((0))
                            pattern = @"\(\((.*?)\)\)";
                            regex = new Regex(pattern);
                            matches = regex.Matches(defaultValue);
                            if (matches.Count > 0)
                            {
                                rel = "\"" + (matches[0].Groups[1].Value) + "\"";
                            }
                            else
                            {
                                //(N'人')
                                pattern = @"\(N\'(.*?)\'\)";
                                regex = new Regex(pattern);
                                matches = regex.Matches(defaultValue);

                                try
                                {
                                    rel = "\"" + (matches[0].Groups[1].Value) + "\"";
                                }
                                catch (Exception e1)
                                {
                                    St.IO.StFileDebug.Log(e1, St.IO.FileDebugPath.CSLog_yyMMdd, msg: fieldInfo + " 默认值错误，以作为\"\"处理  defauleValue=" + defaultValue);
                                    rel = "\"\"";
                                }
                            }
                        }
                        break;
                    case "smallint":
                    case "decimal":
                    case "bigint":
                    case "real":
                    case "money":
                    case "float":
                    case "tinyint":
                    case "int":
                        //((value))
                        pattern = @"\(\((.*?)\)\)";
                        regex = new Regex(pattern);
                        matches = regex.Matches(defaultValue);
                        try
                        {
                            rel = (matches[0].Groups[1].Value);
                        }
                        catch (Exception e1)
                        {
                            St.IO.StFileDebug.Log(e1, St.IO.FileDebugPath.CSLog_yyMMdd, msg: fieldInfo + " 默认值错误，以作为0处理  defauleValue=" + defaultValue);
                            rel = "0";
                        }
                        break;
                    case "bit":
                        //((value))
                        pattern = @"\(\((.*?)\)\)";
                        regex = new Regex(pattern);
                        matches = regex.Matches(defaultValue);
                        rel = (matches[0].Groups[1].Value);
                        rel = rel == "1" ? "true" : "false";
                        //if (rel == "1") { rel = "true"; }
                        //else if (rel == "0") { rel = "false"; }
                        //else rel = "false";
                        break;
                    case "datetimeoffset":
                    case "smalldatetime":
                    case "datetime2":
                    case "datetime":
                    case "date":

                        //(value)
                        //这里不考虑其他情况，只考虑getdate()  因为函数有很多种   这里有待优化
                        //如：(dateadd(day,(2),getdate()))  (dateadd(day,(2),'2016-08-07'))  (dateadd(year,(2),getdate()))

                        //if (defaultValue == "(getdate())")
                        //{
                        rel = "DateTime.Now";
                        //}

                        break;
                    default:
                        rel = "\"\"";
                        St.IO.StFileDebug.Log(fieldInfo + " DBType=\""+dbtype+"\" 有遗漏，请及时补缺", true, 0, "SetErroLog");
                        break;
                }

                return rel;

            }
            catch (Exception e)
            {
                St.IO.StFileDebug.Log(e, St.IO.FileDebugPath.CSLog_yyMMdd);
                throw;
            }
        }

    }
}