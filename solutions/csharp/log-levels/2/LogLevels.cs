static class LogLine
{
    public static string Message(string logLine)
    {
        //Supongamos que recibimos un logLine como "[ERROR]: Stack overflow"
        string logMessage = logLine.Split(new[] { ':' }, 2)[1].Trim();
        return logMessage;
    }

    public static string LogLevel(string logLine)
    {
        //Supongamos que recibimos un logLine como "[ERROR]: Stack overflow"
        string logLevel = logLine.Split(new[] { ']' }, 2)[0].TrimStart('[').ToLower();
        return logLevel;
    }

    public static string Reformat(string logLine)
    {
        return $"{Message(logLine)} ({LogLevel(logLine)})";
    }
}
