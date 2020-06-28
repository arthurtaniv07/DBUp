using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    /// <summary>
    /// 移动操作步骤
    /// </summary>
    public class FieldSortedOption<T>
    {
        public List<T> OldList { get; set; }
        public List<T> NewList { get; set; }
        public List<SortedOption<T>> Options { get; set; }

        public bool Checked { get; set; } = false;
    }
    /// <summary>
    /// 将<see cref="OptionValue"/> 移动到<see cref="OptionType"/>  (于<see cref="NewValue"/>)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedOption<T>
    {
        /// <summary>
        /// 操作的值
        /// </summary>
        public T OptionValue { get; set; }

        public T NewValue { get; set; }
        /// <summary>
        /// <see cref="NewValue"/>在<see cref="OptionValue"/>之后 或者第一位
        /// </summary>
        public SortedOptionType OptionType { get; set; }
    }
}
