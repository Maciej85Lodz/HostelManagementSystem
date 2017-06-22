using System;
using System.Reflection;
using System.Text;

namespace HMS.Web.App.Ui
{
    internal class JsInitMenu
    {
        private const string tempVar = "v";

        private readonly HMSMenu _menu;

        private StringBuilder _sb;

        internal JsInitMenu(HMSMenu menu)
        {
            this._menu = menu;
        }

        internal string GetObject()
        {
            this._sb = new StringBuilder();
            this._sb.AppendLine("<script type='text/javascript'>");
            this._sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this._sb.AppendLine(string.Format("var {0} = (typeof {0} !== 'object') ? new HMS.Menu() : {0};", this._menu.ClientObjectName));
            this._sb.AppendLine("</script>");
            return this._sb.ToString();
        }

        internal string GetConfig()
        {
            this._sb = new StringBuilder();
            this._sb.AppendLine("<script type='text/javascript'>");
            this._sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this._sb.AppendLine("(function () {");
            this._sb.AppendLine("var v = " + this._menu.ClientObjectName + ";");
            this._sb.AppendLine("v.items = " + this._menu.MenuItems.ToJavaScript() + ";");
            this.AppendProp("theme", this._menu.Theme, true);
            this.AppendProp("menuTitle", this._menu.MenuTitle, true);
            this.AppendProp("showMenuTitle", this._menu.ShowMenuTitle);
            this.AppendProp("hideOnMouseOut", this._menu.HideOnMouseOut);
            this.AppendProp("useShadow", this._menu.UseShadow);
            this.AppendProp("zIndex", this._menu.ZIndex);
            this._sb.AppendLine("})();");
            this._sb.AppendLine("</script>");
            return this._sb.ToString();
        }

        private void AppendProp(string property, object val, bool apo)
        {
            if (apo)
            {
                string text = null;
                if (val != null)
                {
                    text = val.ToString().Replace("'", "\\'");
                }
                this._sb.AppendLine(string.Concat(new string[]
                {
                    "v.",
                    property,
                    " = '",
                    text,
                    "';"
                }));
                return;
            }
            this._sb.AppendLine(string.Concat(new object[]
            {
                "v.",
                property,
                " = ",
                val,
                ";"
            }));
        }

        private void AppendProp(string property, object val)
        {
            this.AppendProp(property, val, false);
        }

        private void AppendProp(string property, bool value)
        {
            this.AppendProp(property, value.ToString().ToLower(), false);
        }
    }
}
