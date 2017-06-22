using System;
using System.Collections.Generic;

namespace HMS.Web.Ui.Data
{
    internal class Block
    {
        internal List<EventColumn> Columns;

        internal List<EventPart> eventParts = new List<EventPart>();

        private string[] SortDirections;

        internal DateTime BoxStart
        {
            get
            {
                DateTime dateTime = DateTime.MaxValue;
                foreach (EventPart current in this.eventParts)
                {
                    if (current.BoxStart < dateTime)
                    {
                        dateTime = current.BoxStart;
                    }
                }
                return dateTime;
            }
        }

        internal DateTime BoxEnd
        {
            get
            {
                DateTime dateTime = DateTime.MinValue;
                foreach (EventPart current in this.eventParts)
                {
                    if (current.BoxEnd > dateTime)
                    {
                        dateTime = current.BoxEnd;
                    }
                }
                return dateTime;
            }
        }

        internal Block(string[] sortDirections)
        {
            this.SortDirections = sortDirections;
        }

        internal void Add(EventPart ev)
        {
            this.eventParts.Add(ev);
            this.ArrangeColumns();
        }

        private EventColumn CreateColumn()
        {
            EventColumn eventColumn = new EventColumn();
            this.Columns.Add(eventColumn);
            eventColumn.Block = this;
            return eventColumn;
        }

        private void ArrangeColumns()
        {
            this.Columns = new List<EventColumn>();
            this.eventParts.Sort(new EventPartComparerCustom(this.SortDirections));
            foreach (EventPart current in this.eventParts)
            {
                current.EvColumn = null;
            }
            this.CreateColumn();
            foreach (EventPart current2 in this.eventParts)
            {
                foreach (EventColumn current3 in this.Columns)
                {
                    if (current3.CanAdd(current2))
                    {
                        current3.Add(current2);
                        break;
                    }
                }
                if (current2.EvColumn == null)
                {
                    EventColumn eventColumn = this.CreateColumn();
                    eventColumn.Add(current2);
                }
            }
        }

        internal bool OverlapsWith(EventPart e)
        {
            return this.eventParts.Count != 0 && this.BoxStart < e.BoxEnd && this.BoxEnd > e.BoxStart;
        }
    }
}
