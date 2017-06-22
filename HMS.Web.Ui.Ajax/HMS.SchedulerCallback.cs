using HMS.Web.App.Ui.Enums;
using HMS.Web.App.Ui.Json;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Web.App.Ui.Ajax
{
    public class HMSSchedulerCallBack
    {
        private HMSScheduler scheduler;

        public int CellDuration
        {
            get
            {
                return this.scheduler.CellDuration;
            }
            set
            {
                this.scheduler.CellDuration = value;
            }
        }

        public GroupByEnum CellGroupBy
        {
            get
            {
                return this.scheduler.CellGroupBy;
            }
            set
            {
                this.scheduler.CellGroupBy = value;
            }
        }

        public int CellWidth
        {
            get
            {
                return this.scheduler.CellWidth;
            }
            set
            {
                this.scheduler.CellWidth = value;
            }
        }

        public int Days
        {
            get
            {
                return this.scheduler.Days;
            }
            set
            {
                this.scheduler.Days = value;
            }
        }

        public ResourceCollection Resources
        {
            get
            {
                return this.scheduler.Resources;
            }
        }

        public SeparatorCollection Separators
        {
            get
            {
                return this.scheduler.Separators;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.scheduler.StartDate;
            }
            set
            {
                this.scheduler.StartDate = value;
            }
        }

        public int ScrollX
        {
            get
            {
                return this.scheduler.ScrollX;
            }
            set
            {
                this.scheduler.ScrollX = value;
            }
        }

        public int ScrollY
        {
            get
            {
                return this.scheduler.ScrollY;
            }
            set
            {
                this.scheduler.ScrollY = value;
            }
        }

        public string RowHeaderColumnWidths
        {
            get
            {
                return this.scheduler.RowHeaderColumnWidths;
            }
            set
            {
                this.scheduler.RowHeaderColumnWidths = value;
            }
        }

        internal HMSSchedulerCallBack(HMSScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        internal string GetHash()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["cellDuration"] = this.CellDuration;
            hashtable["cellGroupBy"] = this.CellGroupBy.ToString();
            hashtable["cellWidth"] = this.CellWidth;
            hashtable["days"] = this.Days;
            hashtable["resources"] = this.scheduler.GetResources();
            hashtable["scrollX"] = this.ScrollX;
            hashtable["scrollY"] = this.ScrollY;
            hashtable["startDate"] = this.StartDate.ToString("s");
            hashtable["rowHeaderCols"] = this.RowHeaderColumnWidths;
            byte[] bytes = Encoding.ASCII.GetBytes(SimpleJsonSerializer.Serialize(hashtable));
            return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(bytes));
        }
    }
}
