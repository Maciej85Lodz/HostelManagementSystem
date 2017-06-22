using HMS.Json;
using HMS.Web.App.Ui.Enums.Gantt;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(TaskCollectionConverter))]
    [Serializable]
    public class TaskCollection : CollectionBase
    {
        internal bool designMode;

        public Task this[int index]
        {
            get
            {
                return (Task)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        public int TotalCount
        {
            get
            {
                int num = base.Count;
                foreach (Task task in this)
                {
                    num += task.Children.TotalCount;
                }
                return num;
            }
        }

        public ArrayList ToArrayList()
        {
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < base.Count; i++)
            {
                arrayList.Add(this[i]);
            }
            return arrayList;
        }

        public int Add(Task value)
        {
            return base.List.Add(value);
        }

        public int Add(string name, string id)
        {
            return this.Add(new Task(name, id));
        }

        public int IndexOf(Task value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Task value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Task value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Task value)
        {
            return base.List.Contains(value);
        }

        public TaskCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Task)
                {
                    this.Add((Task)items[i]);
                }
            }
        }

        public TaskCollection()
        {
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            TaskCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(TaskCollection collection, JsonData tree)
        {
            if (tree == null || tree.IsNull)
            {
                return;
            }
            if (!tree.IsArray)
            {
                throw new ArgumentException("Array JsonData expected. Received: " + tree.GetJsonType());
            }
            foreach (JsonData jsonData in ((IEnumerable)tree))
            {
                Task task = new Task();
                task.Text = (string)jsonData["text"];
                task.Id = (string)jsonData["id"];
                task.Start = (DateTime)jsonData["start"];
                task.End = (DateTime)jsonData["end"];
                if (jsonData["complete"] != null)
                {
                    task.Complete = (int)jsonData["complete"];
                }
                task.Type = TaskTypeParser.Parse((string)jsonData["type"]);
                if (jsonData["expanded"] != null)
                {
                    task.Expanded = (bool)jsonData["expanded"];
                }
                if (jsonData["loaded"] != null)
                {
                    task.DynamicChildren = !(bool)jsonData["loaded"];
                }
                else
                {
                    task.DynamicChildren = true;
                }
                task.Tags.RestoreFromJson(jsonData["tags"]);
                collection.Add(task);
                TaskCollection.RestoreCollection(task.Children, jsonData["children"]);
            }
        }

        public Task FindByIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("Negative index is invalid.");
            }
            int num = 0;
            return this.findByIndex(ref num, index);
        }

        private Task findByIndex(ref int i, int index)
        {
            foreach (Task task in this)
            {
                if (i == index)
                {
                    Task result = task;
                    return result;
                }
                i++;
                Task task2 = task.Children.findByIndex(ref i, index);
                if (task2 != null)
                {
                    Task result = task2;
                    return result;
                }
            }
            return null;
        }

        public Task FindById(string id)
        {
            foreach (Task task in this)
            {
                if (task.Id == id)
                {
                    Task result = task;
                    return result;
                }
                Task task2 = task.Children.FindById(id);
                if (task2 != null)
                {
                    Task result = task2;
                    return result;
                }
            }
            return null;
        }

        public void ExpandAll()
        {
            foreach (Task task in this)
            {
                task.Expanded = true;
                task.Children.ExpandAll();
            }
        }

        public void CollapseAll()
        {
            foreach (Task task in this)
            {
                task.Expanded = false;
                task.Children.CollapseAll();
            }
        }

        public Task FindParent(Task task)
        {
            return this.findParent(task, Task.Empty);
        }

        private Task findParent(Task task, Task parent)
        {
            foreach (Task task2 in this)
            {
                if (task2 == task)
                {
                    Task result = parent;
                    return result;
                }
                Task task3 = task2.Children.findParent(task, task2);
                if (task3 != null)
                {
                    Task result = task3;
                    return result;
                }
            }
            return null;
        }

        public void RemoveFromTree(Task task)
        {
            Task task2 = this.FindParent(task);
            if (task2 == null)
            {
                throw new Exception("Task not found.");
            }
            if (task2 == Task.Empty)
            {
                this.Remove(task);
                return;
            }
            task2.Children.Remove(task);
        }

        internal TaskCollection FindParentCollection(Task task)
        {
            Task task2 = this.FindParent(task);
            if (task2 == Task.Empty)
            {
                return this;
            }
            return task2.Children;
        }

        internal List<Hashtable> ToJson()
        {
            List<Hashtable> list = new List<Hashtable>();
            foreach (Task task in this)
            {
                list.Add(task.ToJsonIncludeChildren());
            }
            return list;
        }

        public void Add(string text, string id, DateTime start, DateTime end)
        {
            this.Add(new Task
            {
                Start = start,
                End = end,
                Id = id,
                Text = text
            });
        }
    }
}
