using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace HMS.Web.App.Ui
{
    [ParseChildren(true, "MenuItems")]
    public class MenuItem
    {
        internal class Map : List<MenuItem.KeyVal>
        {
            public MenuItem.Map AddQuoted(string key, string val)
            {
                base.Add(MenuItem.KeyVal.Quoted(key, val));
                return this;
            }

            public MenuItem.Map AddUnquoted(string key, string val)
            {
                base.Add(MenuItem.KeyVal.Unquoted(key, val));
                return this;
            }

            public string ToJavaScript()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("{");
                bool flag = true;
                foreach (MenuItem.KeyVal current in this)
                {
                    string value = current.Val;
                    if (current.Val == null)
                    {
                        value = "null";
                    }
                    if (!flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(current.Key);
                    stringBuilder.Append(":");
                    stringBuilder.Append(value);
                    flag = false;
                }
                stringBuilder.Append("}");
                return stringBuilder.ToString();
            }
        }

        internal class KeyVal
        {
            internal string Key;

            internal string Val;

            private KeyVal(string key, string val)
            {
                this.Key = MenuItem.KeyVal.EscapeAndQuote(key);
                this.Val = val;
            }

            internal static MenuItem.KeyVal Quoted(string key, string val)
            {
                return new MenuItem.KeyVal(key, MenuItem.KeyVal.EscapeAndQuote(val));
            }

            internal static MenuItem.KeyVal Unquoted(string key, string val)
            {
                return new MenuItem.KeyVal(key, val);
            }

            private static string EscapeAndQuote(string input)
            {
                if (input == null)
                {
                    return input;
                }
                return "'" + input.Replace("'", "\\'") + "'";
            }
        }

        private readonly MenuItemCollection _items = new MenuItemCollection();

        private string navigateUrl;

        private string navigateUrlTarget;

        private string toolTip;

        public static MenuItem Separator = new MenuItem
        {
            Text = "-"
        };

        public string Text
        {
            get;
            set;
        }

        public string NavigateUrl
        {
            get
            {
                return this.navigateUrl;
            }
            set
            {
                if (value == null)
                {
                    this.navigateUrl = string.Empty;
                    return;
                }
                this.navigateUrl = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public MenuItemCollection MenuItems
        {
            get
            {
                return this._items;
            }
        }

        public string NavigateUrlTarget
        {
            get
            {
                return this.navigateUrlTarget;
            }
            set
            {
                if (value == null)
                {
                    this.navigateUrlTarget = string.Empty;
                    return;
                }
                this.navigateUrlTarget = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                if (value == null)
                {
                    this.toolTip = string.Empty;
                    return;
                }
                this.toolTip = value;
            }
        }

        public MenuItemAction Action
        {
            get;
            set;
        }

        public string JavaScript
        {
            get;
            set;
        }

        public string Command
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }

        public MenuItem()
        {
            this.JavaScript = null;
            this.Action = MenuItemAction.JavaScript;
            this.Text = null;
        }

        public override string ToString()
        {
            return this.Text;
        }

        internal string ToJavaScript()
        {
            MenuItem.Map map = new MenuItem.Map();
            map.AddQuoted("text", this.Text);
            if (this.Text == "-")
            {
                return map.ToJavaScript();
            }
            if (!string.IsNullOrEmpty(this.Image))
            {
                map.AddQuoted("image", this.Image);
            }
            switch (this.Action)
            {
                case MenuItemAction.NavigateUrl:
                    map.AddQuoted("href", this.NavigateUrl);
                    map.AddQuoted("target", this.navigateUrlTarget);
                    break;
                case MenuItemAction.JavaScript:
                    map.AddUnquoted("onclick", string.Format("function() {{ var e = this.source; var command = this.item.command; {0} }}", this.JavaScript)).AddQuoted("command", this.Command);
                    break;
                case MenuItemAction.CallBack:
                    map.AddQuoted("command", this.Command).AddQuoted("action", this.Action.ToString());
                    break;
                case MenuItemAction.PostBack:
                    map.AddQuoted("command", this.Command).AddQuoted("action", this.Action.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (this._items.Count > 0)
            {
                map.AddUnquoted("items", this.MenuItems.ToJavaScript());
            }
            return map.ToJavaScript();
        }
    }
}
