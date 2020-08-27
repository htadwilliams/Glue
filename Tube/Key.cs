using System;
using System.Windows.Forms;

namespace Glue
{
    public class Key : IEquatable<Keys>
    {
        public string Display { get => display; set => display = value; }
        public bool Bindable { get => bindable; set => bindable = value; }
        public Keys Keys { get => keys; set => keys = value; }
        public Interceptor.Keys InterceptorKey { get => interceptorKey; set => interceptorKey = value; }

        private Keys keys;
        private string display;
        private bool bindable;
        private Interceptor.Keys interceptorKey;

        internal Key(Keys key, Interceptor.Keys interceptorKey, string display, bool bindable)
        {
            Keys = key;
            InterceptorKey = interceptorKey;
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
