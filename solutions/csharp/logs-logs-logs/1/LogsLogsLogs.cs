using System.Text.RegularExpressions;

public enum LogLevel
{
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Fatal,
    Unknown
}

static class LogLine
{
    public static LogLevel ParseLogLevel(string logLine)
    {
        string code = Regex.Match(logLine, @"\[(.*?)\]").Groups[1].Value;
        switch (code)
        {
            case "TRC":
                return LogLevel.Trace;
            case "DBG":
                return LogLevel.Debug;
            case "INF":
                return LogLevel.Info;
            case "WRN":
                return LogLevel.Warning;
            case "ERR":
                return LogLevel.Error;
            case "FTL":
                return LogLevel.Fatal;
            default:
                return LogLevel.Unknown;
        }
    }

    public static string OutputForShortLog(LogLevel logLevel, string message)
    {
        string level = "";
        switch (logLevel)
        {
            case LogLevel.Unknown:
                level = "0";
                break;
            case LogLevel.Trace:
                level = "1";
                break;
            case LogLevel.Debug:
                level = "2";
                break;
            case LogLevel.Info:
                level = "4";
                break;
            case LogLevel.Warning:
                level = "5";
                break;
            case LogLevel.Error:
                level = "6";
                break;
            case LogLevel.Fatal:
                level = "42";
                break;
        }
        return $"{level}:{message}";
    }
}
