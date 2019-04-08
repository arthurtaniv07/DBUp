using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbu.Model
{
    /// <summary>
    /// 数据库表
    /// </summary>
    public class StTable
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 所属对象的类型  U:用户表   V：视图  PK：主键   C：Check约束   D：默认约束  P：存储过程  TR：触发器  F：外键  TT：用户自定义表类型   IF：内嵌表函数   UQ：UNIQUE约束   L：日志    X：扩展存储过程  TF：表函数    TR：触发器   S：系统表    RF：复制筛选存储过程   FN：标量函数
        /// </summary>
        public StObjectType Type { get; set; }
        /// <summary>
        /// 所属对象的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属对象的描述
        /// </summary>
        public string Describe { get; set; }


        /// <summary>
        /// 该对象是否可update
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// 该对象是否可insert
        /// </summary>
        public bool IsInsert { get; set; }

        /// <summary>
        /// 该对象是否可Delete
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 该对象是否可Select
        /// </summary>
        public bool IsSelect { get; set; }



        /// <summary>
        /// 对象的字段
        /// </summary>
        public List<StField> Fields { get; set; }

        /// <summary>
        /// 对象的其他信息
        /// </summary>
        public List<StObject> StObjects { get; set; }

    }
}
