namespace Pliant.Runtime
{
    public class ParseEngineOptions
    {
        public ParseEngineOptions(bool loggingEnabled = false)
        {
            LoggingEnabled = loggingEnabled;
        }

        public bool LoggingEnabled { get; }
    }
}