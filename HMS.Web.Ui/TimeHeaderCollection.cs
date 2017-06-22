using HMS.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(TimeHeaderCollectionConverter))]
    [Serializable]
    public class TimeHeaderCollection : CollectionBase
    {
        public TimeHeader this[int index]
        {
            get
            {
                return (TimeHeader)base.List[index];
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

        public int Add(TimeHeader value)
        {
            return base.List.Add(value);
        }

        public int IndexOf(TimeHeader value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, TimeHeader value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(TimeHeader value)
        {
            base.List.Remove(value);
        }

        public bool Contains(TimeHeader value)
        {
            return base.List.Contains(value);
        }

        public TimeHeaderCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is TimeHeader)
                {
                    this.Add((TimeHeader)items[i]);
                }
            }
        }

        public TimeHeaderCollection()
        {
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            TimeHeaderCollection.RestoreCollection(this, tree);
        }

        internal JsonData ToJson()
        {
            JsonData jsonData = new JsonData();
            foreach (TimeHeader timeHeader in this)
            {
                jsonData.Add(timeHeader.ToJson());
            }
            return jsonData;
        }

        private static void RestoreCollection(TimeHeaderCollection collection, JsonData json)
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
                TimeHeader value = TimeHeader.FromJson(node);
                collection.Add(value);
            }
        }
    }
}
