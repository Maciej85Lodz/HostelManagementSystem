using HMS.Json;
using HMS.Web.App.Ui.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;


namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(RowHeaderColumnCollectionConverter))]
    [Serializable]
    public class RowHeaderColumnCollection : CollectionBase
    {
        internal bool designMode;

        public RowHeaderColumn this[int index]
        {
            get
            {
                return (RowHeaderColumn)base.List[index];
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

        public int Add(RowHeaderColumn value)
        {
            return base.List.Add(value);
        }

        public int Add(string title, int width)
        {
            return this.Add(new RowHeaderColumn(title, width));
        }

        public int IndexOf(RowHeaderColumn value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, RowHeaderColumn value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(RowHeaderColumn value)
        {
            base.List.Remove(value);
        }

        public bool Contains(RowHeaderColumn value)
        {
            return base.List.Contains(value);
        }

        public RowHeaderColumnCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is RowHeaderColumn)
                {
                    this.Add((RowHeaderColumn)items[i]);
                }
            }
        }

        public RowHeaderColumnCollection()
        {
        }

        internal List<JsonData> GetList()
        {
            if (base.Count == 0)
            {
                return null;
            }
            List<JsonData> list = new List<JsonData>();
            foreach (RowHeaderColumn rowHeaderColumn in this)
            {
                list.Add(rowHeaderColumn.ToJson());
            }
            return list;
        }

        internal string GetHash()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(this.GetList()));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            RowHeaderColumnCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(RowHeaderColumnCollection collection, JsonData json)
        {
            if (json == null || json.IsNull)
            {
                return;
            }
            if (!json.IsArray)
            {
                throw new ArgumentException("Array JsonData expected. Received: " + json.GetJsonType());
            }
            foreach (JsonData node in ((IEnumerable)json))
            {
                RowHeaderColumn value = RowHeaderColumn.FromJson(node);
                collection.Add(value);
            }
        }
    }
}
