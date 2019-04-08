using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbu.Build
{
    [Serializable]
    public class TypeManager
    {
        /// <summary>
        /// 
        /// </summary>
        private class TypeInfo
        {
            /// <summary>
            /// 数据库的类型  -- 小写
            /// </summary>
            public string db_DBType { get; set; }
            /// <summary>
            /// 类型转换的Type 用于Convert.ToXXX();
            /// </summary>
            public string convertTo { get; set; }

            /// <summary>
            /// .NET Framework 类型  用于属性的类型
            /// </summary>
            public string fType { get; set; }
            /// <summary>
            /// System.Data.DbType 类型  用于创建XXXParameter时使用
            /// </summary>
            public string dbType { get; set; }

            /// <summary>
            /// 绝对路径（包括命名空间+对象名） 如：System.Int32  可用于global::
            /// </summary>
            public string absPath { get; set; }
        }

        /// <summary>
        /// .net framework基本类型信息
        /// </summary>
        public class ClientType
        {
            /// <summary>
            /// 类型的简写形式
            /// </summary>
            public string simpleName { get; set; }
            /// <summary>
            /// 类型限定名(完全路径)  
            /// </summary>
            public string fullPath { get; set; }

            /// <summary>
            /// 是否可以为null
            /// </summary>
            public bool isNull { get; set; }
        }
        private static Dictionary<string, ClientType> _clientTypeName_typeInfo = new Dictionary<string, ClientType>()
        {
            { "string",new ClientType() { simpleName="string",fullPath="System.String", isNull=true} },
            { "bool",new ClientType() { simpleName="bool",fullPath="System.Boolean", isNull=false} },
            { "byte",new ClientType() { simpleName="byte",fullPath="System.Byte", isNull=false} },
            { "char",new ClientType() { simpleName="char",fullPath="System.Char", isNull=false} },
            { "short",new ClientType() { simpleName="short",fullPath="System.Int32", isNull=false} },
            { "ushort",new ClientType() { simpleName="ushort",fullPath="System.UInt32", isNull=false} },
            { "int",new ClientType() { simpleName="int",fullPath="System.Int32", isNull=false} },
            { "uint",new ClientType() { simpleName="uint",fullPath="System.UInt32", isNull=false} },
            { "long",new ClientType() { simpleName="long",fullPath="System.Int64", isNull=false} },
            { "float",new ClientType() { simpleName="float",fullPath="System.Single", isNull=false} },
            { "double",new ClientType() { simpleName="double",fullPath="System.Double", isNull=false} },
            { "decimal",new ClientType() { simpleName="decimal",fullPath="System.Decimal", isNull=false} },
            { "datetime",new ClientType() { simpleName="datetime",fullPath="System.DateTime", isNull=false} }
        };
        /// <summary>
        /// 获取命名空间字符串 
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        public static string GetTypeFullName(string simpleName)
        {
            simpleName = simpleName.ToLower();
            if (_clientTypeName_typeInfo.ContainsKey(simpleName))
            {
                return _clientTypeName_typeInfo[simpleName].fullPath;
            }
            return "";
        }
        /// <summary>
        /// 获取类型是否可为null
        /// </summary>
        /// <param name="simpleName"></param>
        /// <returns></returns>
        public static bool GetTypeIsNull(string simpleName)
        {
            simpleName = simpleName.ToLower();
            if (_clientTypeName_typeInfo.ContainsKey(simpleName))
            {
                return _clientTypeName_typeInfo[simpleName].isNull;
            }
            return true;
        }

        private static Dictionary<string, TypeInfo> _sqlType_Type = null;

        private static Dictionary<string, TypeInfo> SqlType_Type = new Dictionary<string, TypeInfo>()
        {
            { "bigint"
                ,new TypeInfo() {db_DBType="bigint",fType= "long",convertTo="Convert.ToInt64(##)",dbType="BigInt"}},
            { "binary"
                ,new TypeInfo() {db_DBType="binary",fType= "byte[]",convertTo="(byte[])(##)",dbType="Binary"}},
            { "bit"
                ,new TypeInfo() {db_DBType="bit",fType= "bool",convertTo="Convert.ToBoolean(##)",dbType="Bit"}},
            { "char"
                ,new TypeInfo() {db_DBType="char",fType= "string",convertTo="##.ToString()",dbType="Char"}},
            { "datetime"
                ,new TypeInfo() {db_DBType="datetime",fType= "DateTime",convertTo="Convert.ToDateTime(##)",dbType="DateTime"}},
            { "decimal"
                ,new TypeInfo() {db_DBType="decimal",fType= "decimal",convertTo="Convert.ToDecimal(##)",dbType="Decimal"}},
            { "float"
                ,new TypeInfo() {db_DBType="float",fType= "double",convertTo="Convert.ToDouble(##)",dbType="Float"}},
            { "image"
                ,new TypeInfo() {db_DBType="image",fType= "byte[]",convertTo="(byte[])(##)",dbType="Image"}},
            { "int"
                ,new TypeInfo() {db_DBType="int",fType= "int",convertTo="Convert.ToInt32(##)",dbType="Int"}},
            { "money"
                ,new TypeInfo() {db_DBType="money",fType= "decimal",convertTo="Convert.ToDecimal(##)",dbType="Money"}},
            { "smallmoney"
                ,new TypeInfo() {db_DBType="smallmoney",fType= "decimal",convertTo="Convert.ToDecimal(##)",dbType=""}},
            { "nchar"//与nchar(max) 相同  
                ,new TypeInfo() {db_DBType="nchar",fType= "string",convertTo="##.ToString()",dbType="NChar"}},
            { "ntext"//与nvarchar(max) 相同  
                ,new TypeInfo() {db_DBType="ntext",fType= "string",convertTo="##.ToString()",dbType="NText"}},
            { "nvarchar"
                ,new TypeInfo() {db_DBType="nvarchar",fType= "string",convertTo="##.ToString()",dbType="NVarChar"}},
            { "real"
                ,new TypeInfo() {db_DBType="real",fType= "float",convertTo="##.ToString()",dbType="Real"}},
            { "uniqueidentifier"
                ,new TypeInfo() {db_DBType="uniqueidentifier",fType= "string",convertTo="##.ToString()",dbType="UniqueIdentifier"}},
            { "smalldatetime"
                ,new TypeInfo() {db_DBType="smalldatetime",fType= "DateTime",convertTo="Convert.ToDateTime(##)",dbType="SmallDateTime"}},
            { "smallint"
                ,new TypeInfo() {db_DBType="smallint",fType= "short",convertTo="Convert.ToInt16(##)",dbType="SmallInt"}},
            { "text"
                ,new TypeInfo() {db_DBType="text",fType= "string",convertTo="##.ToString()",dbType="Text"}},
            { "timestamp"
                ,new TypeInfo() {db_DBType="timestamp",fType= "byte[]",convertTo="(byte[])(##)",dbType="Timestamp"}},
            { "tinyint"
                ,new TypeInfo() {db_DBType="tinyint",fType= "byte",convertTo="Convert.ToByte(##)",dbType="TinyInt"}},
            { "varbinary"
                ,new TypeInfo() {db_DBType="varbinary",fType= "byte[]",convertTo="(byte[])(##)",dbType="VarBinary"}},
            { "varchar"
                ,new TypeInfo() {db_DBType="varchar",fType= "string",convertTo="##.ToString()",dbType="VarChar"}},
            { "variant"
                ,new TypeInfo() {db_DBType="variant",fType= "object",convertTo="##",dbType="Variant"}},


            { "date"
                ,new TypeInfo() {db_DBType="date", fType= "DateTime",convertTo="Convert.ToDateTime(##)",dbType="Date"} },
            { "time"
                ,new TypeInfo() {db_DBType="time", fType= "string",convertTo="##.ToString()",dbType="Time"} },
            { "datetime2"
                ,new TypeInfo() {db_DBType="datetime2",fType= "DateTime",convertTo="Convert.ToDateTime(##)",dbType="DateTime2"}},
            { "xml"
                ,new TypeInfo() {db_DBType="xml",fType= "string",convertTo="##.ToString()",dbType="Xml"}},

            { "datetimeoffset"
                ,new TypeInfo() {db_DBType="datetimeoffset",fType= "DateTimeOffset",convertTo="Convert.ToDateTime(##)",dbType="DateTimeOffset"}}, //如：  datetimeoffset(8) 2008-08-08 08:08:08.0 +8:00
            { "sysname"//与nvarchar(128)相同
                ,new TypeInfo() {db_DBType="sysname",fType= "string",convertTo="##.ToString()",dbType="NVarChar"}}, 
        };

        /// <summary>
        /// 组建DBParameter时
        /// </summary>
        /// <param name="db_DBType"></param>
        /// <returns></returns>
        public static string DBTypeToSqlDbType(string db_DBType)
        {
            db_DBType = db_DBType.ToLower();
            if (SqlType_Type.ContainsKey(db_DBType))
            {
                return SqlType_Type[db_DBType].dbType;
            }
            return "";
        }

        /// <summary>
        /// 获取转换的字符串  需要 将##替换为要转换的值
        /// </summary>
        /// <param name="db_DBType"></param>
        /// <returns></returns>
        public static string DBTypeToConvert(string db_DBType)
        {
            
            db_DBType = db_DBType.ToLower();
            if (SqlType_Type.ContainsKey(db_DBType))
            {
                return SqlType_Type[db_DBType].convertTo;
            }
            return "";
        }

        /// <summary>
        /// 获取程序的属性类型字符串
        /// </summary>
        /// <param name="db_DBType"></param>
        /// <returns></returns>
        public static string DBTypeToFType(string db_DBType)
        {

            db_DBType = db_DBType.ToLower();
            if (SqlType_Type.ContainsKey(db_DBType)) {
                return SqlType_Type[db_DBType].fType;
            }
            return "";
        }
    }
}
