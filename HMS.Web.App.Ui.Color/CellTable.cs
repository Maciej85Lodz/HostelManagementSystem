using HMS.Web.App.Ui.Data;
using HMS.Web.App.Ui.Enums.Scheduler;
using HMS.Web.App.Ui.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Web.App.Ui.Color
{
    internal class CellTable
    {
        internal class PropCache
        {
            private readonly int _x;

            private readonly int _y;

            private readonly CellTable.Props[] _array;

            private int _count;

            private readonly List<CellTable.Props> _values = new List<CellTable.Props>();

            public int Count
            {
                get
                {
                    return this._count;
                }
            }

            public IEnumerable<CellTable.Props> Values
            {
                get
                {
                    return this._values;
                }
            }

            internal PropCache(int x, int y)
            {
                this._x = x;
                this._y = y;
                this._array = new CellTable.Props[x * y];
            }

            private int Position(int x, int y)
            {
                return y * this._x + x;
            }

            internal void Set(int x, int y, CellTable.Props props)
            {
                if (props == null)
                {
                    throw new Exception("PropCache doesn't accept null");
                }
                int num = this.Position(x, y);
                CellTable.Props props2 = this._array[num];
                bool flag = props2 != null;
                if (flag)
                {
                    this._values.Remove(props2);
                }
                else
                {
                    this._count++;
                }
                this._values.Add(props);
                this._array[num] = props;
            }

            internal CellTable.Props Get(int x, int y)
            {
                int num = this.Position(x, y);
                return this._array[num];
            }

            public string GetHash(int x, int y)
            {
                CellTable.Props props = this.Get(x, y);
                if (props == null)
                {
                    return null;
                }
                return props.Hash;
            }
        }

        internal class Props
        {
            private readonly Hashtable _data = new Hashtable();

            private string _json;

            private string _hash;

            internal int X
            {
                get;
                private set;
            }

            internal int Y
            {
                get;
                private set;
            }

            internal bool HasItems
            {
                get
                {
                    return this._data.Keys.Count > 0;
                }
            }

            internal object this[string key]
            {
                get
                {
                    return this._data[key];
                }
                set
                {
                    this._data[key] = value;
                }
            }

            internal string Hash
            {
                get
                {
                    this.EnsureHash();
                    return this._hash;
                }
            }

            internal string Json
            {
                get
                {
                    if (string.IsNullOrEmpty(this._json))
                    {
                        this._json = SimpleJsonSerializer.Serialize(this._data);
                    }
                    return this._json;
                }
            }

            internal Hashtable Data
            {
                get
                {
                    return this._data;
                }
            }

            internal Props(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            private void EnsureHash()
            {
                if (string.IsNullOrEmpty(this._hash))
                {
                    if (this.Json.Length < 100)
                    {
                        this._hash = this.Json;
                        return;
                    }
                    byte[] bytes = Encoding.ASCII.GetBytes(this.Json);
                    byte[] inArray = new SHA1CryptoServiceProvider().ComputeHash(bytes);
                    this._hash = Convert.ToBase64String(inArray);
                }
            }
        }

        internal class Cell
        {
            internal string Color
            {
                get;
                set;
            }

            internal string InnerHtml
            {
                get;
                set;
            }

            internal string BackgroundImage
            {
                get;
                set;
            }

            internal string BackgroundRepeat
            {
                get;
                set;
            }

            internal string CssClass
            {
                get;
                set;
            }

            internal bool IsBusiness
            {
                get;
                set;
            }
        }

        private Dictionary<int, string> _firstLine;

        private bool _dirty;

        private object[] _colors;

        private int _yMax;

        private int _xMax;

        private readonly HMSScheduler _scheduler;

        private readonly HMSCalendar _calendar;

        private CellTable.PropCache _properties;

        private Dictionary<string, Hashtable> _optimized;

        private string _defaultHash;

        private CellTable.Props _defaultValue;

        private bool _vertically;

        private bool _horizontally;

        internal CellTable(HMSScheduler scheduler)
        {
            this._scheduler = scheduler;
        }

        internal CellTable(HMSCalendar calendar)
        {
            this._calendar = calendar;
        }
        private void PrepareYMax()
        {
            if (this._scheduler != null)
            {
                this._yMax = this._scheduler.Rows.Count;
                return;
            }
            if (this._gantt != null)
            {
                this._yMax = this._gantt.Tasks.TotalCount;
                return;
            }
            if (this._calendar != null)
            {
                this._yMax = (int)Math.Floor(this._calendar.Duration().TotalMinutes / (double)this._calendar.CellDuration);
                return;
            }
            throw new Exception("Unable to get YMax");
        }

        private void PrepareXMax()
        {
            if (this._scheduler != null)
            {
                this._xMax = this._scheduler.TimeHeader.Timeline.Count;
                return;
            }
            if (this._gantt != null)
            {
                this._xMax = this._gantt.TimeHeader.Timeline.Count;
                return;
            }
            if (this._calendar != null)
            {
                this._xMax = this._calendar.ColumnCount;
                return;
            }
            throw new Exception("Unable to get XMax");
        }

        private Task FindTaskByIndex(int i)
        {
            if (this._ganttTasksFlat != null && i < this._ganttTasksFlat.Count)
            {
                return this._ganttTasksFlat[i];
            }
            return null;
        }

        private CellTable.Cell GetCell(int x, int y)
        {
            if (this._scheduler != null)
            {
                Day day = this._scheduler.Rows[y];
                DateTime from = this._scheduler.TimeHeader.Timeline[x].Start;
                DateTime to = this._scheduler.TimeHeader.Timeline[x].End;
                if (this._scheduler.ViewType == ViewTypeEnum.Days)
                {
                    from = from.AddDays((double)y);
                    to = to.AddDays((double)y);
                }
                return this._scheduler.GetCell(from, to, day.Id);
            }
            if (this._gantt != null)
            {
                Task task = this.FindTaskByIndex(y);
                DateTime start = this._gantt.TimeHeader.Timeline[x].Start;
                DateTime end = this._gantt.TimeHeader.Timeline[x].End;
                return this._gantt.GetCell(start, end, task.Id);
            }
            if (this._calendar != null)
            {
                TimeSpan timeOfDay = this._calendar.VisibleStart.TimeOfDay;
                Column column = this._calendar.GetColumn(x);
                DateTime date = column.Date;
                DateTime from2 = new DateTime(date.Year, date.Month, date.Day).Add(timeOfDay).AddMinutes((double)(y * this._calendar.CellDuration));
                string id = column.Id;
                return this._calendar.GetCell(from2, id);
            }
            throw new Exception("Unable to GetCell");
        }

        internal void Process()
        {
            Stopwatch stopwatch = new Stopwatch();
            this.PrepareXMax();
            this.PrepareYMax();
            this.PrepareGantt();
            this._colors = new object[this._yMax];
            this._firstLine = new Dictionary<int, string>();
            this._properties = new CellTable.PropCache(this._xMax, this._yMax);
            for (int i = 0; i < this._yMax; i++)
            {
                this._colors[i] = new string[this._xMax];
                for (int j = 0; j < this._xMax; j++)
                {
                    CellTable.Cell cell = this.GetCell(j, i);
                    CellTable.Props props = new CellTable.Props(j, i);
                    if (!string.IsNullOrEmpty(cell.InnerHtml))
                    {
                        props["html"] = cell.InnerHtml;
                    }
                    if (!string.IsNullOrEmpty(cell.CssClass))
                    {
                        props["cssClass"] = cell.CssClass;
                    }
                    if (!string.IsNullOrEmpty(cell.BackgroundImage))
                    {
                        props["backImage"] = cell.BackgroundImage;
                    }
                    if (!string.IsNullOrEmpty(cell.BackgroundRepeat))
                    {
                        props["backRepeat"] = cell.BackgroundRepeat;
                    }
                    if (!string.IsNullOrEmpty(cell.Color))
                    {
                        props["backColor"] = cell.Color;
                    }
                    props["business"] = (cell.IsBusiness ? 1 : 0);
                    if (props.HasItems)
                    {
                        this._properties.Set(j, i, props);
                        this._dirty = true;
                    }
                    ((string[])this._colors[i])[j] = cell.Color;
                    if (i == 0)
                    {
                        this._firstLine[j] = cell.Color;
                    }
                    else
                    {
                        string a = (this._firstLine[j] == null) ? null : this._firstLine[j].ToUpper();
                        string b = (cell.Color == null) ? null : cell.Color.ToUpper();
                        if (a != b)
                        {
                            this._dirty = true;
                        }
                    }
                }
            }
            stopwatch.Start();
            this.Optimize();
            stopwatch.Stop();
        }

        private void PrepareGantt()
        {
            if (this._gantt != null)
            {
                this._ganttTasksFlat = this._gantt.GetTasksFlat();
            }
        }

        private void Optimize()
        {
            if (this._xMax == 0)
            {
                return;
            }
            if (this._yMax == 0)
            {
                return;
            }
            this.FindDefaultValue();
            this._vertically = this.IsVerticallyIdentical();
            this._horizontally = this.IsHorizontallyIdentical();
            this._optimized = new Dictionary<string, Hashtable>();
            foreach (CellTable.Props current in this._properties.Values)
            {
                if ((this._defaultValue == null || !(current.Hash == this._defaultHash)) && (!this._vertically || current.Y == 0) && (!this._horizontally || current.X == 0))
                {
                    this._optimized[current.X + "_" + current.Y] = current.Data;
                }
            }
        }

        private void FindDefaultValue()
        {
            if (this._properties.Count == this._yMax * this._xMax)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (CellTable.Props current in this._properties.Values)
                {
                    string hash = current.Hash;
                    if (dictionary.ContainsKey(hash))
                    {
                        Dictionary<string, int> dictionary2;
                        string key;
                        (dictionary2 = dictionary)[key = hash] = dictionary2[key] + 1;
                    }
                    else
                    {
                        dictionary[hash] = 0;
                    }
                }
                stopwatch.Stop();
                int num = 0;
                foreach (KeyValuePair<string, int> current2 in dictionary)
                {
                    if (current2.Value > num)
                    {
                        this._defaultHash = current2.Key;
                        num = current2.Value;
                    }
                }
                this._defaultValue = this.PropsByHash(this._defaultHash);
            }
        }

        private bool IsVerticallyIdentical()
        {
            for (int i = 0; i < this._xMax; i++)
            {
                string hash = this._properties.GetHash(i, 0);
                for (int j = 0; j < this._yMax; j++)
                {
                    string hash2 = this._properties.GetHash(i, j);
                    if (hash2 != hash)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsHorizontallyIdentical()
        {
            for (int i = 0; i < this._yMax; i++)
            {
                string hash = this._properties.GetHash(0, i);
                for (int j = 0; j < this._xMax; j++)
                {
                    string hash2 = this._properties.GetHash(j, i);
                    if (hash2 != hash)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Hashtable GetConfig()
        {
            Hashtable hashtable = new Hashtable();
            if (this._defaultValue != null)
            {
                hashtable["default"] = this._defaultValue.Data;
            }
            hashtable["vertical"] = this._vertically;
            hashtable["horizontal"] = this._horizontally;
            hashtable["x"] = this._xMax;
            hashtable["y"] = this._yMax;
            return hashtable;
        }

        private CellTable.Props PropsByHash(string hash)
        {
            foreach (CellTable.Props current in this._properties.Values)
            {
                if (current.Hash == hash)
                {
                    return current;
                }
            }
            return null;
        }

        internal object[] GetColorsPlain()
        {
            if (this._colors == null)
            {
                throw new Exception("Call process() first.");
            }
            int num = this._dirty ? this._yMax : Math.Min(1, this._yMax);
            object[] array = new Array[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = new string[this._xMax];
                for (int j = 0; j < this._xMax; j++)
                {
                    string text = ((string[])this._colors[i])[j];
                    ((string[])array[i])[j] = text;
                }
            }
            return array;
        }

        internal Dictionary<string, Hashtable> GetProperties()
        {
            return this._optimized;
        }

        internal string GetHash()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["properties"] = this.GetProperties();
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(hashtable));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }
    }
}
