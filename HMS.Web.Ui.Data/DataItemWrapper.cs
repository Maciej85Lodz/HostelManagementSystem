using System;
using System.Reflection;

namespace HMS.Web.Ui.Data
{
    public class DataItemWrapper
    {
        public object Source
        {
            get;
            private set;
        }

        public object this[string property]
        {
            get
            {
                return DataItemWrapper.ReadPropertyValue(this.Source, property);
            }
        }

        public DataItemWrapper(object dataItem)
        {
            this.Source = dataItem;
        }

        private static object ReadPropertyValue(object o, string property)
        {
            Type type = o.GetType();
            PropertyInfo property2 = type.GetProperty(property);
            if (property2 != null)
            {
                return property2.GetValue(o, null);
            }
            MethodInfo method = type.GetMethod("get_Item", new Type[]
            {
                typeof(string)
            });
            if (method != null)
            {
                return method.Invoke(o, new object[]
                {
                    property
                });
            }
            throw new ArgumentException("Property or index not found.");
        }
    }
}
