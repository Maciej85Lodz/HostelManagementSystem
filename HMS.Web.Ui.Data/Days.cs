using HMS.Web.Ui.Enums;
using HMS.Web.Ui.Events.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HMS.Web.Ui.Data
{
    internal class Day : ISerializable
    {
        internal List<EventPart> eventParts = new List<EventPart>();

        internal List<EventPart> allDayEvents = new List<EventPart>();

        internal List<Block> blocks = new List<Block>();

        internal bool IsResourcesView;

        internal bool IsGantt;

        internal int cellDuration;

        protected DateTime start;

        internal DateTime end;

        internal string Name;

        internal string Id;

        internal string ToolTip;

        internal UseBoxesEnum UseEventBoxes;

        internal DateTimeSpec AllDayEnd = DateTimeSpec.DateTime;

        internal List<ResourceColumn> columns;

        internal string[] SortDirections;

        internal BeforeResHeaderRenderEventArgs Args;

        internal DataItemWrapper DataItem
        {
            get;
            set;
        }

        internal DateTime Start
        {
            get
            {
                return this.start;
            }
        }

        internal DateTime End
        {
            get
            {
                return this.end;
            }
        }

        private Block LastBlock
        {
            get
            {
                if (this.blocks.Count == 0)
                {
                    return null;
                }
                return this.blocks[this.blocks.Count - 1];
            }
        }

        internal DateTime MinStart
        {
            get
            {
                if (this.LastBlock == null)
                {
                    return this.end;
                }
                return this.blocks[0].BoxStart;
            }
        }

        internal DateTime MaxEnd
        {
            get
            {
                if (this.LastBlock == null)
                {
                    return this.start;
                }
                return this.LastBlock.BoxEnd;
            }
        }

        internal Day()
        {
        }

        internal Day(DateTime start, DateTime end, string header, string id, int cellDuration, string toolTip, string[] sortDirections)
        {
            this.start = start;
            this.end = end;
            this.Name = header;
            this.Id = id;
            this.cellDuration = cellDuration;
            this.ToolTip = toolTip;
            this.SortDirections = sortDirections;
            this.IsResourcesView = !string.IsNullOrEmpty(this.Id);
        }

        private void stripAndAddEvent(Event e)
        {
            if (this.IsGantt)
            {
                if (this.Id != e.Id)
                {
                    return;
                }
            }
            else if (this.IsResourcesView && this.Id != e.ResourceId)
            {
                return;
            }
            if (e.AllDay)
            {
                if (e.Start > e.End)
                {
                    return;
                }
                if (e.Start > this.End)
                {
                    return;
                }
                if (this.AllDayEnd == DateTimeSpec.DateTime)
                {
                    if (e.End <= this.Start)
                    {
                        return;
                    }
                }
                else if (e.End < this.Start)
                {
                    return;
                }
                EventPart item = new EventPart(this, e);
                this.allDayEvents.Add(item);
                return;
            }
            else
            {
                if (e.End < this.Start)
                {
                    return;
                }
                if (e.End == this.Start && e.Start != e.End)
                {
                    return;
                }
                if (e.Start >= this.End)
                {
                    return;
                }
                if (e.Start > e.End)
                {
                    return;
                }
                EventPart item2 = new EventPart(this, e);
                this.eventParts.Add(item2);
                return;
            }
        }

        internal void Load(ArrayList events)
        {
            if (events == null)
            {
                return;
            }
            foreach (Event e in events)
            {
                this.stripAndAddEvent(e);
            }
            this.putIntoBlocks();
        }

        private void putIntoBlocks()
        {
            foreach (EventPart current in this.eventParts)
            {
                if (this.LastBlock == null)
                {
                    this.blocks.Add(new Block(this.SortDirections));
                }
                else if (!this.LastBlock.OverlapsWith(current))
                {
                    this.blocks.Add(new Block(this.SortDirections));
                }
                this.LastBlock.Add(current);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        internal int MaxColumns()
        {
            int num = 1;
            foreach (Block current in this.blocks)
            {
                if (current.Columns.Count > num)
                {
                    num = current.Columns.Count;
                }
            }
            return num;
        }
    }
}
