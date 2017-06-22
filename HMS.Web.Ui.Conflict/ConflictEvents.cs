using System;
using HMS.Web.Ui.Data;

namespace HMS.Web.Ui.Conflict
{
    public class ConflictEvent
    {
        public DateTime Start
        {
            get;
            private set;
        }

        public DateTime End
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        public DataItemWrapper DataItem
        {
            get;
            private set;
        }

        internal ConflictEvent(EventPart e)
        {
            this.Start = e.Event.Start;
            this.End = e.Event.End;
            this.Value = e.Event.Id;
            this.Position = e.EvColumn.Number;
            this.DataItem = new DataItemWrapper(e.Event.Source);
        }
    }
}
