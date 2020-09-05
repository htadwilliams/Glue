using System;

namespace Glue.PropertyIO
{
    public class PropertyInt : Property<int>
    {
        public int Value { get => value; set => this.value = value; }
        private int value;

        public PropertyInt(string stringValue) : base(stringValue)
        {
            this.propertyType = PropertyType.Int;
            Parse(stringValue);
        }

        public override void Parse(string stringValue)
        {
            value = Int32.Parse(stringValue);
        }

        public override string Format()
        {
            return value.ToString();
        }
    }
}
