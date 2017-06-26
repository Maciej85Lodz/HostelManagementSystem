using HMS.Web.Ui.Serialization;
using System;
using System.ComponentModel;

namespace HMS.Web.Ui.Data
{
    [TypeConverter(typeof(EventConverter))]
    internal class Event
    {
        internal DateTime Start;

        internal DateTime End;

        internal string Text;

        internal string Id;

        internal string ResourceId;

        internal string[] Tags;

        internal string[] ServerTags;

        internal bool AllDay;

        internal string[] Sort;

        internal bool Recurrent;

        internal string RecurrentMasterId;

        internal object Source;

        internal bool RecurrentException
        {
            get
            {
                return this.Recurrent && this.Id != null;
            }
        }

        internal Event()
        {
        }

        internal Event(DateTime start, DateTime end, string id, string text, string columnId, string[] tags, string[] serverTags, bool allDay)
        {
            this.Id = id;
            this.Start = start;
            this.End = end;
            this.Text = text;
            this.ResourceId = columnId;
            this.Tags = tags;
            this.ServerTags = serverTags;
            this.AllDay = allDay;
        }
    }
}
