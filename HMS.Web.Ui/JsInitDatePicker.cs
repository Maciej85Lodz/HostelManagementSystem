using HMS.Utils;
using System;
using System.Reflection;
using System.Text;
using System.Threading;

namespace HMS.Web.App.Ui
{
    internal class JsInitDatePicker
    {
        private const string tempVar = "v";

        private readonly HMSDatePicker _picker;

        private StringBuilder _sb;

        internal JsInitDatePicker(HMSDatePicker picker)
        {
            this._picker = picker;
        }

        internal string GetCode()
        {
            this._sb = new StringBuilder();
            this._sb.AppendLine("<script type='text/javascript'>");
            this._sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this._sb.AppendLine(Locale.RegistrationString(Thread.CurrentThread.CurrentCulture.Name.ToLower()));
            this._sb.AppendLine("var v = new HMS.DatePicker();");
            if (this._picker.Target != null)
            {
                this.AppendProp("target", this._picker.Target.ClientID);
            }
            this.AppendProp("locale", Thread.CurrentThread.CurrentCulture.Name.ToLower(), true);
            this.AppendProp("theme", this._picker.Theme, true);
            this.AppendProp("onTimeRangeSelected", "function(args) { var date = args.date; " + this._picker.TimeRangeSelectedJavaScript + "; }");
            this._sb.AppendLine("v.init();");
            this._sb.AppendLine("var " + this._picker.ClientObjectName + " = v;");
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
}
