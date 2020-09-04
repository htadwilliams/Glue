namespace Glue.Events
{
    /// <summary>
    /// Event containing a message to be displayed to the user. Intended to decouple GUI from engine.
    /// </summary>
    public class EventUserInfo
    {
        public string Message { get; }

        public EventUserInfo(string message)
        {
            Message = message;
        }
    }
}
