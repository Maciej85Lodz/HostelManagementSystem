using HMS.Web.App.Ui.Enums;
using System;
using System.Drawing;

namespace HMS.Web.App.Ui
{
    [Serializable]
    public class Separator
    {
        private DateTime location;

        private Color color;

        private SeparatorLayer layer;

        private int width = 1;

        private int opacity = 100;

        public DateTime Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

        public SeparatorLayer Layer
        {
            get
            {
                return this.layer;
            }
            set
            {
                this.layer = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public int Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                this.opacity = value;
            }
        }

        public Separator()
        {
        }

        public Separator(DateTime location, Color color)
        {
            this.location = location;
            this.color = color;
        }
    }
}
