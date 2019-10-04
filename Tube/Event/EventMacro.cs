namespace Glue.Event
{
    public class EventMacro : Event
    {
        public string MacroName { get; }

        public EventMacro(string macroName) : base()
        {
            MacroName = macroName;
        }
    }
}
