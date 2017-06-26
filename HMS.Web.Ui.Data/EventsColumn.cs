using System;
using System.Collections.Generic;


namespace HMS.Web.Ui.Data
{
    internal class EventsColumn
    {
        private List<EventPart> events = new List<EventPart>();

        internal Block Block;

        internal int WidthPct
        {
            get
            {
                if (this.Block == null)
                {
                    throw new ApplicationException("This EventColumn does not belong to any Block.");
                }
                if (this.Block.Columns.Count == 0)
                {
                    throw new ApplicationException("Internal error: Problem with Block.EventColumn.Counts (it is zero).");
                }
                if (this.isLastInBlock)
                {
                    return 100 / this.Block.Columns.Count + 100 % this.Block.Columns.Count;
                }
                return 100 / this.Block.Columns.Count;
            }
        }

        internal int StartsAtPct
        {
            get
            {
                if (this.Block == null)
                {
                    throw new ApplicationException("This EventColumn does not belong to any Block.");
                }
                if (this.Block.Columns.Count == 0)
                {
                    throw new ApplicationException("Internal error: Problem with Block.EventColumn.Counts (it is zero).");
                }
                return 100 / this.Block.Columns.Count * this.Number;
            }
        }

        private bool isLastInBlock
        {
            get
            {
                return this.Block.Columns[this.Block.Columns.Count - 1] == this;
            }
        }

        internal int Number
        {
            get
            {
                if (this.Block == null)
                {
                    throw new ApplicationException("This EventColumn doesn't belong to any Block.");
                }
                return this.Block.Columns.IndexOf(this);
            }
        }

        internal EventColumn()
        {
        }

        internal bool CanAdd(EventPart e)
        {
            foreach (EventPart current in this.events)
            {
                if (current.OverlapsWith(e))
                {
                    return false;
                }
            }
            return true;
        }

        internal void Add(EventPart e)
        {
            if (e.EvColumn != null)
            {
                throw new ApplicationException("This EventPart was already placed into a EventColumn.");
            }
            this.events.Add(e);
            e.EvColumn = this;
        }
        public class ApplicationException : Exception
        {
            public ApplicationException() : base(Environment.GetResourceString("Arg_ApplicationException"))
            {
                base.SetErrorCode(-2146232832);
            }

            public ApplicationException(string message) : base(message)
            {
                base.SetErrorCode(-2146232832);
            }

            public ApplicationException(string message, Exception innerException) : base(message, innerException)
            {
                base.SetErrorCode(-2146232832);
            }

            protected ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
        internal static string GetResourceString(string key)
        {
            return Environment.GetResourceFromDefault(key);
        }

    }
}
