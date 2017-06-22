using HMS.Web.App.Ui.Data;
using HMS.Web.App.Ui.Enums.Gantt;
using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;

namespace HMS.Web.App.Ui
{
    [Serializable]
    public class Task
    {
        internal class StartEnd
        {
            internal DateTime? Start
            {
                get;
                set;
            }

            internal DateTime? End
            {
                get;
                set;
            }
        }

        private TaskCollection children = new TaskCollection();

        internal static Task Empty = new Task();

        private TaskType _type;

        public string Id
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

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

        public TaskType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                if (value == TaskType.Group)
                {
                    throw new ArgumentException("Task.Type cannot be set to TaskType.Group directly. Use TaskType.Task and specify children.");
                }
                this._type = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), PersistenceMode(PersistenceMode.InnerProperty)]
        public TaskCollection Children
        {
            get
            {
                return this.children;
            }
        }

        public bool Expanded
        {
            get;
            set;
        }

        public bool DynamicChildren
        {
            get;
            set;
        }

        public object DataItem
        {
            get;
            set;
        }

        public int Complete
        {
            get;
            set;
        }

        public TaskTagDictionary Tags
        {
            get;
            private set;
        }

        internal string ParentId
        {
            get;
            set;
        }

        public Task()
        {
            this.Expanded = true;
            this.Type = TaskType.Task;
            this.Tags = new TaskTagDictionary();
        }

        public Task(string text, string id) : this()
        {
            this.Text = text;
            this.Id = id;
        }

        public Task(string text, string id, DateTime start, DateTime end) : this()
        {
            this.Text = text;
            this.Id = id;
            this.Start = start;
            this.End = end;
        }

        public override string ToString()
        {
            return this.Text;
        }

        internal Hashtable ToJson()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["id"] = this.Id;
            hashtable["text"] = this.Text;
            hashtable["start"] = this.Start.ToString("s");
            hashtable["end"] = this.End.ToString("s");
            if (this.Type != TaskType.Task)
            {
                hashtable["type"] = this.Type.ToString();
            }
            if (this.Complete > 0)
            {
                hashtable["complete"] = this.Complete;
            }
            Hashtable hashtable2 = this.Tags.ToJson();
            if (hashtable2.Count > 0)
            {
                hashtable["tags"] = hashtable2;
            }
            return hashtable;
        }

        internal Hashtable ToJsonIncludeChildren()
        {
            Hashtable hashtable = this.ToJson();
            if (this.Children.Count > 0)
            {
                hashtable["children"] = this.Children.ToJson();
            }
            return hashtable;
        }

        internal Task.StartEnd ChildrenStartEnd()
        {
            Task.StartEnd startEnd = new Task.StartEnd();
            if (this.Children.Count == 0)
            {
                startEnd.Start = new DateTime?(this.Start);
                startEnd.End = new DateTime?(this.End);
                if (this.Type == TaskType.Milestone)
                {
                    startEnd.End = new DateTime?(this.Start);
                }
                return startEnd;
            }
            foreach (Task task in this.Children)
            {
                Task.StartEnd startEnd2 = task.ChildrenStartEnd();
                if (!startEnd.Start.HasValue || startEnd2.Start < this.Start)
                {
                    startEnd.Start = startEnd2.Start;
                }
                if (!startEnd.End.HasValue || startEnd2.End > this.End)
                {
                    startEnd.End = startEnd2.End;
                }
            }
            return startEnd;
        }
    }
}
