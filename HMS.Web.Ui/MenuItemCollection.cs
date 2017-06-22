using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Web.App.Ui
{
    public class MenuItemCollection : CollectionBase
    {
        public MenuItem this[int index]
        {
            get
            {
                return (MenuItem)base.List[index];
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

        public int Add(MenuItem value)
        {
            return base.List.Add(value);
        }

        public int IndexOf(MenuItem value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, MenuItem value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(MenuItem value)
        {
            base.List.Remove(value);
        }

        public bool Contains(MenuItem value)
        {
            return base.List.Contains(value);
        }

        public MenuItemCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is MenuItem)
                {
                    this.Add((MenuItem)items[i]);
                }
            }
        }

        public MenuItemCollection()
        {
        }

        public string ToJavaScript()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            foreach (MenuItem menuItem in this)
            {
                string value = menuItem.ToJavaScript();
                stringBuilder.Append(value);
                if (this.IndexOf(menuItem) != base.Count - 1)
                {
                    stringBuilder.AppendLine(",");
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }
}
