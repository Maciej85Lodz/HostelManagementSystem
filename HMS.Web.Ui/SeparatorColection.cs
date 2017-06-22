using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(SeparatorCollectionConverter))]
    [Serializable]
    public class SeparatorCollection : CollectionBase
    {
        internal bool designMode;

        public Separator this[int index]
        {
            get
            {
                return (Separator)base.List[index];
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

        public int Add(Separator value)
        {
            return base.List.Add(value);
        }

        public int Add(DateTime location, Color color)
        {
            return this.Add(new Separator(location, color));
        }

        public int Add(DateTime location, Color color, SeparatorLayer layer, int width, int opacity)
        {
            return this.Add(new Separator(location, color)
            {
                Layer = layer,
                Width = width,
                Opacity = opacity
            });
        }

        public int IndexOf(Separator value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Separator value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Separator value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Separator value)
        {
            return base.List.Contains(value);
        }

        public SeparatorCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Separator)
                {
                    this.Add((Separator)items[i]);
                }
            }
        }

        public SeparatorCollection()
        {
        }

        internal List<Hashtable> GetList()
        {
            List<Hashtable> list = new List<Hashtable>();
            foreach (Separator separator in this)
            {
                Hashtable hashtable = new Hashtable();
                hashtable["Location"] = separator.Location.ToString("s");
                hashtable["Color"] = ColorTranslator.ToHtml(separator.Color);
                if (separator.Width != 1)
                {
                    hashtable["Width"] = separator.Width;
                }
                if (separator.Layer != SeparatorLayer.BelowEvents)
                {
                    hashtable["Layer"] = separator.Layer;
                }
                if (separator.Opacity != 100)
                {
                    hashtable["Opacity"] = separator.Opacity;
                }
                list.Add(hashtable);
            }
            return list;
        }

        internal string GetHash()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(this.GetList()));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }
    }
}
