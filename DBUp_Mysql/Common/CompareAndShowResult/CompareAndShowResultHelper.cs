using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUp_Mysql.Common.CompareAndShowResult
{
    public class CompareAndShowResultHelper : CompareAndShowResultHelperBase, ICompareAndShowResult
    {
        

        public CompareAndShowResultHelper()
        {
        }

        public override bool CompareAndShow(ref DbModels oldItems, ref DbModels newItems, Setting setting, out string errorString)
        {
            throw new NotImplementedException();
        }

        public override bool GetInfoByDb(string connStr, ref DbModels rel)
        {
            throw new NotImplementedException();
        }

        public override string GetInfoByFile(string dirName, string fileName, ref DbModels list)
        {
            throw new NotImplementedException();
        }
    }
}
