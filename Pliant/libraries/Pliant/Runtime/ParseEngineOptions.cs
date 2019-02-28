namespace Pliant.Runtime
{
    public class ParseEngineOptions
    {
        public ParseEngineOptions(bool optimizeRightRecursion = true, bool loggingEnabled = false)
        {
            OptimizeRightRecursion = optimizeRightRecursion;
            LoggingEnabled = loggingEnabled;
        }

        public bool LoggingEnabled { get; }
        public bool OptimizeRightRecursion { get; }
    }
}