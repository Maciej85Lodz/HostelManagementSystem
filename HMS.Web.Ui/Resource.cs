using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

namespace HMS.Web.App.Ui
{
    [Serializable]
    public class Resource
    {
        private ResourceCollection children = new ResourceCollection();

        private List<ResourceColumn> columns = new List<ResourceColumn>();

        internal static Resource Empty = new Resource();

        [Obsolete("Use .Id instead.")]
        public string Value
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }

        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ToolTip
        {
            get;
            set;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), PersistenceMode(PersistenceMode.InnerProperty)]
        public ResourceCollection Children
        {
            get
            {
                return this.children;
            }
        }

        public bool Expanded
        {
            get;
            set;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), PersistenceMode(PersistenceMode.InnerProperty)]
        public List<ResourceColumn> Columns
        {
            get
            {
                return this.columns;
            }
        }

        [Obsolete("Use !DynamicChildren instead.")]
        public bool ChildrenLoaded
        {
            get
            {
                return !this.DynamicChildren;
            }
            set
            {
                this.DynamicChildren = !value;
            }
        }

        public bool DynamicChildren
        {
            get;
            set;
        }

        public bool IsParent
        {
            get;
            set;
        }

        public object DataItem
        {
            get;
            set;
        }

        public Resource()
        {
            this.Expanded = false;
        }

        public Resource(string name, string id)
        {
            this.Expanded = false;
            this.Name = name;
            this.Id = id;
        }

        [Obsolete("Use Resource(name, id) and DynamicChildren property instead.")]
        public Resource(string name, string id, bool childrenLoaded)
        {
            this.Expanded = false;
            this.Name = name;
            this.Id = id;
            this.ChildrenLoaded = childrenLoaded;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
