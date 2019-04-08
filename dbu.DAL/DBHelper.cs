using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using dbu.Model;
using System.Data.Common;

namespace dbu.DAL
{
    public class DBHelper
    {
        private static SqlConnection con = null;
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="userName">用户名  可为空</param>
        /// <param name="passWord"> 密码 可为空</param>
        /// <returns></returns>
        public static string GetLineStr(string serverAddress, string dbName, string userName, string passWord)
        {
           
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(serverAddress))
            {
                sb.Append("Data Source=" + serverAddress + ";");
            }
            else
            {
                throw new Exception("服务器地址不能为空");
            }

            if (!string.IsNullOrWhiteSpace(dbName))
            {
                sb.Append("Initial Catalog=" + dbName + ";");
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                sb.Append("User ID=" + userName + ";Password=" + passWord + ";");
                //_lineStr = "Data Source=" + serverAddress + ";";
            }
            else
            {
                sb.Append("Integrated Security=True;");
            }
            return sb.ToString();
        }


        #region --------1

        /// <summary>
        /// 获取数据库字段详细信息的sql
        /// </summary>
        /// <param name="isAllTab">是否是全部表</param>
        /// <param name="tabName">表名   isAllTab为true时忽视</param>
        /// <param name="types">对象类型   isAllTab为false时忽视</param>
        /// <returns></returns>
        public static string GetDBFieldInfoSQL(bool isAllTab, string tabName, List<string> types = null,DateTime? sTime=null)
        {
            string type = types == null || types.Count == 0 ? "'U'" : "'" + string.Join("','", types) + "'";
            return @"select 
	obj.xtype objtype  -- 对象类型  U:表信息  V：视图  PK：主键   D：默认约束  P：存储过程  TR：触发器  F：外键
	,obj.name  objname  -- 对象的描述
	,obj2.modify_date updTime--对象的更新时间
	,obj2.create_date creTime --对象的创建时间
    ,obj_d.value objdescribe  --对象名  
	,a.id objid  --对象ID  
	, a.colid  --列ID
	,a.name  --列名称
	,b.name typename  --列的类型
	,a.prec  --该列的原长度  推荐使用
	,a.[length]    --该列的长度
	,a.scale  --如果这列是数值列，那么这就是列的小数位数，否则就是0
	,a.isnullable  --该列是否可以为null
	,a.iscomputed --表示是否已计算该列的标志  0 = 未计算。  1 = 已计算。
	,com.[definition] computed_text -- 计算列的表达式
	,a.isoutparam    --表示该过程参数是否是输出参数  0 = 假。  1 = 真。
	,d.value describe -- 字段的描述
	,de.text defaultvalue  -- 字段的默认值
    ,de.id defaultid  -- 这个ID可与sysobjects的ID关联起来查询约束的名字
	,COLUMNPROPERTY(a.id,a.name,'IsIdentity') isidentity  --指示是否是标识列（自增长列）
    ,case when obj.xtype='U' and COLUMNPROPERTY(a.id,a.name,'IsIdentity')=1 then IDENT_SEED(obj.name) else null end identity_seed  -- 种子
    ,case when obj.xtype='U' and COLUMNPROPERTY(a.id,a.name,'IsIdentity')=1 then IDENT_INCR(obj.name) else null end AS identity_increase  -- 增量
	, ISNULL( ind.indid,0) isprimarykey --指示是否是主键列
	,case ISNULL(un.约束列名,'') when '' then 0 else 1 end isunion  --指示是否有唯一索引
	,case ISNULL(chk.object_id,'') when '' then 0 else 1 end ischeck  --指示是否有check索引
    ,chk.[definition] check_text
	,t.parentTabID   --主表ID
	,object_name(t.parentTabID) parentTabName  --主表名
	,t.parColId,t.parColName
from syscolumns a left join systypes b on a.xusertype=b.xusertype
	left join sys.extended_properties d on d.major_id=a.id and d.minor_id =a.colid 
	left join syscomments de on de.id=a.cdefault  
	left join sysindexkeys ind on ind.id=a.id and a.colid=ind.colid and ind.indid=1 
	left join ( 
		select
		a.name as 约束名,
		b.parent_object_id as sonTabID,
		c.name as 子列,
		c.column_id sonColID,
		b.referenced_object_id as parentTabID,
		d.name as parColName
		,d.column_id parColId
		from sys.foreign_keys A
			inner join sys.foreign_key_columns B on A.object_id=b.constraint_object_id
			inner join sys.columns C on B.parent_object_id=C.object_id and B.parent_column_id=C.column_id 
			inner join sys.columns D on B.referenced_object_id=d.object_id and B.referenced_column_id=D.column_id 	
    ) as t on t.sonColID=a.colid and t.sonTabID=a.id  -- 外键
    left join sysobjects obj on obj.id=a.id
    left join sys.extended_properties obj_d on obj_d.major_id=a.id and obj_d.minor_id =0 
    left join sys.check_constraints chk on chk.parent_object_id = obj.id AND chk.parent_column_id = a.colid  -- Check约束
    left join 
    (
		SELECT
		  tab.object_id AS [表ID],
		  tab.name AS [表名],
		  idx.name AS [约束名称],
		  col.column_id AS [约束列ID],
		  col.name AS [约束列名]
		FROM  sys.index_columns idxCol
			left join sys.indexes idx ON (idx.object_id = idxCol.object_id  AND idx.index_id = idxCol.index_id)
			left JOIN sys.tables tab ON (idx.object_id = tab.object_id)
			left JOIN sys.columns col ON (idx.object_id = col.object_id AND idxCol.column_id = col.column_id)
		where idx.is_unique_constraint = 1
    ) un on un.表ID=a.id and un.约束列ID=a.colid
    left join sys.computed_columns com on com.object_id=a.id and com.column_id=a.colid--计算列的表达式
    left join sys.objects obj2 on obj2.object_id=obj.id --创建 修改时间
where " + (isAllTab ? "a.id in (select id from sysobjects where xtype in (" + type + "))" : "a.id=OBJECT_ID('" + tabName + "')") 
        //时间
        + (sTime.HasValue ? " and obj2.modify_date>='"+sTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")+"'" : "")
        +

        " order by obj.name,obj.xtype,obj.id,a.colid";
        }

        /// <summary>
        /// 按照表的引用次数排序  此算法有待优化
        /// </summary>
        /// <param name="fss"></param>
        /// <returns></returns>
        public static Dictionary<string, List<StObjField>> SortByFOREIGN_KEY(Dictionary<string, List<StObjField>> fss)
        {
            Dictionary<string, int> key_fields = new Dictionary<string, int>();
            //初始化集合  每个表的引用数量初始为0
            foreach (var tabName in fss.Keys)
            {
                key_fields.Add(tabName, 0);
            }

            foreach (var tabName in fss.Keys)
            {
                foreach (StObjField fs in fss[tabName])
                {
                    if (!string.IsNullOrWhiteSpace(fs.PrientTableName))
                    {
                        //到这里不用判断表名是否存在  因为已经初始化过了
                        key_fields[fs.PrientTableName] = key_fields[fs.PrientTableName] + 1;
                    }
                }
            }
            if (key_fields.Values.Where(i => i > 0).Count() == 0)
            {
                //没有引用的表 不需要变动
                return fss;
            }

            //下面的算法有待优化 ===========================


            //需要按照表的引用次数排序
            int max = -1;//最大引用次数
            Dictionary<string, List<StObjField>> rel = new Dictionary<string, List<StObjField>>();
            do
            {
                max = key_fields.Values.Max();
                string t = "";
                List<string> tabNames = key_fields.Where(i => i.Value == max).Select(i => i.Key).ToList();
                foreach (var tabName in tabNames)
                {
                    if (key_fields[tabName] == max)
                    {
                        t = tabName;
                        rel.Add(tabName, fss[tabName]);
                        key_fields.Remove(tabName);
                    }
                }
                if (key_fields.Count == 0) { break; }
            } while (true);

            return rel;
        }


        public static DataTable QueryDataTable(string linkStr, string queryString)
        {
            con = null;
            try
            {

                con = new SqlConnection(linkStr);
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter d = new SqlDataAdapter(cmd);
                d.Fill(ds);
                con.Close();
                return ds.Tables[0];
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        public static DataSet QueryDataSet(string linkStr, string queryString)
        {
            con = null;
            try
            {

                con = new SqlConnection(linkStr);
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter d = new SqlDataAdapter(cmd);
                d.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public delegate void DBHander(int c);

        public static DBHander GetDBTableInfohander;
        public static void Set_GetDBTableInfohander(DBHander hander)
        {
            GetDBTableInfohander = hander;
        }
        public static void Clear_GetDBTableInfohander()
        {
            GetDBTableInfohander = null;
        }

        /// <summary>
        /// 获取指定数据库的所有表的字段信息
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static Dictionary<string, List<StObjField>> GetAllTableFieldInfo(string serverAddress, string dbName, string userName, string passWord, List<string> types, DateTime? sTime, string reStr = "_", params string[] replyStr)
        {

            Dictionary<string, List<StObjField>> res = new Dictionary<string, List<StObjField>>(StringComparer.OrdinalIgnoreCase);

            SqlDataReader dr = null;
            try
            {
                string sql = GetLineStr(serverAddress, dbName, userName, passWord);
                con = new SqlConnection(sql);
                con.Open();
                sql = GetDBFieldInfoSQL(true, "", types, sTime);

                SqlCommand cmd = new SqlCommand(sql, con);
                dr = cmd.ExecuteReader();
                StObjField obj = null;
                object o = null;

                bool e = replyStr != null && replyStr.Length > 0;


                string[] isS = { "u", "v" };
                string[] isI = { "u" };
                string[] isU = { "u" };
                string[] isD = { "u" };

                StObjectType stobjtype = default(StObjectType);

                while (dr.Read())
                {
                    stobjtype = default(StObjectType);

                    obj = new StObjField();
                    obj.ObjName = dr["objname"].ToString();
                    if (e)
                    {
                        foreach (var item in replyStr)
                        {
                            obj.ObjName = obj.ObjName.Replace(item, reStr);
                        }
                    }
                    if (obj.ObjName == "vi_AccessIO")
                    {
                        int a = 0;
                    }
                    obj.ColID = Convert.ToInt32(dr["colid"]);
                    o = dr["objdescribe"];
                    obj.ObjDescribe = o is DBNull ? null : o.ToString();
                    obj.ColName = dr["name"].ToString();
                    obj.CreTime = Convert.ToDateTime(dr["creTime"]);
                    obj.UpdTime = Convert.ToDateTime(dr["updTime"]);
                    obj.ColTypeName = dr["typename"].ToString();
                    o = dr["defaultvalue"];
                    obj.DefaultValue = o is DBNull ? null : o.ToString();
                    o = dr["defaultid"];
                    if (!(o is DBNull))
                        obj.DefaultID = Convert.ToInt32(o);
                    o = dr["describe"];
                    obj.Describe = o is DBNull ? null : o.ToString().Replace("\r\n", "");
                    obj.IsComputed = Convert.ToBoolean(dr["iscomputed"]);
                    o = dr["computed_text"];
                    obj.ComputedText = o is DBNull ? null : o.ToString();
                    obj.IsIdentity = Convert.ToBoolean(dr["isidentity"]);
                    obj.IsNull = Convert.ToBoolean(dr["isnullable"]);
                    obj.IsOutParam = Convert.ToBoolean(dr["isoutparam"]);
                    obj.IsPrimaryKey = Convert.ToBoolean(dr["isprimarykey"]);
                    obj.MaxLength = Convert.ToInt32(dr["length"]);
                    obj.ObjID = Convert.ToInt32(dr["objid"]);
                    o = dr["prec"];
                    obj.OldLength = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabID"];
                    obj.PrientTableID = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabName"];
                    obj.PrientTableName = o is DBNull ? null : o.ToString();
                    o = dr["parColId"];
                    obj.ParentColID = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parColName"];
                    obj.ParentColName = o is DBNull ? null : o.ToString();
                    o = dr["scale"];
                    if (!(o is DBNull))
                        obj.Scale = Convert.ToInt32(o);
                    if (Enum.TryParse<StObjectType>(dr["objtype"] + "", false, out stobjtype))
                    {
                        obj.ObjType = stobjtype;
                    }

                    obj.IsUnique = dr["isunion"] + "" == "1";

                    o = dr["identity_seed"];//种子
                    if (!(o is DBNull))
                        obj.IDENTITY_SEED = Convert.ToInt32(o);
                    o = dr["identity_increase"];//增量
                    if (!(o is DBNull))
                        obj.IDENTITY_INCREASE = Convert.ToInt32(o);
                    obj.IsUnique = dr["ischeck"] + "" == "1";
                    o = dr["check_text"];//增量
                    if (!(o is DBNull))
                        obj.CheckText = o is DBNull ? null : o.ToString();

                    string objtype = obj.ObjType.ToString().ToLower();

                    obj.IsObjDelete = isD.Contains(objtype);
                    obj.IsObjInsert = isI.Contains(objtype);
                    obj.IsObjSelect = isS.Contains(objtype);
                    obj.IsObjUpdate = isU.Contains(objtype);

                    obj.IsColDelete = obj.IsObjDelete;
                    obj.IsColInsert = obj.IsObjInsert && !obj.IsPrimaryKey && !obj.IsIdentity;
                    obj.IsColSelect = obj.IsObjSelect;
                    obj.IsColUpdate = obj.IsObjUpdate && !obj.IsPrimaryKey && !obj.IsIdentity && !obj.IsComputed;


                    if (!res.ContainsKey(obj.ObjName))
                    {
                        res.Add(obj.ObjName, new List<StObjField>());
                    }
                    res[obj.ObjName].Add(obj);
                    if (GetDBTableInfohander != null)
                    {
                        GetDBTableInfohander(res.Count);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con != null)
                    con.Close();
            }
            return res;

        }


        /// <summary>
        /// 获取指定数据库的所有表引用排序
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<string> GetAllTableSortInfo(string serverAddress, string dbName, string userName, string passWord)
        {
            List<string> rel = new List<string>();
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            SqlDataReader dr = null;
            try
            {
                string sql = GetLineStr(serverAddress, dbName, userName, passWord);
                con = new SqlConnection(sql);
                con.Open();

                SqlCommand cmd = null;
                object o = null;

                //表信息获取完毕
                //开始获取外键信息
                sql = @"
  SELECT  
    --OBJECT_SCHEMA_NAME (fkey.referenced_object_id) + '.' +  
    OBJECT_NAME (fkey.referenced_object_id) AS ReferenceTableName 
    ,COL_NAME(fcol.referenced_object_id, fcol.referenced_column_id) AS ReferenceColumnName 
    ,--OBJECT_SCHEMA_NAME (fkey.parent_object_id) + '.' +  
    OBJECT_NAME(fkey.parent_object_id) AS TableName 
    ,COL_NAME(fcol.parent_object_id, fcol.parent_column_id) AS ColumnName 
  FROM sys.foreign_keys AS fkey 
    INNER JOIN sys.foreign_key_columns AS fcol ON 
               fkey.OBJECT_ID = fcol.constraint_object_id ";
                cmd = new SqlCommand(sql, con);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string leftTab = dr["ReferenceTableName"] + "";
                    string rightTab = dr["TableName"] + "";
                    if (!res.ContainsKey(leftTab)) { res.Add(leftTab, new List<string>()); }
                    res[leftTab].Add(rightTab);
                }
                Util.Sort(res, ref rel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con != null)
                    con.Close();
            }
            return rel;

        }
        
        /// <summary>
        /// 获取指定数据库的所有视图引用排序
        /// </summary>
        /// <param name="connStr">数据库的链接字符串</param>
        /// <param name="views">要获取关系排序的视图</param>
        /// <returns></returns>
        public static string[] GetAllViewSortInfo(string connStr,ref List<string> result)
        {
            Dictionary<string, List<string>> v = new Dictionary<string, List<string>>();
            string sql = @"select distinct OBJECT_NAME(d.depid) parentName,par.type parentType, o.name 'name',o.type
from sysdepends d,sys.objects o,sys.objects par
where o.object_id = d.id and par.object_id = d.depid and deptype < 2 and par.type in('V','U') and o.type in('V','U')";
            DataTable dt = QueryDataTable(connStr, sql);
            foreach (DataRow item in dt.Rows)
            {
                string parName = item["parentName"] + "";
                string name = item["name"] + "";
                if (!v.ContainsKey(parName))
                {
                    v.Add(parName, new List<string>());
                }
                if (!v[parName].Contains(name))
                {
                    v[parName].Add(name);
                }
            }
            return Util.Sort(v, ref result);
        }

        /// <summary>
        /// 获取指定表的字段信息
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="tabName">要获取信息的表名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<StObjField> GetTableFieldInfo(string serverAddress, string dbName, string tabName, string userName, string passWord)
        {

            List<StObjField> res = new List<StObjField>();

            SqlDataReader dr = null;
            try
            {
                con = new SqlConnection(GetLineStr(serverAddress, dbName, userName, passWord));
                con.Open();
                string sql = GetDBFieldInfoSQL(false, tabName);

                SqlCommand cmd = new SqlCommand(sql, con);
                dr = cmd.ExecuteReader();
                StObjField obj = null;
                object o = null;
                string[] isS = { "u", "v" };
                string[] isI = { "u" };
                string[] isU = { "u" };
                string[] isD = { "u" };

                StObjectType stobjtype = default(StObjectType);
                while (dr.Read())
                {
                    stobjtype = default(StObjectType);

                    obj = new StObjField();
                    obj.ObjName = dr["objname"].ToString().Replace("(", "_").Replace(")", "_");

                    obj.ColID = Convert.ToInt32(dr["colid"]);
                    obj.ColName = dr["name"].ToString();
                    obj.ColTypeName = dr["typename"].ToString();
                    o = dr["defaultvalue"];
                    obj.DefaultValue = o is DBNull ? null : o.ToString();
                    o = dr["describe"];
                    obj.Describe = o is DBNull ? null : o.ToString().Replace("\\r\\n", "").Replace("\r\n", "");
                    obj.IsComputed = Convert.ToBoolean(dr["iscomputed"]);
                    obj.IsIdentity = Convert.ToBoolean(dr["isidentity"]);
                    obj.IsNull = Convert.ToBoolean(dr["isnullable"]);
                    obj.IsOutParam = Convert.ToBoolean(dr["isoutparam"]);
                    obj.IsPrimaryKey = Convert.ToBoolean(dr["isprimarykey"]);
                    obj.MaxLength = Convert.ToInt32(dr["length"]);
                    obj.ObjID = Convert.ToInt32(dr["objid"]);
                    o = dr["prec"];
                    obj.OldLength = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabID"];
                    obj.PrientTableID = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabName"];
                    obj.PrientTableName = o is DBNull ? null : o.ToString();
                    o = dr["scale"];
                    obj.Scale = o is DBNull ? -1 : Convert.ToInt32(o);

                    if (Enum.TryParse<StObjectType>(dr["scale"] + "", false, out stobjtype))
                    {
                        obj.ObjType = stobjtype;
                    }

                    string objtype = obj.ObjType.ToString().ToLower();

                    obj.IsObjDelete = isD.Contains(objtype);
                    obj.IsObjInsert = isI.Contains(objtype);
                    obj.IsObjSelect = isS.Contains(objtype);
                    obj.IsObjUpdate = isU.Contains(objtype);

                    obj.IsColDelete = obj.IsObjDelete;
                    obj.IsColInsert = obj.IsObjInsert && !obj.IsPrimaryKey && !obj.IsIdentity;
                    obj.IsColSelect = obj.IsObjSelect;
                    obj.IsColUpdate = obj.IsObjUpdate && !obj.IsPrimaryKey && !obj.IsIdentity && !obj.IsComputed;
                    res.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con != null)
                    con.Close();
            }
            return res;

        }

        #endregion


        #region --------2


        /// <summary>
        /// 获取数据库字段详细信息的sql
        /// </summary>
        /// <param name="isAllTab">是否是全部表</param>
        /// <param name="tabName">表名   isAllTab为true时忽视</param>
        /// <param name="types">对象类型   isAllTab为false时忽视</param>
        /// <returns></returns>
        public static string GetDBFieldInfoSQL2(bool isAllTab, string tabName, List<string> types = null)
        {
            string type = types == null || types.Count == 0 ? "'U'" : "'" + string.Join("','", types) + "'";
            return GetDBFieldInfoSQL(isAllTab, tabName, types) + ";" + @"
SELECT
  tab.object_id tabid,
  tab.name AS tabname,
  col.column_id AS colid,
  col.name AS colname,
  chk.name AS name,
  chk.definition
FROM
  sys.check_constraints chk
    JOIN sys.tables tab
      ON (chk.parent_object_id = tab.object_id)
    JOIN sys.columns col
      ON (chk.parent_object_id = col.object_id
          AND chk.parent_column_id = col.column_id)";
        }
        /// <summary>
        /// 获取数据库表的check约束
        /// </summary>
        /// <returns></returns>
        public static string GetDBObjectsSQL2_Check()
        {
            return @"
SELECT
  tab.object_id tabid,
  tab.name AS tabname,
  chk.parent_column_id AS colid,
  chk.name AS name,
  chk.definition
FROM
  sys.check_constraints chk
    JOIN sys.tables tab
      ON (chk.parent_object_id = tab.object_id)";
        }

        /// <summary>
        /// 按照表的引用次数排序  此算法有待优化
        /// </summary>
        /// <param name="fss"></param>
        /// <returns></returns>
        public static Dictionary<string, StTable> SortByFOREIGN_KEY2(Dictionary<string, StTable> fss)
        {
            Dictionary<string, int> key_fields = new Dictionary<string, int>();
            //初始化集合  每个表的引用数量初始为0
            foreach (var tabName in fss.Keys)
            {
                key_fields.Add(tabName, 0);
            }

            foreach (var tabName in fss.Keys)
            {
                foreach (StField fs in fss[tabName].Fields)
                {
                    if (!string.IsNullOrWhiteSpace(fs.PrientTableName))
                    {
                        //到这里不用判断表名是否存在  因为已经初始化过了
                        key_fields[fs.PrientTableName] = key_fields[fs.PrientTableName] + 1;
                    }
                }
            }
            if (key_fields.Values.Where(i => i > 0).Count() == 0)
            {
                //没有引用的表 不需要变动
                return fss;
            }

            //下面的算法有待优化 ===========================


            //需要按照表的引用次数排序
            int max = -1;//最大引用次数
            Dictionary<string, StTable> rel = new Dictionary<string, StTable>();
            do
            {
                max = key_fields.Values.Max();
                string t = "";
                List<string> tabNames = key_fields.Where(i => i.Value == max).Select(i => i.Key).ToList();
                foreach (var tabName in tabNames)
                {
                    if (key_fields[tabName] == max)
                    {
                        t = tabName;
                        rel.Add(tabName, fss[tabName]);
                        key_fields.Remove(tabName);
                    }
                }
                if (key_fields.Count == 0) { break; }
            } while (true);

            return rel;
        }

        /// <summary>
        /// 获取指定数据库的所有表的字段信息
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static Dictionary<string, StTable> GetAllTableFieldInfo2(string serverAddress, string dbName, string userName, string passWord, List<string> types, string reStr = "_", params string[] replyStr)
        {

            Dictionary<string, StTable> res = new Dictionary<string, StTable>();
            SqlCommand cmd = null;
            try
            {
                string sql = GetLineStr(serverAddress, dbName, userName, passWord);
                con = new SqlConnection(sql);
                con.Open();
                sql = GetDBFieldInfoSQL2(true, "", types);

                cmd = new SqlCommand(sql, con);


                DataSet ds = new DataSet();
                using (DbDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                StTable obj = null;
                object o = null;

                bool e = replyStr != null && replyStr.Length > 0;


                string[] isS = { "u", "v" };
                string[] isI = { "u" };
                string[] isU = { "u" };
                string[] isD = { "u" };

                StObjectType stobjtype = default(StObjectType);

                StField field = new StField();

                DataTable dt1 = ds.Tables[0];
                foreach (DataRow dr in dt1.Rows)
                {
                    stobjtype = default(StObjectType);

                    field = new StField();


                    string tabName = dr["objname"].ToString();
                    if (!res.ContainsKey(tabName))
                    {
                        obj = new StTable() { Fields = new List<StField>() };
                        obj.Name = dr["objname"].ToString();
                        if (e)
                        {
                            foreach (var item in replyStr)
                            {
                                obj.Name = obj.Name.Replace(item, reStr);
                            }
                        }
                        o = dr["objdescribe"];
                        obj.Describe = o is DBNull ? null : o.ToString();
                        obj.ID = Convert.ToInt32(dr["objid"]);

                        string objtype = obj.Type.ToString().ToLower();

                        if (Enum.TryParse<StObjectType>(dr["scale"] + "", false, out stobjtype))
                        {
                            obj.Type = stobjtype;
                        }
                        obj.IsDelete = isD.Contains(objtype);
                        obj.IsInsert = isI.Contains(objtype);
                        obj.IsSelect = isS.Contains(objtype);
                        obj.IsUpdate = isU.Contains(objtype);
                    }

                    field.ColID = Convert.ToInt32(dr["colid"]);
                    field.ColName = dr["name"].ToString();
                    field.ColTypeName = dr["typename"].ToString();
                    o = dr["defaultvalue"];
                    field.DefaultValue = o is DBNull ? null : o.ToString();
                    o = dr["describe"];
                    field.Describe = o is DBNull ? null : o.ToString().Replace("\r\n", "");
                    field.IsComputed = Convert.ToBoolean(dr["iscomputed"]);
                    field.IsIdentity = Convert.ToBoolean(dr["isidentity"]);
                    field.IsNull = Convert.ToBoolean(dr["isnullable"]);
                    field.IsOutParam = Convert.ToBoolean(dr["isoutparam"]);
                    field.IsPrimaryKey = Convert.ToBoolean(dr["isprimarykey"]);
                    field.MaxLength = Convert.ToInt32(dr["length"]);
                    o = dr["prec"];
                    field.OldLength = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabID"];
                    field.PrientTableID = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabName"];
                    field.PrientTableName = o is DBNull ? null : o.ToString();
                    o = dr["scale"];
                    if (!(o is DBNull))
                        field.Scale = Convert.ToInt32(o);

                    o = dr["identity_seed"];//种子
                    if (!(o is DBNull))
                        field.IDENTITY_SEED = Convert.ToInt32(o);
                    o = dr["identity_increase"];//增量
                    if (!(o is DBNull))
                        field.IDENTITY_INCREASE = Convert.ToInt32(o);
                    o = dr["unique_text"];//增量
                    if (!(o is DBNull))
                        field.UniqueText = o is DBNull ? null : o.ToString();


                    field.IsColDelete = obj.IsDelete;
                    field.IsColInsert = obj.IsInsert && !field.IsPrimaryKey && !field.IsIdentity;
                    field.IsColSelect = obj.IsSelect;
                    field.IsColUpdate = obj.IsUpdate && !field.IsPrimaryKey && !field.IsIdentity && !field.IsComputed;


                    if (!res.ContainsKey(obj.Name))
                    {
                        res.Add(obj.Name, obj);
                    }
                    res[obj.Name].Fields.Add(field);
                }

                #region Check约束

                if (ds.Tables.Count > 1)
                {
                    dt1 = ds.Tables[1];
                    StObject sto = null;
                    foreach (DataRow dr in dt1.Rows)
                    {
                        sto = new StObject();
                        sto.Type = StObjectType.C;

                        o = dr["colid"];
                        sto.ColID = o is DBNull ? -1 : Convert.ToInt32(o.ToString());
                        o = dr["definition"];
                        sto.CreateText = o is DBNull ? "" : o.ToString();
                        o = dr["name"];
                        sto.Name = o is DBNull ? "" : o.ToString();
                        
                        o = dr["tabid"];
                        sto.ObjID = Convert.ToInt32(o.ToString());
                        o = dr["tabname"];
                        sto.ObjName = o is DBNull ? null : o.ToString();
                        StField sf = res[sto.ObjName].Fields.Where(i => i.ColID == sto.ColID).FirstOrDefault();
                        if (sf != null)
                        {
                            //该约束是针对列的
                            sf.UniqueText = sto.CreateText;
                        }

                        if (res[sto.ObjName].StObjects == null)
                            res[sto.ObjName].StObjects = new List<StObject>();
                        res[sto.ObjName].StObjects.Add(sto);

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Clone();
                if (con != null)
                    con.Close();
            }
            return res;
        }


        /// <summary>
        /// 获取指定数据库的所有表的字段信息
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<StObject> GetAllTableObjectsInfo2(string serverAddress, string dbName, string userName, string passWord)
        {
            List<StObject> res = new List<StObject>();
            SqlCommand cmd = null;
            try
            {
                string sql = GetLineStr(serverAddress, dbName, userName, passWord);
                con = new SqlConnection(sql);
                con.Open();
                sql = GetDBObjectsSQL2_Check();

                cmd = new SqlCommand(sql, con);


                DataSet ds = new DataSet();
                using (DbDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }


                #region Check约束

                object o;
                DataTable dt1 = ds.Tables[0];
                StObject sto = null;
                foreach (DataRow dr in dt1.Rows)
                {
                    sto = new StObject();
                    sto.Type = StObjectType.C;

                    o = dr["colid"];
                    sto.ColID = o is DBNull ? -1 : Convert.ToInt32(o.ToString());
                    o = dr["definition"];
                    sto.CreateText = o is DBNull ? "" : o.ToString();
                    o = dr["name"];
                    sto.Name = o is DBNull ? "" : o.ToString();

                    o = dr["tabid"];
                    sto.ObjID = Convert.ToInt32(o.ToString());
                    o = dr["tabname"];
                    sto.ObjName = o is DBNull ? null : o.ToString();

                    res.Add(sto);


                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Clone();
                if (con != null)
                    con.Close();
            }
            return res;
        }

        /// <summary>
        /// 获取指定表的字段信息
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="tabName">要获取信息的表名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<StObjField> GetTableFieldInfo2(string serverAddress, string dbName, string tabName, string userName, string passWord)
        {

            List<StObjField> res = new List<StObjField>();

            SqlDataReader dr = null;
            try
            {
                con = new SqlConnection(GetLineStr(serverAddress, dbName, userName, passWord));
                con.Open();
                string sql = GetDBFieldInfoSQL2(false, tabName);

                SqlCommand cmd = new SqlCommand(sql, con);
                dr = cmd.ExecuteReader();
                StObjField obj = null;
                object o = null;
                string[] isS = { "u", "v" };
                string[] isI = { "u" };
                string[] isU = { "u" };
                string[] isD = { "u" };

                StObjectType stobjtype = default(StObjectType);
                while (dr.Read())
                {
                    stobjtype = default(StObjectType);

                    obj = new StObjField();
                    obj.ObjName = dr["objname"].ToString().Replace("(", "_").Replace(")", "_");

                    obj.ColID = Convert.ToInt32(dr["colid"]);
                    obj.ColName = dr["name"].ToString();
                    obj.ColTypeName = dr["typename"].ToString();
                    o = dr["defaultvalue"];
                    obj.DefaultValue = o is DBNull ? null : o.ToString();
                    o = dr["describe"];
                    obj.Describe = o is DBNull ? null : o.ToString().Replace("\\r\\n", "").Replace("\r\n", "");
                    obj.IsComputed = Convert.ToBoolean(dr["iscomputed"]);
                    obj.IsIdentity = Convert.ToBoolean(dr["isidentity"]);
                    obj.IsNull = Convert.ToBoolean(dr["isnullable"]);
                    obj.IsOutParam = Convert.ToBoolean(dr["isoutparam"]);
                    obj.IsPrimaryKey = Convert.ToBoolean(dr["isprimarykey"]);
                    obj.MaxLength = Convert.ToInt32(dr["length"]);
                    obj.ObjID = Convert.ToInt32(dr["objid"]);
                    o = dr["prec"];
                    obj.OldLength = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabID"];
                    obj.PrientTableID = o is DBNull ? -1 : Convert.ToInt32(o);
                    o = dr["parentTabName"];
                    obj.PrientTableName = o is DBNull ? null : o.ToString();
                    o = dr["scale"];
                    obj.Scale = o is DBNull ? -1 : Convert.ToInt32(o);

                    if (Enum.TryParse<StObjectType>(dr["scale"] + "", false, out stobjtype))
                    {
                        obj.ObjType = stobjtype;
                    }

                    string objtype = obj.ObjType.ToString().ToLower();

                    obj.IsObjDelete = isD.Contains(objtype);
                    obj.IsObjInsert = isI.Contains(objtype);
                    obj.IsObjSelect = isS.Contains(objtype);
                    obj.IsObjUpdate = isU.Contains(objtype);

                    obj.IsColDelete = obj.IsObjDelete;
                    obj.IsColInsert = obj.IsObjInsert && !obj.IsPrimaryKey && !obj.IsIdentity;
                    obj.IsColSelect = obj.IsObjSelect;
                    obj.IsColUpdate = obj.IsObjUpdate && !obj.IsPrimaryKey && !obj.IsIdentity && !obj.IsComputed;
                    res.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con != null)
                    con.Close();
            }
            return res;

        }


        #endregion


        /// <summary>
        /// 获取指定数据库的所有表
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="dbName">要获取信息的数据库名称</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<string> GetAllTableName(string serverAddress, string dbName, string userName, string passWord)
        {

            SqlDataReader dr = null;
            //select name from sysdatabases where dbid>6
            try
            {
                con = new SqlConnection(GetLineStr(serverAddress, dbName, userName, passWord));

                con.Open();
                SqlCommand cmd = new SqlCommand("select name from sysobjects where xtype='U'", con);
                dr = cmd.ExecuteReader();
                //获取所有表名
                List<string> ts = new List<string>();

                while (dr.Read()) ts.Add(dr[0].ToString());
                return ts;
            }
            catch (Exception)
            {
                return new List<string>();
                throw;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="maxSleepTime">指定的毫秒数</param>
        /// <returns></returns>
        public static bool TextLineToServer(string serverAddress, string userName, string passWord, int maxSleepTime = 5000)
        {
            SqlCommand comm = null;


            bool rel = false;
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {

                try
                {
                    using (con = new SqlConnection(GetLineStr(serverAddress, null, userName, passWord)))
                    {
                        con.Open();
                        comm = new SqlCommand("select 1", con);
                        comm.ExecuteScalar();
                        rel = true;
                    }
                }
                catch (Exception)
                {
                    rel = false;
                }
                finally
                {
                    if (comm != null)
                        comm.Dispose();
                    if (con != null)
                        con.Close();
                }
            });
            th.Start();


            int warnTime = 0;
            //设置一个循环来等待子线程结束
            while (th.ThreadState != System.Threading.ThreadState.Stopped)
            {
                int rTime = 10;
                warnTime += rTime;
                th.Join(rTime);
                if (warnTime > maxSleepTime)
                {
                    //如果等待的时间大于 maxSleepTime 则跳出循环
                    break;
                }
            }

            if (th.ThreadState != System.Threading.ThreadState.Stopped)
            {
                th.Abort();
            }
            return rel;




        }


        /// <summary>
        /// 获取所有用户数据库名
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        /// <param name="userName">数据库登录用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public static List<string> getAlldbName(string serverAddress,string userName,string passWord,bool quanxian=true)
        {
            SqlDataReader dr = null;
            //select name from sysdatabases where dbid>6
            try
            {
                con = new SqlConnection(GetLineStr(serverAddress, null, userName, passWord));
                con.Open();
                SqlCommand cmd = new SqlCommand("select name from sysdatabases where 1=1 " + (quanxian ? "and HAS_DBACCESS(name)=1" : "") + " order by dbid desc", con);
                dr = cmd.ExecuteReader();
                List<string> strs = new List<string>();
                while (dr.Read()) strs.Add(dr[0].ToString());
                return strs;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con != null)
                    con.Close();
            }
        }

    }
}
