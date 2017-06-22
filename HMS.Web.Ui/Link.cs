using HMS.Web.App.Ui.Data;
using HMS.Web.App.Ui.Enums.Gantt;
using System;
using System.Collections;

namespace HMS.Web.App.Ui
{
    [Serializable]
    public class Link
    {
        public string Id
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public LinkType Type
        {
            get;
            set;
        }

        public object DataItem
        {
            get;
            set;
        }

        public LinkTagDictionary Tags
        {
            get;
            set;
        }

        public Link()
        {
            this.Type = LinkType.FinishToStart;
            this.Tags = new LinkTagDictionary();
        }

        public Link(string from, string to) : this()
        {
            this.From = from;
            this.To = to;
        }

        internal Hashtable ToJson()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["id"] = this.Id;
            hashtable["from"] = this.From;
            hashtable["to"] = this.To;
            hashtable["type"] = this.Type.ToString();
            Hashtable hashtable2 = this.Tags.ToJson();
            if (hashtable2.Count > 0)
            {
                hashtable["tags"] = hashtable2;
            }
            return hashtable;
        }
    }
}
