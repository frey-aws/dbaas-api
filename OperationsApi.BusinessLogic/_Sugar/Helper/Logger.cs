using System;
using System.IO;

using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.BusinessLogic
{
    /// <summary>
    /// Logger:  Uses Log4Net and CloudWatchLogs.  Can be facaded to use a different implementation, but explicitly concrete at present
    /// </summary>
    internal class Logger
    {
        private static log4net.ILog _log;
        private static log4net.ILog log
        {
            get
            {
                if (null == _log)
                {
                    _log = log4net.LogManager.GetLogger(AppSetting.LOG4NET_LOGGER);  // TODO: Move to a configuration/appsetting
                    log4net.Config.XmlConfigurator.Configure();
                }

                return _log;
            }
        }

        private static AmazonCloudWatchLogsClient client = new AmazonCloudWatchLogsClient();

        internal static void Log(ICommandResult result)
        {                                 
            log.Info(result.PrimaryMessage);
            AddToCloudWatchLogs(result.PrimaryMessage, AppSetting.AWS_LOG_STREAM_ACTION);
        }

        internal static void Error(Exception ex)
        {
            var error = string.Concat(new string[]{ ex.Message, ex.StackTrace});
            
            log.Error(error);
            AddToCloudWatchLogs(error, AppSetting.AWS_LOG_STREAM_EXCEPTION);
        }        

        private static void AddToCloudWatchLogs(string message, string streamName)
        {
            DateTime now = DateTime.Now;

            try
            {
                var logEvent = new InputLogEvent
                {
                    Message = message,
                    Timestamp = now
                };

                var logEvents = new System.Collections.Generic.List<InputLogEvent>();
                logEvents.Add(logEvent);

                // TODO:  Need to add ability to check for the stream here if the configuration is changed ... 

                DescribeLogStreamsRequest lastStreamRequest = new DescribeLogStreamsRequest
                {
                    LogGroupName = AppSetting.AWS_LOG_GROUP_NAME,
                    LogStreamNamePrefix = streamName
                };

                var lastStreamResult = client.DescribeLogStreams(lastStreamRequest);
                var sequenceToken = lastStreamResult.LogStreams[0].UploadSequenceToken;
                
                PutLogEventsRequest request = new PutLogEventsRequest
                {
                    LogGroupName = AppSetting.AWS_LOG_GROUP_NAME,
                    LogStreamName = streamName,
                    LogEvents = logEvents,
                    SequenceToken = sequenceToken
                };

                var result = client.PutLogEvents(request);                

            }
            catch (Exception ex)
            {
                // if the CloudWatchLog write, log locally ...
                log.Error(ex);
            }
        }
    }
}
