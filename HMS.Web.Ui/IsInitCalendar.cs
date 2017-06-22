using HMS.Utils;
using HMS.Web.App.Ui.Colors;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Init;
using HMS.Web.App.Ui.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;

namespace HMS.Web.App.Ui
{
    internal class JsInitCalendar : JsInit
    {
        private readonly HMSCalendar _calendar;

        internal JsInitCalendar(HMSCalendar calendar)
        {
            this._calendar = calendar;
        }

        internal string GetCode()
        {
            this.sb = new StringBuilder();
            string val = "null";
            if (this._calendar.HeightSpec != HeightSpecEnum.Full && this._calendar.HeightSpec != HeightSpecEnum.BusinessHoursNoScroll)
            {
                if (this._calendar.ScrollPos == -1)
                {
                    val = Convert.ToString(this._calendar.CellHeight * (this._calendar.ScrollPositionHour - this._calendar.DayBeginsHour) * 60 / this._calendar.CellDuration);
                }
                else
                {
                    val = this._calendar.ScrollPos.ToString();
                }
            }
            List<Hashtable> events = this._calendar.GetEvents();
            this.sb.AppendLine("<script type='text/javascript'>");
            this.sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this.sb.AppendLine("function " + this._calendar.ClientObjectName + "_Init() {");
            this.sb.AppendLine("var v = new HMS.Calendar('" + this._calendar.ClientID + "');");
            base.appendProp("allDayEnd", this._calendar.AllDayEnd);
            base.appendProp("api", 1);
            base.appendProp("allDayEventBorderColor", this._calendar.AllDayEventBorderColor);
            base.appendProp("allDayEventFontFamily", this._calendar.AllDayEventFontFamily);
            base.appendProp("allDayEventFontSize", this._calendar.AllDayEventFontSize);
            base.appendProp("allDayEventFontColor", this._calendar.AllDayEventFontColor);
            base.appendProp("allDayEventHeight", this._calendar.AllDayEventHeight);
            base.appendProp("allowEventOverlap", this._calendar.AllowEventOverlap);
            base.appendProp("allowMultiSelect", this._calendar.AllowMultiSelect);
            base.appendProp("autoRefreshCommand", this._calendar.AutoRefreshCommand);
            base.appendProp("autoRefreshEnabled", this._calendar.AutoRefreshEnabled);
            base.appendProp("autoRefreshInterval", this._calendar.AutoRefreshInterval);
            base.appendProp("autoRefreshMaxCount", this._calendar.AutoRefreshMaxCount);
            base.appendProp("borderColor", this._calendar.BorderColor);
            base.appendProp("businessBeginsHour", this._calendar.BusinessBeginsHour);
            base.appendProp("businessEndsHour", this._calendar.BusinessEndsHour);
            base.appendProp("clientName", this._calendar.ClientObjectName);
            base.appendProp("cellBackColor", this._calendar.BackColor);
            base.appendProp("cellBackColorNonBusiness", this._calendar.NonBusinessBackColor);
            base.appendProp("cellBorderColor", this._calendar.CellBorderColor);
            base.appendProp("cellHeight", this._calendar.CellHeight);
            base.appendProp("cellDuration", this._calendar.CellDuration);
            base.appendProp("columnMarginRight", this._calendar.ColumnMarginRight);
            base.appendProp("columnWidthSpec", this._calendar.ColumnWidthSpec, true);
            base.appendProp("columnWidth", this._calendar.ColumnWidth);
            base.appendProp("crosshairColor", this._calendar.CrosshairColor);
            base.appendProp("crosshairOpacity", this._calendar.CrosshairOpacity);
            base.appendProp("crosshairType", this._calendar.Crosshair, true);
            base.appendProp("theme", this._calendar.Theme);
            base.appendProp("cssOnly", this._calendar.CssOnly);
            base.appendProp("deleteImageUrl", this._calendar.GetResourceUrl("Images/Delete10x10.gif"), true);
            base.appendProp("scrollDownUrl", this._calendar.GetResourceUrl("Images/Down.png"), true);
            base.appendProp("scrollUpUrl", this._calendar.GetResourceUrl("Images/Up.png"), true);
            base.appendProp("dayBeginsHour", this._calendar.DayBeginsHour);
            base.appendProp("dayEndsHour", this._calendar.DayEndsHour);
            base.appendProp("days", this._calendar.Days);
            base.appendProp("doubleClickTimeout", this._calendar.DoubleClickTimeout);
            base.appendProp("durationBarColor", this._calendar.DurationBarColor);
            base.appendProp("durationBarVisible", this._calendar.DurationBarVisible);
            base.appendProp("durationBarWidth", this._calendar.DurationBarWidth);
            base.appendProp("durationBarImageUrl", this._calendar.DurationBarImageUrl);
            base.appendProp("eventArrangement", this._calendar.EventArrangement);
            base.appendProp("eventBackColor", this._calendar.EventBackColor);
            base.appendProp("eventBorderColor", this._calendar.EventBorderColor);
            base.appendProp("eventFontFamily", this._calendar.EventFontFamily);
            base.appendProp("eventFontSize", this._calendar.EventFontSize);
            base.appendProp("eventFontColor", this._calendar.EventFontColor);
            base.appendProp("eventHeaderFontSize", this._calendar.EventHeaderFontSize);
            base.appendProp("eventHeaderFontColor", this._calendar.EventHeaderFontColor);
            base.appendProp("eventHeaderHeight", this._calendar.EventHeaderHeight);
            base.appendProp("eventHeaderVisible", this._calendar.EventHeaderVisible);
            base.appendProp("eventSelectColor", this._calendar.EventSelectColor);
            base.appendProp("headerFontSize", this._calendar.HeaderFontSize);
            base.appendProp("headerFontFamily", this._calendar.HeaderFontFamily);
            base.appendProp("headerFontColor", this._calendar.HeaderFontColor);
            base.appendProp("headerHeight", this._calendar.HeaderHeight);
            base.appendProp("headerHeightAutoFit", this._calendar.HeaderHeightAutoFit);
            base.appendProp("headerLevels", this._calendar.HeaderLevels);
            base.appendProp("height", this._calendar.Height);
            base.appendProp("heightSpec", this._calendar.HeightSpec);
            base.appendProp("hideFreeCells", this._calendar.HideFreeCells);
            base.appendProp("hourHalfBorderColor", this._calendar.HourHalfBorderColor);
            base.appendProp("hourBorderColor", this._calendar.HourBorderColor);
            base.appendProp("hourFontColor", this._calendar.HourFontColor);
            base.appendProp("hourFontFamily", this._calendar.HourFontFamily);
            base.appendProp("hourFontSize", this._calendar.HourFontSize);
            base.appendProp("hourNameBackColor", this._calendar.HourNameBackColor);
            base.appendProp("hourNameBorderColor", this._calendar.HourNameBorderColor);
            base.appendProp("hourWidth", this._calendar.HourWidth);
            base.appendProp("initScrollPos", val);
            base.appendProp("loadingLabelText", this._calendar.LoadingLabelText);
            base.appendProp("loadingLabelVisible", this._calendar.LoadingLabelVisible);
            base.appendProp("loadingLabelFontSize", this._calendar.LoadingLabelFontSize);
            base.appendProp("loadingLabelFontFamily", this._calendar.LoadingLabelFontFamily);
            base.appendProp("loadingLabelFontColor", this._calendar.LoadingLabelFontColor);
            base.appendProp("loadingLabelBackColor", this._calendar.LoadingLabelBackColor);
            if (!string.IsNullOrEmpty(this._calendar.CallBackMessage))
            {
                base.appendProp("messageHTML", this._calendar.CallBackMessage);
            }
            base.appendProp("messageHideAfter", this._calendar.MessageHideAfter);
            base.appendProp("moveBy", this._calendar.MoveBy, true);
            base.appendProp("notifyCommit", this._calendar.NotifyCommit, true);
            base.appendProp("numberFormat", Assembly.GetExecutingAssembly().FullName.Contains(".1,") ? "0.00" : null);
            base.appendProp("roundedCorners", this._calendar.EventCorners == CornerShape.Rounded);
            base.appendProp("rtl", this._calendar.IsRtl());
            base.appendProp("scrollLabelsVisible", this._calendar.ScrollLabelsVisible);
            base.appendProp("selectedColor", this._calendar.CellSelectColor);
            base.appendProp("shadow", this._calendar.Shadow.ToString(), true);
            base.appendProp("showToolTip", this._calendar.ShowToolTip);
            base.appendProp("showAllDayEvents", this._calendar.ShowAllDayEvents);
            base.appendProp("showAllDayEventStartEnd", this._calendar.ShowAllDayEventStartEnd);
            base.appendProp("showCurrentTime", this._calendar.ShowCurrentTime);
            base.appendProp("showHeader", this._calendar.ShowHeader);
            base.appendProp("showHours", this._calendar.ShowHours);
            base.appendSerialized("sortDirections", this._calendar.SortFields.Directions);
            base.appendProp("startDate", this._calendar.StartDate.ToString("s"), true);
            base.appendProp("timeFormat", Hour.DetectTimeFormat(this._calendar.TimeFormat).ToString(), true);
            base.appendProp("timeHeaderCellDuration", this._calendar.TimeHeaderCellDuration);
            base.appendProp("uniqueID", this._calendar.UniqueID);
            base.appendProp("useEventBoxes", this._calendar.UseEventBoxes, true);
            base.appendProp("useEventSelectionBars", this._calendar.UseEventSelectionBars);
            base.appendProp("viewType", this._calendar.ViewType.ToString());
            base.appendProp("visible", this._calendar.Visible);
            base.appendProp("weekStarts", this._calendar.WeekStartInt);
            base.appendProp("widthUnit", this.GetWidthUnitType());
            if (this._calendar.Width != Unit.Empty)
            {
                base.appendProp("width", this._calendar.Width, true);
            }
            if (this._calendar.TagFields != null)
            {
                base.appendProp("tagFields", SimpleJsonSerializer.Serialize(this._calendar.TagFields), false);
            }
            base.appendProp("cornerHTML", this._calendar.CornerHtml);
            base.appendProp("cornerBackColor", this._calendar.CornerBackColor);
            if (this._calendar.ContextMenu != null)
            {
                base.appendProp("contextMenu", this._calendar.ContextMenu.ClientObjectName, false);
            }
            if (this._calendar.ContextMenuSelection != null)
            {
                base.appendProp("contextMenuSelection", this._calendar.ContextMenuSelection.ClientObjectName, false);
            }
            if (this._calendar.Bubble != null)
            {
                base.appendProp("bubble", this._calendar.Bubble.ClientObjectName, false);
            }
            if (this._calendar.CellBubble != null)
            {
                base.appendProp("cellBubble", this._calendar.CellBubble.ClientObjectName, false);
            }
            if (this._calendar.ColumnBubble != null)
            {
                base.appendProp("columnBubble", this._calendar.ColumnBubble.ClientObjectName, false);
            }
            this._calendar.Page.ClientScript.GetCallbackEventReference(this._calendar, null, null, null, null, true);
            base.appendProp("eventTapAndHoldHandling", this._calendar.EventTapAndHoldHandling, true);
            base.appendProp("timeRangeTapAndHoldHandling", this._calendar.TimeRangeTapAndHoldHandling, true);
            base.appendProp("afterEventRender", "function(e, div) {" + this._calendar.AfterEventRenderJavaScript + "}", false);
            base.appendProp("afterRender", "function(data, isCallBack) {" + this._calendar.AfterRenderJavaScript + "}", false);
            base.appendProp("eventClickHandling", this._calendar.EventClickHandling, true);
            base.appendProp("onEventClick", "function(e) {" + this._calendar.EventClickJavaScript + "}", false);
            base.appendProp("eventDoubleClickHandling", this._calendar.EventDoubleClickHandling, true);
            base.appendProp("onEventDoubleClick", "function(e) {" + this._calendar.EventDoubleClickJavaScript + "}", false);
            base.appendProp("eventHoverHandling", this._calendar.EventHoverHandling, true);
            base.appendProp("eventSelectHandling", this._calendar.EventSelectHandling, true);
            base.appendProp("onEventSelect", "function(e, change) {" + this._calendar.EventSelectJavaScript + "}", false);
            base.appendProp("eventRightClickHandling", this._calendar.EventRightClickHandling, true);
            base.appendProp("onEventRightClick", "function(e) {" + this._calendar.EventRightClickJavaScript + "}", false);
            base.appendProp("eventDeleteHandling", this._calendar.EventDeleteHandling, true);
            base.appendProp("onEventDelete", "function(e) {" + this._calendar.EventDeleteJavaScript + "}", false);
            base.appendProp("headerClickHandling", this._calendar.HeaderClickHandling, true);
            base.appendProp("onHeaderClick", "function(c) {" + this._calendar.HeaderClickJavaScript + "}", false);
            base.appendProp("eventResizeHandling", this._calendar.EventResizeHandling, true);
            base.appendProp("onEventResize", "function(e, newStart, newEnd) { " + this._calendar.EventResizeJavaScript + "}", false);
            base.appendProp("eventMoveHandling", this._calendar.EventMoveHandling, true);
            base.appendProp("onEventMove", "function(e, newStart, newEnd, newResource, external, ctrl, shift) { var newColumn = newResource; var oldColumn = e.resource(); " + this._calendar.EventMoveJavaScript + "}", false);
            base.appendProp("timeRangeSelectedHandling", this._calendar.TimeRangeSelectedHandling, true);
            base.appendProp("onTimeRangeSelected", "function(start, end, column) { var resource = column; " + this._calendar.TimeRangeSelectedJavaScript + "}", false);
            base.appendProp("timeRangeDoubleClickHandling", this._calendar.TimeRangeDoubleClickHandling, true);
            base.appendProp("onTimeRangeDoubleClick", "function(start, end, column) { var resource = column; " + this._calendar.TimeRangeDoubleClickJavaScript + "}", false);
            base.appendProp("eventEditHandling", this._calendar.EventEditHandling, true);
            base.appendProp("onEventEdit", "function(e, newText) {" + this._calendar.EventEditJavaScript + "}", false);
            if (!string.IsNullOrEmpty(this._calendar.CallBackErrorJavaScript))
            {
                base.appendProp("callbackError", "function(result, context) { " + this._calendar.CallBackErrorJavaScript + " }", false);
            }
            CellTable cellTable = new CellTable(this._calendar);
            cellTable.Process();
            base.appendSerialized("cellProperties", cellTable.GetProperties());
            base.appendSerialized("cellConfig", cellTable.GetConfig());
            List<Hashtable> columns = this._calendar.GetColumns();
            List<Hashtable> hours = this._calendar.GetHours();
            base.appendProp("events.list", SimpleJsonSerializer.Serialize(events), false);
            base.appendProp("hours", SimpleJsonSerializer.Serialize(hours), false);
            base.appendProp("columns", SimpleJsonSerializer.Serialize(columns), false);
            Hashtable hashtable = new Hashtable();
            hashtable["callBack"] = this._calendar.CallBack.GetHash();
            hashtable["colors"] = cellTable.GetHash();
            hashtable["columns"] = this._calendar.Hash(columns);
            hashtable["corner"] = this._calendar.CornerHash(this._calendar.CornerHtml, this._calendar.CornerBackColor);
            hashtable["events"] = this._calendar.Hash(events);
            hashtable["hours"] = this._calendar.Hash(hours);
            base.appendSerialized("hashes", hashtable);
            this.sb.AppendLine("v.init();");
            this.sb.AppendLine("return v.internal.initialized() ? v : null;");
            this.sb.AppendLine("}");
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

        private string GetWidthUnitType()
        {
            if (this._calendar.Width == Unit.Empty)
            {
                return "Percentage";
            }
            return this._calendar.Width.Type.ToString();
        }
    }
}
