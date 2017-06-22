using HMS.Json;
using HMS.Web.App.Ui.Enums.Calendar;
using System;
using System.Security.Cryptography;
using System.Text;
namespace HMS.Web.App.Ui.Ajax
{
    public class HMSCalendarCallBack
	{
		private readonly HMSCalendar _calendar;

    public int CellDuration
    {
        get
        {
            return this._calendar.CellDuration;
        }
        set
        {
            this._calendar.CellDuration = value;
        }
    }

    public int Days
    {
        get
        {
            return this._calendar.Days;
        }
        set
        {
            this._calendar.Days = value;
        }
    }

    public DateTime StartDate
    {
        get
        {
            return this._calendar.StartDate;
        }
        set
        {
            this._calendar.StartDate = value;
        }
    }

    public ViewTypeEnum ViewType
    {
        get
        {
            return this._calendar.ViewType;
        }
        set
        {
            this._calendar.ViewType = value;
        }
    }

    internal HMSCalendarCallBack(HMSCalendar calendar)
    {
        this._calendar = calendar;
    }

    internal string GetHash()
    {
        JsonData jsonData = new JsonData();
        jsonData["cellDuration"] = this.CellDuration;
        jsonData["days"] = this.Days;
        jsonData["startDate"] = this.StartDate.ToString("s");
        jsonData["viewType"] = this.ViewType.ToString();
        byte[] bytes = Encoding.ASCII.GetBytes(jsonData.ToJson());
        return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
    }
}
}
