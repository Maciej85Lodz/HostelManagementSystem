using HMS.Web.App.Ui.Colors;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Enums.Scheduler;
using HMS.Web.App.Ui.Init;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;

namespace HMS.Web.App.Ui
{
    internal class JsInitScheduler : JsInit
    {
        private readonly HMSScheduler _calendar;

        internal JsInitScheduler(HMSScheduler scheduler)
        {
            this._calendar = scheduler;
        }

        internal string GetCode()
        {
            this.sb = new StringBuilder();
            this.sb.AppendLine("<script type='text/javascript'>");
            this.sb.AppendLine(string.Format("/* HMSPro: {0} */", Assembly.GetExecutingAssembly().FullName));
            this.sb.AppendLine("function " + this._calendar.ClientObjectName + "_Init() {");
            this.sb.AppendLine("var v = new HMS.Scheduler('" + this._calendar.ClientID + "');");
            base.appendProp("allowEventOverlap", this._calendar.AllowEventOverlap);
            base.appendProp("allowMultiSelect", this._calendar.AllowMultiSelect);
            base.appendProp("api", 1);
            base.appendSerialized("afterRenderData", this._calendar.CallbackData);
            base.appendProp("autoRefreshCommand", this._calendar.AutoRefreshCommand);
            base.appendProp("autoRefreshEnabled", this._calendar.AutoRefreshEnabled);
            base.appendProp("autoRefreshInterval", this._calendar.AutoRefreshInterval);
            base.appendProp("autoRefreshMaxCount", this._calendar.AutoRefreshMaxCount);
            base.appendProp("autoScroll", this._calendar.AutoScroll);
            base.appendProp("blockOnCallBack", this._calendar.BlockOnCallBack);
            base.appendProp("borderColor", this._calendar.BorderColor);
            base.appendProp("businessBeginsHour", this._calendar.BusinessBeginsHour);
            base.appendProp("businessEndsHour", this._calendar.BusinessEndsHour);
            base.appendProp("cellBackColor", this._calendar.BackColor);
            base.appendProp("cellBackColorNonBusiness", this._calendar.NonBusinessBackColor);
            base.appendProp("cellBorderColor", this._calendar.CellBorderColor);
            base.appendProp("cellDuration", this._calendar.CellDuration);
            base.appendProp("cellGroupBy", this._calendar.CellGroupBy, true);
            base.appendProp("cellSelectColor", this._calendar.CellSelectColor);
            base.appendProp("cellSweeping", this._calendar.CellSweeping);
            base.appendProp("cellSweepingCacheSize", this._calendar.CellSweepingCacheSize);
            base.appendProp("cellWidth", this._calendar.CellWidth);
            base.appendProp("cellWidthSpec", this._calendar.CellWidthSpec, true);
            base.appendProp("cornerHtml", this._calendar.CornerHtml, true);
            base.appendProp("cornerBackColor", this._calendar.CornerBackColor, true);
            base.appendProp("crosshairColor", this._calendar.CrosshairColor);
            base.appendProp("crosshairOpacity", this._calendar.CrosshairOpacity);
            base.appendProp("crosshairType", this._calendar.Crosshair, true);
            base.appendProp("theme", this._calendar.Theme, true);
            base.appendProp("cssOnly", this._calendar.CssOnly);
            base.appendProp("days", this._calendar.Days);
            base.appendProp("doubleClickTimeout", this._calendar.DoubleClickTimeout);
            base.appendProp("dragOutAllowed", this._calendar.DragOutAllowed);
            base.appendProp("durationBarColor", this._calendar.DurationBarColor);
            base.appendProp("durationBarHeight", this._calendar.DurationBarHeight);
            base.appendProp("durationBarMode", this._calendar.DurationBarMode);
            base.appendProp("durationBarVisible", this._calendar.DurationBarVisible);
            base.appendProp("dynamicEventRendering", this._calendar.DynamicEventRendering);
            base.appendProp("dynamicEventRenderingCacheSweeping", this._calendar.DynamicEventRenderingCacheSweeping);
            base.appendProp("dynamicEventRenderingCacheSize", this._calendar.DynamicEventRenderingCacheSize);
            base.appendProp("dynamicEventRenderingMargin", this._calendar.DynamicEventRenderingMargin);
            base.appendProp("dynamicLoading", this._calendar.DynamicLoading);
            base.appendProp("emptyBackColor", this._calendar.EmptyBackColor);
            base.appendProp("eventBorderColor", this._calendar.EventBorderColor);
            base.appendProp("eventBorderVisible", this._calendar.EventBorderVisible);
            base.appendProp("eventBackColor", this._calendar.EventBackColor);
            base.appendProp("eventCorners", this._calendar.EventCorners, true);
            base.appendProp("eventEndSpec", this._calendar.EventEndSpec);
            base.appendProp("eventFontColor", this._calendar.EventFontColor);
            base.appendProp("eventFontFamily", this._calendar.EventFontFamily, true);
            base.appendProp("eventFontSize", this._calendar.EventFontSize, true);
            base.appendProp("eventHeight", this._calendar.EventHeight);
            base.appendProp("eventMoveMargin", this._calendar.EventMoveMargin);
            base.appendProp("eventMoveToPosition", this._calendar.EventMoveToPosition);
            base.appendProp("eventResizeMargin", this._calendar.EventResizeMargin);
            base.appendProp("eventStackingLineHeight", this._calendar.EventStackingLineHeight);
            base.appendProp("eventTapAndHoldHandling", this._calendar.EventTapAndHoldHandling, true);
            if (!this._calendar.DynamicLoading)
            {
                base.appendSerialized("events.list", this._calendar.GetEvents());
            }
            else
            {
                base.appendProp("events.list", "[]", false);
            }
            base.appendSerialized("links.list", this._calendar.GetLinksJson());
            base.appendProp("floatingEvents", this._calendar.FloatingEvents);
            base.appendProp("floatingTimeHeaders", this._calendar.FloatingTimeHeaders);
            base.appendProp("groupConcurrentEvents", this._calendar.GroupConcurrentEvents);
            base.appendProp("groupConcurrentEventsLimit", this._calendar.GroupConcurrentEventsLimit);
            base.appendProp("headerFontColor", this._calendar.HeaderFontColor);
            base.appendProp("headerFontFamily", this._calendar.HeaderFontFamily, true);
            base.appendProp("headerFontSize", this._calendar.HeaderFontSize, true);
            base.appendProp("headerHeight", this._calendar.HeaderHeight);
            base.appendProp("height", this._calendar.Height.Value);
            base.appendProp("heightSpec", this._calendar.HeightSpec.ToString(), true);
            base.appendProp("hourBorderColor", this._calendar.HourBorderColor);
            base.appendProp("hourFontFamily", this._calendar.HourFontFamily, true);
            base.appendProp("hourFontSize", this._calendar.HourFontSize, true);
            base.appendProp("hourNameBackColor", this._calendar.HourNameBackColor);
            base.appendProp("hourNameBorderColor", this._calendar.HourNameBorderColor);
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
            base.appendProp("scrollX", (this._calendar.ScrollDateTime != DateTime.MinValue) ? this._calendar.TimeHeader.GetPixels(this._calendar.ScrollDateTime).Left : this._calendar.ScrollX);
            base.appendProp("scrollY", this._calendar.ScrollY);
            base.appendSerialized("selectedRows", this._calendar.GetSelectedRows());
            base.appendProp("snapToGrid", this._calendar.SnapToGrid);
            base.appendProp("layout", this._calendar.Layout.ToString(), true);
            base.appendProp("messageHideAfter", this._calendar.MessageHideAfter);
            base.appendProp("messageBarPosition", this._calendar.MessageBarPosition, true);
            base.appendProp("moveBy", this._calendar.MoveBy);
            base.appendProp("notifyCommit", this._calendar.NotifyCommit);
            base.appendProp("numberFormat", Assembly.GetExecutingAssembly().FullName.Contains(".1,") ? "0.00" : null);
            base.appendProp("progressiveRowRendering", this._calendar.ProgressiveRowRendering);
            base.appendProp("progressiveRowRenderingPreload", this._calendar.ProgressiveRowRenderingPreload);
            base.appendSerialized("resources", this._calendar.GetResources());
            base.appendProp("rowMarginBottom", this._calendar.RowMarginBottom);
            base.appendProp("rowMinHeight", this._calendar.RowMinHeight);
            base.appendSerialized("rowHeaderColumns", this._calendar.HeaderColumns.GetList());
            base.appendProp("rowHeaderHideIconEnabled", this._calendar.RowHeaderHideIconEnabled);
            base.appendProp("rowHeaderScrolling", this._calendar.RowHeaderScrolling);
            base.appendProp("rowHeaderWidth", this._calendar.RowHeaderWidth);
            base.appendProp("rowHeaderWidthAutoFit", this._calendar.RowHeaderWidthAutoFit);
            base.appendSerialized("rowHeaderCols", this._calendar.RowHeaderColumns);
            base.appendProp("scale", this._calendar.Scale, true);
            base.appendProp("scrollDelayDynamic", this._calendar.ScrollDelayDynamic);
            base.appendProp("scrollDelayCells", this._calendar.ScrollDelayCells);
            base.appendProp("scrollDelayEvents", this._calendar.ScrollDelayEvents);
            base.appendProp("scrollDelayFloats", this._calendar.ScrollDelayFloats);
            base.appendSerialized("separators", this._calendar.Separators.GetList());
            base.appendProp("shadow", this._calendar.Shadow.ToString(), true);
            base.appendProp("showBaseTimeHeader", this._calendar.ShowBaseTimeHeader);
            base.appendProp("showNonBusiness", this._calendar.ShowNonBusiness);
            base.appendProp("showToolTip", this._calendar.ShowToolTip);
            base.appendSerialized("sortDirections", this._calendar.SortFields.Directions);
            base.appendProp("startDate", this._calendar.StartDate.ToString("s"));
            base.appendProp("syncResourceTree", this._calendar.SyncResourceTree);
            base.appendProp("syncLinks", this._calendar.SyncLinks);
            base.appendProp("timeBreakColor", this._calendar.TimeBreakColor);
            base.appendProp("timeFormat", this._calendar.TimeFormat, true);
            base.appendSerialized("timeHeader", this._calendar.TimeHeader.GetList());
            if (this._calendar.Scale == TimeScale.Manual)
            {
                base.appendSerialized("timeline", this._calendar.Timeline.ToJson());
            }
            else
            {
                base.appendSerialized("timeline", this._calendar.TimeHeader.Timeline.ToJson());
            }
            base.appendSerialized("timeHeaders", this._calendar.TimeHeadersResolved.ToJson());
            base.appendProp("treeEnabled", this._calendar.TreeEnabled);
            base.appendProp("treeIndent", this._calendar.TreeIndent);
            base.appendProp("treeImageCollapse", this._calendar.ResolveUrlSafe(this._calendar.TreeImageCollapse), true);
            base.appendProp("treeImageExpand", this._calendar.ResolveUrlSafe(this._calendar.TreeImageExpand), true);
            base.appendProp("treeImageNoChildren", this._calendar.ResolveUrlSafe(this._calendar.TreeImageNoChildren), true);
            base.appendProp("treeImageMarginLeft", this._calendar.TreeImageMarginLeft);
            base.appendProp("treeImageMarginTop", this._calendar.TreeImageMarginTop);
            base.appendProp("treePreventParentUsage", this._calendar.TreePreventParentUsage);
            base.appendProp("treeAutoExpand", this._calendar.TreeAutoExpand);
            base.appendProp("uniqueID", this._calendar.UniqueID, true);
            base.appendProp("useEventBoxes", this._calendar.UseEventBoxes, true);
            base.appendProp("viewType", this._calendar.ViewType, true);
            base.appendProp("visible", this._calendar.Visible);
            if (this._calendar.Width != Unit.Empty)
            {
                base.appendProp("width", this._calendar.Width, true);
            }
            base.appendProp("weekStarts", this.WeekStarts());
            if (this._calendar.TagFields != null)
            {
                base.appendSerialized("tagFields", this._calendar.TagFields);
            }
            base.appendProp("eventMovingStartEndEnabled", this._calendar.EventMovingStartEndEnabled);
            base.appendProp("eventResizingStartEndEnabled", this._calendar.EventResizingStartEndEnabled);
            base.appendProp("timeRangeSelectingStartEndEnabled", this._calendar.TimeRangeSelectingStartEndEnabled);
            base.appendProp("eventMovingStartEndFormat", this._calendar.EventMovingStartEndFormat);
            base.appendProp("eventResizingStartEndFormat", this._calendar.EventResizingStartEndFormat);
            base.appendProp("timeRangeSelectingStartEndFormat", this._calendar.TimeRangeSelectingStartEndFormat);
            if (this._calendar.ContextMenuEvent != null)
            {
                base.appendProp("contextMenu", this._calendar.ContextMenuEvent.ClientObjectName, false);
            }
            if (this._calendar.ContextMenuSelection != null)
            {
                base.appendProp("contextMenuSelection", this._calendar.ContextMenuSelection.ClientObjectName, false);
            }
            if (this._calendar.ContextMenuResource != null)
            {
                base.appendProp("contextMenuResource", this._calendar.ContextMenuResource.ClientObjectName, false);
            }
            if (this._calendar.BubbleEvent != null)
            {
                base.appendProp("bubble", this._calendar.BubbleEvent.ClientObjectName, false);
            }
            if (this._calendar.BubbleCell != null)
            {
                base.appendProp("cellBubble", this._calendar.BubbleCell.ClientObjectName, false);
            }
            if (this._calendar.BubbleResource != null)
            {
                base.appendProp("resourceBubble", this._calendar.BubbleResource.ClientObjectName, false);
            }
            if (!string.IsNullOrEmpty(this._calendar.CallBackErrorJavaScript))
            {
                base.appendProp("callbackError", "function(result, context) { " + this._calendar.CallBackErrorJavaScript + " }", false);
            }
            this._calendar.Page.ClientScript.GetCallbackEventReference(this._calendar, null, null, null, null, true);
            base.appendProp("afterEventRender", "function(e, div) {" + this._calendar.AfterEventRenderJavaScript + "}", false);
            base.appendProp("afterRender", "function(data, isCallBack) {" + this._calendar.AfterRenderJavaScript + "}", false);
            base.appendProp("eventClickHandling", this._calendar.EventClickHandling, true);
            base.appendProp("onEventClick", "function(e) {" + this._calendar.EventClickJavaScript + "}", false);
            base.appendProp("eventHoverHandling", this._calendar.EventHoverHandling, true);
            base.appendProp("eventDoubleClickHandling", this._calendar.EventDoubleClickHandling, true);
            base.appendProp("onEventDoubleClick", "function(e) {" + this._calendar.EventDoubleClickJavaScript + "}", false);
            base.appendProp("eventRightClickHandling", this._calendar.EventRightClickHandling, true);
            base.appendProp("onEventRightClick", "function(e) {" + this._calendar.EventRightClickJavaScript + "}", false);
            base.appendProp("eventResizeHandling", this._calendar.EventResizeHandling, true);
            base.appendProp("onEventResize", "function(e, newStart, newEnd) { " + this._calendar.EventResizeJavaScript + "}", false);
            base.appendProp("eventSelectHandling", this._calendar.EventSelectHandling, true);
            base.appendProp("onEventSelect", "function(e, change) {" + this._calendar.EventSelectJavaScript + "}", false);
            base.appendProp("eventMoveHandling", this._calendar.EventMoveHandling, true);
            base.appendProp("onEventMove", "function(e, newStart, newEnd, newResource, external, ctrl, shift) { var newColumn = newResource; " + this._calendar.EventMoveJavaScript + "}", false);
            base.appendProp("timeRangeSelectedHandling", this._calendar.TimeRangeSelectedHandling, true);
            base.appendProp("onTimeRangeSelected", "function(start, end, resource) { var column = resource; " + this._calendar.TimeRangeSelectedJavaScript + "}", false);
            base.appendProp("timeRangeDoubleClickHandling", this._calendar.TimeRangeDoubleClickHandling, true);
            base.appendProp("onTimeRangeDoubleClick", "function(start, end, resource) {" + this._calendar.TimeRangeDoubleClickJavaScript + "}", false);
            base.appendProp("eventEditHandling", this._calendar.EventEditHandling, true);
            base.appendProp("onEventEdit", "function(e, newText) {" + this._calendar.EventEditJavaScript + "}", false);
            base.appendProp("eventDeleteHandling", this._calendar.EventDeleteHandling, true);
            base.appendProp("onEventDelete", "function(e) {" + this._calendar.EventDeleteJavaScript + "}", false);
            if (this._calendar.ResourceHeaderClickHandling == ResourceHeaderClickHandlingType.UseRowClickHandling)
            {
                base.appendProp("rowClickHandling", this._calendar.RowClickHandling, true);
            }
            else
            {
                base.appendProp("rowClickHandling", this._calendar.ResourceHeaderClickHandling, true);
            }
            base.appendProp("onRowClick", "function(row) { var resource = row; " + this._calendar.RowClickJavaScript + "}", false);
            base.appendProp("rowDoubleClickHandling", this._calendar.RowDoubleClickHandling, true);
            base.appendProp("onRowDoubleClick", "function(row) { var resource = row; " + this._calendar.RowDoubleClickJavaScript + "}", false);
            base.appendProp("timeHeaderClickHandling", this._calendar.TimeHeaderClickHandling, true);
            base.appendProp("onTimeHeaderClick", "function(header) {" + this._calendar.TimeHeaderClickJavaScript + "}", false);
            base.appendProp("resourceCollapseHandling", this._calendar.ResourceCollapseHandling, true);
            base.appendProp("onResourceCollapse", "function(resource) {" + this._calendar.ResourceCollapseJavaScript + "}", false);
            base.appendProp("resourceExpandHandling", this._calendar.ResourceExpandHandling, true);
            base.appendProp("onResourceExpand", "function(resource) {" + this._calendar.ResourceExpandJavaScript + "}", false);
            base.appendProp("rowSelectHandling", this._calendar.RowSelectHandling, true);
            base.appendProp("onRowSelect", "function(row, change) {" + this._calendar.RowSelectJavaScript + "}", false);
            base.appendProp("rowEditHandling", this._calendar.RowEditHandling, true);
            base.appendProp("onRowEdit", "function(row, newText) { var resource = row; " + this._calendar.RowEditJavaScript + "}", false);
            base.appendProp("rowCreateHandling", this._calendar.RowCreateHandling, true);
            base.appendProp("onRowCreate", "function(args) { " + this._calendar.RowCreateJavaScript + "}", false);
            base.appendProp("rowMoveHandling", this._calendar.RowMoveHandling, true);
            base.appendProp("onRowMove", "function(source, target, position) { " + this._calendar.RowMoveJavaScript + "}", false);
            base.appendProp("onAutoRefresh", "function(args) {" + this._calendar.AutoRefreshJavaScript + "}", false);
            CellTable cellTable = new CellTable(this._calendar);
            cellTable.Process();
            base.appendSerialized("cellProperties", cellTable.GetProperties());
            base.appendSerialized("cellConfig", cellTable.GetConfig());
            Hashtable hashtable = new Hashtable();
            hashtable["separators"] = this._calendar.Separators.GetHash();
            hashtable["colors"] = cellTable.GetHash();
            hashtable["timeHeader"] = this._calendar.TimeHeader.GetHash();
            hashtable["corner"] = this._calendar.CornerHash(this._calendar.CornerHtml, this._calendar.CornerBackColor);
            hashtable["callBack"] = this._calendar.CallBack.GetHash();
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

        private int WeekStarts()
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
                    throw new ArgumentOutOfRangeException("This WeekStarts value is not supported (" + this._calendar.WeekStarts + ").");
            }
        }
    }
}
