using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class DBSqlHelper
    {

        #region 事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public const string StartTranSQL = "start transaction;";
        /// <summary>
        /// 提交事务
        /// </summary>
        public const string CommitTranSQL = "commit;";
        /// <summary>
        /// 回滚事务
        /// </summary>
        public const string RollbackTranSQL = "rollback;";
        /// <summary>
        /// 当前数据库事务类型
        /// </summary>
        public const string CurrIsolationSQL = "SELECT @@tx_isolation;";

        public const string _ChangeTranIsolationLevelSql = "set {0} transaction isolation level {1};";
        /// <summary>
        /// 获取更改数据库事务隔离级别的sql   如需永久更改需要到mysql.conf文件【transaction-isolation =】项
        /// </summary>
        /// <param name="sessionType">会话类型</param>
        /// <param name="level">事务隔离级别  （<see cref="DBChangeTrancstionConst"/>的值之一）</param>
        /// <returns></returns>
        public static string GetChangeTranIsolationLevelSql(DbSettingSessionType sessionType, string level)
        {
            return string.Format(_ChangeTranIsolationLevelSql, sessionType.ToString(), level);
        }
        #endregion

        /// <summary>
        /// 数据库版本
        /// </summary>
        public const string VersionSQL = "select VERSION();";
        /// <summary>
        /// 数据库sqlmodel
        /// </summary>
        public const string DBMODE_SQL = "select @@global.sql_mode;";


    }
}
