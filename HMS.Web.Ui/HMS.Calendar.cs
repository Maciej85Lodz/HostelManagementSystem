using HMS.Json;
using HMS.Utils;
using HMS.Web.App.Ui.Ajax;
using HMS.Web.App.Ui.Colors;
using HMS.Web.App.Ui.Data;
using HMS.Web.App.Ui.Design;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Enums.Calendar;
using HMS.Web.App.Ui.Events;
using HMS.Web.App.Ui.Events.Calendar;
using HMS.Web.App.Ui.Events.Common;
using HMS.Web.App.Ui.Export;
using HMS.Web.App.Ui.Json;
using HMS.Web.App.Ui.Recurrence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using HMS.Json;
using HMS.Utils;
using HMS.Web.Ui.Ajax;
using HMS.Web.Ui.Colors;
using HMS.Web.Ui.Data;
using HMS.Web.Ui.Design;
using HMS.Web.Ui.Enums;
using HMS.Web.Ui.Enums.Calendar;
using HMS.Web.Ui.Events;
using HMS.Web.Ui.Events.Calendar;
using HMS.Web.Ui.Events.Common;
using HMS.Web.Ui.Export;
using HMS.Web.Ui.Json;
using HMS.Web.Ui.Recurrence;
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HMS.Web.App.Ui
{
    [DefaultProperty(null), Designer(typeof(HMSCalendarDesigner)), ToolboxBitmap(typeof(System.Web.UI.WebControls.Calendar)), ParseChildren(true), PersistChildren(false), Themeable(true)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HMSCalendar : DataBoundControl, IPostBackEventHandler, ICallbackEventHandler
    {
        internal HMSMenu ContextMenu;

        internal HMSMenu ContextMenuSelection;

        internal HMSBubble Bubble;

        internal HMSBubble CellBubble;

        internal HMSBubble ColumnBubble;

        private List<Hashtable> _columns;

        private JsonData _clientState;

        private string _dataStartField;

        private string _dataEndField;

        private string _dataTextField;

        private string _dataValueField;

        private string _dataColumnField;

        private string _dataTagFields;

        private string _dataServerTagFields;

        private string _dataAllDayField;

        private string _dataRecurrenceField;

        internal SortExpression SortFields = new SortExpression();

        private Exception _callbackException;

        private object _callbackData;

        private CallBackUpdateType _callbackUpdateType;

        private HMSCalendarCallBack _callback;

        private string _callbackMessage;

        private bool _databindCalled;

        private Hashtable _hashes = new Hashtable();

        private bool _isExport;

        internal int ScrollPos = -1;

        private string _selectedEventValue;

        public List<EventInfo> SelectedEvents = new List<EventInfo>();

        private ArrayList _items = new ArrayList();

        public DateTimeFormatInfo DateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;

        [Category("Rendering"), Description("Use this event to modify time cell properties before rendering.")]
        public event BeforeCellRenderEventHandler BeforeCellRender;

        [Category("Preprocessing"), Description("Use this event to modify event properties before rendering.")]
        public event BeforeEventRenderEventHandler BeforeEventRender;

        [Category("Preprocessing"), Description("Use this event to modify column header properties before rendering.")]
        public event BeforeHeaderRenderEventHandler BeforeHeaderRender;

        [Category("Preprocessing"), Description("Use this event to modify the hour header cells.")]
        public event HMS.Web.App.Ui.Events.Calendar.BeforeTimeHeaderRenderEventHandler BeforeTimeHeaderRender;

        [Category("Preprocessing"), Description("Use this event to apply custom recurrence rules.")]
        public event BeforeEventRecurrenceHandler BeforeEventRecurrence;

        [Category("User actions"), Description("Fires when a user clicks a time cell or selects cells by mouse dragging. TimeRangeSelectedHandling must be set to PostBack or CallBack.")]
        public event TimeRangeSelectedEventHandler TimeRangeSelected;

        [Category("User actions"), Description("Fires when a user double-clicks a selected time range. TimeRangeDoubleClickHandling must be set to PostBack or CallBack.")]
        public event TimeRangeDoubleClickEventHandler TimeRangeDoubleClick;

        [Category("User actions"), Description("Fires when a user clicks an event.  EventClickHandling must be set to PostBack or CallBack.")]
        public event EventClickEventHandler EventClick;

        [Category("User actions"), Description("Fires when a user double-clicks an event.")]
        public event EventClickEventHandler EventDoubleClick;

        [Category("User actions"), Description("Fires when a user selects an event.")]
        public event EventSelectEventHandler EventSelect;

        [Category("User actions"), Description("This event is fired using client-side .commandCallBack() function.")]
        public event HMS.Web.App.Ui.Events.CommandEventHandler Command;

        [Category("User actions"), Description("Fires when a user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack.")]
        public event TimeRangeMenuClickEventHandler TimeRangeMenuClick;

        [Category("User actions"), Description("This event is fired when a user clicks a column header.")]
        public event HeaderClickEventHandler HeaderClick;

        [Category("User actions"), Description("This event is fired when a user clicks an event with a right mouse button.")]
        public event EventRightClickEventHandler EventRightClick;

        [Category("User actions"), Description("Fires when a user confirms event text changes (after editing). EventClickHandling must be set to Edit and EventEditHandling must be set to PostBack or CallBack.")]
        public event EventEditEventHandler EventEdit;

        [Category("User actions"), Description("Fires when a user resizes an event.  EventResizeHandling must be set to PostBack or CallBack.")]
        public event EventResizeEventHandler EventResize;

        [Category("User actions"), Description("Fires when a user moves an event. EventMoveHandling must be set to PostBack or CallBack.")]
        public event EventMoveEventHandler EventMove;

        [Category("User actions"), Description("This event is fired when a user clicks on a delete 'X' button in the upper right corner of an event.")]
        public event EventDeleteEventHandler EventDelete;

        [Category("User actions"), Description("Fires when a user clicks a menu item. MenuItem.Action must be set to PostBack or CallBack.")]
        public event EventMenuClickEventHandler EventMenuClick;

        [Category("User actions"), Description("Fires when queued client-side operations are notified to the server.")]
        public event NotifyEventHandler Notify;

        [Category("User actions"), Description("Fires when client-side EventUpdate event is notified to the server.")]
        public event EventUpdateEventHandler EventUpdate;

        [Category("User actions"), Description("Fires when client-side EventRemove event is notified to the server.")]
        public event EventRemoveEventHandler EventRemove;

        [Category("User actions"), Description("Fires when client-side EventAdd event is notified to the server.")]
        public event EventAddEventHandler EventAdd;

        public System.Globalization.Calendar Calendar
        {
            get
            {
                return this.DateTimeFormatInfo.Calendar;
            }
        }

        public bool IsExport
        {
            get
            {
                return this._isExport;
            }
        }

        internal string CallBackMessage
        {
            get
            {
                return this._callbackMessage;
            }
        }

        private ColumnCollection DaysModeColumns
        {
            get
            {
                ColumnCollection columnCollection = new ColumnCollection();
                int num = (int)(this.EndDate - this.StartDate).TotalDays + 1;
                for (int i = 0; i < num; i++)
                {
                    DateTime date = this.StartDate.AddDays((double)i);
                    string name = date.ToString(this.HeaderDateFormat, this.DateTimeFormatInfo);
                    columnCollection.Add(new Column(name, null)
                    {
                        Date = date
                    });
                }
                return columnCollection;
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

        public HMSCalendarCallBack CallBack
        {
            get
            {
                HMSCalendarCallBack arg_19_0;
                if ((arg_19_0 = this._callback) == null)
                {
                    arg_19_0 = (this._callback = new HMSCalendarCallBack(this));
                }
                return arg_19_0;
            }
        }

        [Browsable(false)]
        public JsonData ClientState
        {
            get
            {
                if (this._clientState == null)
                {
                    return new JsonData();
                }
                return this._clientState;
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
                if (value < 0)
                {
                    this.ViewState["BusinessEndsHour"] = 0;
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

        [Category("Behavior"), DefaultValue(0), Description("Start of the day column (hour from 0 to 23).")]
        public int DayBeginsHour
        {
            get
            {
                if (this.ViewState["DayBeginsHour"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["DayBeginsHour"];
            }
            set
            {
                if (value < 0)
                {
                    this.ViewState["DayBeginsHour"] = 0;
                    return;
                }
                if (value > 23)
                {
                    this.ViewState["DayBeginsHour"] = 23;
                    return;
                }
                this.ViewState["DayBeginsHour"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(24), Description("End of the day column (hour from 0 to 24).")]
        public int DayEndsHour
        {
            get
            {
                if (this.ViewState["DayEndsHour"] == null)
                {
                    return 0;
                }
                return (int)this.ViewState["DayEndsHour"];
            }
            set
            {
                if (value < 0)
                {
                    this.ViewState["DayEndsHour"] = 0;
                    return;
                }
                if (value > 24)
                {
                    this.ViewState["DayEndsHour"] = 24;
                    return;
                }
                this.ViewState["DayEndsHour"] = value;
            }
        }

        [Category("Layout"), DefaultValue(20), Description("Height of the time cells in pixels. ")]
        public int CellHeight
        {
            get
            {
                if (this.ViewState["CellHeight"] == null)
                {
                    return 20;
                }
                return (int)this.ViewState["CellHeight"];
            }
            set
            {
                this.ViewState["CellHeight"] = value;
            }
        }

        [Category("Layout"), DefaultValue(2), Description("Number of time cells in one hour. Allowed values: 1, 2, 3, 4, 5, 6, 10, 12."), Obsolete("Use CellDuration instead.")]
        public int CellsPerHour
        {
            get
            {
                return 60 / this.CellDuration;
            }
            set
            {
                if (value < 1 || value == 7 || value == 8 || value == 9 || value == 11 || value > 12)
                {
                    return;
                }
                this.CellDuration = 60 / value;
            }
        }

        [Category("Layout"), DefaultValue(30), Description("Duration of a time cell in minutes.")]
        public int CellDuration
        {
            get
            {
                if (this.ViewState["CellDuration"] == null)
                {
                    return 30;
                }
                return (int)this.ViewState["CellDuration"];
            }
            set
            {
                this.ViewState["CellDuration"] = value;
            }
        }

        [Category("Layout"), DefaultValue(60), Description("Duration of a time header cell in minutes.")]
        public int TimeHeaderCellDuration
        {
            get
            {
                if (this.ViewState["TimeHeaderCellDuration"] == null)
                {
                    return 60;
                }
                return (int)this.ViewState["TimeHeaderCellDuration"];
            }
            set
            {
                if (value > 60)
                {
                    throw new ArgumentOutOfRangeException("CellDuration > 60 is not supported yet.");
                }
                this.ViewState["TimeHeaderCellDuration"] = value;
            }
        }

        [Category("Layout"), DefaultValue(45), Description("Width of an hour cell in pixels.")]
        public int HourWidth
        {
            get
            {
                if (this.ViewState["HourWidth"] == null)
                {
                    return 45;
                }
                return (int)this.ViewState["HourWidth"];
            }
            set
            {
                this.ViewState["HourWidth"] = value;
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

        [Category("Events"), DefaultValue("alert('Deleting event with id ' + e.value() + '.')"), Description("Javascript code that is executed when the users clicks the delete icon.")]
        public string EventDeleteJavaScript
        {
            get
            {
                if (this.ViewState["EventDeleteJavaScript"] == null)
                {
                    return "alert('Deleting event with id ' + e.value() + '.')";
                }
                return (string)this.ViewState["EventDeleteJavaScript"];
            }
            set
            {
                this.ViewState["EventDeleteJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Header with id ' + c.value + ' clicked.')"), Description("Javascript code that is executed when the user clicks a column header.")]
        public string HeaderClickJavaScript
        {
            get
            {
                if (this.ViewState["HeaderClickJavaScript"] == null)
                {
                    return "alert('Header with id ' + c.value + ' clicked.')";
                }
                return (string)this.ViewState["HeaderClickJavaScript"];
            }
            set
            {
                this.ViewState["HeaderClickJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert(start.toString() + '\\n' + end.toString());"), Description("Javascript code that is executed when the users selectes a time range.")]
        public string TimeRangeSelectedJavaScript
        {
            get
            {
                if (this.ViewState["TimeRangeSelectedJavaScript"] == null)
                {
                    return "alert(start.toString() + '\\n' + end.toString());";
                }
                return (string)this.ViewState["TimeRangeSelectedJavaScript"];
            }
            set
            {
                this.ViewState["TimeRangeSelectedJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert(start.toString() + '\\n' + end.toString());"), Description("Javascript code that is executed when the double clicks a selected time range.")]
        public string TimeRangeDoubleClickJavaScript
        {
            get
            {
                if (this.ViewState["TimeRangeDoubleClickJavaScript"] == null)
                {
                    return "alert(start.toString() + '\\n' + end.toString());";
                }
                return (string)this.ViewState["TimeRangeDoubleClickJavaScript"];
            }
            set
            {
                this.ViewState["TimeRangeDoubleClickJavaScript"] = value;
            }
        }

        [Category("Behavior"), Description("The first day to be shown. Default is DateTime.Today.")]
        public DateTime StartDate
        {
            get
            {
                DateTime dateTime;
                if (this.ViewState["StartDate"] == null)
                {
                    dateTime = DateTime.Today;
                }
                else
                {
                    dateTime = (DateTime)this.ViewState["StartDate"];
                }
                switch (this.ViewType)
                {
                    case ViewTypeEnum.Week:
                        return Week.FirstDayOfWeek(dateTime, this.ResolvedWeekStart);
                    case ViewTypeEnum.WorkWeek:
                        return Week.FirstWorkingDayOfWeek(dateTime, this.ResolvedWeekStart);
                    default:
                        return dateTime;
                }
            }
            set
            {
                this.ViewState["StartDate"] = new DateTime(value.Year, value.Month, value.Day);
            }
        }

        [Category("Behavior"), DefaultValue(1), Description("The number of days to be displayed on the calendar. Default value is 1.")]
        public int Days
        {
            get
            {
                switch (this.ViewType)
                {
                    case ViewTypeEnum.Day:
                        return 1;
                    case ViewTypeEnum.Week:
                        return 7;
                    case ViewTypeEnum.WorkWeek:
                        return 5;
                    default:
                        if (this.ViewState["Days"] == null)
                        {
                            return 1;
                        }
                        return (int)this.ViewState["Days"];
                }
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

        [Browsable(false)]
        public DateTime EndDate
        {
            get
            {
                return this.StartDate.AddDays((double)(this.Days - 1));
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
        public string DataColumnField
        {
            get
            {
                return this._dataColumnField;
            }
            set
            {
                this._dataColumnField = value;
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

        [Category("Data"), Description("Name of the data source field that contains true for all-day events.")]
        public string DataAllDayField
        {
            get
            {
                return this._dataAllDayField;
            }
            set
            {
                this._dataAllDayField = value;
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

        [Category("Data"), DefaultValue(DateTimeSpec.DateTime), Description("Whether the end of all-day event is specified by date only or by full date and time.")]
        public DateTimeSpec AllDayEnd
        {
            get
            {
                if (this.ViewState["AllDayEnd"] == null)
                {
                    return DateTimeSpec.DateTime;
                }
                return (DateTimeSpec)this.ViewState["AllDayEnd"];
            }
            set
            {
                this.ViewState["AllDayEnd"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(TimeFormat.Auto), Description("The time-format that will be used for the hour numbers.")]
        public TimeFormat TimeFormat
        {
            get
            {
                if (this.ViewState["TimeFormat"] == null)
                {
                    return TimeFormat.Auto;
                }
                return (TimeFormat)this.ViewState["TimeFormat"];
            }
            set
            {
                this.ViewState["TimeFormat"] = value;
            }
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

        [Category("Appearance"), Description("Whether to show the all-day event start and end times.")]
        public bool ShowAllDayEventStartEnd
        {
            get
            {
                return this.ViewState["ShowAllDayEventStartEnd"] == null || (bool)this.ViewState["ShowAllDayEventStartEnd"];
            }
            set
            {
                this.ViewState["ShowAllDayEventStartEnd"] = value;
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

        [Category("Events"), DefaultValue(TimeRangeTapAndHoldHandlingEnum.Select), Description("Specifies action that should be performed for a touch 'tap and hold' gesture.")]
        public TimeRangeTapAndHoldHandlingEnum TimeRangeTapAndHoldHandling
        {
            get
            {
                if (this.ViewState["TimeRangeTapAndHoldHandling"] == null)
                {
                    return TimeRangeTapAndHoldHandlingEnum.Select;
                }
                return (TimeRangeTapAndHoldHandlingEnum)this.ViewState["TimeRangeTapAndHoldHandling"];
            }
            set
            {
                this.ViewState["TimeRangeTapAndHoldHandling"] = value;
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

        [Category("Events"), DefaultValue(EventHoverHandlingEnum.Bubble), Description("What to do when mouse hovers over an event.")]
        public EventHoverHandlingEnum EventHoverHandling
        {
            get
            {
                if (this.ViewState["EventHoverHandling"] == null)
                {
                    return EventHoverHandlingEnum.Bubble;
                }
                return (EventHoverHandlingEnum)this.ViewState["EventHoverHandling"];
            }
            set
            {
                this.ViewState["EventHoverHandling"] = value;
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

        [Category("Events"), DefaultValue(UserActionHandling.Disabled), Description("Handling of column header click.")]
        public UserActionHandling HeaderClickHandling
        {
            get
            {
                if (this.ViewState["HeaderClickHandling"] == null)
                {
                    return UserActionHandling.Disabled;
                }
                return (UserActionHandling)this.ViewState["HeaderClickHandling"];
            }
            set
            {
                this.ViewState["HeaderClickHandling"] = value;
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

        [Category("Appearance"), DefaultValue("d"), Description("Format of the date display in the header columns (e.g. \"d\", \"yyyy-MM-dd\").")]
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

        [Category("Appearance"), DefaultValue(true), Description("Should the header be visible?")]
        public bool ShowHeader
        {
            get
            {
                return this.ViewState["ShowHeader"] == null || (bool)this.ViewState["ShowHeader"];
            }
            set
            {
                this.ViewState["ShowHeader"] = value;
            }
        }

        [Category("Layout"), DefaultValue(20), Description("Header height in pixels.")]
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

        [Category("Appearance"), Description("Color of the inner vertical border.")]
        public Color CellBorderColor
        {
            get
            {
                if (this.ViewState["CellBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["CellBorderColor"];
            }
            set
            {
                this.ViewState["CellBorderColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#316AC5"), Description("Color of the time range selection."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of the horizontal border that separates hour names."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of the border that separates time cells inside one hour."), TypeConverter(typeof(WebColorConverter))]
        public Color HourHalfBorderColor
        {
            get
            {
                if (this.ViewState["HourHalfBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#F3E4B1");
                }
                return (Color)this.ViewState["HourHalfBorderColor"];
            }
            set
            {
                this.ViewState["HourHalfBorderColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the vertical border that separates our names."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of the hour names background."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of an event background."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of an event border."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Color of an all-day event border."), TypeConverter(typeof(WebColorConverter))]
        public Color AllDayEventBorderColor
        {
            get
            {
                if (this.ViewState["AllDayEventBorderColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["AllDayEventBorderColor"];
            }
            set
            {
                this.ViewState["AllDayEventBorderColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the vertical bar on the left side of an event."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), Description("Width of the duration bar on the left side of an event.")]
        public int DurationBarWidth
        {
            get
            {
                if (this.ViewState["DurationBarWidth"] == null)
                {
                    return 5;
                }
                return (int)this.ViewState["DurationBarWidth"];
            }
            set
            {
                this.ViewState["DurationBarWidth"] = value;
            }
        }

        [Category("Appearance"), Description("Background image URL for the duration bar on the left side of an event.")]
        public string DurationBarImageUrl
        {
            get
            {
                if (this.ViewState["DurationBarImageUrl"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["DurationBarImageUrl"];
            }
            set
            {
                this.ViewState["DurationBarImageUrl"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Whether the color bar on the left side of and event should be visible.")]
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

        [Category("Appearance"), DefaultValue(false), Description("Whether the event should have a header at the top.")]
        public bool EventHeaderVisible
        {
            get
            {
                return this.ViewState["EventHeaderVisible"] != null && (bool)this.ViewState["EventHeaderVisible"];
            }
            set
            {
                this.ViewState["EventHeaderVisible"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("8pt"), Description("Font size of the event header.")]
        public string EventHeaderFontSize
        {
            get
            {
                if (this.ViewState["EventHeaderFontSize"] == null)
                {
                    return "8pt";
                }
                return (string)this.ViewState["EventHeaderFontSize"];
            }
            set
            {
                this.ViewState["EventHeaderFontSize"] = value;
            }
        }

        [Category("Appearance"), Description("Event header font color.")]
        public Color EventHeaderFontColor
        {
            get
            {
                if (this.ViewState["EventHeaderFontColor"] == null)
                {
                    return Color.White;
                }
                return (Color)this.ViewState["EventHeaderFontColor"];
            }
            set
            {
                this.ViewState["EventHeaderFontColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(14), Description("Event header height in pixels.")]
        public int EventHeaderHeight
        {
            get
            {
                if (this.ViewState["EventHeaderHeight"] == null)
                {
                    return 14;
                }
                return (int)this.ViewState["EventHeaderHeight"];
            }
            set
            {
                this.ViewState["EventHeaderHeight"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("#FFF4BC"), Description("Background color of time cells outside of the busines hours."), TypeConverter(typeof(WebColorConverter))]
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

        [Category("Appearance"), DefaultValue("Tahoma"), Description(" Font family of the hour titles on the left side, e.g. \"Tahoma\".")]
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

        [Category("Appearance"), DefaultValue("8pt"), Description("Font size of the event text, e.g. \"8pt\".")]
        public string EventFontSize
        {
            get
            {
                if (this.ViewState["EventFontSize"] == null)
                {
                    return "8pt";
                }
                return (string)this.ViewState["EventFontSize"];
            }
            set
            {
                this.ViewState["EventFontSize"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("16pt"), Description("Font size of the hour titles on the left side e.g. \"16pt\".")]
        public string HourFontSize
        {
            get
            {
                if (this.ViewState["HourFontSize"] == null)
                {
                    return "16pt";
                }
                return (string)this.ViewState["HourFontSize"];
            }
            set
            {
                this.ViewState["HourFontSize"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("10pt"), Description("Font size of the column header, e.g. \"10pt\".")]
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

        [Category("Appearance"), Description("Color of the hour header font.")]
        public Color HourFontColor
        {
            get
            {
                if (this.ViewState["HourFontColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["HourFontColor"];
            }
            set
            {
                this.ViewState["HourFontColor"] = value;
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

        [Category("Behavior"), Description("Collection of columns that will be used when ViewType property is set to ViewTypeEnum.Resources."), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), PersistenceMode(PersistenceMode.InnerProperty)]
        public ColumnCollection Columns
        {
            get
            {
                if (this.ViewState["Columns"] == null)
                {
                    ColumnCollection columnCollection = new ColumnCollection();
                    columnCollection.designMode = base.DesignMode;
                    this.ViewState["Columns"] = columnCollection;
                }
                return (ColumnCollection)this.ViewState["Columns"];
            }
        }

        [Category("Behavior"), DefaultValue(ViewTypeEnum.Days), Description("Sets the view type. In days view it shows one or more days in the columns. In in resources view it shows multiple resources in the columns.")]
        public ViewTypeEnum ViewType
        {
            get
            {
                if (this.ViewState["ViewType"] == null)
                {
                    return ViewTypeEnum.Days;
                }
                return (ViewTypeEnum)this.ViewState["ViewType"];
            }
            set
            {
                this.ViewState["ViewType"] = value;
            }
        }

        [Category("Behavior"), Description("Sets the starting scroll position of the scrolling area (in hours). Does not apply when HeightSpec property is set to HeightSpecEnum.Full.")]
        public int ScrollPositionHour
        {
            get
            {
                if (this.ViewState["ScrollPositionHour"] == null)
                {
                    return this.BusinessBeginsHour;
                }
                return (int)this.ViewState["ScrollPositionHour"];
            }
            set
            {
                if (value != this.BusinessBeginsHour)
                {
                    this.ViewState["ScrollPositionHour"] = value;
                }
            }
        }

        [Category("Layout"), DefaultValue(300), Description("Sets or get the height of the scrolling area (in pixels). It only applies when HeightSpec property is set to HeightSpecEnum.Fixed.")]
        public new int Height
        {
            get
            {
                if (this.ViewState["Height"] == null)
                {
                    return 300;
                }
                return (int)this.ViewState["Height"];
            }
            set
            {
                this.ViewState["Height"] = value;
            }
        }

        [Category("Layout"), DefaultValue(HeightSpecEnum.BusinessHours), Description("Sets or get the way how the height of the scrolling area is determined - Fixed (height specified by Height in pixels is used), Full (the full height, prevents scrolling), or BusinessHours (it always shows business hours in full and enables scrolling).")]
        public HeightSpecEnum HeightSpec
        {
            get
            {
                if (this.ViewState["HeightSpec"] == null)
                {
                    return HeightSpecEnum.BusinessHours;
                }
                return (HeightSpecEnum)this.ViewState["HeightSpec"];
            }
            set
            {
                this.ViewState["HeightSpec"] = value;
            }
        }

        [Category("Behavior"), Description("ID of the HMSMenu that will be used for context menu. If no ID is specified, the context menu will be disabled."), TypeConverter(typeof(MenuControlConverter)), IDReferenceProperty(typeof(HMSMenu))]
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

        [Category("Behavior"), Description("ID of the HMSMenu that will be used for context menu for time range selection. If no ID is specified, the context menu will be disabled."), TypeConverter(typeof(MenuControlConverter)), IDReferenceProperty(typeof(HMSMenu))]
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

        [Category("ToolTips"), Description("ID of the advanced tooltip control (HMSBubble) that will be used for calendar events."), TypeConverter(typeof(BubbleControlConverter)), IDReferenceProperty(typeof(HMSBubble))]
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
        public string ColumnBubbleID
        {
            get
            {
                if (this.ViewState["ColumnBubbleID"] == null)
                {
                    return null;
                }
                return (string)this.ViewState["ColumnBubbleID"];
            }
            set
            {
                this.ViewState["ColumnBubbleID"] = value;
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

        [Category("Events"), DefaultValue("alert('Event with id ' + e.id() + ' was moved.');"), Description("Javascript function that is executed when a users moves an event.")]
        public string EventMoveJavaScript
        {
            get
            {
                if (this.ViewState["EventMoveJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.id() + ' was moved.');";
                }
                return (string)this.ViewState["EventMoveJavaScript"];
            }
            set
            {
                this.ViewState["EventMoveJavaScript"] = value;
            }
        }

        [Category("Events"), DefaultValue("alert('Event with id ' + e.id() + ' was resized.');"), Description("Javascript function that is executed when a users moves an event.")]
        public string EventResizeJavaScript
        {
            get
            {
                if (this.ViewState["EventResizeJavaScript"] == null)
                {
                    return "alert('Event with id ' + e.id() + ' was resized.');";
                }
                return (string)this.ViewState["EventResizeJavaScript"];
            }
            set
            {
                this.ViewState["EventResizeJavaScript"] = value;
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

        [Description("Calendar width. If not specified, display:block behavior will be used.")]
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
                if (value.Type != UnitType.Percentage && value.Type != UnitType.Pixel && !(value == Unit.Empty))
                {
                    throw new FormatException("Only Percentage and Pixel units are allowed.");
                }
                this.ViewState["Width"] = value;
            }
        }

        [Description("JavaScript instance name on the client-side. If it is not specified the control ClientID will be used.")]
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

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the hour names column is visible.")]
        public bool ShowHours
        {
            get
            {
                return this.ViewState["ShowHours"] == null || (bool)this.ViewState["ShowHours"];
            }
            set
            {
                this.ViewState["ShowHours"] = value;
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

        [Category("Appearance"), DefaultValue(5), Description("Width of the right margin inside a column (in pixels).")]
        public int ColumnMarginRight
        {
            get
            {
                if (this.ViewState["ColumnMarginRight"] == null)
                {
                    return 5;
                }
                return (int)this.ViewState["ColumnMarginRight"];
            }
            set
            {
                this.ViewState["ColumnMarginRight"] = value;
            }
        }

        [Browsable(false), Description("Keeps the value (from DataValueField) of the selected event (applicable when EventClickHandling=\"Select\").")]
        public string SelectedEventValue
        {
            get
            {
                return this._selectedEventValue;
            }
            set
            {
                this._selectedEventValue = value;
            }
        }

        [Category("Behavior"), DefaultValue(UseBoxesEnum.Always), Description("Specifies whether event box start and end will be aligned with the underlying cell size.")]
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

        [Category("Appearance"), Description("Determines the color of the event border that indicates that the event is selected.")]
        public Color EventSelectColor
        {
            get
            {
                if (this.ViewState["EventSelectColor"] == null)
                {
                    return ColorTranslator.FromHtml("blue");
                }
                return (Color)this.ViewState["EventSelectColor"];
            }
            set
            {
                this.ViewState["EventSelectColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(false), Description("Determines whether the extra row for all-day events will be visible.")]
        public bool ShowAllDayEvents
        {
            get
            {
                return this.ViewState["ShowAllDayEvents"] != null && (bool)this.ViewState["ShowAllDayEvents"];
            }
            set
            {
                this.ViewState["ShowAllDayEvents"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(25), Description("Determines the height of an all-day event.")]
        public int AllDayEventHeight
        {
            get
            {
                if (this.ViewState["AllDayEventHeight"] == null)
                {
                    return 25;
                }
                return (int)this.ViewState["AllDayEventHeight"];
            }
            set
            {
                this.ViewState["AllDayEventHeight"] = value;
            }
        }

        [Category("Appearance"), Description("Determines the background color of all-day events.")]
        public Color AllDayEventBackColor
        {
            get
            {
                if (this.ViewState["AllDayEventBackColor"] == null)
                {
                    return ColorTranslator.FromHtml("#ffffff");
                }
                return (Color)this.ViewState["AllDayEventBackColor"];
            }
            set
            {
                this.ViewState["AllDayEventBackColor"] = value;
            }
        }

        [Category("Appearance"), Description("Color of the all-day event font.")]
        public Color AllDayEventFontColor
        {
            get
            {
                if (this.ViewState["AllDayEventFontColor"] == null)
                {
                    return ColorTranslator.FromHtml("#000000");
                }
                return (Color)this.ViewState["AllDayEventFontColor"];
            }
            set
            {
                this.ViewState["AllDayEventFontColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("8pt"), Description("Font size of the all-day event text, e.g. \"8pt\".")]
        public string AllDayEventFontSize
        {
            get
            {
                if (this.ViewState["AllDayEventFontSize"] == null)
                {
                    return "8pt";
                }
                return (string)this.ViewState["AllDayEventFontSize"];
            }
            set
            {
                this.ViewState["AllDayEventFontSize"] = value;
            }
        }

        [Category("Appearance"), DefaultValue("Tahoma"), Description("Font family of the all-day event text, e.g. \"Tahoma\".")]
        public string AllDayEventFontFamily
        {
            get
            {
                if (this.ViewState["AllDayEventFontFamily"] == null)
                {
                    return "Tahoma";
                }
                return (string)this.ViewState["AllDayEventFontFamily"];
            }
            set
            {
                this.ViewState["AllDayEventFontFamily"] = value;
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

        [Category("Labels"), DefaultValue(true), Description("Whether the 'Loading...' label is visible.")]
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

        [Category("Labels"), DefaultValue(true), Description("Whether the scroll labels ('Scroll up', 'Scroll down') are visible.")]
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

        [Category("Labels"), DefaultValue("Scroll up"), Description("The text of the 'scroll up' label."), Localizable(true), Obsolete("Scroll labels now use a built-in image instead of a text. This property is ignored.")]
        public string ScrollUpLabelText
        {
            get
            {
                if (this.ViewState["ScrollUpLabelText"] == null)
                {
                    return "&lt;";
                }
                return (string)this.ViewState["ScrollUpLabelText"];
            }
            set
            {
                this.ViewState["ScrollUpLabelText"] = value;
            }
        }

        [Category("Labels"), DefaultValue("Scroll down"), Description("The text of the 'scroll down' label."), Localizable(true), Obsolete("Scroll labels now use a built-in image instead of a text. This property is ignored.")]
        public string ScrollDownLabelText
        {
            get
            {
                if (this.ViewState["ScrollDownLabelText"] == null)
                {
                    return "&gt;";
                }
                return (string)this.ViewState["ScrollDownLabelText"];
            }
            set
            {
                this.ViewState["ScrollDownLabelText"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Allows or forbids overlap of the shadow (or selection) during move, resize and time range selection operation.")]
        public bool AllowEventOverlap
        {
            get
            {
                return this.ViewState["AllowEventOverlap"] == null || (bool)this.ViewState["AllowEventOverlap"];
            }
        }

        [Browsable(false), Obsolete("Disabled. Use Theme instead.")]
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

        [Category("Appearance"), Description("Specifies the prefix of the CSS classes that contain style definitions for the elements of this control."), Obsolete("Use Theme property instead.")]
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

        [Description("Calendar direction (RTL/LTR).")]
        public TextDirection Direction
        {
            get
            {
                if (this.ViewState["Direction"] == null)
                {
                    return TextDirection.Auto;
                }
                return (TextDirection)this.ViewState["Direction"];
            }
            set
            {
                this.ViewState["Direction"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(1), Description("How many header levels will be shown (from Columns collection, in Resource mode only).")]
        public int HeaderLevels
        {
            get
            {
                if (this.ViewState["HeaderLevels"] == null)
                {
                    return 1;
                }
                return (int)this.ViewState["HeaderLevels"];
            }
            set
            {
                if (value < 1)
                {
                    this.ViewState["HeaderLevels"] = 1;
                    return;
                }
                this.ViewState["HeaderLevels"] = value;
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

        [Category("Appearance"), DefaultValue(CornerShape.Regular), Description("Corner look: regular or rounded.")]
        public CornerShape EventCorners
        {
            get
            {
                if (this.ViewState["EventCorners"] == null)
                {
                    return CornerShape.Regular;
                }
                return (CornerShape)this.ViewState["EventCorners"];
            }
            set
            {
                this.ViewState["EventCorners"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(ArrangementType.SideBySide), Description("Concurrent events arrangement: side by side or cascading.")]
        public ArrangementType EventArrangement
        {
            get
            {
                if (this.ViewState["EventArrangement"] == null)
                {
                    return ArrangementType.SideBySide;
                }
                return (ArrangementType)this.ViewState["EventArrangement"];
            }
            set
            {
                this.ViewState["EventArrangement"] = value;
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

        internal ArrayList Items
        {
            get
            {
                if (this.StoreEventsInViewState)
                {
                    return (ArrayList)this.ViewState["Items"];
                }
                return this._items;
            }
            private set
            {
                this._items = value;
                if (this.StoreEventsInViewState)
                {
                    this.ViewState["Items"] = this._items;
                }
            }
        }

        internal DateTime VisibleStart
        {
            get
            {
                if (this.HeightSpec == HeightSpecEnum.BusinessHoursNoScroll)
                {
                    return new DateTime(1900, 1, 1, this.BusinessBeginsHour, 0, 0);
                }
                return new DateTime(1900, 1, 1, this.DayBeginsHour, 0, 0);
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

        internal int WeekStartInt
        {
            get
            {
                switch (this.ResolvedWeekStart)
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
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal DateTime VisibleEnd
        {
            get
            {
                if (this.HeightSpec == HeightSpecEnum.BusinessHoursNoScroll && this.BusinessEndsHour != 24)
                {
                    return new DateTime(1900, 1, 1, this.BusinessEndsHour, 0, 0);
                }
                return new DateTime(1900, 1, 2);
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

        [Category("Behavior"), DefaultValue(false), Description("Hide non-business cells if there are no events.")]
        public bool HideFreeCells
        {
            get
            {
                return this.ViewState["HideFreeCells"] != null && (bool)this.ViewState["HideFreeCells"];
            }
            set
            {
                this.ViewState["HideFreeCells"] = value;
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

        [Category("Behavior"), DefaultValue(false), Description("Allows selecting multiple events (using Ctrl + click).")]
        public bool UseEventSelectionBars
        {
            get
            {
                return this.ViewState["UseEventSelectionBars"] != null && (bool)this.ViewState["UseEventSelectionBars"];
            }
            set
            {
                this.ViewState["UseEventSelectionBars"] = value;
            }
        }

        internal int ColumnCount
        {
            get
            {
                if (this.ViewType == ViewTypeEnum.Resources)
                {
                    List<Column> columns = this.Columns.GetColumns(this.HeaderLevels, true);
                    return columns.Count;
                }
                return this._columns.Count;
            }
        }

        [Obsolete("This property is ignored. Added to maintain API compatibility with HMS Lite.")]
        public Color HoverColor
        {
            get;
            set;
        }

        [Obsolete("This property is ignored. Added to maintain API compatibility with HMS Lite.")]
        public Color EventHoverColor
        {
            get;
            set;
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

        [Category("Appearance"), DefaultValue(false), Description("Whether to increase the header height automatically to fit the content.")]
        public bool HeaderHeightAutoFit
        {
            get
            {
                return this.ViewState["HeaderHeightAutoFit"] != null && (bool)this.ViewState["HeaderHeightAutoFit"];
            }
            set
            {
                this.ViewState["HeaderHeightAutoFit"] = value;
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

        [Category("Appearance"), Description("Background color of the upper-left corner.")]
        public string CornerBackColor
        {
            get
            {
                if (this.ViewState["CornerBackColor"] != null)
                {
                    return (string)this.ViewState["CornerBackColor"];
                }
                if (!this.CssOnly)
                {
                    return "#ECE9D8";
                }
                return null;
            }
            set
            {
                this.ViewState["CornerBackColor"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(ColWidthSpec.Auto), Description("How the column width is determined (Fixed, see ColumnWidth; Auto - calculated to fit the width).")]
        public ColWidthSpec ColumnWidthSpec
        {
            get
            {
                if (this.ViewState["ColumnWidthSpec"] == null)
                {
                    return ColWidthSpec.Auto;
                }
                return (ColWidthSpec)this.ViewState["ColumnWidthSpec"];
            }
            set
            {
                this.ViewState["ColumnWidthSpec"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(200), Description("Column width for ColumnWidthSpec=Fixed mode.")]
        public int ColumnWidth
        {
            get
            {
                if (this.ViewState["ColumnWidth"] == null)
                {
                    return 200;
                }
                return (int)this.ViewState["ColumnWidth"];
            }
            set
            {
                this.ViewState["ColumnWidth"] = value;
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

        public int ScrollY
        {
            get
            {
                return this.ScrollPos;
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Displays a vertical line at the current time position.")]
        public bool ShowCurrentTime
        {
            get
            {
                return this.ViewState["ShowCurrentTime"] == null || (bool)this.ViewState["ShowCurrentTime"];
            }
            set
            {
                this.ViewState["ShowCurrentTime"] = value;
            }
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Context.Request.Params[this.ClientID + "_vsupdate"]))
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
            string text = this.Context.Request.Params[this.ClientID + "_scrollpos"];
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    text = text.Replace(",", ".");
                    this.ScrollPos = (int)Convert.ToDouble(text, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }
            string text2 = this.Context.Request.Params[this.ClientID + "_select"];
            if (!string.IsNullOrEmpty(text2))
            {
                this._selectedEventValue = text2;
            }
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSCalendar), "common.js", this.GetResourceUrl("Common.js"));
            ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSCalendar), "calendar.js", this.GetResourceUrl("Calendar.js"));
            if (!string.IsNullOrEmpty(this.ContextMenuID))
            {
                this.ContextMenu = this.FindMenu(this.ContextMenuID);
                if (this.ContextMenu == null)
                {
                    throw new Exception("The context menu specified using ContextMenuID was not found.");
                }
            }
            if (!string.IsNullOrEmpty(this.ContextMenuSelectionID))
            {
                this.ContextMenuSelection = this.FindMenu(this.ContextMenuSelectionID);
                if (this.ContextMenuSelection == null)
                {
                    throw new Exception("The context menu specified using ContextMenuSelectionID was not found.");
                }
            }
            this.Bubble = this.FindBubble(this.BubbleID);
            this.CellBubble = this.FindBubble(this.CellBubbleID);
            this.ColumnBubble = this.FindBubble(this.ColumnBubbleID);
            base.OnLoad(e);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            string a = eventArgument.Substring(0, 4);
            if (a == "JSON")
            {
                this.ExecuteEventJson(eventArgument.Substring(4));
                return;
            }
            throw new Exception("Unsupported PostBack format.");
        }

        protected override void Render(HtmlTextWriter output)
        {
            this.LoadColumns();
            this.RenderClientSide(output);
            JsInitCalendar jsInitCalendar = new JsInitCalendar(this);
            ScriptManagerHelper.RegisterStartupScript(this, typeof(HMSCalendar), this.ClientID + "object", jsInitCalendar.GetCode(), false);
        }

        private HMSMenu FindMenu(string id)
        {
            HMSMenu result = null;
            if (!string.IsNullOrEmpty(id))
            {
                result = (HMSCalendar.RecursiveFind(this.NamingContainer, id) as HMSMenu);
            }
            return result;
        }

        private HMSBubble FindBubble(string id)
        {
            HMSBubble result = null;
            if (!string.IsNullOrEmpty(id))
            {
                result = (HMSCalendar.RecursiveFind(this.NamingContainer, id) as HMSBubble);
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
                control = HMSCalendar.RecursiveFind(parent2, id);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }

        public MemoryStream Export(ImageFormat format)
        {
            return this.Export(format, this.ScrollPos);
        }

        public Bitmap Export()
        {
            return this.ExportBitmap(this.ScrollPos);
        }

        public MemoryStream Export(ImageFormat format, int scrollPosition)
        {
            Bitmap bitmap = this.ExportBitmap(scrollPosition);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, format);
            return memoryStream;
        }

        public Bitmap ExportBitmap()
        {
            return this.ExportBitmap(this.ScrollPos);
        }

        public Bitmap ExportBitmap(int scrollPosition)
        {
            this._isExport = true;
            CalendarExport calendarExport = new CalendarExport(this);
            if (this.HeightSpec == HeightSpecEnum.Fixed || this.HeightSpec == HeightSpecEnum.BusinessHours)
            {
                calendarExport.ScrollPosition = ((scrollPosition > 0) ? scrollPosition : 0);
            }
            Bitmap result = calendarExport.Export();
            this._isExport = false;
            return result;
        }

        private void RenderClientSide(HtmlTextWriter output)
        {
            output.AddAttribute("id", this.ClientID);
            output.RenderBeginTag("div");
            output.RenderEndTag();
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

        internal List<Hashtable> GetEvents()
        {
            List<Hashtable> list = new List<Hashtable>();
            if (this.Items != null)
            {
                foreach (Event e in this.Items)
                {
                    list.Add(this.GetEventMap(e));
                }
            }
            return list;
        }

        internal List<Hashtable> GetHours()
        {
            if (this.TimeHeaderCellDuration % this.CellDuration != 0 && this.CellDuration % this.TimeHeaderCellDuration != 0)
            {
                throw new ArgumentException(string.Format("TimeHeaderCellDuration ({0}) must be divisible by CellDuration ({1}) or vice versa.", this.TimeHeaderCellDuration, this.CellDuration));
            }
            List<Hashtable> list = new List<Hashtable>();
            int num = (int)Math.Floor(this.Duration().TotalHours * 60.0 / (double)this.TimeHeaderCellDuration);
            for (int i = 0; i < num; i++)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes((double)(i * this.TimeHeaderCellDuration)) + TimeSpan.FromHours((double)this.VisibleStart.Hour);
                HMS.Web.App.Ui.Events.Calendar.BeforeTimeHeaderRenderEventArgs beforeTimeHeaderRenderEventArgs = new HMS.Web.App.Ui.Events.Calendar.BeforeTimeHeaderRenderEventArgs();
                beforeTimeHeaderRenderEventArgs.Start = timeSpan;
                beforeTimeHeaderRenderEventArgs.End = timeSpan.Add(TimeSpan.FromMinutes((double)this.TimeHeaderCellDuration));
                if (this.TimeHeaderCellDuration == 60)
                {
                    beforeTimeHeaderRenderEventArgs.Html = this.GetHourHtml(timeSpan.Hours);
                }
                else
                {
                    beforeTimeHeaderRenderEventArgs.Html = TimeFormatter.GetHourMinutes(timeSpan, this.TimeFormat);
                }
                if (this.BeforeTimeHeaderRender != null)
                {
                    this.BeforeTimeHeaderRender(beforeTimeHeaderRenderEventArgs);
                }
                Hashtable hashtable = new Hashtable();
                hashtable["hours"] = timeSpan.Hours;
                hashtable["html"] = beforeTimeHeaderRenderEventArgs.Html;
                hashtable["start"] = TimeFormatter.GetHourMinutes(beforeTimeHeaderRenderEventArgs.Start, TimeFormat.Clock24Hours);
                hashtable["end"] = TimeFormatter.GetHourMinutes(beforeTimeHeaderRenderEventArgs.End, TimeFormat.Clock24Hours);
                hashtable["areas"] = beforeTimeHeaderRenderEventArgs.Areas.GetList();
                list.Add(hashtable);
            }
            return list;
        }

        private string GetHourHtml(int hour)
        {
            if (!this.CssOnly)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<div style='padding:2px; font-family:");
                stringBuilder.Append(this.HourFontFamily);
                stringBuilder.Append(";font-size:");
                stringBuilder.Append(this.HourFontSize);
                stringBuilder.Append(";color:");
                stringBuilder.Append(ColorTranslator.ToHtml(this.HourFontColor));
                stringBuilder.Append(";' unselectable='on'>");
                bool flag = hour / 12 == 0;
                if (Hour.DetectTimeFormat(this.TimeFormat) == TimeFormat.Clock12Hours)
                {
                    hour %= 12;
                    if (hour == 0)
                    {
                        hour = 12;
                    }
                }
                stringBuilder.Append(hour);
                stringBuilder.Append("<span style='font-size:10px; vertical-align: super; ' unselectable='on'>&nbsp;");
                if (Hour.DetectTimeFormat(this.TimeFormat) == TimeFormat.Clock24Hours)
                {
                    stringBuilder.Append("00");
                }
                else
                {
                    stringBuilder.Append(flag ? "AM" : "PM");
                }
                stringBuilder.Append("</span>");
                stringBuilder.Append("</div>");
                return stringBuilder.ToString();
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.Append("<div unselectable='on'>");
            bool flag2 = hour / 12 == 0;
            if (Hour.DetectTimeFormat(this.TimeFormat) == TimeFormat.Clock12Hours)
            {
                hour %= 12;
                if (hour == 0)
                {
                    hour = 12;
                }
            }
            stringBuilder2.Append(hour);
            stringBuilder2.Append("<span class='");
            string value = string.IsNullOrEmpty(this.Theme) ? "calendar_default" : this.Theme;
            stringBuilder2.Append(value);
            stringBuilder2.Append("_rowheader_minutes");
            stringBuilder2.Append("' unselectable='on'>");
            if (Hour.DetectTimeFormat(this.TimeFormat) == TimeFormat.Clock24Hours)
            {
                stringBuilder2.Append("00");
            }
            else
            {
                stringBuilder2.Append(flag2 ? "AM" : "PM");
            }
            stringBuilder2.Append("</span>");
            stringBuilder2.Append("</div>");
            return stringBuilder2.ToString();
        }

        internal Hashtable GetEventMap(Event e)
        {
            BeforeEventRenderEventArgs eva = this.GetEva(e);
            Hashtable hashtable = new Hashtable();
            hashtable["id"] = e.Id;
            hashtable["text"] = e.Text;
            hashtable["start"] = e.Start.ToString("s");
            hashtable["end"] = e.End.ToString("s");
            hashtable["resource"] = e.ResourceId;
            hashtable["tag"] = e.Tags;
            hashtable["allday"] = e.AllDay;
            hashtable["sort"] = e.Sort;
            if (e.Recurrent)
            {
                hashtable["recurrent"] = e.Recurrent;
                hashtable["recurrentMasterId"] = e.RecurrentMasterId;
            }
            if (eva.Areas.Count > 0)
            {
                hashtable["areas"] = eva.Areas.GetList();
            }
            if (!string.IsNullOrEmpty(eva.BubbleHtml))
            {
                hashtable["bubbleHtml"] = eva.BubbleHtml;
            }
            if (this.EventHeaderVisible)
            {
                hashtable["header"] = eva.HeaderHTML;
            }
            if (eva.Html != e.Text)
            {
                hashtable["html"] = eva.Html;
            }
            if (eva.ToolTip != e.Text)
            {
                hashtable["toolTip"] = eva.ToolTip;
            }
            if (this.CssOnly)
            {
                if (!string.IsNullOrEmpty(eva.BackgroundColor))
                {
                    hashtable["backColor"] = eva.BackgroundColor;
                }
                if (!string.IsNullOrEmpty(eva.BorderColor))
                {
                    hashtable["borderColor"] = eva.BorderColor;
                }
            }
            else
            {
                if (eva.BackgroundColor != ColorTranslator.ToHtml(this.EventBackColor) || eva.IsAllDay)
                {
                    hashtable["backColor"] = eva.BackgroundColor;
                }
                if (eva.BorderColor != ColorTranslator.ToHtml(this.EventBorderColor))
                {
                    hashtable["borderColor"] = eva.BorderColor;
                }
            }
            if (!string.IsNullOrEmpty(eva.BackgroundImage))
            {
                hashtable["backgroundImage"] = eva.BackgroundImage;
            }
            if (!string.IsNullOrEmpty(eva.BackgroundRepeat))
            {
                hashtable["backgroundRepeat"] = eva.BackgroundRepeat;
            }
            if (eva.DurationBarColor != ColorTranslator.ToHtml(this.DurationBarColor))
            {
                hashtable["barColor"] = eva.DurationBarColor;
            }
            if (!string.IsNullOrEmpty(eva.DurationBarBackColor))
            {
                hashtable["barBackColor"] = eva.DurationBarBackColor;
            }
            if (eva.FontColor != ColorTranslator.ToHtml(this.EventFontColor))
            {
                hashtable["fontColor"] = eva.FontColor;
            }
            if (this.ContextMenu == null)
            {
                if (!string.IsNullOrEmpty(eva.ContextMenuClientName))
                {
                    hashtable["contextMenu"] = eva.ContextMenuClientName;
                }
            }
            else if (eva.ContextMenuClientName != this.ContextMenu.ClientObjectName)
            {
                hashtable["contextMenu"] = eva.ContextMenuClientName;
            }
            if (!string.IsNullOrEmpty(eva.CssClass))
            {
                hashtable["cssClass"] = eva.CssClass;
            }
            if (!string.IsNullOrEmpty(eva.DurationBarImageUrl) && eva.DurationBarImageUrl != this.DurationBarImageUrl)
            {
                hashtable["durationBarImageUrl"] = eva.DurationBarImageUrl;
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
            if (!eva.EventResizeEnabled)
            {
                hashtable["resizeDisabled"] = true;
            }
            if (!eva.EventRightClickEnabled)
            {
                hashtable["rightClickDisabled"] = true;
            }
            if (!eva.EventDeleteEnabled)
            {
                hashtable["deleteDisabled"] = true;
            }
            return hashtable;
        }

        internal void LoadColumns()
        {
            this._columns = new List<Hashtable>();
            ColumnCollection columnCollection = (this.ViewType == ViewTypeEnum.Resources) ? this.Columns : this.DaysModeColumns;
            foreach (Column column in columnCollection)
            {
                Hashtable mapFromColumn = this.GetMapFromColumn(column);
                this._columns.Add(mapFromColumn);
            }
        }

        internal List<Hashtable> GetColumns()
        {
            return this._columns;
        }

        internal Column GetColumn(int x)
        {
            if (this._columns == null)
            {
                throw new Exception("Columns not initialized.");
            }
            if (this.ViewType == ViewTypeEnum.Resources)
            {
                List<Column> columns = this.Columns.GetColumns(this.HeaderLevels, true);
                return columns[x];
            }
            return this.DaysModeColumns[x];
        }

        private Hashtable GetMapFromColumn(Column column)
        {
            BeforeHeaderRenderEventArgs bhrea = this.GetBhrea(column);
            Hashtable hashtable = new Hashtable();
            hashtable["id"] = bhrea.Id;
            hashtable["name"] = bhrea.Name;
            hashtable["start"] = bhrea.Date;
            hashtable["toolTip"] = bhrea.ToolTip;
            hashtable["html"] = bhrea.Html;
            hashtable["backColor"] = bhrea.BackgroundColor;
            hashtable["areas"] = bhrea.Areas.GetList();
            if (column.Width != 0)
            {
                hashtable["width"] = column.Width;
            }
            if (column.Children.Count > 0)
            {
                List<Hashtable> list = new List<Hashtable>();
                foreach (Column column2 in column.Children)
                {
                    list.Add(this.GetMapFromColumn(column2));
                }
                hashtable["children"] = list;
            }
            return hashtable;
        }

        internal BeforeEventRenderEventArgs GetEva(Event e)
        {
            BeforeEventRenderEventArgs beforeEventRenderEventArgs = new BeforeEventRenderEventArgs(e, this.TagFields, this.ServerTagFields);
            string text = e.Start.ToShortTimeString();
            string text2 = e.End.ToShortTimeString();
            if (e.Start.Date != e.End.Date)
            {
                text = e.Start.ToShortDateString() + " " + text;
                text2 = e.End.ToShortDateString() + " " + text2;
            }
            if (this.EventHeaderVisible)
            {
                beforeEventRenderEventArgs.HeaderHTML = e.Start.ToShortTimeString();
            }
            if (!this.CssOnly)
            {
                beforeEventRenderEventArgs.BorderColor = ColorTranslator.ToHtml(this.EventBorderColor);
                beforeEventRenderEventArgs.FontColor = ColorTranslator.ToHtml(this.EventFontColor);
                beforeEventRenderEventArgs.DurationBarColor = ColorTranslator.ToHtml(this.DurationBarColor);
                beforeEventRenderEventArgs.DurationBarImageUrl = this.DurationBarImageUrl;
                if (e.AllDay)
                {
                    beforeEventRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.AllDayEventBackColor);
                }
                else
                {
                    beforeEventRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.EventBackColor);
                }
            }
            beforeEventRenderEventArgs.EventClickEnabled = (this.EventClickHandling != EventClickHandlingEnum.Disabled);
            beforeEventRenderEventArgs.EventMoveEnabled = (this.EventMoveHandling != UserActionHandling.Disabled);
            beforeEventRenderEventArgs.EventResizeEnabled = (this.EventResizeHandling != UserActionHandling.Disabled);
            beforeEventRenderEventArgs.EventDeleteEnabled = (this.EventDeleteHandling != UserActionHandling.Disabled);
            beforeEventRenderEventArgs.EventRightClickEnabled = (this.EventRightClickHandling != EventRightClickHandlingEnum.Disabled);
            beforeEventRenderEventArgs.EventDoubleClickEnabled = (this.EventDoubleClickHandling != EventClickHandlingEnum.Disabled);
            if (this.ContextMenu != null)
            {
                beforeEventRenderEventArgs.ContextMenuClientName = this.ContextMenu.ClientObjectName;
            }
            if (this.ShowEventStartEnd && !e.AllDay)
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
            if (e.Recurrent)
            {
                beforeEventRenderEventArgs.Html = string.Format("<img src='{0}' /> {1}", e.RecurrentException ? this.ResolveUrlSafe(this.RecurrentEventExceptionImage) : this.ResolveUrlSafe(this.RecurrentEventImage), beforeEventRenderEventArgs.Html);
            }
            if (this.BeforeEventRender != null)
            {
                this.BeforeEventRender(this, beforeEventRenderEventArgs);
            }
            return beforeEventRenderEventArgs;
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

        private BeforeHeaderRenderEventArgs GetBhrea(Column c)
        {
            BeforeHeaderRenderEventArgs beforeHeaderRenderEventArgs = new BeforeHeaderRenderEventArgs();
            beforeHeaderRenderEventArgs.Id = c.Id;
            beforeHeaderRenderEventArgs.Name = c.Name;
            beforeHeaderRenderEventArgs.Date = c.Date;
            if (beforeHeaderRenderEventArgs.Date == DateTime.MinValue)
            {
                beforeHeaderRenderEventArgs.Date = this.StartDate;
            }
            beforeHeaderRenderEventArgs.Html = beforeHeaderRenderEventArgs.Name;
            beforeHeaderRenderEventArgs.ToolTip = c.ToolTip;
            if (!this.CssOnly)
            {
                beforeHeaderRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(this.HourNameBackColor);
            }
            if (this.BeforeHeaderRender != null)
            {
                this.BeforeHeaderRender(this, beforeHeaderRenderEventArgs);
            }
            return beforeHeaderRenderEventArgs;
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
            if (base.DesignMode)
            {
                return;
            }
            this._databindCalled = true;
            base.PerformDataBinding(retrievedData);
            this.Items = new ArrayList();
            if (retrievedData == null)
            {
                return;
            }
            List<RecurEx> list = new List<RecurEx>();
            List<RecurEvent> list2 = new List<RecurEvent>();
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
            if (this.ViewType == ViewTypeEnum.Resources && string.IsNullOrEmpty(this.DataColumnField))
            {
                throw new NullReferenceException("DataColumnField property must be specified if ViewType is set to ViewTypeEnum.Resources.");
            }
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
                if (this.ViewType == ViewTypeEnum.Resources || (!(@event.End < this.StartDate) && !(@event.Start >= this.StartDate.AddDays((double)this.Days).AddHours((double)this.DayBeginsHour))))
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
                foreach (Occurrence current4 in current2.Occurrences(this.StartDate.AddDays((double)this.Days).AddHours((double)this.DayBeginsHour)))
                {
                    Event event3 = new Event(current4.Start, current4.End, current4.Id, event2.Text, event2.ResourceId, event2.Tags, event2.ServerTags, event2.AllDay);
                    event3.Source = event2.Source;
                    if (current4.Modified)
                    {
                        event3 = (Event)current4.Tag;
                    }
                    event3.RecurrentMasterId = event2.Id;
                    event3.Recurrent = true;
                    if (event3.End >= this.StartDate)
                    {
                        this.Items.Add(event3);
                    }
                }
            }
        }

        private Event ParseDataItem(object dataItem)
        {
            DateTime start = Convert.ToDateTime(DataBinder.GetPropertyValue(dataItem, this.DataStartField, null));
            DateTime end = Convert.ToDateTime(DataBinder.GetPropertyValue(dataItem, this.DataEndField, null));
            string text = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.DataTextField, null));
            string id = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.DataIdField, null));
            string columnId = null;
            if (this.ViewType == ViewTypeEnum.Resources)
            {
                columnId = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.DataColumnField, null));
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
                    string text2 = Convert.ToString(DataBinder.GetPropertyValue(dataItem, this.SortFields.Fields[k], null));
                    DateTime dateTime;
                    if (DateTime.TryParse(text2, out dateTime))
                    {
                        array3[k] = dateTime.ToString("s");
                    }
                    else
                    {
                        array3[k] = text2;
                    }
                }
            }
            bool allDay = false;
            if (!string.IsNullOrEmpty(this.DataAllDayField))
            {
                string propertyValue = DataBinder.GetPropertyValue(dataItem, this.DataAllDayField, null);
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    allDay = BoolParser.Parse(propertyValue);
                }
            }
            return new Event(start, end, id, text, columnId, array, array2, allDay)
            {
                Source = dataItem,
                Sort = array3
            };
        }

        private string[] LoadTagFields(string spec)
        {
            if (string.IsNullOrEmpty(spec))
            {
                return new string[0];
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

        internal CellTable.Cell GetCell(DateTime from, string resourceId)
        {
            BeforeCellRenderEventArgs beforeCellRenderEventArgs = new BeforeCellRenderEventArgs();
            beforeCellRenderEventArgs.Start = from;
            beforeCellRenderEventArgs.End = from.AddMinutes((double)this.CellDuration);
            beforeCellRenderEventArgs.ResourceId = resourceId;
            bool flag = this.IsBusinessCell(from);
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
                beforeCellRenderEventArgs.BackgroundColor = text;
            }
            if (this.BeforeCellRender != null)
            {
                this.BeforeCellRender(this, beforeCellRenderEventArgs);
                if (beforeCellRenderEventArgs.BackgroundColor == text && beforeCellRenderEventArgs.IsBusiness != flag)
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

        string ICallbackEventHandler.GetCallbackResult()
        {
            if (this._callbackException == null)
            {
                string result;
                try
                {
                    this.LoadColumns();
                    Hashtable hashtable = new Hashtable();
                    hashtable["Message"] = this._callbackMessage;
                    hashtable["CallBackData"] = this._callbackData;
                    hashtable["ClientState"] = this.ClientState;
                    List<JsonData> selectedEvents = this.GetSelectedEvents();
                    hashtable["SelectedEvents"] = selectedEvents;
                    if (this._callbackUpdateType == CallBackUpdateType.None)
                    {
                        hashtable["UpdateType"] = this._callbackUpdateType.ToString();
                        result = SimpleJsonSerializer.Serialize(hashtable);
                    }
                    else
                    {
                        List<Hashtable> events = this.GetEvents();
                        hashtable["Events"] = events;
                        if (this._callbackUpdateType == CallBackUpdateType.Auto || this._callbackUpdateType == CallBackUpdateType.Full)
                        {
                            Hashtable hashtable2 = new Hashtable();
                            CellTable cellTable = new CellTable(this);
                            cellTable.Process();
                            hashtable2["CellProperties"] = cellTable.GetProperties();
                            hashtable2["CellConfig"] = cellTable.GetConfig();
                            List<Hashtable> columns = this.GetColumns();
                            List<Hashtable> hours = this.GetHours();
                            hashtable2["Columns"] = columns;
                            hashtable2["Days"] = this.Days;
                            hashtable2["StartDate"] = this.StartDate.ToString("s");
                            hashtable2["CellDuration"] = this.CellDuration;
                            hashtable2["HeaderLevels"] = this.HeaderLevels;
                            hashtable2["ViewType"] = this.ViewType;
                            hashtable2["DayBeginsHour"] = this.DayBeginsHour;
                            hashtable2["DayEndsHour"] = this.DayEndsHour;
                            hashtable2["BusinessBeginsHour"] = this.BusinessBeginsHour;
                            hashtable2["BusinessEndsHour"] = this.BusinessEndsHour;
                            hashtable2["Hours"] = hours;
                            hashtable2["CornerHTML"] = this.CornerHtml;
                            hashtable2["CornerBackColor"] = this.CornerBackColor;
                            Hashtable hashtable3 = new Hashtable();
                            hashtable3["callBack"] = this.CallBack.GetHash();
                            hashtable3["colors"] = cellTable.GetHash();
                            hashtable3["columns"] = this.Hash(columns);
                            hashtable3["corner"] = this.CornerHash(this.CornerHtml, this.CornerBackColor);
                            hashtable3["events"] = this.Hash(events);
                            hashtable3["hours"] = this.Hash(hours);
                            hashtable3["selected"] = this.Hash(selectedEvents);
                            hashtable2["Hashes"] = hashtable3;
                            if (this._callbackUpdateType == CallBackUpdateType.Auto)
                            {
                                this._callbackUpdateType = (this.DifferentHashes(hashtable3) ? CallBackUpdateType.Full : CallBackUpdateType.EventsOnly);
                                if (this._callbackUpdateType == CallBackUpdateType.EventsOnly)
                                {
                                    bool flag = false;
                                    if (!this._databindCalled)
                                    {
                                        flag = true;
                                    }
                                    if (!this.DifferentHash(hashtable3, "events") && !this.DifferentHash(hashtable3, "selected"))
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        this._callbackUpdateType = CallBackUpdateType.None;
                                        hashtable.Remove("SelectedEvents");
                                        hashtable.Remove("Events");
                                    }
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
                        if (this._callbackUpdateType == CallBackUpdateType.EventsOnly)
                        {
                            Hashtable hashtable4 = new Hashtable();
                            hashtable4["events"] = this.Hash(events);
                            hashtable4["selectedEvents"] = this.Hash(selectedEvents);
                            hashtable["Hashes"] = hashtable4;
                        }
                        if (this._callbackUpdateType != CallBackUpdateType.None && this.EnableViewState)
                        {
                            using (StringWriter stringWriter = new StringWriter())
                            {
                                LosFormatter losFormatter = new LosFormatter();
                                losFormatter.Serialize(stringWriter, ViewStateHelper.ToHashtable(this.ViewState));
                                hashtable["VsUpdate"] = stringWriter.ToString();
                            }
                        }
                        hashtable["UpdateType"] = this._callbackUpdateType.ToString();
                        result = SimpleJsonSerializer.Serialize(hashtable);
                    }
                }
                catch (Exception ex)
                {
                    result = (HttpContext.Current.IsDebuggingEnabled ? ("$$$" + ex) : ("$$$" + ex.Message));
                }
                return result;
            }
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                return "$$$" + this._callbackException.Message;
            }
            return "$$$" + this._callbackException;
        }

        internal string CornerHash(string html, string back)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["html"] = html;
            hashtable["back"] = back;
            return this.Hash(hashtable);
        }

        internal string Hash(object data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(data));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }

        private bool DifferentHashes(Hashtable hashes)
        {
            bool result = false;
            foreach (string text in hashes.Keys)
            {
                if (!(text == "events") && !(text == "selected") && this.DifferentHash(hashes, text))
                {
                    result = true;
                }
            }
            return result;
        }

        private bool DifferentHash(Hashtable hashes, string key)
        {
            return (string)hashes[key] != (string)this._hashes[key];
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string ea)
        {
            this._callbackException = null;
            try
            {
                string a = ea.Substring(0, 4);
                if (!(a == "JSON"))
                {
                    throw new Exception("Unsupported CallBack data format.");
                }
                this.ExecuteEventJson(ea.Substring(4));
            }
            catch (Exception callbackException)
            {
                this._callbackException = callbackException;
                throw;
            }
        }

        private void ExecuteEventJson(string ea)
        {
            JsonData jsonData = SimpleJsonDeserializer.Deserialize(ea);
            JsonData jsonData2 = jsonData["header"];
            JsonData jsonData3 = jsonData["data"];
            JsonData parameters = jsonData["parameters"];
            this.StartDate = (DateTime)jsonData2["startDate"];
            this.Days = (int)jsonData2["days"];
            this._clientState = jsonData2["clientState"];
            this.ViewType = ViewTypeParser.Parse((string)jsonData2["viewType"]);
            this.CellDuration = (int)jsonData2["cellDuration"];
            this.SelectedEvents = EventInfo.ListFromJson(jsonData2["selected"]);
            this.Columns.RestoreFromJson(jsonData2["columns"]);
            this.DayBeginsHour = (int)jsonData2["dayBeginsHour"];
            this.DayEndsHour = (int)jsonData2["dayEndsHour"];
            this.BusinessBeginsHour = (int)jsonData2["businessBeginsHour"];
            this.BusinessEndsHour = (int)jsonData2["businessEndsHour"];
            this._hashes = new Hashtable();
            this._hashes["callBack"] = this.CallBack.GetHash();
            this._hashes["colors"] = (string)jsonData2["hashes"]["colors"];
            this._hashes["columns"] = (string)jsonData2["hashes"]["columns"];
            this._hashes["corner"] = (string)jsonData2["hashes"]["corner"];
            this._hashes["events"] = (string)jsonData2["hashes"]["events"];
            this._hashes["selectedEvents"] = (string)jsonData2["hashes"]["selectedEvents"];
            this._hashes["hours"] = (string)jsonData2["hashes"]["hours"];
            string key;
            if ((key = (string)jsonData["action"]) != null)
            {
                if (< PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x600103f - 1 == null)
				{

                    < PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x600103f - 1 = new Dictionary<string, int>(18)
                    {
                        {
                            "EventClick",
                            0
                        },
                        {
                            "EventDoubleClick",
                            1
                        },
                        {
                            "EventDelete",
                            2
                        },
                        {
                            "EventMove",
                            3
                        },
                        {
                            "EventResize",
                            4
                        },
                        {
                            "Command",
                            5
                        },
                        {
                            "EventRightClick",
                            6
                        },
                        {
                            "EventMenuClick",
                            7
                        },
                        {
                            "TimeRangeSelected",
                            8
                        },
                        {
                            "TimeRangeDoubleClick",
                            9
                        },
                        {
                            "TimeRangeMenuClick",
                            10
                        },
                        {
                            "EventEdit",
                            11
                        },
                        {
                            "EventSelect",
                            12
                        },
                        {
                            "HeaderClick",
                            13
                        },
                        {
                            "Notify",
                            14
                        },
                        {
                            "EventUpdate",
                            15
                        },
                        {
                            "EventRemove",
                            16
                        },
                        {
                            "EventAdd",
                            17
                        }
                    };
                }
                int num;
                if (< PrivateImplementationDetails >{ D8CB664A - ABD3 - 4BA3 - 88CD - 217DE0438151}.$$method0x600103f - 1.TryGetValue(key, out num))
				{
                    switch (num)
                    {
                        case 0:
                            if (this.EventClick != null)
                            {
                                EventClickEventArgs eventClickEventArgs = new EventClickEventArgs(parameters, jsonData3);
                                eventClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventClick(this, eventClickEventArgs);
                                return;
                            }
                            break;
                        case 1:
                            if (this.EventDoubleClick != null)
                            {
                                EventClickEventArgs eventClickEventArgs2 = new EventClickEventArgs(parameters, jsonData3);
                                eventClickEventArgs2.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventDoubleClick(this, eventClickEventArgs2);
                                return;
                            }
                            break;
                        case 2:
                            if (this.EventDelete != null)
                            {
                                EventDeleteEventArgs eventDeleteEventArgs = new EventDeleteEventArgs(parameters, jsonData3);
                                eventDeleteEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventDelete(this, eventDeleteEventArgs);
                                return;
                            }
                            break;
                        case 3:
                            if (this.EventMove != null)
                            {
                                EventMoveEventArgs eventMoveEventArgs = new EventMoveEventArgs(parameters, jsonData3);
                                eventMoveEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventMove(this, eventMoveEventArgs);
                                return;
                            }
                            break;
                        case 4:
                            if (this.EventResize != null)
                            {
                                EventResizeEventArgs eventResizeEventArgs = new EventResizeEventArgs(parameters, jsonData3);
                                eventResizeEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventResize(this, eventResizeEventArgs);
                                return;
                            }
                            break;
                        case 5:
                            if (this.Command != null)
                            {
                                HMS.Web.App.Ui.Events.CommandEventArgs commandEventArgs = new HMS.Web.App.Ui.Events.CommandEventArgs(parameters, jsonData3);
                                commandEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.Command(this, commandEventArgs);
                                return;
                            }
                            break;
                        case 6:
                            if (this.EventRightClick != null)
                            {
                                EventRightClickEventArgs eventRightClickEventArgs = new EventRightClickEventArgs(parameters, jsonData3);
                                eventRightClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventRightClick(this, eventRightClickEventArgs);
                                return;
                            }
                            break;
                        case 7:
                            if (this.EventMenuClick != null)
                            {
                                EventMenuClickEventArgs eventMenuClickEventArgs = new EventMenuClickEventArgs(parameters, jsonData3);
                                eventMenuClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventMenuClick(this, eventMenuClickEventArgs);
                                return;
                            }
                            break;
                        case 8:
                            if (this.TimeRangeSelected != null)
                            {
                                TimeRangeSelectedEventArgs timeRangeSelectedEventArgs = new TimeRangeSelectedEventArgs(parameters, jsonData3);
                                timeRangeSelectedEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.TimeRangeSelected(this, timeRangeSelectedEventArgs);
                                return;
                            }
                            break;
                        case 9:
                            if (this.TimeRangeDoubleClick != null)
                            {
                                TimeRangeDoubleClickEventArgs timeRangeDoubleClickEventArgs = new TimeRangeDoubleClickEventArgs(parameters, jsonData3);
                                timeRangeDoubleClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.TimeRangeDoubleClick(this, timeRangeDoubleClickEventArgs);
                                return;
                            }
                            break;
                        case 10:
                            if (this.TimeRangeMenuClick != null)
                            {
                                TimeRangeMenuClickEventArgs timeRangeMenuClickEventArgs = new TimeRangeMenuClickEventArgs(parameters, jsonData3);
                                timeRangeMenuClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.TimeRangeMenuClick(this, timeRangeMenuClickEventArgs);
                                return;
                            }
                            break;
                        case 11:
                            if (this.EventEdit != null)
                            {
                                EventEditEventArgs eventEditEventArgs = new EventEditEventArgs(parameters, jsonData3);
                                eventEditEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventEdit(this, eventEditEventArgs);
                                return;
                            }
                            break;
                        case 12:
                            if (this.EventSelect != null)
                            {
                                EventSelectEventArgs eventSelectEventArgs = new EventSelectEventArgs(parameters, jsonData3);
                                eventSelectEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventSelect(this, eventSelectEventArgs);
                                return;
                            }
                            break;
                        case 13:
                            if (this.HeaderClick != null)
                            {
                                HeaderClickEventArgs headerClickEventArgs = new HeaderClickEventArgs(parameters, jsonData3);
                                headerClickEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.HeaderClick(this, headerClickEventArgs);
                                return;
                            }
                            break;
                        case 14:
                            if (this.Notify != null)
                            {
                                NotifyEventArgs notifyEventArgs = new NotifyEventArgs(parameters, jsonData3);
                                notifyEventArgs.Source = EventSource.Notify;
                                this.Notify(this, notifyEventArgs);
                                return;
                            }
                            break;
                        case 15:
                            if (this.EventUpdate != null)
                            {
                                EventUpdateEventArgs eventUpdateEventArgs = new EventUpdateEventArgs(parameters, jsonData3);
                                eventUpdateEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventUpdate(this, eventUpdateEventArgs);
                                return;
                            }
                            break;
                        case 16:
                            if (this.EventRemove != null)
                            {
                                EventRemoveEventArgs eventRemoveEventArgs = new EventRemoveEventArgs(parameters, jsonData3);
                                eventRemoveEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventRemove(this, eventRemoveEventArgs);
                                return;
                            }
                            break;
                        case 17:
                            if (this.EventAdd != null)
                            {
                                EventAddEventArgs eventAddEventArgs = new EventAddEventArgs(parameters, jsonData3);
                                eventAddEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                                this.EventAdd(this, eventAddEventArgs);
                                return;
                            }
                            break;
                        default:
                            goto IL_7CE;
                    }
                    return;
                }
            }
            IL_7CE:
            throw new NotSupportedException("This action type is not supported: " + jsonData["action"]);
        }

        public void Update()
        {
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        public void UpdateWithMessage(string message)
        {
            this._callbackMessage = message;
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        public void Update(CallBackUpdateType updateType)
        {
            this._callbackUpdateType = updateType;
        }

        public void Update(object data)
        {
            this._callbackData = data;
            this._callbackUpdateType = CallBackUpdateType.Auto;
        }

        public void Update(object data, CallBackUpdateType updateType)
        {
            this._callbackData = data;
            this._callbackUpdateType = updateType;
        }

        internal bool IsRtl()
        {
            switch (this.Direction)
            {
                case TextDirection.Auto:
                    return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
                case TextDirection.RTL:
                    return true;
                case TextDirection.LTR:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void OnBeforeHeaderRender(BeforeHeaderRenderEventArgs ea)
        {
            if (this.BeforeHeaderRender != null)
            {
                this.BeforeHeaderRender(this, ea);
            }
        }

        internal void OnBeforeEventRender(BeforeEventRenderEventArgs ea)
        {
            if (this.BeforeEventRender != null)
            {
                this.BeforeEventRender(this, ea);
            }
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
            return this.Page.ClientScript.GetWebResourceUrl(typeof(HMSCalendar), "HMS.Resources." + file.Replace("/", "."));
        }

        private bool IsBusinessCell(DateTime start)
        {
            if (this.BusinessBeginsHour < this.BusinessEndsHour)
            {
                return start.Hour >= this.BusinessBeginsHour && start.Hour < this.BusinessEndsHour && start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday;
            }
            return start.Hour >= this.BusinessBeginsHour || start.Hour < this.BusinessEndsHour;
        }

        internal BeforeCellRenderEventArgs OnBeforeCellRender(DateTime start, string resource)
        {
            BeforeCellRenderEventArgs beforeCellRenderEventArgs = new BeforeCellRenderEventArgs();
            beforeCellRenderEventArgs.Start = start;
            beforeCellRenderEventArgs.End = start.AddMinutes((double)this.CellDuration);
            beforeCellRenderEventArgs.ResourceId = resource;
            bool flag = this.IsBusinessCell(start);
            beforeCellRenderEventArgs.IsBusiness = flag;
            string text = ColorTranslator.ToHtml(beforeCellRenderEventArgs.IsBusiness ? this.BackColor : this.NonBusinessBackColor);
            beforeCellRenderEventArgs.BackgroundColor = text;
            this.OnBeforeCellRender(beforeCellRenderEventArgs);
            if (beforeCellRenderEventArgs.BackgroundColor == text && beforeCellRenderEventArgs.IsBusiness != flag)
            {
                beforeCellRenderEventArgs.BackgroundColor = ColorTranslator.ToHtml(beforeCellRenderEventArgs.IsBusiness ? this.BackColor : this.NonBusinessBackColor);
            }
            return beforeCellRenderEventArgs;
        }

        internal void OnBeforeCellRender(BeforeCellRenderEventArgs cell)
        {
            if (this.BeforeCellRender != null)
            {
                this.BeforeCellRender(this, cell);
            }
        }

        internal TimeSpan Duration()
        {
            int num;
            if (this.HeightSpec == HeightSpecEnum.BusinessHoursNoScroll)
            {
                num = this.BusinessHoursSpan();
            }
            else
            {
                num = this.DayHoursSpan();
            }
            return TimeSpan.FromHours((double)num);
        }

        internal int BusinessHoursSpan()
        {
            if (this.BusinessBeginsHour > this.BusinessEndsHour)
            {
                return 24 - this.BusinessBeginsHour + this.BusinessEndsHour;
            }
            return this.BusinessEndsHour - this.BusinessBeginsHour;
        }

        internal int DayHoursSpan()
        {
            if (this.DayBeginsHour >= this.DayEndsHour)
            {
                return 24 - this.DayBeginsHour + this.DayEndsHour;
            }
            return this.DayEndsHour - this.DayBeginsHour;
        }
    }
}
