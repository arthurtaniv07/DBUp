using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBUp_Mysql
{
    public class ProcCompareAndShowResultHelper : FuncCompareAndShowResultHelper, ICompareAndShowResult
    {

        public ProcCompareAndShowResultHelper()
        {
            base.isFun = false;
        }
    }
}
