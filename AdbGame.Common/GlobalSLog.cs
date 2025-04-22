using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdbGame.Common
{
    public class GlobalSLog
    {
        public static ILogger _log = GetLogger("Default", "");
        public static ILogger _logXYJ = GetLogger("少年西游记2", "");
        public static ILogger _logBHXX = GetLogger("笔绘西行", "");
        public static ILogger _logSGZ = GetLogger("三国志", "");

        public GlobalSLog() { }

        public static ILogger GetLogger(string logname)
        {
            switch (logname)
            {
                case "0":
                case "少年西游记2":
                    return _logXYJ;
                case "1":
                case "笔绘西行":
                    return _logBHXX;
                case "2":
                case "三国志":
                    return _logSGZ;
                case "Default":
                    return _log;
                default:
                    return _log;
            }
        }

        public static ILogger GetLogger(string logname, string para)
        {
            int[] result = null;
            string newname = logname;
            var SerilogOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            string LogFilePath(string LogEvent)
            {
                string folderPath = $@"{AppContext.BaseDirectory}Log\{DateTime.Now.ToString("yyyy_MM_dd")}\{newname}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return $@"{folderPath}\{newname}_{LogEvent}.log";
            }

            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            return loggerConfiguration.Enrich.FromLogContext()
                    .MinimumLevel.Debug() // 所有Sink的最小记录级别
                    .WriteTo.Logger(lg =>
                        lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.File(LogFilePath("Debug"),
                            rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                            retainedFileCountLimit: null))
                    .WriteTo.Logger(lg =>
                        lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(
                            LogFilePath("Information"), rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: null))
                    .WriteTo.Logger(lg =>
                        lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.File(
                            LogFilePath("Warning"), rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: null))
                    .WriteTo.Logger(lg =>
                        lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(LogFilePath("Error"),
                            rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                            retainedFileCountLimit: null))
                    .WriteTo.Logger(lg =>
                        lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.File(LogFilePath("Fatal"),
                            rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                            retainedFileCountLimit: null))
                    .CreateLogger();
        }

    }

    public static class GlobalSLogExtension
    {
        public static void Info(this ILogger log, string message)
        {
            log.Information(message);
        }

        public static void Error(this ILogger log, string message)
        {
            log.Information(message);
        }

        public static void Error(this ILogger log, string message, Exception ex)
        {
            log.Error(message, ex);
        }
    }
}
