using Glue.Triggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Glue.Triggers.JsonContract
{
    /// <summary>
    /// Helper class.  See <see cref="ActionConverter"/>.
    /// </summary>
    internal class TriggerContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Trigger).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
}
