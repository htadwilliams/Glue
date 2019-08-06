namespace Glue.PropertyIO
{
    public interface IProperty
    {
        /// <summary>
        /// Returns human formatted string according to PropertyIO.PropertyType,
        /// possibly adjusted by the current locale for units etc.
        /// </summary>
        /// <returns></returns>
        string Format();

        /// <summary>
        /// Implementors should set StringValue with input from this method if
        /// parse succeeds, and throw if parsing fails.
        /// </summary>
        /// <param name="stringValue"></param>
        void Parse(string stringValue);

        string StringValue
        {
            get;
        }

        PropertyType Type
        {
            get;
        }
    }
}
