using HMS.Json;
using HMS.Utils;
using HMS.Web.App.Ui.Ajax;
using HMS.Web.App.Ui.Design;
using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Enums.Navigator;
using HMS.Web.App.Ui.Events;
using HMS.Web.App.Ui.Events.Navigator;
using HMS.Web.App.Ui.Json;
using HMS.Web.App.Ui.Recurrence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Web.App;
using System.Web.App.UI;
using System.Web.App.UI.Web.AppControls;

namespace HMS.Web.App.Ui
{
    [DefaultProperty(null), ToolboxBitmap(typeof(Calendar)), ParseChildren(true), PersistChildren(false), Themeable(true)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HMSNavigator : DataBoundControl, IPostBackEventHandler, ICallbackEventHandler
        {
		private string dataStartField;

    private string dataEndField;

    private string _dataValueField;

    private string _dataServerTagFields;

    private string _dataRecurrenceField;

    private Hashtable _Items = new Hashtable();

    internal Control boundCalendar;

    internal Exception callbackException;

    [Category("User actions"), Description("Fires when a user clicks a time cell or selects cells by mouse dragging. TimeRangeSelectedHandling must be set to PostBack or CallBack.")]
    public event TimeRangeSelectedEventHandler TimeRangeSelected;

    [Category("User actions"), Description("Fires when a the visible time range is changed and VisibleRangeChangedHandling is set to true.")]
    public event VisibleRangeChangedEventHandler VisibleRangeChanged;

    [Category("Preprocessing"), Description("Use this event to apply custom recurrence rules.")]
    public event BeforeEventRecurrenceHandler BeforeEventRecurrence;

    [Category("Preprocessing"), Description("Use this event to customize day cells.")]
    public event HMS.Web.App.App.Ui.Events.Navigator.BeforeCellRenderEventHandler BeforeCellRender;

    internal string boundClientName
    {
        get
        {
            if (!string.IsNullOrEmpty(this.BoundHMSClientObjectName))
            {
                return this.BoundHMSClientObjectName;
            }
            if (this.boundCalendar is HMSCalendar)
            {
                return ((HMSCalendar)this.boundCalendar).ClientObjectName;
            }
            if (this.boundCalendar is HMSMonth)
            {
                return ((HMSMonth)this.boundCalendar).ClientObjectName;
            }
            if (this.boundCalendar is HMSScheduler)
            {
                return ((HMSScheduler)this.boundCalendar).ClientObjectName;
            }
            return null;
        }
    }
    internal string[] ServerTagFields
    {
        get
        {
            return this.LoadTagFields(this.DataServerTagFields);
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

    [Description("Determines the first day of week (Sunday/Monday/Auto).")]
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

    [DefaultValue(NavigatorSelectMode.Day), Description("Select mode (day/week/month).")]
    public NavigatorSelectMode SelectMode
    {
        get
        {
            if (this.ViewState["SelectMode"] == null)
            {
                return NavigatorSelectMode.Day;
            }
            return (NavigatorSelectMode)this.ViewState["SelectMode"];
        }
        set
        {
            this.ViewState["SelectMode"] = value;
        }
    }

    [DefaultValue(RowsPerMonth.Six), Description("How many rows per month should be displayed (always six or auto).")]
    public RowsPerMonth RowsPerMonth
    {
        get
        {
            if (this.ViewState["RowsPerMonth"] == null)
            {
                return RowsPerMonth.Six;
            }
            return (RowsPerMonth)this.ViewState["RowsPerMonth"];
        }
        set
        {
            this.ViewState["RowsPerMonth"] = value;
        }
    }

    [Category("Behavior"), Description("The month to be shown (only year and month parts are significant). Default is DateTime.Today.")]
    public DateTime StartDate
    {
        get
        {
            if (this.ViewState["StartDate"] == null)
            {
                return DateTime.Today;
            }
            return (DateTime)this.ViewState["StartDate"];
        }
        set
        {
            this.ViewState["StartDate"] = new DateTime(value.Year, value.Month, value.Day);
        }
    }

    [Category("Data"), Description("The name of the column that contains the event starting date and time (must be convertible to DateTime).")]
    public string DataStartField
    {
        get
        {
            return this.dataStartField;
        }
        set
        {
            this.dataStartField = value;
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
            return this.dataEndField;
        }
        set
        {
            this.dataEndField = value;
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

    [Category("Events"), DefaultValue(HMS.Web.App.App.Ui.Enums.Navigator.TimeRangeSelectedHandling.Bind), Description("Handling of user action (clicking a free-time slot).")]
    public HMS.Web.App.App.Ui.Enums.Navigator.TimeRangeSelectedHandling TimeRangeSelectedHandling
    {
        get
        {
            if (this.ViewState["TimeRangeSelectedHandling"] == null)
            {
                return HMS.Web.App.App.Ui.Enums.Navigator.TimeRangeSelectedHandling.Bind;
            }
            return (HMS.Web.App.App.Ui.Enums.Navigator.TimeRangeSelectedHandling)this.ViewState["TimeRangeSelectedHandling"];
        }
        set
        {
            this.ViewState["TimeRangeSelectedHandling"] = value;
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

    [Category("Layout"), DefaultValue(20), Description("Title height in pixels.")]
    public int TitleHeight
    {
        get
        {
            if (this.ViewState["TitleHeight"] == null)
            {
                return 20;
            }
            return (int)this.ViewState["TitleHeight"];
        }
        set
        {
            this.ViewState["TitleHeight"] = value;
        }
    }

    [Category("Layout"), DefaultValue(20), Description("Day header height in pixels.")]
    public int DayHeaderHeight
    {
        get
        {
            if (this.ViewState["DayHeaderHeight"] == null)
            {
                return 20;
            }
            return (int)this.ViewState["DayHeaderHeight"];
        }
        set
        {
            this.ViewState["DayHeaderHeight"] = value;
        }
    }

    [Category("Layout"), DefaultValue(20), Description("Cell height in pixels.")]
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

    [Category("Layout"), DefaultValue(20), Description("Cell width in pixels.")]
    public int CellWidth
    {
        get
        {
            if (this.ViewState["CellWidth"] == null)
            {
                return 20;
            }
            return (int)this.ViewState["CellWidth"];
        }
        set
        {
            this.ViewState["CellWidth"] = value;
        }
    }

    [Category("Behavior"), Description("ID of the HMS control that will be updated after selection change."), TypeConverter(typeof(CalendarControlConverter)), IDReferenceProperty(typeof(Control))]
    public string BoundHMSID
    {
        get
        {
            if (this.ViewState["BoundHMSID"] == null)
            {
                return null;
            }
            return (string)this.ViewState["BoundHMSID"];
        }
        set
        {
            this.ViewState["BoundHMSID"] = value;
        }
    }

    [Category("Behavior"), Description("Client ID (ClientObjectName) of the HMS control that will be updated after selection change.")]
    public string BoundHMSClientObjectName
    {
        get
        {
            if (this.ViewState["BoundHMSClientObjectName"] == null)
            {
                return null;
            }
            return (string)this.ViewState["BoundHMSClientObjectName"];
        }
        set
        {
            this.ViewState["BoundHMSClientObjectName"] = value;
        }
    }

    [Category("Behavior"), DefaultValue("navigate"), Description("A command that will be sent to the bound control when the selection changes.")]
    public string BindCommand
    {
        get
        {
            if (this.ViewState["BindCommand"] == null)
            {
                return "navigate";
            }
            return (string)this.ViewState["BindCommand"];
        }
        set
        {
            this.ViewState["BindCommand"] = value;
        }
    }

    [Category("Behavior"), DefaultValue(false), Description("Specifies action that should be performed after a user changes the visible time range.")]
    public UserActionHandling VisibleRangeChangedHandling
    {
        get
        {
            if (this.ViewState["VisibleRangeChangedHandling"] == null)
            {
                return UserActionHandling.Disabled;
            }
            return (UserActionHandling)this.ViewState["VisibleRangeChangedHandling"];
        }
        set
        {
            this.ViewState["VisibleRangeChangedHandling"] = value;
        }
    }

    [Category("Events"), DefaultValue("alert(start.toString() + '\\n' + end.toString());"), Description("Javascript code that is executed when the users changes the visible time range.")]
    public string VisibleRangeChangedJavaScript
    {
        get
        {
            if (this.ViewState["VisibleRangeChangedJavaScript"] == null)
            {
                return "alert(start.toString() + '\\n' + end.toString());";
            }
            return (string)this.ViewState["VisibleRangeChangedJavaScript"];
        }
        set
        {
            this.ViewState["VisibleRangeChangedJavaScript"] = value;
        }
    }

    [Category("Behavior"), Description("Determines the selection start date (the selection end will be calculated automatically).")]
    public DateTime SelectionStart
    {
        get
        {
            DateTime dateTime;
            if (this.ViewState["SelectionStart"] == null)
            {
                dateTime = DateTime.Today;
            }
            else
            {
                dateTime = (DateTime)this.ViewState["SelectionStart"];
            }
            switch (this.SelectMode)
            {
                case NavigatorSelectMode.Day:
                    return dateTime;
                case NavigatorSelectMode.Week:
                    return Week.FirstDayOfWeek(dateTime, this.ResolvedWeekStart);
                case NavigatorSelectMode.Month:
                    return new DateTime(dateTime.Year, dateTime.Month, 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        set
        {
            this.ViewState["SelectionStart"] = value;
        }
    }

    [Browsable(false)]
    public DateTime SelectionEnd
    {
        get
        {
            switch (this.SelectMode)
            {
                case NavigatorSelectMode.Day:
                    return this.SelectionStart;
                case NavigatorSelectMode.Week:
                    return this.SelectionStart.AddDays(6.0);
                case NavigatorSelectMode.Month:
                    return this.SelectionStart.AddMonths(1).AddDays(-1.0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Browsable(false)]
    public int Days
    {
        get
        {
            switch (this.SelectMode)
            {
                case NavigatorSelectMode.Day:
                    return 1;
                case NavigatorSelectMode.Week:
                    return 7;
                case NavigatorSelectMode.Month:
                    return (int)(this.SelectionEnd - this.SelectionStart).TotalDays + 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Category("Behavior"), DefaultValue(1), Description("How many months should be visible.")]
    public int ShowMonths
    {
        get
        {
            if (this.ViewState["ShowMonths"] == null)
            {
                return 1;
            }
            return (int)this.ViewState["ShowMonths"];
        }
        set
        {
            this.ViewState["ShowMonths"] = value;
        }
    }

    [Category("Behavior"), DefaultValue(1), Description("How many months to skip when clicking previous ('<') and next ('>') links.")]
    public int SkipMonths
    {
        get
        {
            if (this.ViewState["SkipMonths"] == null)
            {
                return 1;
            }
            return (int)this.ViewState["SkipMonths"];
        }
        set
        {
            this.ViewState["SkipMonths"] = value;
        }
    }

    internal Hashtable Items
    {
        get
        {
            if (this.StoreEventsInViewState)
            {
                return (Hashtable)this.ViewState["Items"];
            }
            return this._Items;
        }
        private set
        {
            this._Items = value;
            if (this.StoreEventsInViewState)
            {
                this.ViewState["Items"] = this._Items;
            }
        }
    }

    private DateTime firstDayOfMonth
    {
        get
        {
            return new DateTime(this.StartDate.Year, this.StartDate.Month, 1);
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

    [Description("The directory that contains static files (will be processed using ResolveUrl()).")]
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

    [Category("Behavior"), DefaultValue(false), Description("Whether week numbers should be displayed.")]
    public bool ShowWeekNumbers
    {
        get
        {
            return this.ViewState["ShowWeekNumbers"] != null && (bool)this.ViewState["ShowWeekNumbers"];
        }
        set
        {
            this.ViewState["ShowWeekNumbers"] = value;
        }
    }

    [DefaultValue(WeekNumberAlgorithm.Auto), Description("Which method to use for calculating the week number. 'Auto' selects US for WeekStarts=Sunday.")]
    public WeekNumberAlgorithm WeekNumberAlgorithm
    {
        get
        {
            if (this.ViewState["WeekNumberAlgorithm"] == null)
            {
                return WeekNumberAlgorithm.Auto;
            }
            return (WeekNumberAlgorithm)this.ViewState["WeekNumberAlgorithm"];
        }
        set
        {
            this.ViewState["WeekNumberAlgorithm"] = value;
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

    private DayOfWeek ResolvedWeekStart
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Category("Behavior"), Description("Gets the start of the visible range (date and time).")]
    public DateTime VisibleStart
    {
        get
        {
            return Week.FirstDayOfWeek(this.firstDayOfMonth, this.ResolvedWeekStart);
        }
    }

    [Category("Behavior"), Description("Gets the end of the visible range (date and time).")]
    public DateTime VisibleEnd
    {
        get
        {
            int num = 6;
            if (this.RowsPerMonth == RowsPerMonth.Auto)
            {
                DateTime dateTime = this.StartDate.AddMonths(this.ShowMonths - 1);
                DateTime day = new DateTime(dateTime.Year, dateTime.Month, 1);
                DateTime d = day.AddMonths(1).AddDays(-1.0);
                DateTime firstDayOfWeek = this.GetFirstDayOfWeek(day);
                int num2 = (int)Math.Floor((d - firstDayOfWeek).TotalDays) + 1;
                num = (int)Math.Ceiling((double)num2 / 7.0);
            }
            return Week.FirstDayOfWeek(this.firstDayOfMonth.AddMonths(this.ShowMonths - 1), this.ResolvedWeekStart).AddDays((double)(num * 7));
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

    [Category("Appearance"), DefaultValue(Orientation.Vertical), Description("Orientation of the months table (for ShowMonths > 1).")]
    public Orientation Orientation
    {
        get
        {
            if (this.ViewState["Orientation"] == null)
            {
                return Orientation.Vertical;
            }
            return (Orientation)this.ViewState["Orientation"];
        }
        set
        {
            this.ViewState["Orientation"] = value;
        }
    }

    protected override void Render(HtmlTextWriter output)
    {
        if (base.DesignMode)
        {
            this.designRender(output);
            return;
        }
        output.AddAttribute("id", this.ClientID);
        output.RenderBeginTag("div");
        output.RenderEndTag();
        JsInitNavigator jsInitNavigator = new JsInitNavigator(this);
        ScriptManagerHelper.RegisterStartupScript(this, typeof(HMSNavigator), this.ClientID + "object", jsInitNavigator.GetCode(), false);
    }

    private void designRender(HtmlTextWriter output)
    {
        for (int i = 0; i < this.ShowMonths; i++)
        {
            DateTime month = this.StartDate.AddMonths(i);
            this.designRenderMonth(output, month);
        }
    }

    private void designRenderMonth(HtmlTextWriter output, DateTime month)
    {
        output.AddAttribute("border", "0");
        output.AddAttribute("cellspacing", "0");
        output.AddAttribute("cellpadding", "0");
        output.RenderBeginTag("table");
        output.RenderBeginTag("tr");
        output.AddStyleAttribute("width", this.CellWidth + "px");
        output.AddStyleAttribute("height", this.TitleHeight + "px");
        output.RenderBeginTag("td");
        output.Write("&lt;");
        output.RenderEndTag();
        output.AddStyleAttribute("width", this.CellWidth * 5 + "px");
        output.AddStyleAttribute("height", this.TitleHeight + "px");
        output.AddAttribute("colspan", "5");
        output.AddAttribute("align", "center");
        output.RenderBeginTag("td");
        output.Write(month.ToString("MMMM yyyy"));
        output.RenderEndTag();
        output.AddStyleAttribute("width", this.CellWidth + "px");
        output.AddStyleAttribute("height", this.TitleHeight + "px");
        output.RenderBeginTag("td");
        output.Write("&gt;");
        output.RenderEndTag();
        output.RenderEndTag();
        ArrayList dayNames = Week.GetDayNames();
        output.RenderBeginTag("tr");
        for (int i = 0; i < 7; i++)
        {
            string value = ((string)dayNames[i]).Substring(0, 2);
            output.AddStyleAttribute("height", this.DayHeaderHeight + "px");
            output.AddStyleAttribute("width", this.CellWidth + "px");
            output.RenderBeginTag("td");
            output.Write(value);
            output.RenderEndTag();
        }
        output.RenderEndTag();
        for (int j = 0; j < 6; j++)
        {
            output.RenderBeginTag("tr");
            for (int k = 0; k < 7; k++)
            {
                output.RenderBeginTag("td");
                output.Write("1");
                output.RenderEndTag();
            }
            output.RenderEndTag();
        }
        output.RenderEndTag();
    }

    protected override void OnLoad(EventArgs e)
    {
        string text = this.Context.Request.Params[this.ClientID + "_state"];
        if (!string.IsNullOrEmpty(text))
        {
            JsonData jsonData = SimpleJsonDeserializer.Deserialize(text);
            this.StartDate = (DateTime)jsonData["startDate"];
            DateTime selectionStart = (DateTime)jsonData["selectionStart"];
            this.SelectionStart = selectionStart;
            DateTime arg_69_0 = this.SelectionStart;
        }
        ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSCalendar), "common.js", this.GetResourceUrl("Common.js"));
        ScriptManagerHelper.RegisterClientScriptInclude(this, typeof(HMSNavigator), "navigator.js", this.GetResourceUrl("Navigator.js"));
        if (!string.IsNullOrEmpty(this.BoundHMSID) && !string.IsNullOrEmpty(this.BoundHMSClientObjectName))
        {
            throw new ArgumentException("You can specify either BoundHMSID or BoundHMSClientObjectName but not both.");
        }
        if (!string.IsNullOrEmpty(this.BoundHMSID))
        {
            this.boundCalendar = this.findCalendar(this.BoundHMSID);
        }
        base.OnLoad(e);
    }

    private Control findCalendar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        Control control = HMSNavigator.recursiveFind(this.NamingContainer, id);
        if (control is HMSCalendar)
        {
            return control;
        }
        if (control is HMSMonth)
        {
            return control;
        }
        if (control is HMSScheduler)
        {
            return control;
        }
        throw new ArgumentException("The control specified in BoundHMSID is not a bindable HMS control.");
    }

    private static Control recursiveFind(Control parent, string id)
    {
        Control control = parent.FindControl(id);
        if (control != null)
        {
            return control;
        }
        foreach (Control parent2 in parent.Controls)
        {
            control = HMSNavigator.recursiveFind(parent2, id);
            if (control != null)
            {
                return control;
            }
        }
        return null;
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
        Hashtable hashtable = new Hashtable();
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
        List<RecurEx> list = new List<RecurEx>();
        List<RecurEvent> list2 = new List<RecurEvent>();
        foreach (object current in retrievedData)
        {
            string propertyValue = DataBinder.GetPropertyValue(current, this.DataStartField, null);
            DateTime dateTime;
            if (!DateTime.TryParse(propertyValue, out dateTime))
            {
                throw new FormatException(string.Format("Unable to convert '{0}' (from DataStartField column) to DateTime.", propertyValue));
            }
            string propertyValue2 = DataBinder.GetPropertyValue(current, this.DataEndField, null);
            DateTime dateTime2;
            if (!DateTime.TryParse(propertyValue2, out dateTime2))
            {
                throw new FormatException(string.Format("Unable to convert '{0}' (from DataEndField column) to DateTime.", propertyValue2));
            }
            DateTime t = dateTime.Date;
            while (t < dateTime2)
            {
                hashtable[t.ToString("s")] = 1;
                t = t.AddDays(1.0);
            }
            if (dateTime == dateTime2)
            {
                hashtable[dateTime.ToString("s")] = 1;
            }
            string text = null;
            if (!string.IsNullOrEmpty(this.DataRecurrenceField))
            {
                text = DataBinder.GetPropertyValue(current, this.DataRecurrenceField, null);
            }
            if (string.IsNullOrEmpty(text))
            {
                if (this.BeforeEventRecurrence != null)
                {
                    if (string.IsNullOrEmpty(this.DataIdField))
                    {
                        throw new ArgumentException("DataIdField is required for recurrent events.");
                    }
                    string propertyValue3 = DataBinder.GetPropertyValue(current, this.DataIdField, null);
                    string[] array = null;
                    if (this.ServerTagFields != null)
                    {
                        array = new string[this.ServerTagFields.Length];
                        for (int i = 0; i < this.ServerTagFields.Length; i++)
                        {
                            array[i] = Convert.ToString(DataBinder.GetPropertyValue(current, this.ServerTagFields[i], null));
                        }
                    }
                    BeforeEventRecurrenceEventArgs beforeEventRecurrenceEventArgs = new BeforeEventRecurrenceEventArgs(dateTime, dateTime2, propertyValue3, array, this.ServerTagFields);
                    this.BeforeEventRecurrence(beforeEventRecurrenceEventArgs);
                    if (beforeEventRecurrenceEventArgs.Rule != null)
                    {
                        RecurEvent recurEvent = RecurEvent.FromRule(beforeEventRecurrenceEventArgs.Rule, dateTime, dateTime2, propertyValue3, null);
                        recurEvent.FirstDayOfWeek = this.ResolvedWeekStart;
                        list2.Add(recurEvent);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.DataIdField))
                {
                    throw new ArgumentException("DataIdField is required for recurrent events.");
                }
                string propertyValue4 = DataBinder.GetPropertyValue(current, this.DataIdField, null);
                RecurInfo recurInfo = RecurInfo.Parse(text, propertyValue4, dateTime, dateTime2, null);
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
                RecurType arg_2B4_0 = recurInfo.Type;
            }
        }
        foreach (RecurEvent current2 in list2)
        {
            foreach (RecurEx current3 in list)
            {
                current2.AddRecurexSilent(current3);
            }
            foreach (Occurrence current4 in current2.Occurrences(this.VisibleEnd))
            {
                DateTime t2 = current4.Start.Date;
                while (t2 < current4.End)
                {
                    hashtable[t2.ToString("s")] = 1;
                    t2 = t2.AddDays(1.0);
                }
                if (current4.Start == current4.End)
                {
                    hashtable[current4.Start.ToString("s")] = 1;
                }
            }
        }
        this.Items = hashtable;
    }

    public void RaisePostBackEvent(string eventArgument)
    {
        string a = eventArgument.Substring(0, 4);
        if (a != "JSON")
        {
            throw new ArgumentException("Invalid PostBack format. Missing JSON identifier.");
        }
        this.ExecuteEventJSON(eventArgument.Substring(4));
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
        return this.Page.ClientScript.GetWeb.AppResourceUrl(typeof(HMSBubble), "HMS.Resources." + file);
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        this.callbackException = null;
        try
        {
            string a = eventArgument.Substring(0, 4);
            if (a != "JSON")
            {
                throw new ArgumentException("Unexpected callback format.");
            }
            this.ExecuteEventJSON(eventArgument.Substring(4));
        }
        catch (Exception ex)
        {
            this.callbackException = ex;
            throw;
        }
    }

    private DateTime GetFirstDayOfWeek(DateTime day)
    {
        switch (this.WeekStarts)
        {
            case WeekStartsEnum.Sunday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Sunday);
            case WeekStartsEnum.Monday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Monday);
            case WeekStartsEnum.Tuesday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Tuesday);
            case WeekStartsEnum.Wednesday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Wednesday);
            case WeekStartsEnum.Thursday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Thursday);
            case WeekStartsEnum.Friday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Friday);
            case WeekStartsEnum.Saturday:
                return Week.FirstDayOfWeek(day, DayOfWeek.Saturday);
            case WeekStartsEnum.Auto:
                return Week.FirstDayOfWeek(day);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ExecuteEventJSON(string ea)
    {
        JsonData jsonData = SimpleJsonDeserializer.Deserialize(ea);
        JsonData jsonData2 = jsonData["header"];
        JsonData arg_1E_0 = jsonData["data"];
        JsonData jsonData3 = jsonData["parameters"];
        this.StartDate = (DateTime)jsonData2["startDate"];
        this.SelectionStart = (DateTime)jsonData2["selectionStart"];
        string a;
        if ((a = (string)jsonData["action"]) != null)
        {
            if (!(a == "Visible"))
            {
                if (!(a == "TimeRangeSelected"))
                {
                    goto IL_125;
                }
                if (this.TimeRangeSelected != null)
                {
                    DateTime start = (DateTime)jsonData3["start"];
                    DateTime end = (DateTime)jsonData3["end"];
                    TimeRangeSelectedEventArgs timeRangeSelectedEventArgs = new TimeRangeSelectedEventArgs(start, end, null);
                    timeRangeSelectedEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                    this.TimeRangeSelected(this, timeRangeSelectedEventArgs);
                    return;
                }
            }
            else if (this.VisibleRangeChanged != null)
            {
                VisibleRangeChangedEventArgs visibleRangeChangedEventArgs = new VisibleRangeChangedEventArgs();
                visibleRangeChangedEventArgs.Source = (this.Page.IsCallback ? EventSource.CallBack : EventSource.PostBack);
                this.VisibleRangeChanged(this, visibleRangeChangedEventArgs);
                return;
            }
            return;
        }
        IL_125:
        throw new NotSupportedException("This action type is not supported: " + jsonData["action"]);
    }

    public string GetCallbackResult()
    {
        if (this.callbackException == null)
        {
            string result;
            try
            {
                Hashtable hashtable = new Hashtable();
                hashtable["Items"] = this.Items;
                hashtable["Cells"] = this.GetCells();
                if (this.EnableViewState)
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        LosFormatter losFormatter = new LosFormatter();
                        losFormatter.Serialize(stringWriter, ViewStateHelper.ToHashtable(this.ViewState));
                        hashtable["VsUpdate"] = stringWriter.ToString();
                    }
                }
                result = SimpleJsonSerializer.Serialize(hashtable);
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
            return "$$$" + this.callbackException;
        }
        return "$$$" + this.callbackException.Message;
    }

    internal Dictionary<string, Hashtable> GetCells()
    {
        Dictionary<string, Hashtable> dictionary = new Dictionary<string, Hashtable>();
        DateTime dateTime = this.VisibleStart;
        while (dateTime < this.VisibleEnd)
        {
            HMS.Web.App.App.Ui.Events.Navigator.BeforeCellRenderEventArgs beforeCellRenderEventArgs = new HMS.Web.App.App.Ui.Events.Navigator.BeforeCellRenderEventArgs();
            beforeCellRenderEventArgs.Start = dateTime;
            beforeCellRenderEventArgs.End = dateTime.AddDays(1.0);
            this.DoBeforeCellRender(beforeCellRenderEventArgs);
            if (beforeCellRenderEventArgs.IsDirty)
            {
                Hashtable hashtable = new Hashtable();
                hashtable["html"] = beforeCellRenderEventArgs.InnerHTML;
                hashtable["css"] = beforeCellRenderEventArgs.CssClass;
                dictionary[dateTime.ToString("s")] = hashtable;
            }
            dateTime = dateTime.AddDays(1.0);
        }
        return dictionary;
    }

    internal void DoBeforeCellRender(HMS.Web.App.App.Ui.Events.Navigator.BeforeCellRenderEventArgs ea)
    {
        if (this.BeforeCellRender != null)
        {
            this.BeforeCellRender(this, ea);
        }
    }
}


}

