using System.Collections.Generic;

namespace Glue.PropertyIO
{
    public class PropertyBag : Dictionary<string, IProperty>
    {
        public bool TryGetProperty<TProperty>(string propName, out TProperty propertyOut)
        {
            bool wasFound = TryGetValue(propName, out IProperty property);

            propertyOut = (TProperty) property;

            return wasFound;
        }
    }
}
