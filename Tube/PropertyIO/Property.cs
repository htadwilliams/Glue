namespace Glue.PropertyIO
{
    /// <summary>
    /// Common but partial implementation for IProperty 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class Property<TValue> : IProperty
    {
        public string StringValue { get => stringValue; set => stringValue = value; }

        public PropertyType Type => this.propertyType;

        protected string stringValue = "";
        protected PropertyType propertyType;

        protected Property(string stringValue)
        {
            this.stringValue = stringValue;
        }

        //
        // Implementors must still supply these methods of IProperty
        public abstract void Parse(string stringValue);

        public abstract string Format();
    }
}
