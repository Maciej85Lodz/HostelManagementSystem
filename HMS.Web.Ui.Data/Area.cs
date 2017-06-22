using System;
using System.Collections;

namespace HMS.Web.Ui.Data
{
    public class Area
    {
        private int? _width;

        private int? _height;

        private int? _left;

        private int? _right;

        private int? _top;

        private int? _bottom;

        private string _cssClass;

        private string _style;

        private string _id;

        private string _menu;

        private string _javaScript;

        private string _html;

        private AreaAction _action;

        private AreaVisibility _visibility;

        internal Hashtable ToHashtable()
        {
            Hashtable hashtable = new Hashtable();
            if (this._width.HasValue)
            {
                hashtable["w"] = this._width;
            }
            if (this._height.HasValue)
            {
                hashtable["h"] = this._height;
            }
            if (this._right.HasValue)
            {
                hashtable["right"] = this._right;
            }
            if (this._top.HasValue)
            {
                hashtable["top"] = this._top;
            }
            if (this._left.HasValue)
            {
                hashtable["left"] = this._left;
            }
            if (this._bottom.HasValue)
            {
                hashtable["bottom"] = this._bottom;
            }
            if (this._style != null)
            {
                hashtable["style"] = this._style;
            }
            if (this._cssClass != null)
            {
                hashtable["css"] = this._cssClass;
            }
            if (this._html != null)
            {
                hashtable["html"] = this._html;
            }
            if (this._id != null)
            {
                hashtable["id"] = this._id;
            }
            if (this._menu != null)
            {
                hashtable["menu"] = this._menu;
            }
            if (this._javaScript != null)
            {
                hashtable["js"] = string.Format("(function(e) {{ {0}; }})", this._javaScript);
            }
            hashtable["v"] = this._visibility.ToString();
            hashtable["action"] = this._action.ToString();
            return hashtable;
        }

        public Area Width(int width)
        {
            this._width = new int?(width);
            return this;
        }

        public Area Height(int height)
        {
            this._height = new int?(height);
            return this;
        }

        public Area Left(int left)
        {
            this._left = new int?(left);
            return this;
        }

        public Area Right(int right)
        {
            this._right = new int?(right);
            return this;
        }

        public Area Top(int top)
        {
            this._top = new int?(top);
            return this;
        }

        public Area Bottom(int bottom)
        {
            this._bottom = new int?(bottom);
            return this;
        }

        public Area CssClass(string className)
        {
            this._cssClass = className;
            return this;
        }

        public Area Style(string style)
        {
            this._style = style;
            return this;
        }

        public Area Id(string id)
        {
            this._id = id;
            return this;
        }

        public Area ContextMenu(string contextMenu)
        {
            this._action = AreaAction.ContextMenu;
            this._menu = contextMenu;
            return this;
        }

        public Area HoverMenu(string contextMenu)
        {
            this._action = AreaAction.HoverMenu;
            this._menu = contextMenu;
            return this;
        }

        public Area JavaScript(string javaScript)
        {
            this._action = AreaAction.JavaScript;
            this._javaScript = javaScript;
            return this;
        }

        public Area Bubble()
        {
            this._action = AreaAction.Bubble;
            return this;
        }

        public Area Visible()
        {
            this._visibility = AreaVisibility.Visible;
            return this;
        }

        public Area Hover()
        {
            this._visibility = AreaVisibility.Hover;
            return this;
        }

        [Obsolete("Please use Visible() or Hover() instead.")]
        public Area Visibility(AreaVisibility visibility)
        {
            this._visibility = visibility;
            return this;
        }

        public Area Html(string html)
        {
            this._html = html;
            return this;
        }

        public Area ResizeEnd()
        {
            this._action = AreaAction.ResizeEnd;
            return this;
        }

        public Area Move()
        {
            this._action = AreaAction.Move;
            return this;
        }
    }
}
