using System;
using System.Collections;


namespace HMS.Web.App.Ui
{
    [Serializable]
    public class TimeCell
    {
        public DateTime Start
        {
            get;
            set;
        }

        public DateTime End
        {
            get;
            set;
        }

        public int? Width
        {
            get;
            set;
        }

        public TimeCell()
        {
        }

        public TimeCell(DateTime start, DateTime end, int width) : this()
        {
            this.Start = start;
            this.End = end;
            this.Width = new int?(width);
        }

        public TimeCell(DateTime start, DateTime end) : this()
        {
            this.Start = start;
            this.End = end;
        }

        internal Hashtable GetJson()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["start"] = this.Start;
            hashtable["end"] = this.End;
            if (this.Width.HasValue)
            {
                hashtable["width"] = this.Width;
            }
            return hashtable;
        }
    }
}
