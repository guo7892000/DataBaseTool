namespace Breezee.WorkHelper.DBTool.Entity
{
    using System;

    public class EntityInfo
    {
        private string accessType;
        private string entityName;
        private string nameSpace;
        private string path;

        public EntityInfo()
        {
        }

        public EntityInfo(string nameSpace, string accessType, string entityName, string path)
        {
            this.nameSpace = nameSpace;
            this.accessType = accessType;
            this.entityName = entityName;
            this.path = path;
        }

        public string AccessType
        {
            get
            {
                return this.accessType;
            }
            set
            {
                this.accessType = value;
            }
        }

        public string EntityName
        {
            get
            {
                return this.entityName;
            }
            set
            {
                this.entityName = value;
            }
        }

        public string NameSpace
        {
            get
            {
                return this.nameSpace;
            }
            set
            {
                this.nameSpace = value;
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }
    }
}

