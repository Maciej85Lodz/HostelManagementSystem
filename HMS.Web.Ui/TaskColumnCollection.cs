using HMS.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(TaskColumnCollectionConverter))]
    [Serializable]
    public class TaskColumnCollection : CollectionBase
    {
        internal bool designMode;

        public TaskColumn this[int index]
        {
            get
            {
                return (TaskColumn)base.List[index];
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

        public int Add(TaskColumn value)
        {
            return base.List.Add(value);
        }

        public int Add(string title, int width, string field)
        {
            return this.Add(new TaskColumn(title, width, field));
        }

        public int IndexOf(TaskColumn value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, TaskColumn value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(TaskColumn value)
        {
            base.List.Remove(value);
        }

        public bool Contains(TaskColumn value)
        {
            return base.List.Contains(value);
        }

        public TaskColumnCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is TaskColumn)
                {
                    this.Add((TaskColumn)items[i]);
                }
            }
        }

        public TaskColumnCollection()
        {
        }

        public override string ToString()
        {
            return "(Collection)";
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            TaskColumnCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(TaskColumnCollection collection, JsonData data)
        {
            if (data == null || data.IsNull)
            {
                return;
            }
            if (!data.IsArray)
            {
                throw new ArgumentException("Array JsonData expected. Received: " + data.GetJsonType());
            }
            foreach (JsonData jsonData in ((IEnumerable)data))
            {
                collection.Add(new TaskColumn
                {
                    Title = (string)jsonData["title"],
                    Width = (int)jsonData["width"],
                    Property = (string)jsonData["property"]
                });
            }
        }

        internal JsonData ToJson()
        {
            JsonData jsonData = new JsonData();
            foreach (TaskColumn taskColumn in this)
            {
                jsonData.Add(taskColumn.ToJson());
            }
            return jsonData;
        }
    }
}
