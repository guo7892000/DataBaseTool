using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breezee.WorkHelper.Core
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class DBaseEntity
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public abstract string TABLE_NAME { get; }
        public SaveTypeEnum SaveType { get; set; }
    }

    public enum SaveTypeEnum
    {
        Add,
        Update,
        Delete
    }
}
