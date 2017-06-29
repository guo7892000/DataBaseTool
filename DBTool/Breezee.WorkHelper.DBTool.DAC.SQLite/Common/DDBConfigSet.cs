using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Breezee.Framework.DataAccess.INF;
using Breezee.Global.Context;
using Breezee.WorkHelper.DBTool.IDAC;

namespace Breezee.WorkHelper.DBTool.DAC.SQLite
{
    public class DDBConfigSet : Breezee.WorkHelper.DBTool.DAC.DDBConfigSet
    {
        /// <summary>
        /// SQLite需要对时间字符串做转换
        /// STRFTIME(A.CREATE_TIME) AS CREATE_TIME
        /// </summary>
        /// <param name="dicQuery"></param>
        /// <returns></returns>
        public override DataTable QueryDbConfig(IDictionary<string, string> dicQuery)
        {
            IDataAccess dac = ContainerContext.Container.Resolve<IDataAccess>();
            return dac.QueryDataFromConfigPath(@"Sql/DbTool/DbConfigSet/QueryDbConfig/SQLite", dicQuery);
        }
    }
}
