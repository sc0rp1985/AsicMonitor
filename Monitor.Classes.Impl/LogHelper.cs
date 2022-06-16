using Monitor.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Monitor.Classes.Impl
{
    public class LogHelper : ILogHelper
    {        
        IUnityContainer Cfg { get; set; }
        [Dependency]
        public СonnectionConfig СonnectionConfig { get; set; }
        

        [InjectionConstructor]
        public LogHelper(IUnityContainer _cfg)
        {
            Cfg = _cfg;
        }

        public async Task <string> GetJsonLog(MinerEnum miner)
        {
            var ip = СonnectionConfig.Ip ;
            var port = СonnectionConfig.Port;
            var logReader = GetLogReader(MinerEnum.Whatsminer, Cfg);
            var logParser = GetLogParser(MinerEnum.Whatsminer, Cfg);            

            var summaryLog = await logReader.ReadLog(ip, port, LogMode.Summary);
            var summaryJson = await logParser.Parse(summaryLog);
            var devLog = await logReader.ReadLog(ip,port , LogMode.Dev);
            var devJson = await logParser.Parse(devLog);

            var fullJson = $"{{ {summaryJson}, \"details\":[{devJson}] }}";
            return fullJson;
        }

        IMinerLogReader GetLogReader(MinerEnum miner, IUnityContainer cfg)
        {
            if (LogReaderCahce == null)
                LogReaderCahce = new Dictionary<MinerEnum, IMinerLogReader>();
            if (LogReaderCahce.TryGetValue(miner, out var result))
                return result;
            result = cfg.Resolve<IMinerLogReader>(miner.ToString());
            LogReaderCahce.Add(miner, result);
            return result;
        }

        IMinerLogParser GetLogParser(MinerEnum miner, IUnityContainer cfg)
        {
            if (LogParserCahce == null)
                LogParserCahce = new Dictionary<MinerEnum, IMinerLogParser>();
            if (LogParserCahce.TryGetValue(miner, out var result))
                return result;
            result = cfg.Resolve<IMinerLogParser>(miner.ToString());
            LogParserCahce.Add(miner, result);
            return result;
        }
        /// <summary>
        /// Кешируем обработчики, т.к. Cfg.Resolve занимает заметное время 
        /// </summary>
        [ThreadStatic]
        static Dictionary<MinerEnum, IMinerLogReader>? LogReaderCahce;
        [ThreadStatic]
        static Dictionary<MinerEnum, IMinerLogParser>? LogParserCahce;
    }
}
