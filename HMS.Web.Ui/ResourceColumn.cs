using System;


namespace HMS.Web.App.Ui
{
    [Serializable]
    public class ResourceColumn
    {
        [Obsolete("Use .Html instead.")]
        public string InnerHTML
        {
            get
            {
                return this.Html;
            }
            set
            {
                this.Html = value;
            }
        }

        public string Html
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public ResourceColumn()
        {
        }

        public ResourceColumn(string innerHTML)
        {
            this.InnerHTML = innerHTML;
        }
    }
}
