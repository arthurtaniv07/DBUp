using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbu.Model
{
    /// <summary>
    /// 数据库表的信息
    /// </summary>
    public class StObjField
    {
        
        /// <summary>
        /// 所属对象的类型  U:用户表   V：视图  PK：主键   C：Check约束   D：默认约束  P：存储过程  TR：触发器  F：外键  TT：用户自定义表类型   IF：内嵌表函数   UQ：UNIQUE约束   L：日志    X：扩展存储过程  TF：表函数    TR：触发器   S：系统表    RF：复制筛选存储过程   FN：标量函数
        /// </summary>
        public StObjectType ObjType { get; set; }
        /// <summary>
        /// 所属对象的名称
        /// </summary>
        public string ObjName { get; set; }
        /// <summary>
        /// 所属对象的创建时间
        /// </summary>
        public DateTime CreTime { get; set; }
        /// <summary>
        /// 所属对象的修改时间
        /// </summary>
        public DateTime UpdTime { get; set; }
        /// <summary>
        /// 所属对象的描述
        /// </summary>
        public string ObjDescribe { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public int ObjID { get; set; }

        /// <summary>
        /// 列ID
        /// </summary>
        public int ColID { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 数据库类型名称
        /// </summary>
        public string ColTypeName { get; set; }
        /// <summary>
        /// 定义时的长度
        /// </summary>
        public int OldLength { get; set; }
        /// <summary>
        /// 最大长度  
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 精度 为数值时有效
        /// </summary>
        public int? Scale { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNull { get; set; }
        /// <summary>
        /// 是否已计算该列的标志
        /// </summary>
        public bool IsComputed { get; set; }
        /// <summary>
        /// 计算列的表达式
        /// </summary>
        public string ComputedText { get; set; }
        /// <summary>
        /// 该过程参数是否是输出参数  为存储过程时有效
        /// </summary>
        public bool IsOutParam { get; set; }
        /// <summary>
        /// 字段的描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 字段的默认值
        /// </summary>
        public int? DefaultID { get; set; }
        /// <summary>
        /// 字段的默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 指示是否是标识列（自增长列）
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 标识规范的种子  为null表示不是标识
        /// </summary>
        public int? IDENTITY_SEED { get; set; }
        /// <summary>
        /// 标识规范的增量  为null表示不是标识
        /// </summary>
        public int? IDENTITY_INCREASE { get; set; }
        /// <summary>
        /// 指示是否是主键列
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否有唯一约束
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// 是否有Check约束
        /// </summary>
        public bool IsCheck { get; set; }
        /// <summary>
        /// Check约束内容  --申明语句
        /// </summary>
        public string CheckText { get; set; }
        /// <summary>
        /// 主表ID  如果有的话
        /// </summary>
        public int PrientTableID { get; set; }
        /// <summary>
        /// 主表名称  如果有的话
        /// </summary>
        public string PrientTableName { get; set; }
        /// <summary>
        /// 主表列ID  如果有的话
        /// </summary>
        public int ParentColID { get; set; }
        /// <summary>
        /// 主表列名称  如果有的话
        /// </summary>
        public string ParentColName { get; set; }


        /// <summary>
        /// 该列是否可update
        /// </summary>
        public bool IsColUpdate { get; set; }
        /// <summary>
        /// 该对象是否可update
        /// </summary>
        public bool IsObjUpdate { get; set; }

        /// <summary>
        /// 该列是否可insert
        /// </summary>
        public bool IsColInsert { get; set; }
        /// <summary>
        /// 该对象是否可insert
        /// </summary>
        public bool IsObjInsert { get; set; }

        /// <summary>
        /// 该列是否可Delete
        /// </summary>
        public bool IsColDelete { get; set; }
        /// <summary>
        /// 该对象是否可Delete
        /// </summary>
        public bool IsObjDelete { get; set; }

        /// <summary>
        /// 该列是否可Select
        /// </summary>
        public bool IsColSelect { get; set; }
        /// <summary>
        /// 该对象是否可Select
        /// </summary>
        public bool IsObjSelect { get; set; }
    }
}
