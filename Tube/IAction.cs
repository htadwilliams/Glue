namespace Glue
{
    public interface IAction
    {
        long TimeScheduledMS
        {
            get;
        }

        void Play();
        IAction[] Schedule();
    }
}
