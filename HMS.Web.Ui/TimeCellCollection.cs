using HMS.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(TimeCellCollectionConverter))]
    [Serializable]
    public class TimeCellCollection : CollectionBase
    {
        public TimeCell this[int index]
        {
            get
            {
                return (TimeCell)base.List[index];
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

        public int Add(TimeCell value)
        {
            return base.List.Add(value);
        }

        public int Add(DateTime start, DateTime end)
        {
            return this.Add(new TimeCell(start, end));
        }

        public int IndexOf(TimeCell value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, TimeCell value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(TimeCell value)
        {
            base.List.Remove(value);
        }

        public bool Contains(TimeCell value)
        {
            return base.List.Contains(value);
        }

        public TimeCellCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is TimeCell)
                {
                    this.Add((TimeCell)items[i]);
                }
            }
        }

        public TimeCellCollection()
        {
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            this.restoreCollection(this, tree);
        }

        private void restoreCollection(TimeCellCollection collection, JsonData tree)
        {
            if (tree == null)
            {
                return;
            }
            if (tree.IsNull)
            {
                return;
            }
            if (!tree.IsArray)
            {
                throw new ArgumentException("Array JsonData expected. Received: " + tree.GetJsonType());
            }
            foreach (JsonData jsonData in ((IEnumerable)tree))
            {
                TimeCell timeCell = new TimeCell();
                timeCell.Start = (DateTime)jsonData["start"];
                timeCell.End = (DateTime)jsonData["end"];
                if (jsonData["width"] != null)
                {
                    timeCell.Width = new int?((int)jsonData["width"]);
                }
                collection.Add(timeCell);
            }
        }

        internal List<Hashtable> ToJson()
        {
            List<Hashtable> list = new List<Hashtable>();
            foreach (TimeCell timeCell in this)
            {
                list.Add(timeCell.GetJson());
            }
            return list;
        }

        public int Add(DateTime start, DateTime end, int width)
        {
            return this.Add(new TimeCell(start, end, width));
        }
    }
}
