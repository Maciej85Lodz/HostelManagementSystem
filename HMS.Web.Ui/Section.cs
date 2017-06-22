using System;
using System.Collections.Generic;


namespace HMS.Web.App.Ui
{
    public class Section
    {
        internal DateTime start;

        internal DateTime end;

        internal List<string> resources;

        public DateTime Start
        {
            get
            {
                return this.start;
            }
        }

        public DateTime End
        {
            get
            {
                return this.end;
            }
        }

        public List<string> Resources
        {
            get
            {
                return this.resources;
            }
        }

        internal Section()
        {
        }

        internal Section(DateTime start, DateTime end, List<string> resources)
        {
            this.start = start;
            this.end = end;
            this.resources = resources;
        }
    }
}
