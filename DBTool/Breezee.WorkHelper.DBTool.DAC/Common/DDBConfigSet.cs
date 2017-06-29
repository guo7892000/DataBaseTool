using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Breezee.Framework.DataAccess.INF;
using Breezee.Global.Context;
using Breezee.WorkHelper.DBTool.IDAC;

namespace Breezee.WorkHelper.DBTool.DAC
{
    public class DDBConfigSet : IDDBConfigSet
    {
        public override DataTable QueryDbConfig(IDictionary<string, string> dicQuery)
        {
            IDataAccess dac = ContainerContext.Container.Resolve<IDataAccess>();
            return dac.QueryDataFromConfigPath(@"Sql/DbTool/DbConfigSet/QueryDbConfig", dicQuery);
        }

        #region 查询供应商是否存在
        public override DataTable QueryDbConfigExist(IDictionary<string, string> dicQuery)
        {
            //查询并返回结果
            var querySqlBuilder = new QuerySqlBuilder(DataAccess);
            querySqlBuilder.Sql(
            @"SELECT 1 FROM WH_BD_DB_CONFIG A
                WHERE 1=1
                AND A.DB_CONFIG_CODE = @DB_CONFIG_CODE
            ");
            querySqlBuilder.Sql("DB_CONFIG_ID", "AND A.DB_CONFIG_ID <> @DB_CONFIG_ID");
            querySqlBuilder.Sql("DB_CONFIG_CODE", ""); //此处必须添加
            return querySqlBuilder.Query(dicQuery);
        }
        #endregion
    }
}
