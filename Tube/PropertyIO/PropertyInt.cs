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
            // TODO Decide how IProperty and Property handle parsing exceptions
            // For now just throw whatever Int32 does when parsing
            value = Int32.Parse(stringValue);
        }

        public override string Format()
        {
            return value.ToString();
        }
    }
}
