using HMS.Json;
using System;


namespace HMS.Web.App.Ui
{
    [Serializable]
    public class RowHeaderColumn
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

        public RowHeaderColumn()
        {
        }

        public RowHeaderColumn(string title, int width)
        {
            this.Title = title;
            this.Width = width;
        }

        public override string ToString()
        {
            return this.Title;
        }

        public JsonData ToJson()
        {
            JsonData jsonData = new JsonData();
            jsonData["width"] = this.Width;
            jsonData["title"] = this.Title;
            return jsonData;
        }

        public static RowHeaderColumn FromJson(JsonData node)
        {
            return new RowHeaderColumn
            {
                Title = (string)node["title"],
                Width = (int)node["width"]
            };
        }
    }
}
