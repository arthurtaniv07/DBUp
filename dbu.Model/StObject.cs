using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace dbu.Model
{
    /// <summary>
    /// 对象   如唯一约束
    /// </summary>
    public class StObject
    {
        /// <summary>
        /// 对象的ID
        /// </summary>
        public int ObjID { get; set; }
        /// <summary>
        /// 对象的名称
        /// </summary>
        public string ObjName { get; set; }
        /// <summary>
        /// 列的ID  如果有的话  如果没有则默认为-1
        /// </summary>
        public int ColID { get; set; }
        /// <summary>
        /// 对象的类型
        /// </summary>
        public StObjectType Type { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建的文本
        /// </summary>
        public string CreateText { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum StObjectType
    {
        /// <summary>
        /// 用户表   
        /// </summary>
        U,
        /// <summary>
        /// 视图 
        /// </summary>
        V,
        /// <summary>
        /// 主键  
        /// </summary>
        PK,
        /// <summary>
        /// Check约束   
        /// </summary>
        C,
        /// <summary>
        /// 默认约束  
        /// </summary>
        D,
        /// <summary>
        /// 存储过程  
        /// </summary>
        P,
        /// <summary>
        /// 触发器 
        /// </summary>
        TR,
        /// <summary>
        /// 外键  
        /// </summary>
        F,
        /// <summary>
        /// 用户自定义表类型   
        /// </summary>
        TT,
        /// <summary>
        /// 内嵌表函数   
        /// </summary>
        IF,
        /// <summary>
        /// UNIQUE约束   
        /// </summary>
        UQ,
        /// <summary>
        /// 日志    
        /// </summary>
        L,
        /// <summary>
        /// 扩展存储过程  
        /// </summary>
        X,
        /// <summary>
        /// 表函数    
        /// </summary>
        TF,
        /// <summary>
        /// 系统表    
        /// </summary>
        S,
        /// <summary>
        /// 复制筛选存储过程   
        /// </summary>
        RF,
        /// <summary>
        /// 标量函数
        /// </summary>
        FN
    }
}
