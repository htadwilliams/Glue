using Glue.Util;

namespace Glue.PropertyIO
{
    public class PropertyDuration : Property<long>
    {
        public long Value { get => value; set => this.value = value; }
        private long value = 0L;

        public PropertyDuration(string stringValue) : base(stringValue)
        {
            this.propertyType = PropertyType.Duration;
            Parse(stringValue);
        }

        public long GetValue()
        {
            return Value;
        }

        public override void Parse(string stringValue)
        {
            // TODO FormatDuration.MilisFromString should throw parsing exceptions
            Value = FormatDuration.MillisFromString(stringValue);
        }

        public override string Format()
        {
            return FormatDuration.StringFromMillis(Value);
        }
    }
}
