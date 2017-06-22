using HMS.Utils;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading;

namespace HMS.Web.App.Ui
{
    internal class JsInitNavigator
    {
        private const string tempVar = "v";

        private HMSNavigator _calendar;

        private StringBuilder sb;

        internal JsInitNavigator(HMSNavigator calendar)
        {
            this._calendar = calendar;
        }

        internal string GetCode()
        {
            this.sb = new StringBuilder();
            this.sb.AppendLine("<script type='text/javascript'>");
            this.sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this.sb.AppendLine("function " + this._calendar.ClientObjectName + "_Init() {");
            this.sb.AppendLine("var v = new HMS.Navigator('" + this._calendar.ClientID + "');");
            this.appendProp("api", 1);
            if (this._calendar.boundClientName != null)
            {
                this.appendProp("bound", this._calendar.boundClientName, true);
            }
            this.appendProp("cellHeight", this._calendar.CellHeight);
            this.appendProp("cellWidth", this._calendar.CellWidth);
            this.appendProp("clientName", this._calendar.ClientObjectName, true);
            this.appendProp("command", this._calendar.BindCommand, true);
            this.appendProp("cssOnly", this._calendar.CssOnly);
            this.appendProp("theme", this._calendar.Theme, true);
            this.appendProp("dayHeaderHeight", this._calendar.DayHeaderHeight);
            this.appendProp("items", SimpleJsonSerializer.Serialize(this._calendar.Items));
            this.appendProp("cells", SimpleJsonSerializer.Serialize(this._calendar.GetCells()));
            this.appendProp("locale", Thread.CurrentThread.CurrentCulture.Name.ToLower(), true);
            this.appendProp("month", this._calendar.StartDate.Month);
            this.appendProp("orientation", this._calendar.Orientation, true);
            this.appendProp("rowsPerMonth", this._calendar.RowsPerMonth, true);
            this.appendProp("selectMode", this._calendar.SelectMode.ToString().ToLower(), true);
            this.appendProp("selectionStart", "new HMS.Date('" + this._calendar.SelectionStart.ToString("s") + "')");
            this.appendProp("selectionEnd", "new HMS.Date('" + this._calendar.SelectionEnd.ToString("s") + "')");
            this.appendProp("showMonths", this._calendar.ShowMonths);
            this.appendProp("showWeekNumbers", this._calendar.ShowWeekNumbers);
            this.appendProp("skipMonths", this._calendar.SkipMonths);
            this.appendProp("titleHeight", this._calendar.TitleHeight);
            this.appendProp("uniqueID", this._calendar.UniqueID, true);
            this.appendProp("visible", this._calendar.Visible);
            this.appendProp("weekStarts", this.weekStarts());
            this.appendProp("weekNumberAlgorithm", this._calendar.WeekNumberAlgorithm, true);
            this.appendProp("year", this._calendar.StartDate.Year);
            if (!string.IsNullOrEmpty(this._calendar.CallBackErrorJavaScript))
            {
                this.appendProp("callbackError", "function(result, context) { " + this._calendar.CallBackErrorJavaScript + " }", false);
            }
            this.appendProp("timeRangeSelectedHandling", this._calendar.TimeRangeSelectedHandling, true);
            this.appendProp("onTimeRangeSelected", "function(start, end, day) {" + this._calendar.TimeRangeSelectedJavaScript + "}", false);
            this.appendProp("visibleRangeChangedHandling", this._calendar.VisibleRangeChangedHandling, true);
            this.appendProp("onVisibleRangeChanged", "function(start, end) {" + this._calendar.VisibleRangeChangedJavaScript + "}", false);
            this.sb.AppendLine("v.init();");
            this.sb.AppendLine("return v.internal.initialized() ? v : null;");
            this.sb.AppendLine("}");
            this.sb.AppendLine(Locale.RegistrationString(Thread.CurrentThread.CurrentCulture.Name.ToLower()));
            this.sb.AppendLine(string.Concat(new string[]
            {
                "var ",
                this._calendar.ClientObjectName,
                " = ",
                this._calendar.ClientObjectName,
                "_Init() || ",
                this._calendar.ClientObjectName,
                ";"
            }));
            this.sb.AppendLine("</script>");
            return this.sb.ToString();
        }

        private int weekStarts()
        {
            switch (this._calendar.WeekStarts)
            {
                case WeekStartsEnum.Sunday:
                    return 0;
                case WeekStartsEnum.Monday:
                    return 1;
                case WeekStartsEnum.Tuesday:
                    return 2;
                case WeekStartsEnum.Wednesday:
                    return 3;
                case WeekStartsEnum.Thursday:
                    return 4;
                case WeekStartsEnum.Friday:
                    return 5;
                case WeekStartsEnum.Saturday:
                    return 6;
                case WeekStartsEnum.Auto:
                    switch (Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            return 0;
                        case DayOfWeek.Monday:
                            return 1;
                        case DayOfWeek.Tuesday:
                            return 2;
                        case DayOfWeek.Wednesday:
                            return 3;
                        case DayOfWeek.Thursday:
                            return 4;
                        case DayOfWeek.Friday:
                            return 5;
                        case DayOfWeek.Saturday:
                            return 6;
                        default:
                            throw new NotSupportedException(string.Format("This day is not supported as a first day of week ({0})", Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void appendProp(string property, object val, bool apo)
        {
            if (apo)
            {
                string text = null;
                if (val != null)
                {
                    text = val.ToString().Replace("'", "\\'");
                }
                this.sb.AppendLine(string.Concat(new string[]
                {
                    "v.",
                    property,
                    " = '",
                    text,
                    "';"
                }));
                return;
            }
            this.sb.AppendLine(string.Concat(new object[]
            {
                "v.",
                property,
                " = ",
                val,
                ";"
            }));
        }

        private void appendProp(string property, object val)
        {
            this.appendProp(property, val, false);
        }

        private void appendProp(string property, bool value)
        {
            this.appendProp(property, value.ToString().ToLower(), false);
        }
    }
}
