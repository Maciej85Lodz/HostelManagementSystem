using HMS.Json;
using HMS.Utils;
using HMS.Web.App.Ui.Enums;
using System;

namespace HMS.Web.App.Ui
{
    [Serializable]
    public class TimeHeader
    {
        public GroupByEnum GroupBy
        {
            get;
            set;
        }

        public string Format
        {
            get;
            set;
        }

        public TimeHeader()
        {
        }

        public TimeHeader(GroupByEnum groupBy)
        {
            this.GroupBy = groupBy;
        }

        public TimeHeader(GroupByEnum groupBy, string format)
        {
            this.GroupBy = groupBy;
            this.Format = format;
        }

        public JsonData ToJson()
        {
            JsonData jsonData = new JsonData();
            jsonData["groupBy"] = this.GroupBy.ToString();
            if (!string.IsNullOrEmpty(this.Format))
            {
                jsonData["format"] = this.Format;
            }
            return jsonData;
        }

        public static TimeHeader FromJson(JsonData node)
        {
            return new TimeHeader
            {
                GroupBy = GroupByEnumParser.Parse((string)node["groupBy"]),
                Format = (string)node["format"]
            };
        }
    }
}
