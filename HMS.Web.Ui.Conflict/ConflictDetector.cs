using HMS.Web.Ui.Data;
using HMS.Web.Ui.Enums;
using HMS.Web.Ui.Recurrence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;


namespace HMS.Web.Ui.Conflict
{
    public class ConflictDetector
    {
        private class Task
        {
            internal DataTable Table;

            internal string StartFieldName;

            internal string EndFieldName;

            internal string ResourceFieldName;

            internal string RecurrenceFieldName;

            internal string IdFieldName;

            internal string RecurrenceMasterIdFieldName;
        }

        private readonly List<Day> _days = new List<Day>();

        private DateTime _rangeStart;

        private DateTime _rangeEnd;

        private readonly List<Conflict> _conflicts = new List<Conflict>();

        private readonly ArrayList _events = new ArrayList();

        private readonly List<ConflictDetector.Task> _tasks = new List<ConflictDetector.Task>();

        private bool _isResourcesView;

        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        public List<Conflict> List
        {
            get
            {
                return this._conflicts;
            }
        }

        public void Load(DataTable source, string startFieldName, string endFieldName, string resourceFieldName, DateTime rangeStart, DateTime rangeEnd)
        {
            if (!string.IsNullOrEmpty(resourceFieldName))
            {
                this._isResourcesView = true;
            }
            this._rangeStart = rangeStart;
            this._rangeEnd = rangeEnd;
            this._events.Clear();
            this.AddTable(source, startFieldName, endFieldName, resourceFieldName);
            this.CreateList();
        }

        public ConflictDetector Add(DataTable source, string startFieldName, string endFieldName, string resourceFieldName)
        {
            if (!string.IsNullOrEmpty(resourceFieldName))
            {
                this._isResourcesView = true;
            }
            ConflictDetector.Task item = new ConflictDetector.Task
            {
                Table = source,
                StartFieldName = startFieldName,
                EndFieldName = endFieldName,
                ResourceFieldName = resourceFieldName
            };
            this._tasks.Add(item);
            return this;
        }

        public ConflictDetector AddRecurring(DataTable source, string startFieldName, string endFieldName, string resourceFieldName, string recurrenceFieldName, string idFieldName, string recurrenceMasterIdFieldName)
        {
            if (!string.IsNullOrEmpty(resourceFieldName))
            {
                this._isResourcesView = true;
            }
            ConflictDetector.Task item = new ConflictDetector.Task
            {
                Table = source,
                StartFieldName = startFieldName,
                EndFieldName = endFieldName,
                ResourceFieldName = resourceFieldName,
                RecurrenceFieldName = recurrenceFieldName,
                IdFieldName = idFieldName,
                RecurrenceMasterIdFieldName = recurrenceMasterIdFieldName
            };
            this._tasks.Add(item);
            return this;
        }

        private void CreateList()
        {
            this._events.Sort(new EventComparer());
            foreach (Day current in this._days)
            {
                current.Load(this._events);
            }
            foreach (Day current2 in this._days)
            {
                foreach (Block current3 in current2.blocks)
                {
                    if (current3.Columns.Count > 1)
                    {
                        this._conflicts.Add(new Conflict(current2, current3));
                    }
                }
            }
        }

        private Day dayForResource(string resource)
        {
            foreach (Day current in this._days)
            {
                if (current.Id == resource)
                {
                    return current;
                }
            }
            Day day = new Day(this._rangeStart, this._rangeEnd, null, resource, 1, null, null);
            day.UseEventBoxes = UseBoxesEnum.Never;
            day.IsResourcesView = this._isResourcesView;
            this._days.Add(day);
            return day;
        }

        public ConflictDetector ForRange(DateTime start, DateTime end)
        {
            this._rangeStart = start;
            this._rangeEnd = end;
            foreach (ConflictDetector.Task current in this._tasks)
            {
                if (string.IsNullOrEmpty(current.RecurrenceFieldName))
                {
                    this.AddTable(current.Table, current.StartFieldName, current.EndFieldName, current.ResourceFieldName);
                }
                else
                {
                    DataTable source = RecurrenceExpander.Expand(current.Table, current.RecurrenceFieldName, current.StartFieldName, current.EndFieldName, current.IdFieldName, current.RecurrenceMasterIdFieldName, start, end);
                    this.AddTable(source, current.StartFieldName, current.EndFieldName, current.ResourceFieldName);
                }
            }
            this.CreateList();
            return this;
        }

        private void AddTable(DataTable source, string startFieldName, string endFieldName, string resourceFieldName)
        {
            foreach (DataRow dataRow in source.Rows)
            {
                DateTime start = Convert.ToDateTime(dataRow[startFieldName]);
                DateTime end = Convert.ToDateTime(dataRow[endFieldName]);
                string text = string.IsNullOrEmpty(resourceFieldName) ? null : Convert.ToString(dataRow[resourceFieldName]);
                Event @event = new Event(start, end, null, null, text, null, null, false);
                @event.Source = dataRow;
                this._events.Add(@event);
                this.dayForResource(text);
            }
        }
    }
}
