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

        public PropertyDuration(long durationMS) : base(durationMS.ToString())
        {
            this.stringValue = durationMS.ToString();
            this.value = durationMS;
        }

        public long GetValue()
        {
            return Value;
        }

        public override void Parse(string stringValue)
        {
            // TODO FormatDuration.MilisFromString should throw parsing exceptions
            Value = FormatDuration.Parse(stringValue);
        }

        public override string Format()
        {
            return FormatDuration.Format(Value);
        }
    }
}
