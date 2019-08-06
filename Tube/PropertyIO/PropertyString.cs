namespace Glue.PropertyIO
{
    /// <summary>
    /// String property is provided for raw passthrough with liittle or no validation
    /// Most operations will be a no-op
    /// </summary>
    public class PropertyString : Property<string>
    {
        public PropertyString(string stringValue) : base(stringValue)
        {
            this.propertyType = PropertyType.String;
        }

        public override string Format()
        {
            // Plain strings aren't formatted in any special way 
            // Could be used for filtering output e.g. don't allow HTML
            return this.StringValue;
        }

        public override void Parse(string stringValue)
        {
            // Plain strings allow any input but this could be used for
            // input filtering e.g. don't allow encoding of any kind
            this.StringValue = stringValue;
        }
    }
}
