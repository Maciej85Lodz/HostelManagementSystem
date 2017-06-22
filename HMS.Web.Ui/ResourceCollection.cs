using HMS.Json;
using HMS.Web.App.Ui.Serialization;
using System;
using System.Collections;
using System.ComponentModel;

namespace HMS.Web.App.Ui
{
    [TypeConverter(typeof(ResourceCollectionConverter))]
    [Serializable]
    public class ResourceCollection : CollectionBase
    {
        internal bool designMode;

        public Resource this[int index]
        {
            get
            {
                return (Resource)base.List[index];
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

        public int Add(Resource value)
        {
            return base.List.Add(value);
        }

        public int Add(string name, string id)
        {
            return this.Add(new Resource(name, id));
        }

        public int IndexOf(Resource value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Resource value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(Resource value)
        {
            base.List.Remove(value);
        }

        public bool Contains(Resource value)
        {
            return base.List.Contains(value);
        }

        public ResourceCollection(ArrayList items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is Resource)
                {
                    this.Add((Resource)items[i]);
                }
            }
        }

        public ResourceCollection()
        {
        }

        internal void RestoreFromJson(JsonData tree)
        {
            base.Clear();
            ResourceCollection.RestoreCollection(this, tree);
        }

        private static void RestoreCollection(ResourceCollection collection, JsonData tree)
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
                Resource resource = new Resource();
                resource.Name = (string)jsonData["Name"];
                resource.Id = (string)jsonData["Value"];
                resource.ToolTip = (string)jsonData["ToolTip"];
                if (jsonData["Expanded"] != null)
                {
                    resource.Expanded = (bool)jsonData["Expanded"];
                }
                if (jsonData["IsParent"] != null)
                {
                    resource.IsParent = (bool)jsonData["IsParent"];
                }
                if (jsonData["Loaded"] != null)
                {
                    resource.DynamicChildren = !(bool)jsonData["Loaded"];
                }
                else
                {
                    resource.DynamicChildren = true;
                }
                JsonData jsonData2 = jsonData["Columns"];
                if (jsonData2 != null && !jsonData2.IsNull && jsonData2.IsArray)
                {
                    foreach (JsonData jsonData3 in ((IEnumerable)jsonData2))
                    {
                        resource.Columns.Add(new ResourceColumn((string)jsonData3["InnerHTML"]));
                    }
                }
                collection.Add(resource);
                ResourceCollection.RestoreCollection(resource.Children, jsonData["Children"]);
            }
        }

        public Resource FindByIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("Negative index is invalid.");
            }
            int num = 0;
            return this.findByIndex(ref num, index);
        }

        private Resource findByIndex(ref int i, int index)
        {
            foreach (Resource resource in this)
            {
                if (i == index)
                {
                    Resource result = resource;
                    return result;
                }
                i++;
                Resource resource2 = resource.Children.findByIndex(ref i, index);
                if (resource2 != null)
                {
                    Resource result = resource2;
                    return result;
                }
            }
            return null;
        }

        [Obsolete("Use .FindById() instead.")]
        public Resource FindByValue(string value)
        {
            return this.FindById(value);
        }

        public Resource FindById(string id)
        {
            foreach (Resource resource in this)
            {
                if (resource.Id == id)
                {
                    Resource result = resource;
                    return result;
                }
                Resource resource2 = resource.Children.FindById(id);
                if (resource2 != null)
                {
                    Resource result = resource2;
                    return result;
                }
            }
            return null;
        }

        public void ExpandAll()
        {
            foreach (Resource resource in this)
            {
                resource.Expanded = true;
                resource.Children.ExpandAll();
            }
        }

        public void CollapseAll()
        {
            foreach (Resource resource in this)
            {
                resource.Expanded = false;
                resource.Children.CollapseAll();
            }
        }

        public Resource FindParent(Resource resource)
        {
            return this.findParent(resource, Resource.Empty);
        }

        private Resource findParent(Resource resource, Resource parent)
        {
            foreach (Resource resource2 in this)
            {
                if (resource2 == resource)
                {
                    Resource result = parent;
                    return result;
                }
                Resource resource3 = resource2.Children.findParent(resource, resource2);
                if (resource3 != null)
                {
                    Resource result = resource3;
                    return result;
                }
            }
            return null;
        }

        public void RemoveFromTree(Resource resource)
        {
            Resource resource2 = this.FindParent(resource);
            if (resource2 == null)
            {
                throw new Exception("Resource not found.");
            }
            if (resource2 == Resource.Empty)
            {
                this.Remove(resource);
                return;
            }
            resource2.Children.Remove(resource);
        }

        internal ResourceCollection FindParentCollection(Resource resource)
        {
            Resource resource2 = this.FindParent(resource);
            if (resource2 == Resource.Empty)
            {
                return this;
            }
            return resource2.Children;
        }
    }
}
