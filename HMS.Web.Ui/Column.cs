using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
namespace HMS.Web.App.Ui
{
    [Serializable]
    public class Column
    {
        private readonly ColumnCollection _children = new ColumnCollection();

        private static readonly Column Empty = new Column();

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

        public DateTime Date
        {
            get;
            set;
        }

        public int Width
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
        public ColumnCollection Children
        {
            get
            {
                return this._children;
            }
        }

        public Column()
        {
            this.Date = DateTime.MinValue;
        }

        public Column(string name, string id)
        {
            this.Date = DateTime.MinValue;
            this.Name = name;
            this.Id = id;
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal int GetChildrenCount(int level)
        {
            int num = 0;
            if (this._children.Count <= 0 || level <= 1)
            {
                return 1;
            }
            foreach (Column column in this._children)
            {
                num += column.GetChildrenCount(level - 1);
            }
            return num;
        }

        internal List<Column> GetChildren(int level, bool inherit)
        {
            List<Column> list = new List<Column>();
            if (level <= 1)
            {
                list.Add(this);
                return list;
            }
            if (this._children.Count == 0)
            {
                if (inherit)
                {
                    list.Add(this);
                }
                else
                {
                    list.Add(Column.Empty);
                }
                return list;
            }
            foreach (Column column in this._children)
            {
                List<Column> children = column.GetChildren(level - 1, inherit);
                foreach (Column current in children)
                {
                    list.Add(current);
                }
            }
            return list;
        }
    }
}
