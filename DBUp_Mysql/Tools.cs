using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class Tools
    {
        public static string GetTimeSpan(TimeSpan ts)
        {
            List<int> rel = new List<int>();
            if (ts.Hours > 0)
            {
                rel.Add(ts.Hours);
            }

            if (ts.Minutes > 0)
                rel.Add(ts.Minutes);
            else if (rel.Count > 0)
                rel.Add(0);
            rel.Add(ts.Seconds);
            return string.Join(":", rel.Select(i => i.ToString().PadLeft(2, '0')));
        }

    }
    public class SortingOption<T>
    {
        private FieldSortedOption<T> _fieldSortedOption = null;
        public SortingOption()
        {
            //默认当字符串处理
            Equa = (T t1, T t2) =>
            {
                if (t1 == null && t2 == null)
                    return true;
                if (t1 == null || t2 == null)
                    return false;

                return t1.ToString().Equals(t2.ToString());
            };
        }

        public SortingOption(Func<T, T, bool> func)
        {
            Equa = (T t1, T t2) => func(t1, t2);
        }


        public bool GetSortedOption(ref FieldSortedOption<T> fieldSortedOption)
        {
            if (fieldSortedOption.OldList == null || fieldSortedOption.NewList == null)
                return false;
            if (fieldSortedOption.OldList.Count != fieldSortedOption.NewList.Count)
                return false;
            if (fieldSortedOption.Options == null)
                fieldSortedOption.Options = new List<SortedOption<T>>();
            else return true;

            _fieldSortedOption = fieldSortedOption;

            SortedOption<T> optionItem;
            for (int inx = 0; inx < fieldSortedOption.OldList.Count; inx++)
            {
                T oldItem = fieldSortedOption.OldList[inx];
                optionItem = new SortedOption<T>()
                {
                    NewValue = inx == 0 ? default(T) : fieldSortedOption.NewList[inx - 1],
                    OptionValue = fieldSortedOption.NewList[inx],
                    OptionType = inx == 0 ? SortedOptionType.FIRST : SortedOptionType.AFTER
                };
                if (Equa(oldItem, fieldSortedOption.NewList[inx]))
                {
                    optionItem.OptionType = SortedOptionType.NONE;
                    fieldSortedOption.Options.Add(optionItem);
                    continue;
                }
                fieldSortedOption.Checked = true;
                int newInx = _getIndex(fieldSortedOption.OldList, fieldSortedOption.NewList[inx]);
                fieldSortedOption.Options.Add(optionItem);
                _changeSort(fieldSortedOption.OldList, newInx, inx);
            }
            return true;
        }


        public bool IsChecked(T value,out SortedOption<T> sortedOption)
        {
            sortedOption = null;
            if (_fieldSortedOption.Options == null || _fieldSortedOption.Options.Count == 0)
                GetSortedOption(ref _fieldSortedOption);
            if (_fieldSortedOption.Options?.Count != _fieldSortedOption.OldList.Count)
                return false;
            if (_fieldSortedOption.Checked == false)
                return false;
            sortedOption = _fieldSortedOption.Options.FirstOrDefault(i => Equa(i.OptionValue, value));
            if (sortedOption?.OptionType != SortedOptionType.NONE)
                return true;
            return false;
        }

        private bool _changeSort(List<T> list, int delInx,int insInx)
        {
            T inxItem = list[delInx];
            //if (Equa(inxItem, sortType.NewValue))
            //{
            //    sortType.OptionType = SortedOptionType.NONE;
            //    return true;
            //}
            list.RemoveAt(delInx);
            list.Insert(insInx, inxItem);
            return true;
        }

        private int _getIndex(List<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (Equa(item, list[i]))
                    return i;
            }
            return -1;
        }

        private delegate bool EquaHander(T t1, T t2);

        private EquaHander Equa;

    }

}

