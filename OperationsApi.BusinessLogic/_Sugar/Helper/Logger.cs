using System;

using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.BusinessLogic
{
    /// <summary>
    /// LogHelper:  Concrete implemetation of a logger that uses AWS Cloudwatch can be facaded, extended, or overridden as warranted ...
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
                    _log = log4net.LogManager.GetLogger("RollingFileAppenderAll");
                    log4net.Config.XmlConfigurator.Configure();
                }

                return _log;
            }
        }

        private static AmazonCloudWatchLogsClient client = new AmazonCloudWatchLogsClient();        

        internal static void Log(ICommandResult result)
        {            
            // log locally ... then attempt to write to CloudWatch ... 
            log.Info(result.PrimaryMessage);
                     
            //PutLogEventsRequest request = new PutLogEventsRequest
            //{
            //    LogGroupName = AppSetting.AWS_LOG_GROUP_NAME,
            //    LogStreamName = 
            //    LogEvents = new System.Collections.Generic.List<InputLogEvent>().Add(
            //        )                
            //};
            
            //// TODO: Determine if this response needs to be handled
            //PutLogEventsResponse x = await client.PutLogEventsAsync(request);
        }

        internal static void Error(ICommandResult result, Exception ex)
        {
            // log locally ... then attempt to write to CloudWatch ... 
            log.Error(result.PrimaryMessage);

            //PutLogEventsRequest request = new PutLogEventsRequest
            //{
            //    LogGroupName = AppSetting.AWS_LOG_GROUP_NAME,
            //    LogStreamName = 
            //    LogEvents = new System.Collections.Generic.List<InputLogEvent>().Add(
            //        )                
            //};

            //// TODO: Determine if this response needs to be handled
            //PutLogEventsResponse x = await client.PutLogEventsAsync(request);
        }
    }
}
