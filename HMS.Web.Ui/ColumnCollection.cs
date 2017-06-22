using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using HMS.Json;
using HMS.Web.App.Ui.Serialization;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(ColumnCollectionConverter))]
    [Serializable]
    public class ColumnCollection : CollectionBase
    {
        internal bool designMode;

        public Column this[int index]
        {
            get
            {
                return (Column)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        public ArrayList ToArrayList()
        {
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < base.Count; i++)
            {
                arrayList.Add(this[i]);
            }
            return arrayList;
        }

        public int Add(Column value)
        {
            return base.List.Add(value);
        }

        public int Add(string name, string id)
        {
            return this.Add(new Column(name, id));
        }

        public int IndexOf(Column value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Column value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Column value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Column value)
        {
            return base.List.Contains(value);
        }

        public ColumnCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Column)
                {
                    this.Add((Column)items[i]);
                }
            }
        }

        public ColumnCollection()
        {
        }

        public int GetColumnCount(int level)
        {
            int num = 0;
            foreach (Column column in base.List)
            {
                num += column.GetChildrenCount(level);
            }
            return num;
        }

        public List<Column> GetColumns(int level, bool inherit)
        {
            List<Column> list = new List<Column>();
            foreach (Column column in base.List)
            {
                List<Column> children = column.GetChildren(level, inherit);
                list.AddRange(children);
            }
            return list;
        }

        public override string ToString()
        {
            return "(Collection)";
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            ColumnCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(ColumnCollection collection, JsonData tree)
        {
            if (tree == null || tree.IsNull)
            {
                return;
            }
            if (!tree.IsArray)
            {
                throw new ArgumentException("Array JsonData expected. Received: " + tree.GetJsonType());
            }
            foreach (JsonData jsonData in ((IEnumerable)tree))
            {
                Column column = new Column();
                column.Name = (string)jsonData["Name"];
                column.Id = (string)jsonData["Value"];
                column.ToolTip = (string)jsonData["ToolTip"];
                column.Date = (DateTime)jsonData["Date"];
                collection.Add(column);
                ColumnCollection.RestoreCollection(column.Children, jsonData["Children"]);
            }
        }
    }
}
