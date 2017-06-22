using System;
using System.Collections.Generic;
using HMS.Web.Ui.Data;


namespace HMS.Web.Ui.Conflict
{
    public class Conflict
    {
        private Block b;

        private Day d;

        public List<ConflictEvent> Events
        {
            get;
            private set;
        }

        public DateTime Start
        {
            get
            {
                return this.b.BoxStart;
            }
        }

        public DateTime End
        {
            get
            {
                return this.b.BoxEnd;
            }
        }

        public int Level
        {
            get
            {
                return this.b.Columns.Count;
            }
        }

        public string Resource
        {
            get
            {
                return this.d.Id;
            }
        }

        internal Conflict(Day d, Block b)
        {
            this.b = b;
            this.d = d;
            this.Events = new List<ConflictEvent>();
            foreach (EventPart current in b.eventParts)
            {
                ConflictEvent item = new ConflictEvent(current);
                this.Events.Add(item);
            }
        }
    }
}
