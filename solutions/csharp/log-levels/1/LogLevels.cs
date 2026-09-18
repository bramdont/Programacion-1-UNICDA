static class LogLine
{
    public static string Message(string logLine)
    {
        //Supongamos que revibimos un logLine como "[ERROR]: Stack overflow"
        string logMessage = logLine.Split(new[] { ':' }, 2)[1].Trim();
        return logMessage;
    }

    public static string LogLevel(string logLine)
    {
        throw new NotImplementedException("Please implement the (static) LogLine.LogLevel() method");
    }

    public static string Reformat(string logLine)
    {
        throw new NotImplementedException("Please implement the (static) LogLine.Reformat() method");
    }
}
