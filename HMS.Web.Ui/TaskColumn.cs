using HMS.Json;
using System;
namespace HMS.Web.App.Ui
{
    [Serializable]
    public class TaskColumn
    {
        public int Width
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Property
        {
            get;
            set;
        }

        public TaskColumn()
        {
            this.Width = 50;
            this.Property = "text";
        }

        public TaskColumn(string title, int width, string property) : this()
        {
            this.Title = title;
            this.Width = width;
            this.Property = property;
        }

        public override string ToString()
        {
            return this.Title;
        }

        internal JsonData ToJson()
        {
            JsonData jsonData = new JsonData();
            jsonData["title"] = this.Title;
            jsonData["width"] = this.Width;
            jsonData["property"] = this.Property;
            return jsonData;
        }
    }
}
