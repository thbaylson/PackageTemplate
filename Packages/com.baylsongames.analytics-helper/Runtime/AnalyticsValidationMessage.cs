namespace BaylsonGames.Analytics
{
    public readonly struct AnalyticsValidationMessage
    {
        public AnalyticsValidationMessage(
            AnalyticsValidationSeverity severity,
            string message,
            string context = null)
        {
            Severity = severity;
            Message = message;
            Context = context;
        }

        public AnalyticsValidationSeverity Severity { get; }

        public string Message { get; }

        public string Context { get; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Context))
            {
                return string.Format("[{0}] {1}", Severity, Message);
            }

            return string.Format("[{0}] {1}: {2}", Severity, Context, Message);
        }
    }
}
