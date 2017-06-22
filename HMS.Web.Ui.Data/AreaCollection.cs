using HMS.Web.Ui.Json;
using System;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Web.Ui.Data
{
    public class AreaCollection : CollectionBase
    {
        public Area this[int index]
        {
            get
            {
                return (Area)base.List[index];
            }
            set
            {
                base.List[index] = value;
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

        public int Add(Area value)
        {
            return base.List.Add(value);
        }

        public int IndexOf(Area value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Area value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Area value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Area value)
        {
            return base.List.Contains(value);
        }

        public AreaCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Area)
                {
                    this.Add((Area)items[i]);
                }
            }
        }

        public AreaCollection()
        {
        }

        internal List<Hashtable> GetList()
        {
            if (base.Count == 0)
            {
                return null;
            }
            List<Hashtable> list = new List<Hashtable>();
            foreach (Area area in this)
            {
                list.Add(area.ToHashtable());
            }
            return list;
        }

        internal string GetHash()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(this.GetList()));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }
    }

    public abstract class CollectionBase : IList, ICollection, IEnumerable
    {
        private ArrayList list;

        protected ArrayList InnerList
        {
            get
            {
                if (this.list == null)
                {
                    this.list = new ArrayList();
                }
                return this.list;
            }
        }

        protected IList List
        {
            get
            {
                return this;
            }
        }

        [ComVisible(false)]
        public int Capacity
        {
            get
            {
                return this.InnerList.Capacity;
            }
            set
            {
                this.InnerList.Capacity = value;
            }
        }

        public int Count
        {
            get
            {
                if (this.list != null)
                {
                    return this.list.Count;
                }
                return 0;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return this.InnerList.IsReadOnly;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return this.InnerList.IsFixedSize;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return this.InnerList.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.InnerList.SyncRoot;
            }
        }

        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= this.InnerList.Count)
                {
                    throw new ArgumentOutOfRangeException("index", ("ArgumentOutOfRange_Index"));
                }
                return this.InnerList[index];
            }
            set
            {
                if (index < 0 || index >= this.InnerList.Count)
                {
                    throw new ArgumentOutOfRangeException("index", ("ArgumentOutOfRange_Index"));
                }
                this.OnValidate(value);
                object obj = this.InnerList[index];
                this.OnSet(index, obj, value);
                this.InnerList[index] = value;
                try
                {
                    this.OnSetComplete(index, obj, value);
                }
                catch
                {
                    this.InnerList[index] = obj;
                    throw;
                }
            }
        }

        protected CollectionBase()
        {
            this.list = new ArrayList();
        }

        protected CollectionBase(int capacity)
        {
            this.list = new ArrayList(capacity);
        }

        public void Clear()
        {
            this.OnClear();
            this.InnerList.Clear();
            this.OnClearComplete();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.InnerList.Count)
            {
                throw new ArgumentOutOfRangeException("index", ("ArgumentOutOfRange_Index"));
            }
            object value = this.InnerList[index];
            this.OnValidate(value);
            this.OnRemove(index, value);
            this.InnerList.RemoveAt(index);
            try
            {
                this.OnRemoveComplete(index, value);
            }
            catch
            {
                this.InnerList.Insert(index, value);
                throw;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        bool IList.Contains(object value)
        {
            return this.InnerList.Contains(value);
        }

        int IList.Add(object value)
        {
            this.OnValidate(value);
            this.OnInsert(this.InnerList.Count, value);
            int num = this.InnerList.Add(value);
            try
            {
                this.OnInsertComplete(num, value);
            }
            catch
            {
                this.InnerList.RemoveAt(num);
                throw;
            }
            return num;
        }

        void IList.Remove(object value)
        {
            this.OnValidate(value);
            int num = this.InnerList.IndexOf(value);
            if (num < 0)
            {
                throw new ArgumentException("Arg_RemoveArgNotFound");
            }
            this.OnRemove(num, value);
            this.InnerList.RemoveAt(num);
            try
            {
                this.OnRemoveComplete(num, value);
            }
            catch
            {
                this.InnerList.Insert(num, value);
                throw;
            }
        }

        int IList.IndexOf(object value)
        {
            return this.InnerList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            if (index < 0 || index > this.InnerList.Count)
            {
                throw new ArgumentOutOfRangeException("index",("ArgumentOutOfRange_Index"));
            }
            this.OnValidate(value);
            this.OnInsert(index, value);
            this.InnerList.Insert(index, value);
            try
            {
                this.OnInsertComplete(index, value);
            }
            catch
            {
                this.InnerList.RemoveAt(index);
                throw;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        protected virtual void OnSet(int index, object oldValue, object newValue)
        {
        }

        protected virtual void OnInsert(int index, object value)
        {
        }

        protected virtual void OnClear()
        {
        }

        protected virtual void OnRemove(int index, object value)
        {
        }

        protected virtual void OnValidate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
        }

        protected virtual void OnSetComplete(int index, object oldValue, object newValue)
        {
        }

        protected virtual void OnInsertComplete(int index, object value)
        {
        }

        protected virtual void OnClearComplete()
        {
        }

        protected virtual void OnRemoveComplete(int index, object value)
        {
        }
    }
}
