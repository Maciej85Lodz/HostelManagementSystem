using HMS.Json;
using HMS.Utils;
using HMS.Web.App.Ui.Ajax;
using HMS.Web.App.Ui.Colors;
using HMS.Web.App.Ui.Data;
using HMS.Web.App.Ui.Design;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Enums.Scheduler;
using HMS.Web.App.Ui.Events;
using HMS.Web.App.Ui.Events.Common;
using HMS.Web.App.Ui.Events.Scheduler;
using HMS.Web.App.Ui.Export;
using HMS.Web.App.Ui.Json;
using HMS.Web.App..Ui.Recurrence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web.App;
using System.Web.App.UI;
using System.Web.App.UI.Web.AppControls;

namespace HMS.Web.App.Ui
{
    [DefaultProperty(null), ToolboxBitmap(typeof(System.Web.App.UI.Web.AppControls.Calendar)), ParseChildren(true), PersistChildren(false), Themeable(true)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HMSScheduler : DataBoundControl, IPostBackEventHandler, ICallbackEventHandler
    {
        internal List<Day> Rows;

        internal HMSMenu ContextMenuEvent;

        internal HMSMenu ContextMenuSelection;

        internal HMSMenu ContextMenuResource;

        internal HMSBubble BubbleEvent;

        internal HMSBubble BubbleCell;

        internal HMSBubble BubbleResource;

        private string _dataStartField;

        private string _dataEndField;

        private string _dataTextField;

        private string _dataValueField;

        private string _dataResourceField;

        private string _dataTagFields;

        private string _dataServerTagFields;

        private string _dataRecurrenceField;

        private ArrayList _items = new ArrayList();

        private LinkCollection _links;

        private bool isExport;

        internal DateTime ScrollDateTime = DateTime.MinValue;

        internal List<int> RowHeaderColumns;

        internal SortExpression SortFields = new SortExpression();

        private JsonData _clientState;

        private Section _viewPort;

        private HMSSchedulerCallBack _callback;

        private Hashtable _hashes = new Hashtable();

        private Exception _callbackException;

        private CallBackUpdateType _callbackUpdateType;

        internal object CallbackData;

        private string _callbackMessage;

        private string _callbackAction;

        private EventSource _callbackSource;

        internal SchedulerTimeHeader TimeHeader;

        public DateTimeFormatInfo DateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;

        public List<EventInfo> SelectedEvents = new List<EventInfo>();

        private List<Hashtable> resourcesCache;

        private List<Hashtable> linksCache;

        [Category("User actions"), Description("This event is fired using client-side .commandCallBack() function.")]
        public event HMS.Web.App.App.Ui.Events.CommandEventHandler Command;

        [Category("User actions"), Description("Fires when a user resizes an event.  EventResizeHandling must be set to PostBack or CallBack.")]
        public event EventResizeEventHandler EventResize;

        [Category("User actions"), Description("Fires when a user moves an event. EventMoveHandling must be set to PostBack or CallBack.")]
        public event EventMoveEventHandler EventMove;

        [Category("User actions"), Description("Fires when a user clicks an event.  EventClickHandling must be set to PostBack or CallBack.")]
        public event EventClickEventHandler EventClick;

        [Category("User actions"), Description("Fires when the user selects an event.")]
        public event EventSelectEventHandler EventSelect;

        [Category("User actions"), Description("This event is fired when the user clicks on a delete 'X' button in the upper-right corner of an event.")]
        public event EventDeleteEventHandler EventDelete;

        [Category("User actions"), Description("Fires when a user double-clicks an event. EventDoubleClickHandling must be set to PostBack or CallBack.")]
        public event EventClickEventHandler EventDoubleClick;

        [Category("User actions"), Description("Fires when queued client-side operations are notified to the server.")]
        public event NotifyEventHandler Notify;

        [Category("User actions"), Description("Fires when client-side EventUpdate event is notified to the server.")]
        public event EventUpdateEventHandler EventUpdate;

        [Category("User actions"), Description("Fires when client-side EventRemove event is notified to the server.")]
        public event EventRemoveEventHandler EventRemove;

        [Category("User actions"), Description("Fires when client-side EventAdd event is notified to the server.")]
        public event EventAddEventHandler EventAdd;

        [Category("User actions"), Description("Fires when a user confirms event text changes (after editing). EventClickHandling must be set to Edit and EventEditHandling must be set to PostBack or CallBack.")]
        public event EventEditEventHandler EventEdit;

        [Category("User actions"), Description("This event is fired when a user clicks an event with a right mouse button.")]
        public event EventRightClickEventHandler EventRightClick;

        [Category("User actions"), Description("Fires when a user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack.")]
        public event EventMenuClickEventHandler EventMenuClick;

        [Category("User actions"), Description("Fires when a user clicks a time cell or selects cells by mouse dragging. TimeRangeSelectedHandling must be set to PostBack or CallBack.")]
        public event TimeRangeSelectedEventHandler TimeRangeSelected;

        [Category("User actions"), Description("Fires when a user double-clicks a selected time range. TimeRangeDoubleClickHandling must be set to PostBack or CallBack.")]
        public event TimeRangeDoubleClickEventHandler TimeRangeDoubleClick;

        [Category("User actions"), Description("Fires when the expand icon of a resource with ChildrenLoaded=\"false\" is clicked.")]
        public event LoadNodeEventHandler LoadNode;

        [Category("User actions"), Description("Fires when the user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack.")]
        public event TimeRangeMenuClickEventHandler TimeRangeMenuClick;

        [Browsable(false), Category("User actions"), Description("Fires when the user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack. Obsolete: Use RowMenuClick event instead."), Obsolete("Use RowMenuClick event instead.")]
        public event ResourceHeaderMenuClickEventHandler ResourceHeaderMenuClick;

        [Category("User actions"), Description("Fires when the user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack.")]
        public event RowMenuClickEventHandler RowMenuClick;

        [Browsable(false), Category("User actions"), Description("Fires when the user clicks a row header."), Obsolete("Use RowClick event instead.")]
        public event ResourceHeaderClickEventHandler ResourceHeaderClick;

        [Category("User actions"), Description("Fires when the user clicks a row header.")]
        public event RowClickEventHandler RowClick;

        [Category("User actions"), Description("Fires when the user selects a row.")]
        public event RowSelectEventHandler RowSelect;

        [Category("User actions"), Description("Fires when the user finishes inline row name editing.")]
        public event RowEditEventHandler RowEdit;

        [Category("User actions"), Description("Fires when the user drops a resource at the new location.")]
        public event RowMoveEventHandler RowMove;

        [Category("User actions"), Description("Fires when the user finishes editing a new row field.")]
        public event RowCreateEventHandler RowCreate;

        [Category("User actions"), Description("Fires when the user clicks a row header.")]
        public event TimeHeaderClickEventHandler TimeHeaderClick;

        [Category("User actions"), Description("Fires when the user collapses a resource tree node.")]
        public event ResourceCollapseEventHandler ResourceCollapse;

        [Category("User actions"), Description("Fires when the user expands a resource tree node.")]
        public event ResourceExpandEventHandler ResourceExpand;

        [Category("User actions"), Description("This event is fired when the user scrolls using either the horizontal or vertical scrollbar.")]
        public event ScrollEventHandler Scroll;

        [Category("Rendering"), Description("Use this event to modify event properties before rendering.")]
        public event BeforeEventRenderEventHandler BeforeEventRender;

        [Category("Preprocessing"), Description("Use this event to apply custom recurrence rules.")]
        public event BeforeEventRecurrenceHandler BeforeEventRecurrence;

        [Category("Rendering"), Description("Use this event to modify time header (X axis) properties before rendering.")]
        public event BeforeTimeHeaderRenderEventHandler BeforeTimeHeaderRender;

        [Category("Rendering"), Description("Use this event to modify resource header (Y axis) properties before rendering.")]
        public event BeforeResHeaderRenderEventHandler BeforeResHeaderRender;

        [Category("Rendering"), Description("Use this event to modify time cell properties before rendering.")]
        public event BeforeCellRenderEventHandler BeforeCellRender;

        [Category("Rendering"), Description("Use this event to modify link properties before rendering.")]
        public event BeforeLinkRenderEventHandler BeforeLinkRender;

        [Category("Rendering"), Description("Use this event to hide timeline cells.")]
        public event IncludeCellEventHandler IncludeCell;

        public System.Globalization.Calendar Calendar
        {
            get
            {
                return this.DateTimeFormatInfo.Calendar;
            }
        }

        public List<RowInfo> SelectedRows
        {
            get;
            private set;
        }

        public HMSSchedulerCallBack CallBack
        {
            get
            {
                if (this._callback == null)
                {
                    this._callback = new HMSSchedulerCallBack(this);
                }
                return this._callback;
            }
        }

        internal int TotalRowHeaderWidth
        {
            get
            {
                int num = 0;
                if (this.HeaderColumns != null && this.HeaderColumns.Count > 0)
                {
                    foreach (RowHeaderColumn rowHeaderColumn in this.HeaderColumns)
                    {
                        num += rowHeaderColumn.Width;
                    }
                    return num;
                }
                if (this.RowHeaderColumns != null)
                {
                    foreach (int current in this.RowHeaderColumns)
                    {
                        num += current;
                    }
                    return num;
                }
                return this.RowHeaderWidth;
            }
        }

        private ArrayList Items
        {
            get
            {
                if (this.StoreEventsInViewState)
                {
                    return (ArrayList)this.ViewState["Items"];
                }
                return this._items;
            }
            set
            {
                this._items = value;
                if (this.StoreEventsInViewState)
                {
                    this.ViewState["Items"] = this._items;
                }
            }
        }

        internal string[] TagFields
        {
            get
            {
                return (string[])this.ViewState["TagFields"];
            }
        }

        internal string[] ServerTagFields
        {
            get
            {
                return this.LoadTagFields(this.DataServerTagFields);
            }
        }

        internal DayOfWeek ResolvedWeekStart
        {
            get
            {
                switch (this.WeekStarts)
                {
                    case WeekStartsEnum.Sunday:
                        return DayOfWeek.Sunday;
                    case WeekStartsEnum.Monday:
                        return DayOfWeek.Monday;
                    case WeekStartsEnum.Tuesday:
                        return DayOfWeek.Tuesday;
                    case WeekStartsEnum.Wednesday:
                        return DayOfWeek.Wednesday;
                    case WeekStartsEnum.Thursday:
                        return DayOfWeek.Thursday;
                    case WeekStartsEnum.Friday:
                        return DayOfWeek.Friday;
                    case WeekStartsEnum.Saturday:
                        return DayOfWeek.Saturday;
                    case WeekStartsEnum.Auto:
                        return Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                    default:
                        throw new ArgumentOutOfRangeException("This WeekStarts value is not supported (" + this.WeekStarts + ").");
                }
            }
        }

        public DateTime VisibleStart
        {
            get
            {
                DateTime result = this.StartDate;
                if (this.Scale == TimeScale.Manual)
                {
                    if (this.Timeline.Count == 0)
                    {
                        throw new ArgumentException("The Timeline doesn't contain any cells (Scale='Manual' mode)");
                    }
                    result = this.Timeline[0].Start;
                }
                return result;
            }
        }

        public DateTime VisibleEnd
        {
            get
            {
                if (this.Scale != TimeScale.Manual)
                {
                    return this.Calendar.AddDays(this.StartDate, this.Days);
                }
                if (this.Timeline.Count == 0)
                {
                    throw new ArgumentException("The Timeline doesn't contain any cells (Scale='Manual' mode)");
                }
                return this.Timeline[this.Timeline.Count - 1].End;
            }
        }

        [Category("Behavior"), Description("The first day to be shown. Default is DateTime.Today.")]
        public DateTime StartDate
        {
            get
            {
                DateTime result;
                if (this.ViewState["StartDate"] == null)
                {
                    result = DateTime.Today;
                }
                else
                {
                    result = (DateTime)this.ViewState["StartDate"];
                }
                return result;
            }
            set
            {
                this.ViewState["StartDate"] = new DateTime(value.Year, value.Month, value.Day);
            }
        }

        [Browsable(false)]
        public DateTime EndDate
        {
            get
            {
                return this.StartDate.AddDays((double)this.Days);
            }
        }

        [Category("Behavior"), DefaultValue(1), Description("The number of days to be displayed on the calendar. Default value is 1.")]
        public int Days
        {
            get
            {
                if (this.ViewState["Days"] == null)
                {
                    return 1;
                }
                return (int)this.ViewState["Days"];
            }
            set
            {
                int num = value;
                if (num < 1)
                {
                    num = 1;
                }
                this.ViewState["Days"] = num;
            }
        }

        [Category("Appearance"), DefaultValue("d"), Description("Format of the date in the row header (e.g. \"d\", \"yyyy-MM-dd\").")]
        public string HeaderDateFormat
        {
            get
            {
                if (this.ViewState["HeaderDateFormat"] == null)
                {
                    return "d";
                }
                return (string)this.ViewState["HeaderDateFormat"];
            }
            set
            {
                this.ViewState["HeaderDateFormat"] = value;
            }
        }

        [Category("Layout"), DefaultValue(40), Description("Cell width in pixels.")]
        public int CellWidth
        {
            get
            {
                if (this.ViewState["CellWidth"] == null)
                {
                    return 40;
                }
                return (int)this.ViewState["CellWidth"];
            }
            set
            {
                this.ViewState["CellWidth"] = value;
            }
        }

        [Category("Layout"), DefaultValue(60), Description("Cell size in minutes.")]
        public int CellDuration
        {
            get
            {
                if (this.ViewState["CellDuration"] == null)
                {
                    return 60;
                }
                return (int)this.ViewState["CellDuration"];
            }
            set
            {
                this.ViewState["CellDuration"] = value;
            }
        }

        [Category("Behavior"), Description("Collection of rows that will be used when ViewType property is set to ViewTypeEnum.Resources."), PersistenceMode(PersistenceMode.InnerProperty)]
        public ResourceCollection Resources
        {
            get
            {
                if (this.ViewState["Resources"] == null)
                {
                    ResourceCollection resourceCollection = new ResourceCollection();
                    resourceCollection.designMode = base.DesignMode;
                    this.ViewState["Resources"] = resourceCollection;
                }
                return (ResourceCollection)this.ViewState["Resources"];
            }
        }

        [Category("Behavior"), Description("Collection of separators. Separators are customizable vertical lines drawn at a certain time position."), PersistenceMode(PersistenceMode.InnerProperty)]
        public SeparatorCollection Separators
        {
            get
            {
                if (this.ViewState["Separators"] == null)
                {
                    SeparatorCollection separatorCollection = new SeparatorCollection();
                    separatorCollection.designMode = base.DesignMode;
                    this.ViewState["Separators"] = separatorCollection;
                }
                return (SeparatorCollection)this.ViewState["Separators"];
            }
        }

        [Category("Data"), Description("The name of the column that contains the event starting date and time (must be convertible to DateTime).")]
        public string DataStartField
        {
            get
            {
                return this._dataStartField;
            }
            set
            {
                this._dataStartField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("The name of the column that contains the event ending date and time (must be convertible to DateTime).")]
        public string DataEndField
        {
            get
            {
                return this._dataEndField;
            }
            set
            {
                this._dataEndField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("The name of the column that contains the name of an event.")]
        public string DataTextField
        {
            get
            {
                return this._dataTextField;
            }
            set
            {
                this._dataTextField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("The name of the column that contains the id (primary key). Obsolete. Use .DataIdField instead."), Obsolete("Use .DataIdField instead.")]
        public string DataValueField
        {
            get
            {
                return this.DataIdField;
            }
            set
            {
                this.DataIdField = value;
            }
        }

        [Category("Data"), Description("The name of the column that contains the id (primary key).")]
        public string DataIdField
        {
            get
            {
                return this._dataValueField;
            }
            set
            {
                this._dataValueField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("The name of the column that contains the column id.")]
        public string DataResourceField
        {
            get
            {
                return this._dataResourceField;
            }
            set
            {
                this._dataResourceField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("Names of the data source fields (comma separated) that contain custom event data.")]
        public string DataTagFields
        {
            get
            {
                return this._dataTagFields;
            }
            set
            {
                this._dataTagFields = value;
                this.ViewState["TagFields"] = this.LoadTagFields(this._dataTagFields);
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("Names of the data source fields (comma separated) that contain custom event data.")]
        public string DataServerTagFields
        {
            get
            {
                return this._dataServerTagFields;
            }
            set
            {
                this._dataServerTagFields = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Data"), Description("Name of the data source field that contains recurrence information string.")]
        public string DataRecurrenceField
        {
            get
            {
                return this._dataRecurrenceField;
            }
            set
            {
                this._dataRecurrenceField = value;
                if (base.Initialized)
                {
                    this.OnDataPropertyChanged();
                }
            }
        }

        [Category("Appearance"), Description("Color of the hour names background."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color HourNameBackColor
        {
            get
            {
                if (this.ViewState["HourNameBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#ECE9D8");
                }
                return (Color)this.ViewState["HourNameBackColor"];
            }
            set
            {
                this.ViewState["HourNameBackColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the horizontal border that separates our names."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color HourNameBorderColor
        {
            get
            {
                if (this.ViewState["HourNameBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#ACA899");
                }
                return (Color)this.ViewState["HourNameBorderColor"];
            }
            set
            {
                this.ViewState["HourNameBorderColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of an event border."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color EventBorderColor
        {
            get
            {
                if (this.ViewState["EventBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["EventBorderColor"];
            }
            set
            {
                this.ViewState["EventBorderColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether the event border is visible.")]
        public bool EventBorderVisible
        {
            get
            {
                return this.ViewState["EventBorderVisible"] == null || (bool)this.ViewState["EventBorderVisible"];
            }
            set
            {
                this.ViewState["EventBorderVisible"] = value;
            }
        }

        [Category("Appearance"), Description("Color of an event background."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color EventBackColor
        {
            get
            {
                if (this.ViewState["EventBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#FFFFFF");
                }
                return (Color)this.ViewState["EventBackColor"];
            }
            set
            {
                this.ViewState["EventBackColor"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Allows selecting an exact position in the target cell when moving.")]
        public bool EventMoveToPosition
        {
            get
            {
                return this.ViewState["EventMoveToPosition"] != null && (bool)this.ViewState["EventMoveToPosition"];
            }
            set
            {
                this.ViewState["EventMoveToPosition"] = value;
            }
        }

        public override Color BackColor
        {
            get
            {
                if (this.ViewState["BackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#FFFFD5");
                }
                return (Color)this.ViewState["BackColor"];
            }
            set
            {
                this.ViewState["BackColor"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(9), Description("Start of the business day (hour from 0 to 23).")]
        public int BusinessBeginsHour
        {
            get
            {
                if (this.ViewState["BusinessBeginsHour"] == null)
                {
                    return 9;
                }
                return (int)this.ViewState["BusinessBeginsHour"];
            }
            set
            {
                if (value < 0)
                {
                    this.ViewState["BusinessBeginsHour"] = 0;
                    return;
                }
                if (value > 23)
                {
                    this.ViewState["BusinessBeginsHour"] = 23;
                    return;
                }
                this.ViewState["BusinessBeginsHour"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(18), Description("End of the business day (hour from 1 to 24).")]
        public int BusinessEndsHour
        {
            get
            {
                if (this.ViewState["BusinessEndsHour"] == null)
                {
                    return 18;
                }
                return (int)this.ViewState["BusinessEndsHour"];
            }
            set
            {
                if (value < this.BusinessBeginsHour)
                {
                    this.ViewState["BusinessEndsHour"] = this.BusinessBeginsHour + 1;
                    return;
                }
                if (value > 24)
                {
                    this.ViewState["BusinessEndsHour"] = 24;
                    return;
                }
                this.ViewState["BusinessEndsHour"] = value;
            }
        }

        [DefaultValue(true), Description("True if the non-business hour cells should be visible.")]
        public bool ShowNonBusiness
        {
            get
            {
                return this.ViewState["ShowNonBusiness"] == null || (bool)this.ViewState["ShowNonBusiness"];
            }
            set
            {
                this.ViewState["ShowNonBusiness"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("Tahoma"), Description("Font family of the event text, e.g. \"Tahoma\".")]
        public string HeaderFontFamily
        {
            get
            {
                if (this.ViewState["HeaderFontFamily"] == null)
                {
                    return "Tahoma";
                }
                return (string)this.ViewState["HeaderFontFamily"];
            }
            set
            {
                this.ViewState["HeaderFontFamily"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the column header font.")]
        public Color HeaderFontColor
        {
            get
            {
                if (this.ViewState["HeaderFontColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["HeaderFontColor"];
            }
            set
            {
                this.ViewState["HeaderFontColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("10pt"), Description("Font size of the row header, e.g. \"10pt\".")]
        public string HeaderFontSize
        {
            get
            {
                if (this.ViewState["HeaderFontSize"] == null)
                {
                    return "10pt";
                }
                return (string)this.ViewState["HeaderFontSize"];
            }
            set
            {
                this.ViewState["HeaderFontSize"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("Tahoma"), Description("Font family of the hour names (horizontal axis), e.g. \"Tahoma\".")]
        public string HourFontFamily
        {
            get
            {
                if (this.ViewState["HourFontFamily"] == null)
                {
                    return "Tahoma";
                }
                return (string)this.ViewState["HourFontFamily"];
            }
            set
            {
                this.ViewState["HourFontFamily"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("10pt"), Description("Font size of the hour names (horizontal axis), e.g. \"10pt\".")]
        public string HourFontSize
        {
            get
            {
                if (this.ViewState["HourFontSize"] == null)
                {
                    return "10pt";
                }
                return (string)this.ViewState["HourFontSize"];
            }
            set
            {
                this.ViewState["HourFontSize"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("Tahoma"), Description("Font family of the event text, e.g. \"Tahoma\".")]
        public string EventFontFamily
        {
            get
            {
                if (this.ViewState["EventFontFamily"] == null)
                {
                    return "Tahoma";
                }
                return (string)this.ViewState["EventFontFamily"];
            }
            set
            {
                this.ViewState["EventFontFamily"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("7pt"), Description("Font size of the event text, e.g. \"7pt\".")]
        public string EventFontSize
        {
            get
            {
                if (this.ViewState["EventFontSize"] == null)
                {
                    return "7pt";
                }
                return (string)this.ViewState["EventFontSize"];
            }
            set
            {
                this.ViewState["EventFontSize"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the event font.")]
        public Color EventFontColor
        {
            get
            {
                if (this.ViewState["EventFontColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["EventFontColor"];
            }
            set
            {
                this.ViewState["EventFontColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("25"), Description("Height of the event cell in pixels.")]
        public int EventHeight
        {
            get
            {
                if (this.ViewState["EventHeight"] == null)
                {
                    return 25;
                }
                return (int)this.ViewState["EventHeight"];
            }
            set
            {
                this.ViewState["EventHeight"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("20"), Description("Height of the header cells (with hour names) in pixels.")]
        public int HeaderHeight
        {
            get
            {
                if (this.ViewState["HeaderHeight"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["HeaderHeight"];
            }
            set
            {
                this.ViewState["HeaderHeight"] = value;
            }
        }

        [Description("JavaScript instance name on the client side. If it is not specified the control ClientID will be used.")]
        public string ClientObjectName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ViewState["ClientObjectName"] as string))
                {
                    return (string)this.ViewState["ClientObjectName"];
                }
                if (base.DesignMode)
                {
                    return null;
                }
                return this.ClientID;
            }
            set
            {
                this.ViewState["ClientObjectName"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#FFF4BC"), Description("Background color of time cells outside of the busines hours."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color NonBusinessBackColor
        {
            get
            {
                if (this.ViewState["NonBusinessBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#FFF4BC");
                }
                return (Color)this.ViewState["NonBusinessBackColor"];
            }
            set
            {
                this.ViewState["NonBusinessBackColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#FFFFFF"), Description("Main area background color (no time cells)."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color EmptyBackColor
        {
            get
            {
                if (this.ViewState["EmptyBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#FFFFFF");
                }
                return (Color)this.ViewState["EmptyBackColor"];
            }
            set
            {
                this.ViewState["EmptyBackColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the vertical border that separates hour names."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color HourBorderColor
        {
            get
            {
                if (this.ViewState["HourBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#EAD098");
                }
                return (Color)this.ViewState["HourBorderColor"];
            }
            set
            {
                this.ViewState["HourBorderColor"] = value;
            }
        }

        public override Color BorderColor
        {
            get
            {
                if (this.ViewState["BorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["BorderColor"];
            }
            set
            {
                this.ViewState["BorderColor"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Determines the action to be executed after a user resizes an event. If set to Disabled resizing is not enabled on the client side.")]
        public UserActionHandling EventResizeHandling
        {
            get
            {
                if (this.ViewState["EventResizeHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["EventResizeHandling"];
            }
            set
            {
                this.ViewState["EventResizeHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.value() + ' was resized.');"), Description("Javascript function that is executed when a users moves an event.")]
        public string EventResizeJavaScript
        {
            get
            {
                if (this.ViewState["EventResizeJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.value() + ' was resized.');";
                }
                return (string)this.ViewState["EventResizeJavaScript"];
            }
            set
            {
                this.ViewState["EventResizeJavaScript"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(EventRendering.Progressive), Description("Specifies how the events should be rendered.")]
        public EventRendering DynamicEventRendering
        {
            get
            {
                if (this.ViewState["DynamicEventRendering"] == null)
                {
                    return EventRendering.Progressive;
                }
                return (EventRendering)this.ViewState["DynamicEventRendering"];
            }
            set
            {
                this.ViewState["DynamicEventRendering"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(50), Description("How many additional horizontal pixels of events should be pre-rendered on each side of the current viewport.")]
        public int DynamicEventRenderingMargin
        {
            get
            {
                if (this.ViewState["DynamicEventRenderingMargin"] == null)
                {
                    return 50;
                }
                return (int)this.ViewState["DynamicEventRenderingMargin"];
            }
            set
            {
                this.ViewState["DynamicEventRenderingMargin"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Determines the action to be executed after a user moves an event. If set to Disabled moving is not enabled  on the client side.")]
        public UserActionHandling EventMoveHandling
        {
            get
            {
                if (this.ViewState["EventMoveHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["EventMoveHandling"];
            }
            set
            {
                this.ViewState["EventMoveHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.value() + ' was moved.');"), Description("Javascript function that is executed when a users moves an event.")]
        public string EventMoveJavaScript
        {
            get
            {
                if (this.ViewState["EventMoveJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.value() + ' was moved.');";
                }
                return (string)this.ViewState["EventMoveJavaScript"];
            }
            set
            {
                this.ViewState["EventMoveJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Specifies delete action handling.")]
        public UserActionHandling EventDeleteHandling
        {
            get
            {
                if (this.ViewState["EventDeleteHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["EventDeleteHandling"];
            }
            set
            {
                this.ViewState["EventDeleteHandling"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the users clicks the delete icon.")]
        public string EventDeleteJavaScript
        {
            get
            {
                if (this.ViewState["EventDeleteJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["EventDeleteJavaScript"];
            }
            set
            {
                this.ViewState["EventDeleteJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(EventClickHandlingEnum.Disabled), Description("Specifies action that should be performed after a user clicks the event.")]
        public EventClickHandlingEnum EventClickHandling
        {
            get
            {
                if (this.ViewState["EventClickHandling"] == null)
                {
                    return EventClickHandlingEnum.Disabled;
                }
                return (EventClickHandlingEnum)this.ViewState["EventClickHandling"];
            }
            set
            {
                this.ViewState["EventClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(HMS.Web.App.App.Ui.Enums.Scheduler.EventHoverHandlingEnum.Bubble), Description("What to do when mouse hovers over an event.")]
        public HMS.Web.App.App.Ui.Enums.Scheduler.EventHoverHandlingEnum EventHoverHandling
        {
            get
            {
                if (this.ViewState["EventHoverHandling"] == null)
                {
                    return HMS.Web.App.App.Ui.Enums.Scheduler.EventHoverHandlingEnum.Bubble;
                }
                return (HMS.Web.App.App.Ui.Enums.Scheduler.EventHoverHandlingEnum)this.ViewState["EventHoverHandling"];
            }
            set
            {
                this.ViewState["EventHoverHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(EventClickHandlingEnum.Disabled), Description("Specifies action that should be performed after a user double-clicks the event.")]
        public EventClickHandlingEnum EventDoubleClickHandling
        {
            get
            {
                if (this.ViewState["EventDoubleClickHandling"] == null)
                {
                    return EventClickHandlingEnum.Disabled;
                }
                return (EventClickHandlingEnum)this.ViewState["EventDoubleClickHandling"];
            }
            set
            {
                this.ViewState["EventDoubleClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.value() + ' clicked.')"), Description("Javascript code that is executed when the user clicks an event.")]
        public string EventClickJavaScript
        {
            get
            {
                if (this.ViewState["EventClickJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.value() + ' clicked.')";
                }
                return (string)this.ViewState["EventClickJavaScript"];
            }
            set
            {
                this.ViewState["EventClickJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.value() + ' double-clicked.')"), Description("Javascript code that is executed when the user double-clicks an event.")]
        public string EventDoubleClickJavaScript
        {
            get
            {
                if (this.ViewState["EventDoubleClickJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.value() + ' double-clicked.')";
                }
                return (string)this.ViewState["EventDoubleClickJavaScript"];
            }
            set
            {
                this.ViewState["EventDoubleClickJavaScript"] = value;
            }
        }

        [Browsable(false), Category("Events"), DefaultValue(ResourceHeaderClickHandlingType.UseRowClickHandling), Description("Specifies action that should be performed after a user clicks a resource header. Obsolete: Use RowClickHandling instead."), Obsolete("Use RowClickHandling instead.")]
        public ResourceHeaderClickHandlingType ResourceHeaderClickHandling
        {
            get
            {
                if (this.ViewState["ResourceHeaderClickHandling"] == null)
                {
                    return ResourceHeaderClickHandlingType.UseRowClickHandling;
                }
                return (ResourceHeaderClickHandlingType)this.ViewState["ResourceHeaderClickHandling"];
            }
            set
            {
                this.ViewState["ResourceHeaderClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowClickHandlingType.Disabled), Description("Specifies action that should be performed after a user clicks a row header.")]
        public RowClickHandlingType RowClickHandling
        {
            get
            {
                if (this.ViewState["RowClickHandling"] == null)
                {
                    return RowClickHandlingType.Disabled;
                }
                return (RowClickHandlingType)this.ViewState["RowClickHandling"];
            }
            set
            {
                this.ViewState["RowClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowDoubleClickHandlingType.Disabled), Description("Specifies action that should be performed after the user double-clicks a row header.")]
        public RowDoubleClickHandlingType RowDoubleClickHandling
        {
            get
            {
                if (this.ViewState["RowDoubleClickHandling"] == null)
                {
                    return RowDoubleClickHandlingType.Disabled;
                }
                return (RowDoubleClickHandlingType)this.ViewState["RowDoubleClickHandling"];
            }
            set
            {
                this.ViewState["RowDoubleClickHandling"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the user double-clicks a row header.")]
        public string RowDoubleClickJavaScript
        {
            get
            {
                if (this.ViewState["RowDoubleClickJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowDoubleClickJavaScript"];
            }
            set
            {
                this.ViewState["RowDoubleClickJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowEditHandlingType.Disabled), Description("Specifies action that should be performed after the user finishes inline editing of a row name.")]
        public RowEditHandlingType RowEditHandling
        {
            get
            {
                if (this.ViewState["RowEditHandling"] == null)
                {
                    return RowEditHandlingType.Disabled;
                }
                return (RowEditHandlingType)this.ViewState["RowEditHandling"];
            }
            set
            {
                this.ViewState["RowEditHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowMoveHandlingType.Disabled), Description("Specifies action that should be performed after the user finishes row drag and drop moving.")]
        public RowMoveHandlingType RowMoveHandling
        {
            get
            {
                if (this.ViewState["RowMoveHandling"] == null)
                {
                    return RowMoveHandlingType.Disabled;
                }
                return (RowMoveHandlingType)this.ViewState["RowMoveHandling"];
            }
            set
            {
                this.ViewState["RowMoveHandling"] = value;
            }
        }

        [Browsable(false), Category("Events"), DefaultValue("alert('Resource with id ' + resource.value + ' clicked.')"), Description("Javascript code that is executed when the user clicks a resource header."), Obsolete("Use RowClickJavaScript instead.")]
        public string ResourceHeaderClickJavaScript
        {
            get
            {
                return this.RowClickJavaScript;
            }
            set
            {
                this.RowClickJavaScript = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the user clicks a row header.")]
        public string RowClickJavaScript
        {
            get
            {
                if (this.ViewState["RowClickJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowClickJavaScript"];
            }
            set
            {
                this.ViewState["RowClickJavaScript"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the user selects a row.")]
        public string RowSelectJavaScript
        {
            get
            {
                if (this.ViewState["RowSelectJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowSelectJavaScript"];
            }
            set
            {
                this.ViewState["RowSelectJavaScript"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the user finishes inline row editing.")]
        public string RowEditJavaScript
        {
            get
            {
                if (this.ViewState["RowEditJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowEditJavaScript"];
            }
            set
            {
                this.ViewState["RowEditJavaScript"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed when the user finishes drag and drop row moving.")]
        public string RowMoveJavaScript
        {
            get
            {
                if (this.ViewState["RowMoveJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowMoveJavaScript"];
            }
            set
            {
                this.ViewState["RowMoveJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Determines the action to be executed after a user edits an event. If set to Disabled editing is not enabled on the client side.")]
        public UserActionHandling EventEditHandling
        {
            get
            {
                if (this.ViewState["EventEditHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["EventEditHandling"];
            }
            set
            {
                this.ViewState["EventEditHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('The text of event ' + e.value() + ' was changed to ' + newText + '.');"), Description("Javascript function that is executed after a users edits an event.")]
        public string EventEditJavaScript
        {
            get
            {
                if (this.ViewState["EventEditJavaScript"] == null)
                {
                    return "alert('The text of event ' + e.value() + ' was changed to ' + newText + '.');";
                }
                return (string)this.ViewState["EventEditJavaScript"];
            }
            set
            {
                this.ViewState["EventEditJavaScript"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the horizontal bar on the top of an event."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color DurationBarColor
        {
            get
            {
                if (this.ViewState["DurationBarColor"] == null)
                {
                    return ColorTranslator.FromHtml("blue");
                }
                return (Color)this.ViewState["DurationBarColor"];
            }
            set
            {
                this.ViewState["DurationBarColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the horizontal bar on the top of an event."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color DurationBarBackColor
        {
            get
            {
                if (this.ViewState["DurationBarBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#fff");
                }
                return (Color)this.ViewState["DurationBarBackColor"];
            }
            set
            {
                this.ViewState["DurationBarBackColor"] = value;
            }
        }

        [Category("Behavior"), Description("Size of the drag&drop-sensitive margin of event (resize cursor) in pixels.")]
        public int EventResizeMargin
        {
            get
            {
                if (this.ViewState["EventResizeMargin"] == null)
                {
                    return 5;
                }
                return (int)this.ViewState["EventResizeMargin"];
            }
            set
            {
                this.ViewState["EventResizeMargin"] = value;
            }
        }

        [Category("Behavior"), Description("Size of the drag&drop sensitive margin of event (move cursor) in pixels.")]
        public int EventMoveMargin
        {
            get
            {
                if (this.ViewState["EventMoveMargin"] == null)
                {
                    return 5;
                }
                return (int)this.ViewState["EventMoveMargin"];
            }
            set
            {
                this.ViewState["EventMoveMargin"] = value;
            }
        }

        [Category("Behavior"), Description("ID of the HMSMenu that will be used for context menu for calendar events. If no ID is specified, the context menu will be disabled."), TypeConverter(typeof(MenuControlConverter)), IDReferenceProperty(typeof(HMSMenu))]
        public string ContextMenuID
        {
            get
            {
                if (this.ViewState["ContextMenuID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["ContextMenuID"];
            }
            set
            {
                this.ViewState["ContextMenuID"] = value;
            }
        }

        [Category("Behavior"), Description("ID of the HMSMenu control that will be used for context menu for time range selection. If no ID is specified, the context menu will be disabled."), TypeConverter(typeof(MenuControlConverter)), IDReferenceProperty(typeof(HMSMenu))]
        public string ContextMenuSelectionID
        {
            get
            {
                if (this.ViewState["ContextMenuSelectionID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["ContextMenuSelectionID"];
            }
            set
            {
                this.ViewState["ContextMenuSelectionID"] = value;
            }
        }

        [Category("Behavior"), Description("ID of the HMSMenu control that will be used for context menu for resource headers. If no ID is specified, the context menu will be disabled."), TypeConverter(typeof(MenuControlConverter)), IDReferenceProperty(typeof(HMSMenu))]
        public string ContextMenuResourceID
        {
            get
            {
                if (this.ViewState["ContextMenuResourceID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["ContextMenuResourceID"];
            }
            set
            {
                this.ViewState["ContextMenuResourceID"] = value;
            }
        }

        [Category("ToolTips"), Description("ID of the HMSBubble control that will be used for showing event details on hover."), TypeConverter(typeof(BubbleControlConverter)), IDReferenceProperty(typeof(HMSBubble))]
        public string BubbleID
        {
            get
            {
                if (this.ViewState["BubbleID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["BubbleID"];
            }
            set
            {
                this.ViewState["BubbleID"] = value;
            }
        }

        [Category("ToolTips"), Description("ID of the advanced tooltip control (HMSBubble) that will be used for cells."), TypeConverter(typeof(BubbleControlConverter)), IDReferenceProperty(typeof(HMSBubble))]
        public string CellBubbleID
        {
            get
            {
                if (this.ViewState["CellBubbleID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["CellBubbleID"];
            }
            set
            {
                this.ViewState["CellBubbleID"] = value;
            }
        }

        [Category("ToolTips"), Description("ID of the advanced tooltip control (HMSBubble) that will be used for column headers."), TypeConverter(typeof(BubbleControlConverter)), IDReferenceProperty(typeof(HMSBubble))]
        public string ResourceBubbleID
        {
            get
            {
                if (this.ViewState["ResourceBubbleID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["ResourceBubbleID"];
            }
            set
            {
                this.ViewState["ResourceBubbleID"] = value;
            }
        }

        [Category("Events"), DefaultValue(EventRightClickHandlingEnum.ContextMenu), Description("Specifies action that should be performed after a user clicks the event.")]
        public EventRightClickHandlingEnum EventRightClickHandling
        {
            get
            {
                if (this.ViewState["EventRightClickHandling"] == null)
                {
                    return EventRightClickHandlingEnum.ContextMenu;
                }
                return (EventRightClickHandlingEnum)this.ViewState["EventRightClickHandling"];
            }
            set
            {
                this.ViewState["EventRightClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.value() + ' clicked.')"), Description("Javascript code that is executed when the user clicks an event.")]
        public string EventRightClickJavaScript
        {
            get
            {
                if (this.ViewState["EventRightClickJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.value() + ' clicked.')";
                }
                return (string)this.ViewState["EventRightClickJavaScript"];
            }
            set
            {
                this.ViewState["EventRightClickJavaScript"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(TimeFormat.Clock12Hours), Description("The time-format that will be used for the hour numbers.")]
        public TimeFormat TimeFormat
        {
            get
            {
                if (this.ViewState["TimeFormat"] == null)
                {
                    return TimeFormat.Clock12Hours;
                }
                return (TimeFormat)this.ViewState["TimeFormat"];
            }
            set
            {
                this.ViewState["TimeFormat"] = value;
            }
        }

        [Category("Events"), DefaultValue(TimeRangeSelectedHandling.Disabled), Description("Whether clicking a free-time slot should do a postback or run a javascript action. By default, it calls the javascript code specified in TimeRangeSelectedJavaScript property.")]
        public TimeRangeSelectedHandling TimeRangeSelectedHandling
        {
            get
            {
                if (this.ViewState["TimeRangeSelectedHandling"] == null)
                {
                    return TimeRangeSelectedHandling.Disabled;
                }
                return (TimeRangeSelectedHandling)this.ViewState["TimeRangeSelectedHandling"];
            }
            set
            {
                this.ViewState["TimeRangeSelectedHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Determines time range double click handling.")]
        public UserActionHandling TimeRangeDoubleClickHandling
        {
            get
            {
                if (this.ViewState["TimeRangeDoubleClickHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["TimeRangeDoubleClickHandling"];
            }
            set
            {
                this.ViewState["TimeRangeDoubleClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert(start.toString() + '\\n' + end.toString() + '\\n' + column);"), Description("Javascript code that is executed when the users selectes a time range.")]
        public string TimeRangeSelectedJavaScript
        {
            get
            {
                if (this.ViewState["TimeRangeSelectedJavaScript"] == null)
                {
                    return "alert(start.toString() + '\\n' + end.toString() + '\\n' + column);";
                }
                return (string)this.ViewState["TimeRangeSelectedJavaScript"];
            }
            set
            {
                this.ViewState["TimeRangeSelectedJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert(start.toString() + '\\n' + end.toString() + '\\n' + resource);"), Description("Javascript code that is executed when the double clicks a selected time range.")]
        public string TimeRangeDoubleClickJavaScript
        {
            get
            {
                if (this.ViewState["TimeRangeDoubleClickJavaScript"] == null)
                {
                    return "alert(start.toString() + '\\n' + end.toString() + '\\n' + resource);";
                }
                return (string)this.ViewState["TimeRangeDoubleClickJavaScript"];
            }
            set
            {
                this.ViewState["TimeRangeDoubleClickJavaScript"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#316AC5"), Description("Color of the time range selection."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color CellSelectColor
        {
            get
            {
                if (this.ViewState["CellSelectColor"] == null)
                {
                    return ColorTranslator.FromHtml("#316AC5");
                }
                return (Color)this.ViewState["CellSelectColor"];
            }
            set
            {
                this.ViewState["CellSelectColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#EAD098"), Description("Color of the time cell border."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color CellBorderColor
        {
            get
            {
                if (this.ViewState["CellBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#EAD098");
                }
                return (Color)this.ViewState["CellBorderColor"];
            }
            set
            {
                this.ViewState["CellBorderColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#000000"), Description("Color of the time break (indicates a hidden column)."), TypeConverter(typeof(Web.AppColorConverter))]
        public Color TimeBreakColor
        {
            get
            {
                if (this.ViewState["TimeBreakColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["TimeBreakColor"];
            }
            set
            {
                this.ViewState["TimeBreakColor"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('An exception was thrown in the server-side event handler:\\n\\n' + result.substring(result.indexOf('$$$')+3));"), Description("Javascript code that is executed on error during AJAX callback.")]
        public string CallBackErrorJavaScript
        {
            get
            {
                if (this.ViewState["CallBackErrorJavaScript"] == null)
                {
                    return "alert('An exception was thrown in the server-side event handler:\\n\\n' + result.substring(result.indexOf('$$$')+3));";
                }
                return (string)this.ViewState["CallBackErrorJavaScript"];
            }
            set
            {
                this.ViewState["CallBackErrorJavaScript"] = value;
            }
        }

        [Category("Layout"), Description("Defines column group size (first row of the column headers on top of the Scheduler)")]
        public GroupByEnum CellGroupBy
        {
            get
            {
                if (this.ViewState["CellGroupBy"] == null)
                {
                    return GroupByEnum.Day;
                }
                if ((GroupByEnum)this.ViewState["CellGroupBy"] == GroupByEnum.Default)
                {
                    return GroupByEnum.Day;
                }
                return (GroupByEnum)this.ViewState["CellGroupBy"];
            }
            set
            {
                this.ViewState["CellGroupBy"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the event tooltip is active.")]
        public bool ShowToolTip
        {
            get
            {
                return this.ViewState["ShowToolTip"] == null || (bool)this.ViewState["ShowToolTip"];
            }
            set
            {
                this.ViewState["ShowToolTip"] = value;
            }
        }

        [Description("Width of the control (pixels and percentage values supported).")]
        public override Unit Width
        {
            get
            {
                if (this.ViewState["Width"] == null)
                {
                    return Unit.Empty;
                }
                return (Unit)this.ViewState["Width"];
            }
            set
            {
                if (value.Type != UnitType.Pixel && value.Type != UnitType.Percentage && value != Unit.Empty)
                {
                    throw new NotSupportedException("Only pixel and percentage units are supported.");
                }
                this.ViewState["Width"] = value;
            }
        }

        public override Unit Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                if (value.Type != UnitType.Pixel)
                {
                    throw new NotSupportedException("Only pixel units are allowed.");
                }
                base.Height = value;
            }
        }

        [DefaultValue(SchedulerHeightSpec.Auto), Description("Specifies whether the height should be adjusted automatically to show all resources (Auto) or fixed (Fixed). Fixed height is specified using Height property.")]
        public SchedulerHeightSpec HeightSpec
        {
            get
            {
                if (this.ViewState["HeightSpec"] == null)
                {
                    return SchedulerHeightSpec.Auto;
                }
                return (SchedulerHeightSpec)this.ViewState["HeightSpec"];
            }
            set
            {
                this.ViewState["HeightSpec"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(80), Description("Width of the row header (resource names) in pixels.")]
        public int RowHeaderWidth
        {
            get
            {
                if (this.ViewState["RowHeaderWidth"] == null)
                {
                    return 80;
                }
                return (int)this.ViewState["RowHeaderWidth"];
            }
            set
            {
                this.ViewState["RowHeaderWidth"] = value;
            }
        }

        [Category("Behavior"), Description("Specify column widths in pixels (separated by commas) to enable multiple columns in row headers.")]
        public string RowHeaderColumnWidths
        {
            get
            {
                return (string)this.ViewState["RowHeaderColumnWidths"];
            }
            set
            {
                if (!WidthCollectionParser.IsValid(value))
                {
                    throw new ArgumentException("Invalid RowHeaderColumnWidths format. Valid example: \"60, 20, 20\".");
                }
                this.RowHeaderColumns = WidthCollectionParser.Parse(value);
                this.ViewState["RowHeaderColumnWidths"] = value;
            }
        }

        [Category("Behavior"), Description("Horizontal scroll position in pixels.")]
        public int ScrollX
        {
            get
            {
                if (this.ScrollDateTime != DateTime.MinValue)
                {
                    return this.TimeHeader.GetPixels(this.ScrollDateTime).Left;
                }
                if (this.ViewState["ScrollX"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["ScrollX"];
            }
            set
            {
                this.ScrollDateTime = DateTime.MinValue;
                this.ViewState["ScrollX"] = value;
            }
        }

        [Category("Behavior"), Description("Vertical scroll position in pixels.")]
        public int ScrollY
        {
            get
            {
                if (this.ViewState["ScrollY"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["ScrollY"];
            }
            set
            {
                this.ViewState["ScrollY"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(UseBoxesEnum.Always)]
        public UseBoxesEnum UseEventBoxes
        {
            get
            {
                if (this.ViewState["UseEventBoxes"] == null)
                {
                    return UseBoxesEnum.Always;
                }
                return (UseBoxesEnum)this.ViewState["UseEventBoxes"];
            }
            set
            {
                this.ViewState["UseEventBoxes"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(ViewTypeEnum.Resources), Description("Sets the view type. In the days view it shows one or more days in the columns. In the resources view it shows multiple resources in the columns.")]
        public ViewTypeEnum ViewType
        {
            get
            {
                if (this.ViewState["ViewType"] == null)
                {
                    return ViewTypeEnum.Resources;
                }
                return (ViewTypeEnum)this.ViewState["ViewType"];
            }
            set
            {
                this.ViewState["ViewType"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether the duration bar on the top of and event should be visible.")]
        public bool DurationBarVisible
        {
            get
            {
                return this.ViewState["DurationBarVisible"] == null || (bool)this.ViewState["DurationBarVisible"];
            }
            set
            {
                this.ViewState["DurationBarVisible"] = value;
            }
        }

        [Category("Events"), Description("The JavaScript code that will be executed after rendering an event on the client side. Two variables are available: e (Event object), div (event DIV element).")]
        public string AfterEventRenderJavaScript
        {
            get
            {
                if (this.ViewState["AfterEventRenderJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["AfterEventRenderJavaScript"];
            }
            set
            {
                this.ViewState["AfterEventRenderJavaScript"] = value;
            }
        }

        [Category("Events"), Description("The JavaScript code that will be executed after client-side rendering is finished (initial, after PostBack, or CallBack).")]
        public string AfterRenderJavaScript
        {
            get
            {
                if (this.ViewState["AfterRenderJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["AfterRenderJavaScript"];
            }
            set
            {
                this.ViewState["AfterRenderJavaScript"] = value;
            }
        }

        [Category("Labels"), DefaultValue(true), Description("Whether the loading label ('Loading...') should be visible during control initialization.")]
        public bool LoadingLabelVisible
        {
            get
            {
                return this.ViewState["LoadingLabelVisible"] == null || (bool)this.ViewState["LoadingLabelVisible"];
            }
            set
            {
                this.ViewState["LoadingLabelVisible"] = value;
            }
        }

        [Category("Labels"), DefaultValue("Loading..."), Description("The text of the 'Loading...' label."), Localizable(true)]
        public string LoadingLabelText
        {
            get
            {
                if (this.ViewState["LoadingLabelText"] == null)
                {
                    return "Loading...";
                }
                return (string)this.ViewState["LoadingLabelText"];
            }
            set
            {
                this.ViewState["LoadingLabelText"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("Tahoma"), Description("Font family of the Loading label text, e.g. \"Tahoma\".")]
        public string LoadingLabelFontFamily
        {
            get
            {
                if (this.ViewState["LoadingLabelFontFamily"] == null)
                {
                    return "Tahoma";
                }
                return (string)this.ViewState["LoadingLabelFontFamily"];
            }
            set
            {
                this.ViewState["LoadingLabelFontFamily"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the Loading label text.")]
        public Color LoadingLabelFontColor
        {
            get
            {
                if (this.ViewState["LoadingLabelFontColor"] == null)
                {
                    return ColorTranslator.FromHtml("#ffffff");
                }
                return (Color)this.ViewState["LoadingLabelFontColor"];
            }
            set
            {
                this.ViewState["LoadingLabelFontColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the Loading label background.")]
        public Color LoadingLabelBackColor
        {
            get
            {
                if (this.ViewState["LoadingLabelBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("red");
                }
                return (Color)this.ViewState["LoadingLabelBackColor"];
            }
            set
            {
                this.ViewState["LoadingLabelBackColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("10pt"), Description("Font size of Loading label text, e.g. '10pt'.")]
        public string LoadingLabelFontSize
        {
            get
            {
                if (this.ViewState["LoadingLabelFontSize"] == null)
                {
                    return "10pt";
                }
                return (string)this.ViewState["LoadingLabelFontSize"];
            }
            set
            {
                this.ViewState["LoadingLabelFontSize"] = value;
            }
        }

        [Category("Labels"), DefaultValue(true), Description("Whether the scroll labels (active when there are hidden events) should be visible.")]
        public bool ScrollLabelsVisible
        {
            get
            {
                return this.ViewState["ScrollLabelsVisible"] == null || (bool)this.ViewState["ScrollLabelsVisible"];
            }
            set
            {
                this.ViewState["ScrollLabelsVisible"] = value;
            }
        }

        [Browsable(false), Obsolete("Disabled. Use CssClassPrefix instead.")]
        public override string CssClass
        {
            get
            {
                return (string)this.ViewState["CssClass"];
            }
            set
            {
                this.ViewState["CssClass"] = value;
            }
        }

        [Category("Appearance"), Description("Specifies the prefix of the CSS classes that contain style definitions for the elements of this control. Obsolete: Use Theme property instead."), Obsolete("Use Theme property instead.")]
        public string CssClassPrefix
        {
            get
            {
                return this.Theme;
            }
            set
            {
                this.Theme = value;
            }
        }

        [Category("Appearance"), Description("Specifies the CSS theme.")]
        public string Theme
        {
            get
            {
                return (string)this.ViewState["Theme"];
            }
            set
            {
                this.ViewState["Theme"] = value;
            }
        }

        internal int CellCount
        {
            get
            {
                return this.TimeHeader.Timeline.Count;
            }
        }

        internal int DaysHorizontally
        {
            get
            {
                switch (this.ViewType)
                {
                    case ViewTypeEnum.Days:
                        return 1;
                    case ViewTypeEnum.Resources:
                        return this.Days;
                    case ViewTypeEnum.Gantt:
                        return this.Days;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Section ViewPort
        {
            get
            {
                this.TimeHeader.PrepareTimeline();
                if (this._viewPort != null)
                {
                    return this._viewPort;
                }
                throw new InvalidOperationException("ViewPort property is only available during CallBacks.");
            }
        }

        public bool IsExport
        {
            get
            {
                return this.isExport;
            }
        }

        internal string CallBackMessage
        {
            get
            {
                return this._callbackMessage;
            }
        }

        [DefaultValue(false), Description("Whether the resources (.js and image files) should be loaded statically (from a directory specified in ResourcesPath.")]
        public bool ResourcesStatic
        {
            get
            {
                return this.ViewState["ResourcesStatic"] != null && (bool)this.ViewState["ResourcesStatic"];
            }
            set
            {
                this.ViewState["ResourcesStatic"] = value;
            }
        }

        [Description("Directory part of the URL where static resources are located (ending with a slash). E.g. '/resources/'.")]
        public string ResourcesPath
        {
            get
            {
                return (string)this.ViewState["ResourcesPath"];
            }
            set
            {
                this.ViewState["ResourcesPath"] = value;
            }
        }

        [DefaultValue(LayoutEnum.Auto), Description("Determines which layout should be used (old table-based, new div-based).")]
        public LayoutEnum Layout
        {
            get
            {
                if (this.ViewState["Layout"] == null)
                {
                    return LayoutEnum.Auto;
                }
                return (LayoutEnum)this.ViewState["Layout"];
            }
            set
            {
                this.ViewState["Layout"] = value;
            }
        }

        [Browsable(false)]
        public JsonData ClientState
        {
            get
            {
                if (this._clientState == null || this._clientState.IsNull)
                {
                    this._clientState = new JsonData();
                    this._clientState.SetJsonType(JsonType.Object);
                }
                return this._clientState;
            }
        }

        [Category("Resources"), Description("URL of the image that will appear before a collapsed resource (typically a 'plus' sign).")]
        public string TreeImageExpand
        {
            get
            {
                return (string)this.ViewState["TreeImageExpand"];
            }
            set
            {
                this.ViewState["TreeImageExpand"] = value;
            }
        }

        [Category("Resources"), Description("URL of the image that will appear before an expanded resource (typically a 'minus' sign).")]
        public string TreeImageCollapse
        {
            get
            {
                return (string)this.ViewState["TreeImageCollapse"];
            }
            set
            {
                this.ViewState["TreeImageCollapse"] = value;
            }
        }

        [Category("Resources"), Description("URL of the image that will appear before a resource without children.")]
        public string TreeImageNoChildren
        {
            get
            {
                return (string)this.ViewState["TreeImageNoChildren"];
            }
            set
            {
                this.ViewState["TreeImageNoChildren"] = value;
            }
        }

        [Category("Resources"), DefaultValue(false), Description("Whether to show resources children in a tree.")]
        public bool TreeEnabled
        {
            get
            {
                return this.ViewState["TreeEnabled"] != null && (bool)this.ViewState["TreeEnabled"];
            }
            set
            {
                this.ViewState["TreeEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(CrosshairType.Header), Description("Crosshair type.")]
        public CrosshairType Crosshair
        {
            get
            {
                if (this.ViewState["Crosshair"] == null)
                {
                    return CrosshairType.Header;
                }
                return (CrosshairType)this.ViewState["Crosshair"];
            }
            set
            {
                this.ViewState["Crosshair"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the crosshair.")]
        public Color CrosshairColor
        {
            get
            {
                if (this.ViewState["CrosshairColor"] == null)
                {
                    return ColorTranslator.FromHtml("gray");
                }
                return (Color)this.ViewState["CrosshairColor"];
            }
            set
            {
                this.ViewState["CrosshairColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(20), Description("Crosshair opacity in percent.")]
        public int CrosshairOpacity
        {
            get
            {
                if (this.ViewState["CrosshairOpacity"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["CrosshairOpacity"];
            }
            set
            {
                this.ViewState["CrosshairOpacity"] = value;
            }
        }

        [Category("Resources"), DefaultValue(20), Description("Left indent of the children in pixels.")]
        public int TreeIndent
        {
            get
            {
                if (this.ViewState["TreeIndent"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["TreeIndent"];
            }
            set
            {
                this.ViewState["TreeIndent"] = value;
            }
        }

        [Category("Resources"), DefaultValue(2), Description("Top margin of the collapse/expand image in pixels.")]
        public int TreeImageMarginTop
        {
            get
            {
                if (this.ViewState["TreeImageMarginTop"] == null)
                {
                    return 2;
                }
                return (int)this.ViewState["TreeImageMarginTop"];
            }
            set
            {
                this.ViewState["TreeImageMarginTop"] = value;
            }
        }

        [Category("Resources"), DefaultValue(2), Description("Left margin of the collapse/expand image in pixels.")]
        public int TreeImageMarginLeft
        {
            get
            {
                if (this.ViewState["TreeImageMarginLeft"] == null)
                {
                    return 2;
                }
                return (int)this.ViewState["TreeImageMarginLeft"];
            }
            set
            {
                this.ViewState["TreeImageMarginLeft"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Snap events to grid when moving or resizing.")]
        public bool SnapToGrid
        {
            get
            {
                return this.ViewState["SnapToGrid"] == null || (bool)this.ViewState["SnapToGrid"];
            }
            set
            {
                this.ViewState["SnapToGrid"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(3), Description("Height of the duration bar in pixels.")]
        public int DurationBarHeight
        {
            get
            {
                if (this.ViewState["DurationBarHeight"] == null)
                {
                    return 3;
                }
                return (int)this.ViewState["DurationBarHeight"];
            }
            set
            {
                this.ViewState["DurationBarHeight"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("Row margin below the events. In pixels.")]
        public int RowMarginBottom
        {
            get
            {
                if (this.ViewState["RowMarginBottom"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["RowMarginBottom"];
            }
            set
            {
                this.ViewState["RowMarginBottom"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("Minimum row height in pixels.")]
        public int RowMinHeight
        {
            get
            {
                if (this.ViewState["RowMinHeight"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["RowMinHeight"];
            }
            set
            {
                this.ViewState["RowMinHeight"] = value;
            }
        }

        [Category("Behavior"), Description("Determines the first day of week (Sunday/Monday/Auto).")]
        public WeekStartsEnum WeekStarts
        {
            get
            {
                if (this.ViewState["WeekStarts"] == null)
                {
                    return WeekStartsEnum.Auto;
                }
                return (WeekStartsEnum)this.ViewState["WeekStarts"];
            }
            set
            {
                this.ViewState["WeekStarts"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(ShadowType.Fill), Description("Type of the event shadow that is used for moving and resizing.")]
        public ShadowType Shadow
        {
            get
            {
                if (this.ViewState["Shadow"] == null)
                {
                    return ShadowType.Fill;
                }
                return (ShadowType)this.ViewState["Shadow"];
            }
            set
            {
                this.ViewState["Shadow"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Enables the AutoRefresh feature.")]
        public bool AutoRefreshEnabled
        {
            get
            {
                return this.ViewState["AutoRefreshEnabled"] != null && (bool)this.ViewState["AutoRefreshEnabled"];
            }
            set
            {
                this.ViewState["AutoRefreshEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(60), Description("Auto-refresh interval in seconds. Minimum 10 seconds.")]
        public int AutoRefreshInterval
        {
            get
            {
                if (this.ViewState["AutoRefreshInterval"] == null)
                {
                    return 60;
                }
                return (int)this.ViewState["AutoRefreshInterval"];
            }
            set
            {
                if (value < 10)
                {
                    throw new ArgumentException("The minimum AutoRefreshInterval value is 10.");
                }
                this.ViewState["AutoRefreshInterval"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(20), Description("Maximum auto-refresh count (after inactivity).")]
        public int AutoRefreshMaxCount
        {
            get
            {
                if (this.ViewState["AutoRefreshMaxCount"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["AutoRefreshMaxCount"];
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("The minimum AutoRefreshMaxCount value is 1.");
                }
                this.ViewState["AutoRefreshMaxCount"] = value;
            }
        }

        [Category("Behavior"), DefaultValue("refresh"), Description("The command that will be passed to Command event during automatic refresh.")]
        public string AutoRefreshCommand
        {
            get
            {
                if (this.ViewState["AutoRefreshCommand"] == null)
                {
                    return "refresh";
                }
                return (string)this.ViewState["AutoRefreshCommand"];
            }
            set
            {
                this.ViewState["AutoRefreshCommand"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Enables dynamic event loading (events are loaded from the server side using Scroll event).")]
        public bool DynamicLoading
        {
            get
            {
                return this.ViewState["DynamicLoading"] != null && (bool)this.ViewState["DynamicLoading"];
            }
            set
            {
                this.ViewState["DynamicLoading"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether the events should be stored in the ViewState.")]
        public bool StoreEventsInViewState
        {
            get
            {
                return this.ViewState["StoreEventsInViewState"] != null && (bool)this.ViewState["StoreEventsInViewState"];
            }
            set
            {
                this.ViewState["StoreEventsInViewState"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the Resources tree should be synchronized with the server during CallBacks.")]
        public bool SyncResourceTree
        {
            get
            {
                return this.ViewState["SyncResourceTree"] == null || (bool)this.ViewState["SyncResourceTree"];
            }
            set
            {
                this.ViewState["SyncResourceTree"] = value;
            }
        }

        [Category("Data"), Description("Sorting expression for ordering concurrent events , e.g. 'priority asc, name desc'.")]
        public string EventSortExpression
        {
            get
            {
                return (string)this.ViewState["EventSortExpression"];
            }
            set
            {
                if (!SortExpression.IsValid(value))
                {
                    throw new ArgumentException("Invalid EventSortExpression format. Valid example: \"start asc, end desc\"");
                }
                this.SortFields = SortExpression.Parse(value);
                this.ViewState["EventSortExpression"] = value;
            }
        }

        [Category("Appearance"), Description("URL of the image that indicates a recurring event.")]
        public string RecurrentEventImage
        {
            get
            {
                return (string)this.ViewState["RecurrentEventImage"];
            }
            set
            {
                this.ViewState["RecurrentEventImage"] = value;
            }
        }

        [Category("Appearance"), Description("URL of the image that indicates a recurring event (exception from the series).")]
        public string RecurrentEventExceptionImage
        {
            get
            {
                return (string)this.ViewState["RecurrentEventExceptionImage"];
            }
            set
            {
                this.ViewState["RecurrentEventExceptionImage"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(AutoScrollType.Drag), Description("Enables or disables autoscroll when hovering the scheduler edges.")]
        public AutoScrollType AutoScroll
        {
            get
            {
                if (this.ViewState["AutoScroll"] == null)
                {
                    return AutoScrollType.Drag;
                }
                return (AutoScrollType)this.ViewState["AutoScroll"];
            }
            set
            {
                this.ViewState["AutoScroll"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(HMS.Web.App.App.Ui.Enums.Scheduler.CornerShape.Regular), Description("Corner look: regular or rounded. Rounded corners are not supported in IE up to IE 8.")]
        public HMS.Web.App.App.Ui.Enums.Scheduler.CornerShape EventCorners
        {
            get
            {
                if (this.ViewState["EventCorners"] == null)
                {
                    return HMS.Web.App.App.Ui.Enums.Scheduler.CornerShape.Regular;
                }
                return (HMS.Web.App.App.Ui.Enums.Scheduler.CornerShape)this.ViewState["EventCorners"];
            }
            set
            {
                this.ViewState["EventCorners"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(NotifyCommitType.Immediate)]
        public NotifyCommitType NotifyCommit
        {
            get
            {
                if (this.ViewState["NotifyCommit"] == null)
                {
                    return NotifyCommitType.Immediate;
                }
                return (NotifyCommitType)this.ViewState["NotifyCommit"];
            }
            set
            {
                this.ViewState["NotifyCommit"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false)]
        public bool DragOutAllowed
        {
            get
            {
                return this.ViewState["DragOutAllowed"] != null && (bool)this.ViewState["DragOutAllowed"];
            }
            set
            {
                this.ViewState["DragOutAllowed"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(DragArea.Full), Description("Which event border shoud be used for moving.")]
        public DragArea MoveBy
        {
            get
            {
                if (this.ViewState["MoveBy"] == null)
                {
                    return DragArea.Full;
                }
                return (DragArea)this.ViewState["MoveBy"];
            }
            set
            {
                this.ViewState["MoveBy"] = value;
            }
        }

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Specifies action that should be performed after a user selects the event.")]
        public UserActionHandling EventSelectHandling
        {
            get
            {
                if (this.ViewState["EventSelectHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["EventSelectHandling"];
            }
            set
            {
                this.ViewState["EventSelectHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event selected.')"), Description("Javascript code that is executed when the user selects an event.")]
        public string EventSelectJavaScript
        {
            get
            {
                if (this.ViewState["EventSelectJavaScript"] == null)
                {
                    return "alert('Event selected.')";
                }
                return (string)this.ViewState["EventSelectJavaScript"];
            }
            set
            {
                this.ViewState["EventSelectJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowSelectHandlingType.Disabled), Description("Specifies action that should be performed after a user selects a row.")]
        public RowSelectHandlingType RowSelectHandling
        {
            get
            {
                if (this.ViewState["RowSelectHandling"] == null)
                {
                    return RowSelectHandlingType.Disabled;
                }
                return (RowSelectHandlingType)this.ViewState["RowSelectHandling"];
            }
            set
            {
                this.ViewState["RowSelectHandling"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Allows selecting multiple events (using Ctrl + click).")]
        public bool AllowMultiSelect
        {
            get
            {
                return this.ViewState["AllowMultiSelect"] == null || (bool)this.ViewState["AllowMultiSelect"];
            }
            set
            {
                this.ViewState["AllowMultiSelect"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Enables automatic deleting of hidden event objects for DynamicEventRendering='Progressive'.")]
        public bool DynamicEventRenderingCacheSweeping
        {
            get
            {
                return this.ViewState["DynamicEventRenderingCacheSweeping"] != null && (bool)this.ViewState["DynamicEventRenderingCacheSweeping"];
            }
            set
            {
                this.ViewState["DynamicEventRenderingCacheSweeping"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(200), Description("Maximum number of rendered (but not visible) events that are kept in the cache.")]
        public int DynamicEventRenderingCacheSize
        {
            get
            {
                if (this.ViewState["DynamicEventRenderingCacheSize"] == null)
                {
                    return 200;
                }
                return (int)this.ViewState["DynamicEventRenderingCacheSize"];
            }
            set
            {
                this.ViewState["DynamicEventRenderingCacheSize"] = value;
            }
        }

        [Category("Events"), DefaultValue(ResourceCollapseHandlingType.Disabled), Description("Specifies action that should be performed after a user collapses a resource tree node.")]
        public ResourceCollapseHandlingType ResourceCollapseHandling
        {
            get
            {
                if (this.ViewState["ResourceCollapseHandling"] == null)
                {
                    return ResourceCollapseHandlingType.Disabled;
                }
                return (ResourceCollapseHandlingType)this.ViewState["ResourceCollapseHandling"];
            }
            set
            {
                this.ViewState["ResourceCollapseHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Resource collapsed:' + resource.value)"), Description("Javascript code that is executed after a user collapses a resource tree node.")]
        public string ResourceCollapseJavaScript
        {
            get
            {
                if (this.ViewState["ResourceCollapseJavaScript"] == null)
                {
                    return "alert('Resource collapsed:' + resource.value)";
                }
                return (string)this.ViewState["ResourceCollapseJavaScript"];
            }
            set
            {
                this.ViewState["ResourceCollapseJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue(ResourceExpandHandlingType.Disabled), Description("Specifies action that should be performed after a user expands a resource tree node.")]
        public ResourceExpandHandlingType ResourceExpandHandling
        {
            get
            {
                if (this.ViewState["ResourceExpandHandling"] == null)
                {
                    return ResourceExpandHandlingType.Disabled;
                }
                return (ResourceExpandHandlingType)this.ViewState["ResourceExpandHandling"];
            }
            set
            {
                this.ViewState["ResourceExpandHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Resource expanded:' + resource.value)"), Description("Javascript code that is executed after a user expands a resource tree node.")]
        public string ResourceExpandJavaScript
        {
            get
            {
                if (this.ViewState["ResourceExpandJavaScript"] == null)
                {
                    return "alert('Resource expanded:' + resource.value)";
                }
                return (string)this.ViewState["ResourceExpandJavaScript"];
            }
            set
            {
                this.ViewState["ResourceExpandJavaScript"] = value;
            }
        }

        [Category("Events"), Description("Javascript code that is executed before an auto refresh is requested.")]
        public string AutoRefreshJavaScript
        {
            get
            {
                return (string)this.ViewState["AutoRefreshJavaScript"];
            }
            set
            {
                this.ViewState["AutoRefreshJavaScript"] = value;
            }
        }

        [Category("Behavior"), Description("Collection time header groups. One row matching the cells will be added automatically."), PersistenceMode(PersistenceMode.InnerProperty)]
        public TimeHeaderCollection TimeHeaders
        {
            get
            {
                if (this.ViewState["TimeHeaders"] == null)
                {
                    TimeHeaderCollection value = new TimeHeaderCollection();
                    this.ViewState["TimeHeaders"] = value;
                }
                return (TimeHeaderCollection)this.ViewState["TimeHeaders"];
            }
        }

        [Obsolete("This property is ignored. Added to maintain compatibility with HMS Lite.")]
        public Color HoverColor
        {
            get;
            set;
        }

        [Category("Appearance"), DefaultValue(false), Description("Whether to show the event start and end times.")]
        public bool ShowEventStartEnd
        {
            get
            {
                return this.ViewState["ShowEventStartEnd"] != null && (bool)this.ViewState["ShowEventStartEnd"];
            }
            set
            {
                this.ViewState["ShowEventStartEnd"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether to ignore all style properties. Only CSS styling will be used.")]
        public bool CssOnly
        {
            get
            {
                return this.ViewState["CssOnly"] == null || (bool)this.ViewState["CssOnly"];
            }
            set
            {
                this.ViewState["CssOnly"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(500), Description("The delay in milliseconds before the Scroll event is fired during scrolling (for DynamicLoading=true).")]
        public int ScrollDelayDynamic
        {
            get
            {
                if (this.ViewState["ScrollDelayDynamic"] == null)
                {
                    return 500;
                }
                return (int)this.ViewState["ScrollDelayDynamic"];
            }
            set
            {
                this.ViewState["ScrollDelayDynamic"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(20), Description("The delay in milliseconds before the grid cells are rendered during scrolling.")]
        public int ScrollDelayCells
        {
            get
            {
                if (this.ViewState["ScrollDelayCells"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["ScrollDelayCells"];
            }
            set
            {
                this.ViewState["ScrollDelayCells"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(200), Description("The delay in milliseconds before the events are rendered during scrolling (for DynamicEventRendering='Progressive').")]
        public int ScrollDelayEvents
        {
            get
            {
                if (this.ViewState["ScrollDelayEvents"] == null)
                {
                    return 200;
                }
                return (int)this.ViewState["ScrollDelayEvents"];
            }
            set
            {
                this.ViewState["ScrollDelayEvents"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("The delay in milliseconds before the floating events and headers are updated during scrolling.")]
        public int ScrollDelayFloats
        {
            get
            {
                if (this.ViewState["ScrollDelayFloats"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["ScrollDelayFloats"];
            }
            set
            {
                this.ViewState["ScrollDelayFloats"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether to increase the row header width automatically to fit the content.")]
        public bool RowHeaderWidthAutoFit
        {
            get
            {
                return this.ViewState["RowHeaderWidthAutoFit"] == null || (bool)this.ViewState["RowHeaderWidthAutoFit"];
            }
            set
            {
                this.ViewState["RowHeaderWidthAutoFit"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether to prevent scheduling events on parent tree nodes.")]
        public bool TreePreventParentUsage
        {
            get
            {
                return this.ViewState["TreePreventParentUsage"] != null && (bool)this.ViewState["TreePreventParentUsage"];
            }
            set
            {
                this.ViewState["TreePreventParentUsage"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(DurationBarMode.Duration), Description("What the duration bar will display (Duration = real event start and end, PercentComplete = level of completeness).")]
        public DurationBarMode DurationBarMode
        {
            get
            {
                if (this.ViewState["DurationBarMode"] == null)
                {
                    return DurationBarMode.Duration;
                }
                return (DurationBarMode)this.ViewState["DurationBarMode"];
            }
            set
            {
                this.ViewState["DurationBarMode"] = value;
            }
        }

        [Category("Events"), DefaultValue(TimeHeaderClickHandlingType.Disabled), Description("Specifies action that should be performed after a user clicks a time header.")]
        public TimeHeaderClickHandlingType TimeHeaderClickHandling
        {
            get
            {
                if (this.ViewState["TimeHeaderClickHandling"] == null)
                {
                    return TimeHeaderClickHandlingType.Disabled;
                }
                return (TimeHeaderClickHandlingType)this.ViewState["TimeHeaderClickHandling"];
            }
            set
            {
                this.ViewState["TimeHeaderClickHandling"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Time header clicked: ' + header.start + '.')"), Description("Javascript code that is executed when the user clicks a time header.")]
        public string TimeHeaderClickJavaScript
        {
            get
            {
                if (this.ViewState["TimeHeaderClickJavaScript"] == null)
                {
                    return "alert('Time header clicked: ' + header.start + '.')";
                }
                return (string)this.ViewState["TimeHeaderClickJavaScript"];
            }
            set
            {
                this.ViewState["TimeHeaderClickJavaScript"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(5000), Description("The timeout delay of the message bar (in milliseconds).")]
        public int MessageHideAfter
        {
            get
            {
                if (this.ViewState["MessageHideAfter"] == null)
                {
                    return 5000;
                }
                return (int)this.ViewState["MessageHideAfter"];
            }
            set
            {
                this.ViewState["MessageHideAfter"] = value;
            }
        }

        public TimeHeaderCollection TimeHeadersResolved
        {
            get
            {
                if (this.TimeHeaders.Count > 0)
                {
                    return this.TimeHeaders;
                }
                return new TimeHeaderCollection
                {
                    new TimeHeader(GroupByEnum.Default),
                    new TimeHeader(GroupByEnum.Cell)
                };
            }
        }

        [Category("Behavior"), Description("Collection of row header columns that will be displayed on the vertical axis."), PersistenceMode(PersistenceMode.InnerProperty)]
        public RowHeaderColumnCollection HeaderColumns
        {
            get
            {
                if (this.ViewState["HeaderColumns"] == null)
                {
                    RowHeaderColumnCollection rowHeaderColumnCollection = new RowHeaderColumnCollection();
                    rowHeaderColumnCollection.designMode = base.DesignMode;
                    this.ViewState["HeaderColumns"] = rowHeaderColumnCollection;
                }
                return (RowHeaderColumnCollection)this.ViewState["HeaderColumns"];
            }
        }

        [Category("Appearance"), Description("HTML of the upper-left corner."), Localizable(true)]
        public string CornerHtml
        {
            get
            {
                if (this.ViewState["CornerHtml"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["CornerHtml"];
            }
            set
            {
                this.ViewState["CornerHtml"] = value;
            }
        }

        [Category("Appearance"), Description("Background color of the upper-left corner."), Localizable(true)]
        public string CornerBackColor
        {
            get
            {
                if (this.ViewState["CornerBackColor"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["CornerBackColor"];
            }
            set
            {
                this.ViewState["CornerBackColor"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(MessageBarPosition.Top), Description("Message bar position (top/bottom).")]
        public MessageBarPosition MessageBarPosition
        {
            get
            {
                if (this.ViewState["MessageBarPosition"] == null)
                {
                    return MessageBarPosition.Top;
                }
                return (MessageBarPosition)this.ViewState["MessageBarPosition"];
            }
            set
            {
                this.ViewState["MessageBarPosition"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether to display the base time header (cells corresponding to CellDuration).")]
        public bool ShowBaseTimeHeader
        {
            get
            {
                return this.ViewState["ShowBaseTimeHeader"] == null || (bool)this.ViewState["ShowBaseTimeHeader"];
            }
            set
            {
                this.ViewState["ShowBaseTimeHeader"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(TimeScale.CellDuration), Description("Time scale unit.")]
        public TimeScale Scale
        {
            get
            {
                if (this.ViewState["Scale"] == null)
                {
                    return TimeScale.CellDuration;
                }
                return (TimeScale)this.ViewState["Scale"];
            }
            set
            {
                this.ViewState["Scale"] = value;
            }
        }

        [Category("Behavior"), Description("Time line cell collection.")]
        public TimeCellCollection Timeline
        {
            get
            {
                if (this.ViewState["Timeline"] == null)
                {
                    TimeCellCollection value = new TimeCellCollection();
                    this.ViewState["Timeline"] = value;
                }
                return (TimeCellCollection)this.ViewState["Timeline"];
            }
        }

        [Category("Behavior"), DefaultValue(300), Description("Double-click timeout in milliseconds.")]
        public int DoubleClickTimeout
        {
            get
            {
                if (this.ViewState["DoubleClickTimeout"] == null)
                {
                    return 300;
                }
                return (int)this.ViewState["DoubleClickTimeout"];
            }
            set
            {
                this.ViewState["DoubleClickTimeout"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Events will display a special floating object when not visible in full.")]
        public bool FloatingEvents
        {
            get
            {
                return this.ViewState["FloatingEvents"] == null || (bool)this.ViewState["FloatingEvents"];
            }
            set
            {
                this.ViewState["FloatingEvents"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Time header cells will display a special floating object when not visible in full.")]
        public bool FloatingTimeHeaders
        {
            get
            {
                return this.ViewState["FloatingTimeHeaders"] == null || (bool)this.ViewState["FloatingTimeHeaders"];
            }
            set
            {
                this.ViewState["FloatingTimeHeaders"] = value;
            }
        }

        [DefaultValue(CellWidthSpec.Fixed), Description("Specifies whether the cell width should be fixed (Fixed) or adjusted automatically to show all columns (Auto).")]
        public CellWidthSpec CellWidthSpec
        {
            get
            {
                if (this.ViewState["CellWidthSpec"] == null)
                {
                    return CellWidthSpec.Fixed;
                }
                return (CellWidthSpec)this.ViewState["CellWidthSpec"];
            }
            set
            {
                this.ViewState["CellWidthSpec"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether to automatically expand parent tree nodes during drag and drop event moving.")]
        public bool TreeAutoExpand
        {
            get
            {
                return this.ViewState["TreeAutoExpand"] == null || (bool)this.ViewState["TreeAutoExpand"];
            }
            set
            {
                this.ViewState["TreeAutoExpand"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether to display real-time position indicators during event moving.")]
        public bool EventMovingStartEndEnabled
        {
            get
            {
                return this.ViewState["EventMovingStartEndEnabled"] != null && (bool)this.ViewState["EventMovingStartEndEnabled"];
            }
            set
            {
                this.ViewState["EventMovingStartEndEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether to display real-time position indicators during event resizing.")]
        public bool EventResizingStartEndEnabled
        {
            get
            {
                return this.ViewState["EventResizingStartEndEnabled"] != null && (bool)this.ViewState["EventResizingStartEndEnabled"];
            }
            set
            {
                this.ViewState["EventResizingStartEndEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether to display real-time position indicators during time range selecting.")]
        public bool TimeRangeSelectingStartEndEnabled
        {
            get
            {
                return this.ViewState["TimeRangeSelectingStartEndEnabled"] != null && (bool)this.ViewState["TimeRangeSelectingStartEndEnabled"];
            }
            set
            {
                this.ViewState["TimeRangeSelectingStartEndEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue("MMMM d, yyyy"), Description("Date/time format used for position indicators during event moving.")]
        public string EventMovingStartEndFormat
        {
            get
            {
                if (this.ViewState["EventMovingStartEndFormat"] == null)
                {
                    return "MMMM d, yyyy";
                }
                return (string)this.ViewState["EventMovingStartEndFormat"];
            }
            set
            {
                this.ViewState["EventMovingStartEndFormat"] = value;
            }
        }

        [Category("Behavior"), DefaultValue("MMMM d, yyyy"), Description("Date/time format used for position indicators during event resizing.")]
        public string EventResizingStartEndFormat
        {
            get
            {
                if (this.ViewState["EventResizingStartEndFormat"] == null)
                {
                    return "MMMM d, yyyy";
                }
                return (string)this.ViewState["EventResizingStartEndFormat"];
            }
            set
            {
                this.ViewState["EventResizingStartEndFormat"] = value;
            }
        }

        [Category("Behavior"), DefaultValue("MMMM d, yyyy"), Description("Date/time format used for position indicators during time range selecting.")]
        public string TimeRangeSelectingStartEndFormat
        {
            get
            {
                if (this.ViewState["TimeRangeSelectingStartEndFormat"] == null)
                {
                    return "MMMM d, yyyy";
                }
                return (string)this.ViewState["TimeRangeSelectingStartEndFormat"];
            }
            set
            {
                this.ViewState["TimeRangeSelectingStartEndFormat"] = value;
            }
        }

        [Category("Data"), DefaultValue(DateTimeSpec.DateTime), Description("Whether the event end is specified by date only or by full date and time.")]
        public DateTimeSpec EventEndSpec
        {
            get
            {
                if (this.ViewState["EventEndSpec"] == null)
                {
                    return DateTimeSpec.DateTime;
                }
                return (DateTimeSpec)this.ViewState["EventEndSpec"];
            }
            set
            {
                this.ViewState["EventEndSpec"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the row headers should be rendered progressively (after scrolling).")]
        public bool ProgressiveRowRendering
        {
            get
            {
                return this.ViewState["ProgressiveRowRendering"] == null || (bool)this.ViewState["ProgressiveRowRendering"];
            }
            set
            {
                this.ViewState["ProgressiveRowRendering"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(25), Description("The number of row headers that will be preloaded (in both directions) when ProgressiveRowRendering is enabled.")]
        public int ProgressiveRowRenderingPreload
        {
            get
            {
                if (this.ViewState["ProgressiveRowRenderingPreload"] == null)
                {
                    return 25;
                }
                return (int)this.ViewState["ProgressiveRowRenderingPreload"];
            }
            set
            {
                this.ViewState["ProgressiveRowRenderingPreload"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Enables the row header hide icon in the upper-left corner of the viewport.")]
        public bool RowHeaderHideIconEnabled
        {
            get
            {
                return this.ViewState["RowHeaderHideIconEnabled"] != null && (bool)this.ViewState["RowHeaderHideIconEnabled"];
            }
            set
            {
                this.ViewState["RowHeaderHideIconEnabled"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether the horizontal scrollbar is enabled (RowHeaderWidth is enforced).")]
        public bool RowHeaderScrolling
        {
            get
            {
                return this.ViewState["RowHeaderScrolling"] != null && (bool)this.ViewState["RowHeaderScrolling"];
            }
            set
            {
                this.ViewState["RowHeaderScrolling"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Enables grouping of concurrent events into a single block.")]
        public bool GroupConcurrentEvents
        {
            get
            {
                return this.ViewState["GroupConcurrentEvents"] != null && (bool)this.ViewState["GroupConcurrentEvents"];
            }
            set
            {
                this.ViewState["GroupConcurrentEvents"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(1), Description("Do not group events if number of concurrent events doesn't exceed this limit.")]
        public int GroupConcurrentEventsLimit
        {
            get
            {
                if (this.ViewState["GroupConcurrentEventsLimit"] == null)
                {
                    return 1;
                }
                return (int)this.ViewState["GroupConcurrentEventsLimit"];
            }
            set
            {
                this.ViewState["GroupConcurrentEventsLimit"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Allows creating overlapping events using drag and drop operations.")]
        public bool AllowEventOverlap
        {
            get
            {
                return this.ViewState["AllowEventOverlap"] == null || (bool)this.ViewState["AllowEventOverlap"];
            }
            set
            {
                this.ViewState["AllowEventOverlap"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Blocks the Scheduler UI using an overlay div during a callback.")]
        public bool BlockOnCallBack
        {
            get
            {
                return this.ViewState["BlockOnCallBack"] != null && (bool)this.ViewState["BlockOnCallBack"];
            }
            set
            {
                this.ViewState["BlockOnCallBack"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Enables sweeping grid cell DOM elements from visited areas.")]
        public bool CellSweeping
        {
            get
            {
                return this.ViewState["CellSweeping"] == null || (bool)this.ViewState["CellSweeping"];
            }
            set
            {
                this.ViewState["CellSweeping"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(1000), Description("Specifies the number of grid cells (outside of the viewport) that will not be cleared when CellSweeping is enabled.")]
        public int CellSweepingCacheSize
        {
            get
            {
                if (this.ViewState["CellSweepingCacheSize"] == null)
                {
                    return 1000;
                }
                return (int)this.ViewState["CellSweepingCacheSize"];
            }
            set
            {
                this.ViewState["CellSweepingCacheSize"] = value;
            }
        }

        [Category("Events"), DefaultValue(RowCreateHandlingType.Disabled), Description("RowCreate event handling. Setting the value to 'Disabled' will deactivate the feature.")]
        public RowCreateHandlingType RowCreateHandling
        {
            get
            {
                if (this.ViewState["RowCreateHandling"] == null)
                {
                    return RowCreateHandlingType.Disabled;
                }
                return (RowCreateHandlingType)this.ViewState["RowCreateHandling"];
            }
            set
            {
                this.ViewState["RowCreateHandling"] = value;
            }
        }

        [Category("Events"), Description("Gets or sets the Javascript code that is executed when the user finishes editing a new row field (before the default action).")]
        public string RowCreateJavaScript
        {
            get
            {
                if (this.ViewState["RowCreateJavaScript"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["RowCreateJavaScript"];
            }
            set
            {
                this.ViewState["RowCreateJavaScript"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(100), Description("Specifies the line height (in percent) when stacking concurrent events.")]
        public int EventStackingLineHeight
        {
            get
            {
                if (this.ViewState["EventStackingLineHeight"] == null)
                {
                    return 100;
                }
                return (int)this.ViewState["EventStackingLineHeight"];
            }
            set
            {
                this.ViewState["EventStackingLineHeight"] = value;
            }
        }

        [Category("Data"), Description("Collection of tasks."), PersistenceMode(PersistenceMode.InnerProperty)]
        public LinkCollection Links
        {
            get
            {
                if (this.StoreLinksInViewState && this.ViewState["Links"] != null)
                {
                    return (LinkCollection)this.ViewState["Links"];
                }
                if (this._links == null)
                {
                    this._links = new LinkCollection
                    {
                        designMode = base.DesignMode
                    };
                }
                return this._links;
            }
        }

        [Category("Behavior"), DefaultValue(false), Description("Whether the link should be stored in the ViewState.")]
        public bool StoreLinksInViewState
        {
            get
            {
                return this.ViewState["StoreLinksInViewState"] != null && (bool)this.ViewState["StoreLinksInViewState"];
            }
            set
            {
                this.ViewState["StoreLinksInViewState"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Whether the links should be synchronized with the server during CallBacks.")]
        public bool SyncLinks
        {
            get
            {
                return this.ViewState["SyncLinks"] == null || (bool)this.ViewState["SyncLinks"];
            }
            set
            {
                this.ViewState["SyncLinks"] = value;
            }
        }

        [Category("Events"), DefaultValue(EventTapAndHoldHandlingEnum.Move), Description("Specifies the action that should be performed for a touch 'tap and hold' gesture.")]
        public EventTapAndHoldHandlingEnum EventTapAndHoldHandling
        {
            get
            {
                if (this.ViewState["EventTapAndHoldHandling"] == null)
                {
                    return EventTapAndHoldHandlingEnum.Move;
                }
                return (EventTapAndHoldHandlingEnum)this.ViewState["EventTapAndHoldHandling"];
            }
            set
            {
                this.ViewState["EventTapAndHoldHandling"] = value;
            }
        }

        public HMSScheduler()
        {
            this.TimeHeader = new SchedulerTimeHeader(new SchedulerTimeHeaderParent(this));
            this.SelectedRows = new List<RowInfo>();
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            if (this.Context.Request.Params[this.ClientID + "_vsupdate"] != null)
            {
                LosFormatter losFormatter = new LosFormatter();
                Hashtable hashtable = (Hashtable)losFormatter.Deserialize(this.Context.Request.Params[this.ClientID + "_vsupdate"]);
                foreach (DictionaryEntry dictionaryEntry in hashtable)
                {
                    string key = (string)dictionaryEntry.Key;
                    object value = dictionaryEntry.Value;
                    this.ViewState[key] = value;
                }
            }
            string text = this.Context.Request.Params[this.ClientID + "_state"];
            if (!string.IsNullOrEmpty(text))
            {
                string input = HMS.Utils.Encoder.HtmlDecode(text);
                JsonData jsonData = SimpleJsonDeserializer.Deserialize(input);
                this.ScrollX = (int)jsonData["scrollX"];
                this.ScrollY = (int)jsonData["scrollY"];
                this.Resources.RestoreFromJson(jsonData["tree"]);
            }
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSCalendar), "common.js", this.GetResourceUrl("Common.js"));
            ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSScheduler), "scheduler.js", this.GetResourceUrl("Scheduler.js"));
            this.ContextMenuEvent = this.FindMenu(this.ContextMenuID);
            this.ContextMenuSelection = this.FindMenu(this.ContextMenuSelectionID);
            this.ContextMenuResource = this.FindMenu(this.ContextMenuResourceID);
            this.BubbleEvent = this.FindBubble(this.BubbleID);
            this.BubbleCell = this.FindBubble(this.CellBubbleID);
            this.BubbleResource = this.FindBubble(this.ResourceBubbleID);
            base.OnLoad(e);
        }

        private HMSMenu FindMenu(string id)
        {
            HMSMenu result = null;
            if (!string.IsNullOrEmpty(id))
            {
                result = (HMSScheduler.RecursiveFind(this.NamingContainer, id) as HMSMenu);
            }
            return result;
        }

        private HMSBubble FindBubble(string id)
        {
            HMSBubble result = null;
            if (!string.IsNullOrEmpty(id))
            {
                result = (HMSScheduler.RecursiveFind(this.NamingContainer, id) as HMSBubble);
            }
            return result;
        }

        private static Control RecursiveFind(Control parent, string id)
        {
            Control control = parent.FindControl(id);
            if (control != null)
            {
                return control;
            }
            foreach (Control parent2 in parent.Controls)
            {
                control = HMSScheduler.RecursiveFind(parent2, id);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }

        protected override void Render(HtmlTextWriter output)
        {
            this.LoadRows();
            this.TimeHeader.PrepareTimeline();
            this.TimeHeader.PrepareGrouplines();
            if (base.DesignMode)
            {
                this.Design(output);
            }
            else
            {
                this.RenderClientSide(output);
            }
            JsInitScheduler jsInitScheduler = new JsInitScheduler(this);
            ScriptManagerHelper.RegisterStartupScript(this, typeof(HMSScheduler), this.ClientID + "object", jsInitScheduler.GetCode(), false);
        }

        private void RenderClientSide(HtmlTextWriter output)
        {
            output.AddAttribute("id", this.ClientID);
            output.RenderBeginTag("div");
            output.RenderEndTag();
        }

        private void Design(HtmlTextWriter output)
        {
            this.DesignMainTable(output);
        }

        private void DesignMainTable(HtmlTextWriter output)
        {
            output.AddAttribute("id", this.ClientID);
            output.AddStyleAttribute("width", this.Width.ToString());
            output.AddStyleAttribute("height", this.GetTotalHeight() + "px");
            output.AddStyleAttribute("line-height", "1.2");
            output.AddStyleAttribute("position", "relative");
            output.RenderBeginTag("div");
            output.AddAttribute("cellspacing", "0");
            output.AddAttribute("cellpadding", "0");
            output.AddAttribute("border", "0");
            output.AddStyleAttribute("background-color", ColorTranslator.ToHtml(this.HourNameBackColor));
            output.AddStyleAttribute("position", "absolute");
            output.RenderBeginTag("table");
            output.RenderBeginTag("tr");
            output.RenderBeginTag("td");
            this.DesignCorner(output);
            output.RenderEndTag();
            output.RenderBeginTag("td");
            if (this.Width.Type == UnitType.Pixel)
            {
                output.AddStyleAttribute("width", this.Width.Value - (double)this.TotalRowHeaderWidth + "px");
            }
            else if (this.Width.Type == UnitType.Percentage)
            {
                if (base.DesignMode)
                {
                    output.AddStyleAttribute("width", "500px");
                }
                else
                {
                    output.AddStyleAttribute("width", "100%");
                    output.AddStyleAttribute("width", string.Concat(new object[]
                    {
                        "expression(document.getElementById('",
                        this.ClientID,
                        "').offsetWidth - 6 - ",
                        this.TotalRowHeaderWidth,
                        ")"
                    }));
                }
            }
            this.DesignTimeHeader(output);
            output.RenderEndTag();
            output.RenderEndTag();
            output.RenderBeginTag("tr");
            output.AddAttribute("valign", "top");
            output.RenderBeginTag("td");
            this.DesignResourceHeader(output);
            output.RenderEndTag();
            output.RenderBeginTag("td");
            this.DesignMainContent(output);
            output.RenderEndTag();
            output.RenderEndTag();
            output.RenderEndTag();
            this.DesignHiddenFields(output);
            output.RenderEndTag();
        }

        private void DesignHiddenFields(HtmlTextWriter output)
        {
            output.AddAttribute("id", this.ClientID + "_vsph");
            output.AddStyleAttribute("display", "none");
            output.RenderBeginTag("div");
            output.RenderEndTag();
            output.AddAttribute("type", "hidden");
            output.AddAttribute("id", this.ClientID + "_state");
            output.AddAttribute("name", this.ClientID + "_state");
            output.RenderBeginTag("input");
            output.RenderEndTag();
        }

        private void DesignMainContent(HtmlTextWriter output)
        {
            this.DesignScrollable(output);
            if (this.ScrollLabelsVisible)
            {
                this.DesignScrollLabels(output);
            }
        }

        private void DesignResourceHeader(HtmlTextWriter output)
        {
            output.AddStyleAttribute("border-top", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-bottom", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-left", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-right", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("width", this.TotalRowHeaderWidth - 1 + "px");
            output.AddStyleAttribute("height", this.GetScrollableHeight() + "px");
            output.AddStyleAttribute("overflow", "hidden");
            output.AddStyleAttribute("position", "relative");
            output.AddAttribute("id", this.ClientID + "_resscroll");
            output.RenderBeginTag("div");
            this.DesignRowHeader(output);
            output.RenderEndTag();
        }

        private void DesignTimeHeader(HtmlTextWriter output)
        {
            output.AddStyleAttribute("overflow", "hidden");
            output.AddStyleAttribute("background-color", ColorTranslator.ToHtml(this.HourNameBackColor));
            output.AddStyleAttribute("border-right", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddAttribute("id", this.ClientID + "_timescroll");
            output.RenderBeginTag("div");
            output.AddStyleAttribute("border-top", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("width", this.CellCount * this.CellWidth + 1000 + "px");
            output.AddAttribute("id", this.ClientID + "_north");
            output.RenderBeginTag("div");
            output.AddAttribute("cellspacing", "0");
            output.AddAttribute("cellpadding", "0");
            output.RenderBeginTag("table");
            this.DesignHeader(output);
            output.RenderEndTag();
            output.RenderEndTag();
            output.RenderEndTag();
        }

        private void DesignScrollLabels(HtmlTextWriter output)
        {
            output.AddStyleAttribute("position", "relative");
            output.AddStyleAttribute("display", "block");
            output.RenderBeginTag("div");
            output.AddStyleAttribute("position", "absolute");
            output.AddStyleAttribute("top", -2 - this.GetScrollableHeight() + "px");
            output.AddStyleAttribute("text-align", "left");
            output.AddStyleAttribute("height", "1px");
            output.AddStyleAttribute("width", "100%");
            output.AddStyleAttribute("font-size", "1px");
            output.RenderBeginTag("div");
            output.AddAttribute("id", this.ClientID + "_left");
            output.AddAttribute("unselectable", "on");
            output.AddStyleAttribute("-khtml-user-select", "none");
            output.AddStyleAttribute("-moz-user-select", "none");
            output.AddStyleAttribute("position", "absolute");
            output.AddAttribute("src", this.GetResourceUrl("Images/Left10x10.gif"));
            output.AddStyleAttribute("display", "none");
            output.AddAttribute("width", "10");
            output.AddAttribute("height", "10");
            output.RenderBeginTag("img");
            output.RenderEndTag();
            output.RenderEndTag();
            output.Write("<div style='position: absolute; top:" + (-2 - this.GetScrollableHeight()) + "px; text-align: right; height: 1px; font-size: 1px; width: 100%;'>");
            output.Write(string.Concat(new string[]
            {
                "<img id='",
                this.ClientID,
                "_right' unselectable='on' src='",
                this.GetResourceUrl("Images/Right10x10.gif"),
                "' width='10' height='10' style='-moz-user-select:none;-khtml-user-select:none;margin-right:1px; display:none;' />"
            }));
            output.Write("</div>");
            output.RenderEndTag();
        }

        private void DesignScrollable(HtmlTextWriter output)
        {
            output.AddStyleAttribute("overflow", "auto");
            if (!base.DesignMode)
            {
                output.AddStyleAttribute("overflow-x", "auto");
                output.AddStyleAttribute("overflow-y", "auto");
            }
            output.AddStyleAttribute("position", "relative");
            int scrollableHeight = this.GetScrollableHeight();
            output.AddStyleAttribute("height", scrollableHeight + "px");
            output.AddStyleAttribute("border-right", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-bottom", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-top", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("background-color", ColorTranslator.ToHtml(this.HourNameBackColor));
            output.AddAttribute("id", this.ClientID + "_scroll");
            output.RenderBeginTag("div");
            int num = this.CellCount * this.CellWidth;
            if (base.DesignMode)
            {
                num = ((this.Width.Type == UnitType.Pixel) ? ((int)this.Width.Value - this.TotalRowHeaderWidth) : 500);
            }
            output.AddStyleAttribute("width", num + "px");
            output.RenderBeginTag("div");
            if (this.LoadingLabelVisible)
            {
                output.AddStyleAttribute("position", "absolute");
                output.AddStyleAttribute("background-color", ColorTranslator.ToHtml(this.LoadingLabelBackColor));
                output.AddStyleAttribute("color", ColorTranslator.ToHtml(this.LoadingLabelFontColor));
                output.AddStyleAttribute("padding", "2px");
                output.AddStyleAttribute("font-size", this.LoadingLabelFontSize);
                output.AddStyleAttribute("font-family", this.EventFontFamily);
                output.RenderBeginTag("div");
                output.Write(this.LoadingLabelText);
                output.RenderEndTag();
            }
            output.RenderEndTag();
            output.AddAttribute("id", this.ClientID + "_maind");
            output.AddStyleAttribute("-moz-user-select", "none");
            output.AddAttribute("unselectable", "on");
            output.RenderBeginTag("div");
            output.RenderEndTag();
            output.RenderEndTag();
        }

        private int GetInnerHeight()
        {
            int num = 0;
            foreach (Day current in this.Rows)
            {
                num += current.MaxColumns() * this.EventHeight;
            }
            return Math.Max(num - 1, 0);
        }

        private int GetTotalHeight()
        {
            return this.GetScrollableHeight() + 2 * this.HeaderHeight;
        }

        private int GetScrollableHeight()
        {
            if (this.HeightSpec == SchedulerHeightSpec.Fixed)
            {
                return (int)this.Height.Value;
            }
            int num = 2;
            num += this.GetInnerHeight();
            num += 16;
            if (this.HeightSpec == SchedulerHeightSpec.Max && (double)num > this.Height.Value)
            {
                return (int)this.Height.Value;
            }
            return num;
        }

        private void DesignRowHeader(HtmlTextWriter output)
        {
            output.AddStyleAttribute("border-collapse", "collapse");
            output.AddStyleAttribute("-khtml-user-select", "none");
            output.AddStyleAttribute("-moz-user-select", "none");
            output.AddAttribute("cellspacing", "0");
            output.AddAttribute("cellpadding", "0");
            output.AddAttribute("id", this.ClientID + "_header");
            output.RenderBeginTag("table");
            foreach (Day current in this.Rows)
            {
                output.RenderBeginTag("tr");
                BeforeResHeaderRenderEventArgs rowHeader = this.GetRowHeader(current);
                int num = current.MaxColumns() * this.EventHeight - 1;
                output.AddStyleAttribute("width", this.TotalRowHeaderWidth - 1 + "px");
                output.AddStyleAttribute("border-right", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
                output.AddStyleAttribute("border-bottom", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
                output.AddStyleAttribute("background-color", rowHeader.BackgroundColor);
                output.AddStyleAttribute("font-family", this.HeaderFontFamily);
                output.AddStyleAttribute("font-size", this.HeaderFontSize);
                output.AddStyleAttribute("color", ColorTranslator.ToHtml(this.HeaderFontColor));
                output.AddStyleAttribute("cursor", "default");
                if (!string.IsNullOrEmpty(rowHeader.ToolTip))
                {
                    output.AddAttribute("title", rowHeader.ToolTip);
                }
                output.AddAttribute("unselectable", "on");
                output.AddAttribute("class", this.ApplyCssClass("rowheader"));
                output.AddAttribute("resource", rowHeader.Id);
                output.AddAttribute("onmousemove", this.ClientObjectName + ".onResMouseMove(this)");
                output.AddAttribute("onmouseout", this.ClientObjectName + ".onResMouseOut(this)");
                output.RenderBeginTag("td");
                output.Write(string.Concat(new object[]
                {
                    "<div unselectable='on' style='margin-left:4px; height:",
                    num,
                    "px; line-height:",
                    num,
                    "px; overflow:hidden;'>"
                }));
                output.Write(rowHeader.Html);
                output.Write("</div>");
                output.RenderEndTag();
                output.RenderEndTag();
            }
            output.RenderBeginTag("tr");
            output.AddStyleAttribute("width", this.TotalRowHeaderWidth + "px");
            output.AddStyleAttribute("height", "17px");
            output.AddStyleAttribute("border-right", "1px solid black");
            output.AddStyleAttribute("background-color", ColorTranslator.ToHtml(this.HourNameBackColor));
            output.AddStyleAttribute("font-size", "1px");
            output.AddStyleAttribute("cursor", "default");
            output.AddAttribute("unselectable", "on");
            output.AddAttribute("class", this.ApplyCssClass("rowheader"));
            output.RenderBeginTag("td");
            output.Write("&nbsp;");
            output.RenderEndTag();
            output.RenderEndTag();
            output.RenderEndTag();
        }

        internal BeforeResHeaderRenderEventArgs GetRowHeader(Day d)
        {
            BeforeResHeaderRenderEventArgs beforeResHeaderRenderEventArgs = new BeforeResHeaderRenderEventArgs();
            beforeResHeaderRenderEventArgs.Id = d.Id;
            beforeResHeaderRenderEventArgs.Name = d.Name;
            beforeResHeaderRenderEventArgs.Date = this.StartDate;
            beforeResHeaderRenderEventArgs.DataItem = d.DataItem;
            if (this.ViewType == ViewTypeEnum.Days)
            {
                beforeResHeaderRenderEventArgs.Date = d.Start;
            }
            beforeResHeaderRenderEventArgs.Html = HMS.Utils.Encoder.HtmlEncode(d.Name);
            beforeResHeaderRenderEventArgs.ToolTip = d.ToolTip;
            beforeResHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
            beforeResHeaderRenderEventArgs.ContextMenuClientName = ((this.ContextMenuResource != null) ? this.ContextMenuResource.ClientObjectName : null);
            beforeResHeaderRenderEventArgs.Columns = new List<ResourceColumn>();
            beforeResHeaderRenderEventArgs.MinHeight = this.RowMinHeight;
            beforeResHeaderRenderEventArgs.MarginBottom = this.RowMarginBottom;
            beforeResHeaderRenderEventArgs.EventHeight = this.EventHeight;
            bool flag = true;
            foreach (RowHeaderColumn arg_DE_0 in this.HeaderColumns)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    beforeResHeaderRenderEventArgs.Columns.Add(new ResourceColumn());
                }
            }
            this.OnBeforeResHeaderRender(beforeResHeaderRenderEventArgs);
            return beforeResHeaderRenderEventArgs;
        }

        public void LoadStylesDefaultTheme()
        {
            this.HourNameBackColor = ColorTranslator.FromHtml("#eee");
            this.NonBusinessBackColor = ColorTranslator.FromHtml("#f9f9f9");
            this.BackColor = ColorTranslator.FromHtml("#fff");
            this.CellBorderColor = ColorTranslator.FromHtml("#eee");
            this.EventBorderColor = ColorTranslator.FromHtml("#ccc");
            this.BorderColor = ColorTranslator.FromHtml("#aaa");
            this.DurationBarColor = ColorTranslator.FromHtml("#1066a8");
            this.DurationBarBackColor = ColorTranslator.FromHtml("#9dc8e8");
            this.EventFontSize = "10pt";
            this.EventFontColor = ColorTranslator.FromHtml("#666");
            this.HeaderFontColor = ColorTranslator.FromHtml("#666");
            this.DurationBarHeight = 5;
        }

        private BeforeResHeaderRenderEventArgs GetResourceHeaderDaysView(DateTime d)
        {
            BeforeResHeaderRenderEventArgs beforeResHeaderRenderEventArgs = new BeforeResHeaderRenderEventArgs();
            beforeResHeaderRenderEventArgs.Id = d.ToString("s");
            beforeResHeaderRenderEventArgs.Name = d.ToShortDateString();
            beforeResHeaderRenderEventArgs.Date = d.Date;
            beforeResHeaderRenderEventArgs.Html = HMS.Utils.Encoder.HtmlEncode(beforeResHeaderRenderEventArgs.Name);
            beforeResHeaderRenderEventArgs.ToolTip = beforeResHeaderRenderEventArgs.Name;
            beforeResHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
            beforeResHeaderRenderEventArgs.ContextMenuClientName = ((this.ContextMenuResource != null) ? this.ContextMenuResource.ClientObjectName : null);
            beforeResHeaderRenderEventArgs.Columns = new List<ResourceColumn>();
            beforeResHeaderRenderEventArgs.MinHeight = this.RowMinHeight;
            beforeResHeaderRenderEventArgs.MarginBottom = this.RowMarginBottom;
            beforeResHeaderRenderEventArgs.EventHeight = this.EventHeight;
            bool flag = true;
            foreach (RowHeaderColumn arg_C6_0 in this.HeaderColumns)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    beforeResHeaderRenderEventArgs.Columns.Add(new ResourceColumn());
                }
            }
            this.OnBeforeResHeaderRender(beforeResHeaderRenderEventArgs);
            return beforeResHeaderRenderEventArgs;
        }

        private BeforeResHeaderRenderEventArgs GetResourceHeader(Resource r)
        {
            BeforeResHeaderRenderEventArgs beforeResHeaderRenderEventArgs = new BeforeResHeaderRenderEventArgs();
            beforeResHeaderRenderEventArgs.Id = r.Id;
            beforeResHeaderRenderEventArgs.Name = r.Name;
            beforeResHeaderRenderEventArgs.Date = this.StartDate;
            beforeResHeaderRenderEventArgs.Html = HMS.Utils.Encoder.HtmlEncode(r.Name);
            beforeResHeaderRenderEventArgs.ToolTip = r.ToolTip;
            beforeResHeaderRenderEventArgs.MinHeight = this.RowMinHeight;
            beforeResHeaderRenderEventArgs.MarginBottom = this.RowMarginBottom;
            beforeResHeaderRenderEventArgs.EventHeight = this.EventHeight;
            beforeResHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
            beforeResHeaderRenderEventArgs.ContextMenuClientName = ((this.ContextMenuResource != null) ? this.ContextMenuResource.ClientObjectName : null);
            if (r.DataItem != null)
            {
                beforeResHeaderRenderEventArgs.DataItem = new DataItemWrapper(r.DataItem);
            }
            if (r.Columns != null && r.Columns.Count > 0)
            {
                using (List<ResourceColumn>.Enumerator enumerator = r.Columns.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ResourceColumn current = enumerator.Current;
                        beforeResHeaderRenderEventArgs.Columns.Add(new ResourceColumn(current.Html));
                    }
                    goto IL_15C;
                }
            }
            bool flag = true;
            foreach (object arg_124_0 in this.HeaderColumns)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    beforeResHeaderRenderEventArgs.Columns.Add(new ResourceColumn());
                }
            }
            IL_15C:
            this.OnBeforeResHeaderRender(beforeResHeaderRenderEventArgs);
            return beforeResHeaderRenderEventArgs;
        }

        private void DesignCorner(HtmlTextWriter output)
        {
            output.AddStyleAttribute("width", this.TotalRowHeaderWidth + "px");
            output.AddStyleAttribute("border-right", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("border-top", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddStyleAttribute("background-color", this.CornerBackColor);
            output.AddStyleAttribute("font-family", this.HourFontFamily);
            output.AddStyleAttribute("font-size", this.HourFontSize);
            output.AddStyleAttribute("cursor", "default");
            output.AddStyleAttribute("overflow", "hidden");
            output.AddStyleAttribute("position", "relative");
            output.AddAttribute("class", this.ApplyCssClass("corner"));
            output.AddAttribute("unselectable", "on");
            output.RenderBeginTag("div");
            output.AddStyleAttribute("width", this.TotalRowHeaderWidth - 1 + "px");
            output.AddStyleAttribute("height", this.HeaderHeight * 2 - 1 + "px");
            output.AddStyleAttribute("border-left", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
            output.AddAttribute("id", this.ClientID + "_corner");
            output.RenderBeginTag("div");
            output.Write(this.CornerHtml);
            output.RenderEndTag();
            output.RenderEndTag();
        }

        internal void OnBeforeResHeaderRender(BeforeResHeaderRenderEventArgs ea)
        {
            if (this.BeforeResHeaderRender != null)
            {
                this.BeforeResHeaderRender(this, ea);
            }
        }

        internal void DoBeforeTimeHeaderRender(BeforeTimeHeaderRenderEventArgs ea)
        {
            if (this.BeforeTimeHeaderRender != null)
            {
                this.BeforeTimeHeaderRender(this, ea);
            }
        }

        private BeforeEventRenderEventArgs GetEva(Event e)
        {
            BeforeEventRenderEventArgs beforeEventRenderEventArgs = new BeforeEventRenderEventArgs(e, this.TagFields, this.ServerTagFields);
            string text = e.Start.ToShortDateString() + " " + e.Start.ToShortTimeString();
            string text2 = e.End.ToShortDateString() + " " + e.End.ToShortTimeString();
            if (this.ShowEventStartEnd)
            {
                beforeEventRenderEventArgs.Html = string.Concat(new string[]
                {
                    e.Text,
                    " (",
                    text,
                    " - ",
                    text2,
                    ")"
                });
            }
            else
            {
                beforeEventRenderEventArgs.Html = e.Text;
            }
            beforeEventRenderEventArgs.ToolTip = beforeEventRenderEventArgs.Html;
            if (!this.CssOnly || this.IsExport)
            {
                beforeEventRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.EventBackColor);
                beforeEventRenderEventArgs.DurationBarColor = ColorTranslator.ToHtml(this.DurationBarColor);
                beforeEventRenderEventArgs.DurationBarBackColor = ColorTranslator.ToHtml(this.DurationBarBackColor);
            }
            beforeEventRenderEventArgs.DurationBarVisible = this.DurationBarVisible;
            beforeEventRenderEventArgs.EventClickEnabled = (this.EventClickHandling != EventClickHandlingEnum.Disabled);
            beforeEventRenderEventArgs.EventMoveEnabled = (this.EventMoveHandling != UserActionHandling.Disabled);
            beforeEventRenderEventArgs.EventResizeEnabled = (this.EventResizeHandling != UserActionHandling.Disabled);
            beforeEventRenderEventArgs.EventRightClickEnabled = (this.EventRightClickHandling != EventRightClickHandlingEnum.Disabled);
            beforeEventRenderEventArgs.EventDoubleClickEnabled = (this.EventDoubleClickHandling != EventClickHandlingEnum.Disabled);
            beforeEventRenderEventArgs.EventMoveVerticalEnabled = beforeEventRenderEventArgs.EventMoveEnabled;
            beforeEventRenderEventArgs.EventMoveHorizontalEnabled = beforeEventRenderEventArgs.EventMoveEnabled;
            if (this.ContextMenuEvent != null)
            {
                beforeEventRenderEventArgs.ContextMenuClientName = this.ContextMenuEvent.ClientObjectName;
            }
            if (e.Recurrent)
            {
                if (e.RecurrentException)
                {
                    beforeEventRenderEventArgs.Html = string.Format("<img src='{0}' /> {1}", this.ResolveUrlSafe(this.RecurrentEventExceptionImage), beforeEventRenderEventArgs.Html);
                }
                else
                {
                    beforeEventRenderEventArgs.Html = string.Format("<img src='{0}' /> {1}", this.ResolveUrlSafe(this.RecurrentEventImage), beforeEventRenderEventArgs.Html);
                }
            }
            if (this.BeforeEventRender != null)
            {
                this.BeforeEventRender(this, beforeEventRenderEventArgs);
            }
            return beforeEventRenderEventArgs;
        }

        internal BeforeEventRenderEventArgs GetEva(EventPart p)
        {
            return this.GetEva(p.Event);
        }

        private int TicksToPixels(TimeSpan ticks)
        {
            return (int)Math.Floor((double)this.CellWidth * ticks.TotalMinutes / (double)this.CellDuration);
        }

        private void DesignHeader(HtmlTextWriter output)
        {
            output.RenderBeginTag("tr");
            this.DesignHeaderGroups(output);
            output.RenderEndTag();
            output.RenderBeginTag("tr");
            this.DesignHeaderCols(output);
            output.RenderEndTag();
        }

        internal void DesignHeaderCols(HtmlTextWriter output)
        {
            for (int i = 0; i < this.CellCount; i++)
            {
                DateTime dateTime = this.StartDate.AddMinutes((double)(this.CellDuration * i));
                DateTime end = dateTime.AddMinutes((double)this.CellDuration);
                BeforeTimeHeaderRenderEventArgs beforeTimeHeaderRenderEventArgs = BeforeTimeHeaderRenderEventArgs.FromCell(new TimeHeaderCell(new SchedulerTimeHeaderParent(this), dateTime, end)
                {
                    IsColGroup = false
                });
                beforeTimeHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
                if (this.CellDuration < 60)
                {
                    beforeTimeHeaderRenderEventArgs.InnerHTML = string.Format("<span style='color:gray'>{0:00}</span>", dateTime.Minute);
                    beforeTimeHeaderRenderEventArgs.ToolTip = dateTime.Minute.ToString("00");
                }
                else if (this.CellDuration < 1440)
                {
                    beforeTimeHeaderRenderEventArgs.InnerHTML = TimeFormatter.GetHour(dateTime, this.TimeFormat, "{0} {1}");
                }
                else
                {
                    beforeTimeHeaderRenderEventArgs.InnerHTML = dateTime.Day.ToString();
                }
                if (string.IsNullOrEmpty(beforeTimeHeaderRenderEventArgs.ToolTip))
                {
                    beforeTimeHeaderRenderEventArgs.ToolTip = beforeTimeHeaderRenderEventArgs.InnerHTML;
                }
                if (this.BeforeTimeHeaderRender != null)
                {
                    this.BeforeTimeHeaderRender(this, beforeTimeHeaderRenderEventArgs);
                }
                output.AddStyleAttribute("border-top", "1px solid " + ColorTranslator.ToHtml(this.BorderColor));
                output.AddStyleAttribute("width", this.CellWidth + "px");
                output.AddStyleAttribute("height", this.HeaderHeight - 1 + "px");
                output.AddStyleAttribute("overflow", "hidden");
                output.AddStyleAttribute("text-align", "center");
                output.AddStyleAttribute("background-color", beforeTimeHeaderRenderEventArgs.BackgroundColor);
                output.AddStyleAttribute("font-family", this.HourFontFamily);
                output.AddStyleAttribute("font-size", this.HourFontSize);
                output.AddStyleAttribute("color", ColorTranslator.ToHtml(this.HeaderFontColor));
                output.AddAttribute("unselectable", "on");
                output.AddStyleAttribute("-khtml-user-select", "none");
                output.AddStyleAttribute("-moz-user-select", "none");
                output.AddStyleAttribute("cursor", "default");
                output.AddAttribute("class", this.ApplyCssClass("timeheadercol"));
                output.RenderBeginTag("td");
                output.Write(string.Concat(new object[]
                {
                    "<div unselectable='on' style='height:",
                    this.HeaderHeight - 1,
                    "px;border-right: 1px solid ",
                    ColorTranslator.ToHtml(this.HourNameBorderColor),
                    "; width:",
                    this.CellWidth - 1,
                    "px;overflow:hidden;' title='",
                    beforeTimeHeaderRenderEventArgs.ToolTip,
                    "'>"
                }));
                output.Write(beforeTimeHeaderRenderEventArgs.InnerHTML);
                output.Write("</div>");
                output.RenderEndTag();
            }
        }

        private void DesignHeaderGroups(HtmlTextWriter output)
        {
            if (this.ViewType == ViewTypeEnum.Days && this.CellGroupBy != GroupByEnum.Hour && this.CellGroupBy != GroupByEnum.None)
            {
                throw new ArgumentException("Set CellGroupBy property to either Hour or None. Other values are not allowed in Days view.");
            }
            DayOfWeek resolvedWeekStart = this.ResolvedWeekStart;
            DateTime dateTime = this.StartDate.AddDays((double)this.DaysHorizontally);
            TimeSpan timeSpan = TimeSpan.FromDays((double)this.DaysHorizontally);
            while (timeSpan > TimeSpan.Zero && timeSpan >= TimeSpan.FromMinutes((double)this.CellDuration))
            {
                DateTime dateTime2 = dateTime - timeSpan;
                DateTime dateTime3;
                switch (this.CellGroupBy)
                {
                    case GroupByEnum.Hour:
                        dateTime3 = dateTime2.AddHours(1.0);
                        break;
                    case GroupByEnum.Day:
                        dateTime3 = dateTime2.AddDays(1.0);
                        break;
                    case GroupByEnum.Week:
                        dateTime3 = dateTime2.AddDays(1.0);
                        while (dateTime3.DayOfWeek != resolvedWeekStart)
                        {
                            dateTime3 = dateTime3.AddDays(1.0);
                        }
                        break;
                    case GroupByEnum.Month:
                        {
                            dateTime3 = dateTime2.AddMonths(1);
                            dateTime3 = new DateTime(dateTime3.Year, dateTime3.Month, 1);
                            bool flag = Math.Floor((dateTime3 - dateTime2).TotalMinutes / (double)this.CellDuration) == (dateTime3 - dateTime2).TotalMinutes / (double)this.CellDuration;
                            while (!flag)
                            {
                                dateTime3 = dateTime3.AddHours(1.0);
                                flag = (Math.Floor((dateTime3 - dateTime2).TotalMinutes / (double)this.CellDuration) == (dateTime3 - dateTime2).TotalMinutes / (double)this.CellDuration);
                            }
                            break;
                        }
                    case GroupByEnum.Year:
                        {
                            dateTime3 = dateTime2.AddYears(1);
                            dateTime3 = new DateTime(dateTime3.Year, 1, 1);
                            bool flag = (dateTime3 - dateTime2).TotalMinutes % (double)this.CellDuration == 0.0;
                            int num = DateTime.IsLeapYear(dateTime2.Year) ? 366 : 365;
                            int val = num * 24 * 60;
                            TimeSpan t = TimeSpan.FromMinutes((double)Math.Min(val, this.CellDuration));
                            while (!flag)
                            {
                                if (!(dateTime3 - dateTime2 < t))
                                {
                                    break;
                                }
                                dateTime3 = dateTime3.AddHours(1.0);
                                flag = ((dateTime3 - dateTime2).TotalMinutes % (double)this.CellDuration == 0.0);
                            }
                            break;
                        }
                    case GroupByEnum.None:
                        dateTime3 = dateTime2 + timeSpan;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (dateTime3 > dateTime)
                {
                    dateTime3 = dateTime;
                }
                int num2 = (int)Math.Floor((dateTime3 - dateTime2).TotalMinutes);
                int num3 = num2 / this.CellDuration;
                BeforeTimeHeaderRenderEventArgs beforeTimeHeaderRenderEventArgs = BeforeTimeHeaderRenderEventArgs.FromCell(new TimeHeaderCell(new SchedulerTimeHeaderParent(this), dateTime2, dateTime3)
                {
                    IsColGroup = true
                });
                beforeTimeHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
                int num4 = this.CellWidth * num3;
                beforeTimeHeaderRenderEventArgs.ToolTip = string.Empty;
                switch (this.CellGroupBy)
                {
                    case GroupByEnum.Hour:
                        beforeTimeHeaderRenderEventArgs.InnerHTML = TimeFormatter.GetHour(dateTime2, this.TimeFormat, "{0} {1}");
                        break;
                    case GroupByEnum.Day:
                        beforeTimeHeaderRenderEventArgs.InnerHTML = dateTime2.ToLongDateString();
                        break;
                    case GroupByEnum.Week:
                        {
                            DateTime dateTime4 = dateTime3.AddSeconds(-1.0);
                            if (num4 < 115)
                            {
                                beforeTimeHeaderRenderEventArgs.InnerHTML = string.Format("{0}", Week.WeekNrISO8601(dateTime2));
                                beforeTimeHeaderRenderEventArgs.ToolTip = string.Format("Week {0} ({1:MMMM yyyy})", Week.WeekNrISO8601(dateTime2), dateTime4);
                            }
                            else
                            {
                                beforeTimeHeaderRenderEventArgs.InnerHTML = string.Format("Week {0} ({1:MMMM yyyy})", Week.WeekNrISO8601(dateTime2), dateTime4);
                            }
                            break;
                        }
                    case GroupByEnum.Month:
                        beforeTimeHeaderRenderEventArgs.InnerHTML = dateTime2.ToString("MMMM yyyy");
                        break;
                    case GroupByEnum.Year:
                        beforeTimeHeaderRenderEventArgs.InnerHTML = dateTime2.ToString("yyyy");
                        break;
                    case GroupByEnum.None:
                        beforeTimeHeaderRenderEventArgs.InnerHTML = string.Empty;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (beforeTimeHeaderRenderEventArgs.ToolTip == string.Empty)
                {
                    beforeTimeHeaderRenderEventArgs.ToolTip = beforeTimeHeaderRenderEventArgs.InnerHTML;
                }
                if (this.BeforeTimeHeaderRender != null)
                {
                    this.BeforeTimeHeaderRender(this, beforeTimeHeaderRenderEventArgs);
                }
                int num5 = num3;
                output.AddAttribute("colspan", num5.ToString());
                output.AddStyleAttribute("height", this.HeaderHeight - 1 + "px");
                output.AddStyleAttribute("text-align", "center");
                output.AddStyleAttribute("background-color", beforeTimeHeaderRenderEventArgs.BackgroundColor);
                output.AddStyleAttribute("font-family", this.HourFontFamily);
                output.AddStyleAttribute("font-size", this.HourFontSize);
                output.AddStyleAttribute("color", ColorTranslator.ToHtml(this.HeaderFontColor));
                output.AddAttribute("unselectable", "on");
                output.AddStyleAttribute("-khtml-user-select", "none");
                output.AddStyleAttribute("-moz-user-select", "none");
                output.AddStyleAttribute("white-space", "nowrap");
                output.AddStyleAttribute("overflow", "hidden");
                output.AddStyleAttribute("width", num4 + "px");
                output.AddStyleAttribute("cursor", "default");
                output.AddAttribute("class", this.ApplyCssClass("timeheadergroup"));
                output.RenderBeginTag("td");
                output.Write(string.Concat(new object[]
                {
                    "<div unselectable='on' style='height:",
                    this.HeaderHeight - 1,
                    "px; border-right: 1px solid ",
                    ColorTranslator.ToHtml(this.HourNameBorderColor),
                    ";overflow:hidden;width:",
                    num4 - 1,
                    "px; vertical-align:center;' title='",
                    beforeTimeHeaderRenderEventArgs.ToolTip,
                    "'>"
                }));
                output.Write(beforeTimeHeaderRenderEventArgs.InnerHTML);
                output.Write("</div>");
                output.RenderEndTag();
                timeSpan -= dateTime3 - dateTime2;
            }
        }

        private void LoadRows()
        {
            this.Rows = new List<Day>();
            if (this.ViewType != ViewTypeEnum.Resources)
            {
                if (this.ViewType == ViewTypeEnum.Gantt)
                {
                    using (IEnumerator enumerator = this.Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Event @event = (Event)enumerator.Current;
                            DateTime startDate = this.StartDate;
                            Day day = new Day(startDate, startDate.AddDays((double)this.Days), @event.Text, @event.Id, this.CellDuration, @event.Text, this.SortFields.Directions);
                            day.IsGantt = true;
                            day.DataItem = new DataItemWrapper(@event.Source);
                            this.Rows.Add(day);
                        }
                        return;
                    }
                }
                for (int i = 0; i < this.Days; i++)
                {
                    DateTime start = this.StartDate.AddDays((double)i);
                    Day item = new Day(start, start.AddDays(1.0), start.ToString(this.HeaderDateFormat), null, this.CellDuration, start.ToString(this.HeaderDateFormat), this.SortFields.Directions);
                    this.Rows.Add(item);
                }
                return;
            }
            if (this.Resources == null)
            {
                return;
            }
            this.LoadResourcesTree(this.TreeEnabled);
        }

        private void LoadEventsToDays()
        {
            if (this.Rows == null)
            {
                throw new Exception("Days must be initialized before calling loadEventsToDays().");
            }
            this.Items.Sort(new EventComparer());
            foreach (Day current in this.Rows)
            {
                current.UseEventBoxes = this.UseEventBoxes;
                current.Load(this.Items);
            }
        }

        private void LoadResourcesTree(bool recursively)
        {
            int num = 0;
            this.LoadResources(this.Resources, ref num, 0, null, recursively, false);
        }

        private void LoadResources(ResourceCollection resources, ref int i, int level, TreeDay parent, bool recursively, bool hidden)
        {
            if (resources == null)
            {
                return;
            }
            foreach (Resource resource in resources)
            {
                TreeDay treeDay = new TreeDay(resource, this.StartDate, this.EndDate, this.CellDuration, this.SortFields.Directions);
                treeDay.columns = resource.Columns;
                treeDay.loaded = !resource.DynamicChildren;
                treeDay.level = level;
                treeDay.index = i;
                treeDay.hidden = hidden;
                treeDay.DataItem = new DataItemWrapper(resource.DataItem);
                this.Rows.Add(treeDay);
                if (parent != null)
                {
                    parent.children.Add(i);
                }
                i++;
                if (recursively)
                {
                    bool hidden2 = hidden || !treeDay.expanded;
                    this.LoadResources(resource.Children, ref i, level + 1, treeDay, true, hidden2);
                }
            }
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string ea)
        {
            this._callbackException = null;
            try
            {
                string a = ea.Substring(0, 4);
                if (a == "LZJB")
                {
                    string s = ea.Substring(4);
                    byte[] input = Convert.FromBase64String(s);
                    byte[] bytes = Lzjb.Decompress(input);
                    string @string = Encoding.UTF8.GetString(bytes);
                    this.ExecuteEventJSON(@string);
                }
                else
                {
                    if (!(a == "JSON"))
                    {
                        throw new Exception("Unsupported CallBack data format.");
                    }
                    this.ExecuteEventJSON(ea.Substring(4));
                }
            }
            catch (Exception callbackException)
            {
                this._callbackException = callbackException;
                throw;
            }
        }

        string ICallbackEventHandler.GetCallbackResult()
        {
            if (this._callbackException == null)
            {
                string result;
                try
                {
                    this.LoadRows();
                    Hashtable hashtable = new Hashtable();
                    hashtable["CallBackData"] = this.CallbackData;
                    hashtable["Message"] = this._callbackMessage;
                    hashtable["ClientState"] = this.ClientState;
                    hashtable["Action"] = this._callbackAction;
                    List<JsonData> selectedEvents = this.GetSelectedEvents();
                    hashtable["SelectedEvents"] = selectedEvents;
                    bool flag = false;
                    if (this._callbackUpdateType == CallBackUpdateType.None)
                    {
                        flag = true;
                    }
                    if (this._callbackUpdateType == CallBackUpdateType.Auto && this._callbackSource == EventSource.Notify)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        hashtable["UpdateType"] = "None";
                        result = SimpleJsonSerializer.Serialize(hashtable);
                    }
                    else
                    {
                        hashtable["Days"] = this.Days;
                        hashtable["CellDuration"] = this.CellDuration;
                        hashtable["CellGroupBy"] = this.CellGroupBy;
                        hashtable["CellWidth"] = this.CellWidth;
                        hashtable["StartDate"] = this.StartDate.ToString("s");
                        if (this._callbackUpdateType == CallBackUpdateType.Auto || this._callbackUpdateType == CallBackUpdateType.Full)
                        {
                            this.TimeHeader.PrepareTimeline();
                            this.TimeHeader.PrepareGrouplines();
                            CellTable cellTable = new CellTable(this);
                            cellTable.Process();
                            Hashtable hashtable2 = new Hashtable();
                            hashtable2["ScrollX"] = this.ScrollX;
                            hashtable2["ScrollY"] = this.ScrollY;
                            hashtable2["Separators"] = this.Separators.GetList();
                            hashtable2["CellProperties"] = cellTable.GetProperties();
                            hashtable2["CellConfig"] = cellTable.GetConfig();
                            hashtable2["Resources"] = this.GetResources();
                            hashtable2["Links"] = this.GetLinksJson();
                            hashtable2["TimeHeader"] = this.TimeHeader.GetList();
                            hashtable2["TimeHeaders"] = this.TimeHeadersResolved.ToJson();
                            hashtable2["Timeline"] = this.TimeHeader.Timeline.ToJson();
                            hashtable2["CornerHTML"] = this.CornerHtml;
                            hashtable2["CornerBackColor"] = this.CornerBackColor;
                            hashtable2["RowHeaderCols"] = this.RowHeaderColumns;
                            hashtable2["RowHeaderColumns"] = this.HeaderColumns.GetList();
                            Hashtable hashtable3 = new Hashtable();
                            hashtable3["separators"] = this.Separators.GetHash();
                            hashtable3["colors"] = cellTable.GetHash();
                            hashtable3["timeHeader"] = this.TimeHeader.GetHash();
                            hashtable3["corner"] = this.CornerHash(this.CornerHtml, this.CornerBackColor);
                            hashtable3["callBack"] = this.CallBack.GetHash();
                            hashtable3["headerColumns"] = this.HeaderColumns.GetHash();
                            hashtable3["links"] = this.GetLinksHash();
                            hashtable2["Hashes"] = hashtable3;
                            if (this._callbackUpdateType == CallBackUpdateType.Auto)
                            {
                                if (this._callbackAction == "Scroll")
                                {
                                    this._callbackUpdateType = CallBackUpdateType.EventsOnly;
                                }
                                else
                                {
                                    this._callbackUpdateType = (this.DifferentHashes(hashtable3) ? CallBackUpdateType.Full : CallBackUpdateType.EventsOnly);
                                }
                            }
                            if (this._callbackUpdateType == CallBackUpdateType.Full)
                            {
                                foreach (string key in hashtable2.Keys)
                                {
                                    hashtable[key] = hashtable2[key];
                                }
                            }
                        }
                        hashtable["UpdateType"] = this._callbackUpdateType.ToString();
                        if (this.EnableViewState)
                        {
                            using (StringWriter stringWriter = new StringWriter())
                            {
                                LosFormatter losFormatter = new LosFormatter();
                                losFormatter.Serialize(stringWriter, ViewStateHelper.ToHashtable(this.ViewState));
                                hashtable["VsUpdate"] = stringWriter.ToString();
                            }
                        }
                        hashtable["Events"] = this.GetEvents();
                        result = SimpleJsonSerializer.Serialize(hashtable);
                    }
                }
                catch (Exception ex)
                {
                    if (HttpContext.Current.IsDebuggingEnabled)
                    {
                        result = "$$$" + ex;
                    }
                    else
                    {
                        result = "$$$" + ex.Message;
                    }
                }
                return result;
            }
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                return "$$$" + this._callbackException;
            }
            return "$$$" + this._callbackException.Message;
        }

        private object GetLinksHash()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(this.GetLinksJson()));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }

        private bool DifferentHashes(Hashtable hashes)
        {
            bool result = false;
            foreach (string key in hashes.Keys)
            {
                if ((string)hashes[key] != (string)this._hashes[key])
                {
                    result = true;
                }
            }
            return result;
        }

        internal string CornerHash(string html, string back)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["html"] = html;
            hashtable["back"] = back;
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(hashtable));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }

        internal ArrayList GetSelectedRows()
        {
            ArrayList arrayList = new ArrayList();
            foreach (RowInfo current in this.SelectedRows)
            {
                arrayList.Add(current.Id);
            }
            return arrayList;
        }

        internal ArrayList GetEvents()
        {
            ArrayList arrayList = new ArrayList();
            if (this.Items != null)
            {
                foreach (Event e in this.Items)
                {
                    arrayList.Add(this.GetEventMap(e));
                }
            }
            return arrayList;
        }

        private Hashtable GetEventMap(Event e)
        {
            BeforeEventRenderEventArgs eva = this.GetEva(e);
            Hashtable hashtable = new Hashtable();
            hashtable["id"] = e.Id;
            hashtable["text"] = e.Text;
            hashtable["start"] = e.Start.ToString("s");
            hashtable["end"] = e.End.ToString("s");
            hashtable["resource"] = e.ResourceId;
            hashtable["tag"] = e.Tags;
            if (e.Sort != null && e.Sort.Length > 0)
            {
                hashtable["sort"] = e.Sort;
            }
            if (e.Recurrent)
            {
                hashtable["recurrent"] = e.Recurrent;
                hashtable["recurrentMasterId"] = e.RecurrentMasterId;
            }
            if (this.DurationBarMode == DurationBarMode.PercentComplete)
            {
                hashtable["complete"] = eva.PercentComplete;
            }
            if (eva.Areas.Count > 0)
            {
                hashtable["areas"] = eva.Areas.GetList();
            }
            if (!string.IsNullOrEmpty(eva.BubbleHtml))
            {
                hashtable["bubbleHtml"] = eva.BubbleHtml;
            }
            if (eva.Html != e.Text)
            {
                hashtable["html"] = eva.Html;
            }
            if (eva.ToolTip != e.Text)
            {
                hashtable["toolTip"] = eva.ToolTip;
            }
            if (!string.IsNullOrEmpty(eva.BackgroundColor) && eva.BackgroundColor != ColorTranslator.ToHtml(this.EventBackColor))
            {
                hashtable["backColor"] = eva.BackgroundColor;
            }
            if (!string.IsNullOrEmpty(eva.DurationBarColor) && eva.DurationBarColor != ColorTranslator.ToHtml(this.DurationBarColor))
            {
                hashtable["barColor"] = eva.DurationBarColor;
            }
            if (!string.IsNullOrEmpty(eva.DurationBarBackColor) && eva.DurationBarBackColor != ColorTranslator.ToHtml(this.DurationBarBackColor))
            {
                hashtable["barBackColor"] = eva.DurationBarBackColor;
            }
            if (this.DurationBarVisible && !eva.DurationBarVisible)
            {
                hashtable["barHidden"] = true;
            }
            if (!string.IsNullOrEmpty(eva.DurationBarImageUrl))
            {
                hashtable["barImageUrl"] = eva.DurationBarImageUrl;
            }
            if (this.ContextMenuEvent == null)
            {
                if (!string.IsNullOrEmpty(eva.ContextMenuClientName))
                {
                    hashtable["contextMenu"] = eva.ContextMenuClientName;
                }
            }
            else if (this.ContextMenuEvent.ClientObjectName != eva.ContextMenuClientName)
            {
                hashtable["contextMenu"] = eva.ContextMenuClientName;
            }
            if (!string.IsNullOrEmpty(eva.CssClass))
            {
                hashtable["cssClass"] = eva.CssClass;
            }
            if (!string.IsNullOrEmpty(eva.BackgroundImage))
            {
                hashtable["backImage"] = eva.BackgroundImage;
            }
            if (!string.IsNullOrEmpty(eva.BackgroundRepeat))
            {
                hashtable["backRepeat"] = eva.BackgroundRepeat;
            }
            if (!string.IsNullOrEmpty(eva.BorderColor))
            {
                hashtable["borderColor"] = eva.BorderColor;
            }
            if (!string.IsNullOrEmpty(eva.FontColor))
            {
                hashtable["fontColor"] = eva.FontColor;
            }
            if (!eva.EventClickEnabled)
            {
                hashtable["clickDisabled"] = true;
            }
            if (!eva.EventDoubleClickEnabled)
            {
                hashtable["doubleClickDisabled"] = true;
            }
            if (!eva.EventMoveEnabled)
            {
                hashtable["moveDisabled"] = true;
            }
            if (!eva.EventMoveVerticalEnabled)
            {
                hashtable["moveVDisabled"] = true;
            }
            if (!eva.EventMoveHorizontalEnabled)
            {
                hashtable["moveHDisabled"] = true;
            }
            if (!eva.EventResizeEnabled)
            {
                hashtable["resizeDisabled"] = true;
            }
            if (!eva.EventRightClickEnabled)
            {
                hashtable["rightClickDisabled"] = true;
            }
            return hashtable;
        }

        private List<Hashtable> GetEventAreas()
        {
            List<Hashtable> list = new List<Hashtable>();
            Hashtable hashtable = new Hashtable();
            hashtable["w"] = 17;
            hashtable["h"] = 17;
            hashtable["className"] = "action";
            hashtable["right"] = 3;
            hashtable["top"] = 3;
            hashtable["visibility"] = "hover";
            hashtable["handling"] = "ContextMenu";
            hashtable["id"] = "testing";
            hashtable["menu"] = "cmSpecial";
            list.Add(hashtable);
            return list;
        }

        internal List<Hashtable> GetResources()
        {
            if (this.resourcesCache == null)
            {
                this.resourcesCache = this.GetResourcesCache();
            }
            return this.resourcesCache;
        }

        private List<Hashtable> GetResourcesCache()
        {
            switch (this.ViewType)
            {
                case ViewTypeEnum.Days:
                    return this.GetResourcesDaysView();
                case ViewTypeEnum.Resources:
                    return this.GetResources(this.Resources);
                case ViewTypeEnum.Gantt:
                    return this.GetResourcesGanttView();
                default:
                    throw new Exception("Unrecognized ViewType value.");
            }
        }

        internal List<Hashtable> GetResourcesDaysView()
        {
            List<Hashtable> list = new List<Hashtable>();
            for (int i = 0; i < this.Days; i++)
            {
                DateTime d = this.StartDate.AddDays((double)i);
                BeforeResHeaderRenderEventArgs resourceHeaderDaysView = this.GetResourceHeaderDaysView(d);
                Hashtable hashtable = new Hashtable();
                hashtable["id"] = resourceHeaderDaysView.Id;
                hashtable["start"] = d.ToString("s");
                hashtable["html"] = resourceHeaderDaysView.Html;
                hashtable["toolTip"] = resourceHeaderDaysView.ToolTip;
                if (resourceHeaderDaysView.BackgroundColor != ColorTranslator.ToHtml(this.HourNameBackColor))
                {
                    hashtable["backColor"] = resourceHeaderDaysView.BackgroundColor;
                }
                if (this.ContextMenuResource != null && resourceHeaderDaysView.ContextMenuClientName != this.ContextMenuResource.ClientObjectName)
                {
                    hashtable["contextMenu"] = resourceHeaderDaysView.ContextMenuClientName;
                }
                if (!string.IsNullOrEmpty(resourceHeaderDaysView.CssClass))
                {
                    hashtable["cssClass"] = resourceHeaderDaysView.CssClass;
                }
                if (resourceHeaderDaysView.Columns.Count > 0)
                {
                    List<Hashtable> list2 = new List<Hashtable>();
                    foreach (ResourceColumn current in resourceHeaderDaysView.Columns)
                    {
                        Hashtable hashtable2 = new Hashtable();
                        hashtable2["html"] = current.Html;
                        list2.Add(hashtable2);
                    }
                    hashtable["columns"] = list2;
                }
                list.Add(hashtable);
            }
            return list;
        }

        internal List<Hashtable> GetResourcesGanttView()
        {
            ResourceCollection resourceCollection = new ResourceCollection();
            if (this.Items != null)
            {
                foreach (Event @event in this.Items)
                {
                    resourceCollection.Add(new Resource
                    {
                        Name = @event.Text,
                        Id = @event.Id,
                        DataItem = @event.Source
                    });
                }
            }
            return this.GetResources(resourceCollection);
        }

        internal List<Hashtable> GetResources(ResourceCollection resources)
        {
            List<Hashtable> list = new List<Hashtable>();
            foreach (Resource resource in resources)
            {
                BeforeResHeaderRenderEventArgs resourceHeader = this.GetResourceHeader(resource);
                Hashtable hashtable = new Hashtable();
                hashtable["id"] = resource.Id;
                hashtable["name"] = resource.Name;
                hashtable["html"] = resourceHeader.Html;
                hashtable["areas"] = resourceHeader.Areas.GetList();
                if (!resourceHeader.MoveEnabled)
                {
                    hashtable["moveDisabled"] = true;
                }
                if (!string.IsNullOrEmpty(resourceHeader.ToolTip))
                {
                    hashtable["toolTip"] = resourceHeader.ToolTip;
                }
                if (resourceHeader.BackgroundColor != ColorTranslator.ToHtml(this.HourNameBackColor))
                {
                    hashtable["backColor"] = resourceHeader.BackgroundColor;
                }
                if (!string.IsNullOrEmpty(resourceHeader.CssClass))
                {
                    hashtable["cssClass"] = resourceHeader.CssClass;
                }
                if (this.ContextMenuResource != null && resourceHeader.ContextMenuClientName != this.ContextMenuResource.ClientObjectName)
                {
                    hashtable["contextMenu"] = resourceHeader.ContextMenuClientName;
                }
                if (resource.DynamicChildren)
                {
                    hashtable["dynamicChildren"] = true;
                }
                if (resourceHeader.MinHeight != this.RowMinHeight)
                {
                    hashtable["minHeight"] = resourceHeader.MinHeight;
                }
                if (resourceHeader.MarginBottom != this.RowMarginBottom)
                {
                    hashtable["marginBottom"] = resourceHeader.MarginBottom;
                }
                if (resourceHeader.EventHeight != this.EventHeight)
                {
                    hashtable["eventHeight"] = resourceHeader.EventHeight;
                }
                if (resource.Expanded)
                {
                    hashtable["expanded"] = true;
                }
                if (resource.IsParent)
                {
                    hashtable["isParent"] = true;
                }
                if (resource.Children.Count > 0)
                {
                    hashtable["children"] = this.GetResources(resource.Children);
                }
                if (resourceHeader.Columns.Count > 0)
                {
                    List<Hashtable> list2 = new List<Hashtable>();
                    foreach (ResourceColumn current in resourceHeader.Columns)
                    {
                        Hashtable hashtable2 = new Hashtable();
                        if (!string.IsNullOrEmpty(current.ImageUrl))
                        {
                            string arg = this.ResolveUrlSafe(current.ImageUrl);
                            hashtable2["html"] = string.Format("<img src='{0}' />", arg);
                        }
                        else
                        {
                            hashtable2["html"] = current.Html;
                        }
                        list2.Add(hashtable2);
                    }
                    hashtable["columns"] = list2;
                }
                list.Add(hashtable);
            }
            return list;
        }

        private List<string> decodeResourcesFromJson(JsonData array)
        {
            List<string> list = new List<string>();
            if (!array.IsArray)
            {
                return list;
            }
            int count = array.Count;
            for (int i = 0; i < count; i++)
            {
                list.Add((string)array[i]);
            }
            return list;
        }

        private void ExecuteEventJSON(string ea)
        {
            JsonData jsonData = SimpleJsonDeserializer.Deserialize(ea);
            JsonData jsonData2 = jsonData["header"];
            JsonData jsonData3 = jsonData["data"];
            JsonData parameters = jsonData["parameters"];
            this._callbackSource = EventSourceParser.Parse((string)jsonData["type"]);
            EventSource callbackSource = this._callbackSource;
            this.StartDate = (DateTime)jsonData2["startDate"];
            this.Days = (int)jsonData["header"]["days"];
            this.CellDuration = (int)jsonData2["cellDuration"];
            this.CellGroupBy = GroupByEnumParser.Parse((string)jsonData2["cellGroupBy"]);
            this.CellWidth = (int)jsonData2["cellWidth"];
            this._clientState = jsonData2["clientState"];
            this.ScrollX = (int)jsonData2["scrollX"];
            this.ScrollY = (int)jsonData2["scrollY"];
            this.SelectedEvents = EventInfo.ListFromJson(jsonData2["selected"]);
            this.SelectedRows = RowInfo.ListFromJson(this, jsonData2["selectedRows"]);
            this._viewPort = new Section();
            this._viewPort.start = (DateTime)jsonData2["rangeStart"];
            this._viewPort.end = (DateTime)jsonData2["rangeEnd"];
            this._viewPort.resources = this.decodeResourcesFromJson(jsonData2["resources"]);
            this._hashes = new Hashtable();
            this._hashes["separators"] = (string)jsonData2["hashes"]["separators"];
            this._hashes["colors"] = (string)jsonData2["hashes"]["colors"];
            this._hashes["timeHeader"] = (string)jsonData2["hashes"]["timeHeader"];
            this._hashes["corner"] = (string)jsonData2["hashes"]["corner"];
            this._hashes["headerColumns"] = (string)jsonData2["hashes"]["headerColumns"];
            this._hashes["callBack"] = (string)jsonData2["hashes"]["callBack"];
            if (this.SyncResourceTree)
            {
                this.Resources.RestoreFromJson(jsonData2["tree"]);
            }
            if (this.SyncLinks)
            {
                this.Links.RestoreFromJson(jsonData2["links"]);
            }
            this.TimeHeaders.RestoreFromJson(jsonData2["timeHeaders"]);
            this.HeaderColumns.RestoreFromJson(jsonData2["rowHeaderColumns"]);
            this._callbackAction = (string)jsonData["action"];
            string callbackAction;
            if ((callbackAction = this._callbackAction) != null)
            {
                if (< PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x6000cde - 1 == null)
				{

                    < PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x6000cde - 1 = new Dictionary<string, int>(28)
                    {
                        {
                            "Scroll",
                            0
                        },
                        {
                            "Command",
                            1
                        },
                        {
                            "EventClick",
                            2
                        },
                        {
                            "EventDelete",
                            3
                        },
                        {
                            "EventMove",
                            4
                        },
                        {
                            "EventResize",
                            5
                        },
                        {
                            "EventRightClick",
                            6
                        },
                        {
                            "EventDoubleClick",
                            7
                        },
                        {
                            "EventSelect",
                            8
                        },
                        {
                            "EventMenuClick",
                            9
                        },
                        {
                            "TimeRangeSelected",
                            10
                        },
                        {
                            "TimeRangeDoubleClick",
                            11
                        },
                        {
                            "TimeRangeMenuClick",
                            12
                        },
                        {
                            "RowMenuClick",
                            13
                        },
                        {
                            "RowClick",
                            14
                        },
                        {
                            "RowSelect",
                            15
                        },
                        {
                            "RowCreate",
                            16
                        },
                        {
                            "RowEdit",
                            17
                        },
                        {
                            "RowMove",
                            18
                        },
                        {
                            "TimeHeaderClick",
                            19
                        },
                        {
                            "ResourceCollapse",
                            20
                        },
                        {
                            "ResourceExpand",
                            21
                        },
                        {
                            "EventEdit",
                            22
                        },
                        {
                            "LoadNode",
                            23
                        },
                        {
                            "Notify",
                            24
                        },
                        {
                            "EventUpdate",
                            25
                        },
                        {
                            "EventRemove",
                            26
                        },
                        {
                            "EventAdd",
                            27
                        }
                    };
                }
                int num;
                if (< PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x6000cde - 1.TryGetValue(callbackAction, out num))
				{
                    switch (num)
                    {
                        case 0:
                            if (this.Scroll != null)
                            {
                                ScrollEventArgs scrollEventArgs = new ScrollEventArgs();
                                scrollEventArgs.Source = callbackSource;
                                this.Scroll(this, scrollEventArgs);
                                return;
                            }
                            break;
                        case 1:
                            if (this.Command != null)
                            {
                                HMS.Web.App.App.Ui.Events.CommandEventArgs commandEventArgs = new HMS.Web.App.App.Ui.Events.CommandEventArgs(parameters, jsonData3);
                                commandEventArgs.Source = callbackSource;
                                this.Command(this, commandEventArgs);
                                return;
                            }
                            break;
                        case 2:
                            if (this.EventClick != null)
                            {
                                EventClickEventArgs eventClickEventArgs = new EventClickEventArgs(parameters, jsonData3);
                                eventClickEventArgs.Source = callbackSource;
                                this.EventClick(this, eventClickEventArgs);
                                return;
                            }
                            break;
                        case 3:
                            if (this.EventDelete != null)
                            {
                                EventDeleteEventArgs eventDeleteEventArgs = new EventDeleteEventArgs(parameters, jsonData3);
                                eventDeleteEventArgs.Source = callbackSource;
                                this.EventDelete(this, eventDeleteEventArgs);
                                return;
                            }
                            break;
                        case 4:
                            if (this.EventMove != null)
                            {
                                EventMoveEventArgs eventMoveEventArgs = new EventMoveEventArgs(parameters, jsonData3);
                                eventMoveEventArgs.Source = callbackSource;
                                this.EventMove(this, eventMoveEventArgs);
                                return;
                            }
                            break;
                        case 5:
                            if (this.EventResize != null)
                            {
                                EventResizeEventArgs eventResizeEventArgs = new EventResizeEventArgs(parameters, jsonData3);
                                eventResizeEventArgs.Source = callbackSource;
                                this.EventResize(this, eventResizeEventArgs);
                                return;
                            }
                            break;
                        case 6:
                            if (this.EventRightClick != null)
                            {
                                EventRightClickEventArgs eventRightClickEventArgs = new EventRightClickEventArgs(parameters, jsonData3);
                                eventRightClickEventArgs.Source = callbackSource;
                                this.EventRightClick(this, eventRightClickEventArgs);
                                return;
                            }
                            break;
                        case 7:
                            if (this.EventDoubleClick != null)
                            {
                                EventClickEventArgs eventClickEventArgs2 = new EventClickEventArgs(parameters, jsonData3);
                                eventClickEventArgs2.Source = callbackSource;
                                this.EventDoubleClick(this, eventClickEventArgs2);
                                return;
                            }
                            break;
                        case 8:
                            if (this.EventSelect != null)
                            {
                                EventSelectEventArgs eventSelectEventArgs = new EventSelectEventArgs(parameters, jsonData3);
                                eventSelectEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventSelect(this, eventSelectEventArgs);
                                return;
                            }
                            break;
                        case 9:
                            if (this.EventMenuClick != null)
                            {
                                EventMenuClickEventArgs eventMenuClickEventArgs = new EventMenuClickEventArgs(parameters, jsonData3);
                                eventMenuClickEventArgs.Source = callbackSource;
                                this.EventMenuClick(this, eventMenuClickEventArgs);
                                return;
                            }
                            break;
                        case 10:
                            if (this.TimeRangeSelected != null)
                            {
                                TimeRangeSelectedEventArgs timeRangeSelectedEventArgs = new TimeRangeSelectedEventArgs(parameters, jsonData3);
                                timeRangeSelectedEventArgs.Source = callbackSource;
                                this.TimeRangeSelected(this, timeRangeSelectedEventArgs);
                                return;
                            }
                            break;
                        case 11:
                            if (this.TimeRangeDoubleClick != null)
                            {
                                TimeRangeDoubleClickEventArgs timeRangeDoubleClickEventArgs = new TimeRangeDoubleClickEventArgs(parameters, jsonData3);
                                timeRangeDoubleClickEventArgs.Source = callbackSource;
                                this.TimeRangeDoubleClick(this, timeRangeDoubleClickEventArgs);
                                return;
                            }
                            break;
                        case 12:
                            if (this.TimeRangeMenuClick != null)
                            {
                                TimeRangeMenuClickEventArgs timeRangeMenuClickEventArgs = new TimeRangeMenuClickEventArgs(parameters, jsonData3);
                                timeRangeMenuClickEventArgs.Source = callbackSource;
                                this.TimeRangeMenuClick(this, timeRangeMenuClickEventArgs);
                                return;
                            }
                            break;
                        case 13:
                            if (this.RowMenuClick != null)
                            {
                                RowMenuClickEventArgs rowMenuClickEventArgs = new RowMenuClickEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    rowMenuClickEventArgs.resource = this.Resources.FindByIndex(rowMenuClickEventArgs.Index);
                                }
                                rowMenuClickEventArgs.Source = callbackSource;
                                this.RowMenuClick(this, rowMenuClickEventArgs);
                            }
                            if (this.ResourceHeaderMenuClick != null)
                            {
                                ResourceHeaderMenuClickEventArgs resourceHeaderMenuClickEventArgs = new ResourceHeaderMenuClickEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    resourceHeaderMenuClickEventArgs.resource = this.Resources.FindByIndex(resourceHeaderMenuClickEventArgs.Index);
                                }
                                resourceHeaderMenuClickEventArgs.Source = callbackSource;
                                this.ResourceHeaderMenuClick(this, resourceHeaderMenuClickEventArgs);
                                return;
                            }
                            break;
                        case 14:
                            if (this.RowClick != null)
                            {
                                RowClickEventArgs rowClickEventArgs = new RowClickEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    rowClickEventArgs.resource = this.Resources.FindByIndex(rowClickEventArgs.Index);
                                }
                                rowClickEventArgs.Source = callbackSource;
                                this.RowClick(this, rowClickEventArgs);
                            }
                            if (this.ResourceHeaderClick != null)
                            {
                                ResourceHeaderClickEventArgs resourceHeaderClickEventArgs = new ResourceHeaderClickEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    resourceHeaderClickEventArgs.resource = this.Resources.FindByIndex(resourceHeaderClickEventArgs.Index);
                                }
                                resourceHeaderClickEventArgs.Source = callbackSource;
                                this.ResourceHeaderClick(this, resourceHeaderClickEventArgs);
                                return;
                            }
                            break;
                        case 15:
                            if (this.RowSelect != null)
                            {
                                RowSelectEventArgs rowSelectEventArgs = new RowSelectEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    rowSelectEventArgs.resource = this.Resources.FindByIndex(rowSelectEventArgs.Index);
                                }
                                rowSelectEventArgs.Source = callbackSource;
                                this.RowSelect(this, rowSelectEventArgs);
                                return;
                            }
                            break;
                        case 16:
                            if (this.RowCreate != null)
                            {
                                RowCreateEventArgs rowCreateEventArgs = new RowCreateEventArgs(parameters, jsonData3);
                                rowCreateEventArgs.Source = callbackSource;
                                this.RowCreate(this, rowCreateEventArgs);
                                return;
                            }
                            break;
                        case 17:
                            if (this.RowEdit != null)
                            {
                                RowEditEventArgs rowEditEventArgs = new RowEditEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    rowEditEventArgs.Resource = this.Resources.FindByIndex(rowEditEventArgs.Index);
                                }
                                rowEditEventArgs.Source = callbackSource;
                                this.RowEdit(this, rowEditEventArgs);
                                return;
                            }
                            break;
                        case 18:
                            if (this.RowMove != null)
                            {
                                RowMoveEventArgs rowMoveEventArgs = new RowMoveEventArgs(parameters, jsonData3, this.Resources);
                                rowMoveEventArgs.EventSource = callbackSource;
                                this.RowMove(this, rowMoveEventArgs);
                                return;
                            }
                            break;
                        case 19:
                            if (this.TimeHeaderClick != null)
                            {
                                TimeHeaderClickEventArgs timeHeaderClickEventArgs = new TimeHeaderClickEventArgs(parameters, jsonData3);
                                timeHeaderClickEventArgs.Source = callbackSource;
                                this.TimeHeaderClick(this, timeHeaderClickEventArgs);
                                return;
                            }
                            break;
                        case 20:
                            if (this.ResourceCollapse != null)
                            {
                                ResourceCollapseEventArgs resourceCollapseEventArgs = new ResourceCollapseEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    resourceCollapseEventArgs.resource = this.Resources.FindByIndex(resourceCollapseEventArgs.Index);
                                }
                                resourceCollapseEventArgs.Source = callbackSource;
                                this.ResourceCollapse(this, resourceCollapseEventArgs);
                                return;
                            }
                            break;
                        case 21:
                            if (this.ResourceExpand != null)
                            {
                                ResourceExpandEventArgs resourceExpandEventArgs = new ResourceExpandEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    resourceExpandEventArgs.resource = this.Resources.FindByIndex(resourceExpandEventArgs.Index);
                                }
                                resourceExpandEventArgs.Source = callbackSource;
                                this.ResourceExpand(this, resourceExpandEventArgs);
                                return;
                            }
                            break;
                        case 22:
                            if (this.EventEdit != null)
                            {
                                EventEditEventArgs eventEditEventArgs = new EventEditEventArgs(parameters, jsonData3);
                                eventEditEventArgs.Source = callbackSource;
                                this.EventEdit(this, eventEditEventArgs);
                                return;
                            }
                            break;
                        case 23:
                            if (this.LoadNode != null)
                            {
                                LoadNodeEventArgs loadNodeEventArgs = new LoadNodeEventArgs(parameters, jsonData3);
                                if (this.Resources != null)
                                {
                                    loadNodeEventArgs.resource = this.Resources.FindByIndex(loadNodeEventArgs.Index);
                                }
                                loadNodeEventArgs.Source = callbackSource;
                                this.LoadNode(this, loadNodeEventArgs);
                                if (loadNodeEventArgs.Resource != null)
                                {
                                    loadNodeEventArgs.Resource.DynamicChildren = false;
                                    return;
                                }
                            }
                            break;
                        case 24:
                            if (this.Notify != null)
                            {
                                NotifyEventArgs notifyEventArgs = new NotifyEventArgs(parameters, jsonData3);
                                notifyEventArgs.Source = EventSource.Notify;
                                this.Notify(this, notifyEventArgs);
                                return;
                            }
                            break;
                        case 25:
                            if (this.EventUpdate != null)
                            {
                                EventUpdateEventArgs eventUpdateEventArgs = new EventUpdateEventArgs(parameters, jsonData3);
                                eventUpdateEventArgs.Source = callbackSource;
                                this.EventUpdate(this, eventUpdateEventArgs);
                                return;
                            }
                            break;
                        case 26:
                            if (this.EventRemove != null)
                            {
                                EventRemoveEventArgs eventRemoveEventArgs = new EventRemoveEventArgs(parameters, jsonData3);
                                eventRemoveEventArgs.Source = callbackSource;
                                this.EventRemove(this, eventRemoveEventArgs);
                                return;
                            }
                            break;
                        case 27:
                            if (this.EventAdd != null)
                            {
                                EventAddEventArgs eventAddEventArgs = new EventAddEventArgs(parameters, jsonData3);
                                eventAddEventArgs.Source = callbackSource;
                                this.EventAdd(this, eventAddEventArgs);
                                return;
                            }
                            break;
                        default:
                            goto IL_BA1;
                    }
                    return;
                }
            }
            IL_BA1:
            throw new NotSupportedException("This action type is not supported: " + jsonData["action"]);
        }

        public void SetScrollX(DateTime position)
        {
            this.ScrollDateTime = position;
        }

        public void UpdateWithMessage(string message)
        {
            this._callbackMessage = message;
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        public void Update()
        {
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        public void Update(CallBackUpdateType updateType)
        {
            this._callbackUpdateType = updateType;
        }

        public void Update(object data, CallBackUpdateType updateType)
        {
            this.CallbackData = data;
            this._callbackUpdateType = updateType;
        }

        public void Update(object data)
        {
            this.CallbackData = data;
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        private string[] LoadTagFields(string spec)
        {
            if (string.IsNullOrEmpty(spec))
            {
                return null;
            }
            string[] array = spec.Split(new char[]
            {
                ','
            });
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }
            return array;
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            string a = eventArgument.Substring(0, 4);
            if (a == "JSON")
            {
                this.ExecuteEventJSON(eventArgument.Substring(4));
                return;
            }
            throw new Exception("Unsupported PostBack data format.");
        }

        protected override void PerformSelect()
        {
            if (!base.IsBoundUsingDataSourceID)
            {
                this.OnDataBinding(EventArgs.Empty);
            }
            this.GetData().Select(this.CreateDataSourceSelectArguments(), new DataSourceViewSelectCallback(this.OnDataSourceViewSelectCallback));
            base.RequiresDataBinding = false;
            base.MarkAsDataBound();
            this.OnDataBound(EventArgs.Empty);
        }

        private void OnDataSourceViewSelectCallback(IEnumerable retrievedData)
        {
            if (base.IsBoundUsingDataSourceID)
            {
                this.OnDataBinding(EventArgs.Empty);
            }
            this.PerformDataBinding(retrievedData);
        }

        protected override void PerformDataBinding(IEnumerable retrievedData)
        {
            this.Items = new ArrayList();
            List<RecurEx> list = new List<RecurEx>();
            List<RecurEvent> list2 = new List<RecurEvent>();
            if (base.DesignMode)
            {
                return;
            }
            base.PerformDataBinding(retrievedData);
            if (retrievedData == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(this.DataStartField))
            {
                throw new NullReferenceException("DataStartField property must be specified.");
            }
            if (string.IsNullOrEmpty(this.DataEndField))
            {
                throw new NullReferenceException("DataEndField property must be specified.");
            }
            if (string.IsNullOrEmpty(this.DataTextField))
            {
                throw new NullReferenceException("DataTextField property must be specified.");
            }
            if (string.IsNullOrEmpty(this.DataIdField))
            {
                throw new NullReferenceException("DataValueField property must be specified.");
            }
            if (string.IsNullOrEmpty(this.DataResourceField) && this.ViewType == ViewTypeEnum.Resources)
            {
                throw new NullReferenceException("DataResourceField property must be specified.");
            }
            bool flag = this._callbackAction == "Scroll";
            DateTime t = flag ? this.ViewPort.Start : this.VisibleStart;
            DateTime t2 = flag ? this.ViewPort.End : this.VisibleEnd;
            foreach (object current in retrievedData)
            {
                Event @event = this.ParseDataItem(current);
                string text = null;
                if (!string.IsNullOrEmpty(this.DataRecurrenceField))
                {
                    text = DataBinder.GetPropertyValue(current, this.DataRecurrenceField, null);
                }
                if (string.IsNullOrEmpty(text))
                {
                    if (this.BeforeEventRecurrence != null)
                    {
                        BeforeEventRecurrenceEventArgs beforeEventRecurrenceEventArgs = new BeforeEventRecurrenceEventArgs(@event, this.TagFields);
                        this.BeforeEventRecurrence(beforeEventRecurrenceEventArgs);
                        if (beforeEventRecurrenceEventArgs.Rule != null)
                        {
                            RecurEvent recurEvent = RecurEvent.FromRule(beforeEventRecurrenceEventArgs.Rule, @event.Start, @event.End, @event.Id, @event);
                            recurEvent.FirstDayOfWeek = this.ResolvedWeekStart;
                            list2.Add(recurEvent);
                            continue;
                        }
                    }
                }
                else
                {
                    RecurInfo recurInfo = RecurInfo.Parse(text, @event.Id, @event.Start, @event.End, @event);
                    switch (recurInfo.Type)
                    {
                        case RecurType.Deleted:
                            list.Add((RecurEx)recurInfo);
                            break;
                        case RecurType.Modified:
                            list.Add((RecurEx)recurInfo);
                            break;
                        case RecurType.Event:
                            list2.Add((RecurEvent)recurInfo);
                            break;
                    }
                    if (recurInfo.Type != RecurType.None)
                    {
                        continue;
                    }
                }
                if (!(@event.End <= t) && !(@event.Start >= t2))
                {
                    this.Items.Add(@event);
                }
            }
            foreach (RecurEvent current2 in list2)
            {
                foreach (RecurEx current3 in list)
                {
                    current2.AddRecurexSilent(current3);
                }
                Event event2 = (Event)current2.Tag;
                foreach (Occurrence current4 in current2.Occurrences(this.StartDate.AddDays((double)this.Days)))
                {
                    Event event3 = new Event(current4.Start, current4.End, current4.Id, event2.Text, event2.ResourceId, event2.Tags, event2.ServerTags, event2.AllDay);
                    event3.Source = event2.Source;
                    if (current4.Modified)
                    {
                        event3 = (Event)current4.Tag;
                        string arg_360_0 = event3.RecurrentMasterId;
                    }
                    event3.RecurrentMasterId = event2.Id;
                    event3.Recurrent = true;
                    if (event3.End > this.VisibleStart)
                    {
                        this.Items.Add(event3);
                    }
                }
            }
        }

        private Event ParseDataItem(object dataItem)
        {
            string propertyValue = DataBinder.GetPropertyValue(dataItem, this.DataStartField, null);
            DateTime start;
            if (!DateTime.TryParse(propertyValue, out start))
            {
                throw new FormatException(string.Format("Unable to convert '{0}' (from DataStartField column) to DateTime.", propertyValue));
            }
            string propertyValue2 = DataBinder.GetPropertyValue(dataItem, this.DataEndField, null);
            DateTime end;
            if (!DateTime.TryParse(propertyValue2, out end))
            {
                throw new FormatException(string.Format("Unable to convert '{0}' (from DataEndField column) to DateTime.", propertyValue2));
            }
            string propertyValue3 = DataBinder.GetPropertyValue(dataItem, this.DataTextField, null);
            string propertyValue4 = DataBinder.GetPropertyValue(dataItem, this.DataIdField, null);
            string columnId = null;
            if (this.ViewType == ViewTypeEnum.Resources)
            {
                columnId = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.DataResourceField, null));
            }
            string[] array = null;
            if (this.TagFields != null)
            {
                array = new string[this.TagFields.Length];
                for (int i = 0; i < this.TagFields.Length; i++)
                {
                    array[i] = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.TagFields[i], null));
                }
            }
            string[] array2 = null;
            if (this.ServerTagFields != null)
            {
                array2 = new string[this.ServerTagFields.Length];
                for (int j = 0; j < this.ServerTagFields.Length; j++)
                {
                    array2[j] = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.ServerTagFields[j], null));
                }
            }
            string[] array3 = null;
            if (this.SortFields != null && this.SortFields.Fields.Length > 0)
            {
                array3 = new string[this.SortFields.Fields.Length];
                for (int k = 0; k < this.SortFields.Fields.Length; k++)
                {
                    string text = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.SortFields.Fields[k], null));
                    DateTime dateTime;
                    if (DateTime.TryParse(text, out dateTime))
                    {
                        array3[k] = dateTime.ToString("s");
                    }
                    else
                    {
                        array3[k] = text;
                    }
                }
            }
            return new Event(start, end, propertyValue4, propertyValue3, columnId, array, array2, false)
            {
                Source = dataItem,
                Sort = array3
            };
        }

        private string ApplyCssClass(string part)
        {
            if (!string.IsNullOrEmpty(this.CssClassPrefix))
            {
                return this.CssClassPrefix + part;
            }
            return string.Empty;
        }

        internal CellTable.Cell GetCell(DateTime from, DateTime to, string resourceId)
        {
            BeforeCellRenderEventArgs beforeCellRenderEventArgs = new BeforeCellRenderEventArgs();
            beforeCellRenderEventArgs.Start = from;
            beforeCellRenderEventArgs.End = to;
            beforeCellRenderEventArgs.ResourceId = resourceId;
            bool flag = this.IsBusinessCell(from, to);
            beforeCellRenderEventArgs.IsBusiness = flag;
            string text = null;
            if (!this.CssOnly)
            {
                if (beforeCellRenderEventArgs.IsBusiness)
                {
                    text = ColorTranslator.ToHtml(this.BackColor);
                }
                else
                {
                    text = ColorTranslator.ToHtml(this.NonBusinessBackColor);
                }
            }
            beforeCellRenderEventArgs.BackgroundColor = text;
            if (this.BeforeCellRender != null)
            {
                this.BeforeCellRender(this, beforeCellRenderEventArgs);
                if (!this.CssOnly && beforeCellRenderEventArgs.BackgroundColor == text && beforeCellRenderEventArgs.IsBusiness != flag)
                {
                    if (beforeCellRenderEventArgs.IsBusiness)
                    {
                        beforeCellRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.BackColor);
                    }
                    else
                    {
                        beforeCellRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.NonBusinessBackColor);
                    }
                }
            }
            return new CellTable.Cell
            {
                Color = beforeCellRenderEventArgs.BackgroundColor,
                InnerHtml = beforeCellRenderEventArgs.Html,
                BackgroundImage = this.ResolveUrlSafe(beforeCellRenderEventArgs.BackgroundImage),
                BackgroundRepeat = beforeCellRenderEventArgs.BackgroundRepeat,
                CssClass = beforeCellRenderEventArgs.CssClass,
                IsBusiness = beforeCellRenderEventArgs.IsBusiness
            };
        }

        private bool IsBusinessCell(DateTime from, DateTime to)
        {
            int num = (int)(to - from).TotalMinutes;
            return to - from > TimeSpan.FromDays(1.0) || (from.DayOfWeek != DayOfWeek.Saturday && from.DayOfWeek != DayOfWeek.Sunday && (num >= 720 || (from.Hour >= this.BusinessBeginsHour && from.Hour < this.BusinessEndsHour)));
        }

        public string ResolveUrlSafe(string url)
        {
            if (url == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            return base.ResolveUrl(url);
        }

        public MemoryStream Export(ImageFormat format)
        {
            MemoryStream memoryStream = new MemoryStream();
            Bitmap bitmap = this.ExportBitmap();
            bitmap.Save(memoryStream, format);
            return memoryStream;
        }

        public Bitmap ExportBitmap()
        {
            this.isExport = true;
            this.LoadRows();
            this.LoadEventsToDays();
            this.TimeHeader.PrepareTimeline();
            this.TimeHeader.PrepareGrouplines();
            SchedulerExport schedulerExport = new SchedulerExport(this);
            Bitmap result = schedulerExport.Export();
            this.isExport = false;
            return result;
        }

        internal string GetResourceUrl(string file)
        {
            if (this.ResourcesStatic)
            {
                string text = this.ResourcesPath;
                if (string.IsNullOrEmpty(text))
                {
                    text = "./";
                }
                if (!text.EndsWith("/"))
                {
                    text += '/';
                }
                return base.ResolveUrl(text + file);
            }
            return this.Page.ClientScript.GetWeb.AppResourceUrl(typeof(HMSBubble), "HMS.Resources." + file.Replace("/", "."));
        }

        internal List<JsonData> GetSelectedEvents()
        {
            List<JsonData> list = new List<JsonData>();
            if (this.SelectedEvents == null || this.SelectedEvents.Count == 0)
            {
                return list;
            }
            foreach (EventInfo current in this.SelectedEvents)
            {
                list.Add(current.ToJson());
            }
            return list;
        }

        public void DoIncludeCell(IncludeCellEventArgs ea)
        {
            if (this.IncludeCell != null)
            {
                this.IncludeCell(this, ea);
            }
        }

        public List<Hashtable> GetLinksJson()
        {
            if (this.linksCache != null)
            {
                return this.linksCache;
            }
            this.linksCache = new List<Hashtable>();
            foreach (BeforeLinkRenderEventArgs current in this.ProcessLinks())
            {
                this.linksCache.Add(current.ToJson());
            }
            return this.linksCache;
        }

        private List<BeforeLinkRenderEventArgs> ProcessLinks()
        {
            List<BeforeLinkRenderEventArgs> list = new List<BeforeLinkRenderEventArgs>();
            foreach (Link link in this.Links)
            {
                BeforeLinkRenderEventArgs beforeLinkRenderEventArgs = BeforeLinkRenderEventArgs.FromLink(link);
                this.DoBeforeLinkRender(beforeLinkRenderEventArgs);
                list.Add(beforeLinkRenderEventArgs);
            }
            return list;
        }

        private void DoBeforeLinkRender(BeforeLinkRenderEventArgs ea)
        {
            if (this.BeforeLinkRender != null)
            {
                this.BeforeLinkRender(this, ea);
            }
        }
    }
}
