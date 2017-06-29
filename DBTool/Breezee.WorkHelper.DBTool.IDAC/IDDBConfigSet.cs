using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Breezee.Framework.DataAccess.INF;

namespace Breezee.WorkHelper.DBTool.IDAC
{
    public abstract class IDDBConfigSet : BaseDAC
    {
        public abstract DataTable QueryDbConfig(IDictionary<string, string> dicQuery);

        #region 查询供应商是否存在
        public abstract DataTable QueryDbConfigExist(IDictionary<string, string> dicQuery);
        #endregion
    }
}
