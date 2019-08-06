using System.Collections.Generic;

namespace Glue.PropertyIO
{
    public class PropertyBag : Dictionary<string, IProperty>
    {
        public TProperty GetProperty<TProperty>(string propName)
        {
            TryGetValue(propName, out IProperty property);

            // TODO cast will throw if incorrect type is specified - catch and return null instead?
            return (TProperty) property;
        }
    }
}
