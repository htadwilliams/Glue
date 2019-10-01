using System.Collections.ObjectModel;

namespace Glue.Actions
{
    public delegate void OnQueueChange(ReadOnlyCollection<Action> queue);

    public interface IActionScheduler
    {
        void Schedule(Action action);

        void Cancel(string name);

        event OnQueueChange QueueChangeEvent;
    }
}
