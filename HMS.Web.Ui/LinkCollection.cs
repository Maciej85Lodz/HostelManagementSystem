using HMS.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(LinkCollectionConverter))]
    [Serializable]
    public class LinkCollection : CollectionBase
    {
        internal bool designMode;

        public Link this[int index]
        {
            get
            {
                return (Link)base.List[index];
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

        public int Add(Link value)
        {
            return base.List.Add(value);
        }

        public int Add(string from, string to)
        {
            return this.Add(new Link(from, to));
        }

        public int IndexOf(Link value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Link value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Link value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Link value)
        {
            return base.List.Contains(value);
        }

        public LinkCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Link)
                {
                    this.Add((Link)items[i]);
                }
            }
        }

        public LinkCollection()
        {
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            LinkCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(LinkCollection collection, JsonData tree)
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
                Link link = new Link();
                link.Id = (string)jsonData["id"];
                link.From = (string)jsonData["from"];
                link.To = (string)jsonData["to"];
                link.Type = LinkTypeParser.Parse((string)jsonData["type"]);
                link.Tags.RestoreFromJson(jsonData["tags"]);
                collection.Add(link);
            }
        }

        public Link FindById(string id)
        {
            foreach (Link link in this)
            {
                if (link.Id == id)
                {
                    return link;
                }
            }
            return null;
        }

        public Link FindByFromTo(string from, string to)
        {
            foreach (Link link in this)
            {
                if (link.From == from && link.To == to)
                {
                    return link;
                }
            }
            return null;
        }

        internal List<Hashtable> ToJson()
        {
            List<Hashtable> list = new List<Hashtable>();
            foreach (Link link in this)
            {
                list.Add(link.ToJson());
            }
            return list;
        }

        public void Add(string from, string to, LinkType type, string id)
        {
            this.Add(new Link
            {
                From = from,
                To = to,
                Id = id,
                Type = type
            });
        }
    }
}
