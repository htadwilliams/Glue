using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glue
{
    class Macro
    {
        private readonly long delayTimeMS;         // Time delay before playing first action
        private readonly List<Action> actions = new List<Action>();

        public Macro(long delayTimeMS)
        {
            this.delayTimeMS = delayTimeMS;
        }

        public Macro AddAction(Action action)
        {
            actions.Add(action);
            return this;
        }
    }
}
