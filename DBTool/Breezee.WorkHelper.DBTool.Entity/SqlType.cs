using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breezee.WorkHelper.DBTool.Entity
{
    public enum SqlType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Insert=1,

        /// <summary>
        /// 修改
        /// </summary>
        Update=2,

        /// <summary>
        /// 查询
        /// </summary>
        Query = 3,

        /// <summary>
        /// 删除
        /// </summary>
        Delete=4,

        /// <summary>
        /// 参数
        /// </summary>
        Parameter
    }
}
