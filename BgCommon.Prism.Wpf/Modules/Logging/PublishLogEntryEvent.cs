using BgCommon.Prism.Wpf.Modules.Logging.Models;

namespace BgCommon.Prism.Wpf.Modules.Logging;

internal class PublishLogEntryEvent : PubSubEvent<List<LogEntry>>
{
}