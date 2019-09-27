using System;
using System.Windows.Forms;

namespace Glue
{
    public class Key : IEquatable<Keys>
    {
        public string Display { get => display; set => display = value; }
        public bool Bindable { get => bindable; set => bindable = value; }
        public Keys Keys { get => keys; set => keys = value; }

        private Keys keys;
        private string display;
        private bool bindable;

        public Key(Keys key, string display, bool bindable)
        {
            this.Keys = key;
            Display = display;
            Bindable = bindable;
        }

        public bool Equals(Keys other)
        {
            return other == this.Keys;
        }

        public override string ToString()
        {
            if (null == display || display.Equals(""))
            {
                return Keys.ToString();
            }
            else
            {
                return display;
            }
        }
    }
}
