using Monitor.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Classes.Impl
{
    public class WhatsminerLogParser : IMinerLogParser
    {
        async public Task<string> Parse(string log)
        {
            if (log.Contains("Msg=Summary"))
                return await ParseSummary(log);
            else
                return await ParseDevs(log);
        }

        async Task<string> ParseSummary(string log) 
        {
            var tmpList = log.Split("|");
            var header = tmpList[0];
            var body = tmpList[1];
            
            var fieldDic = new Dictionary<string, string>();
            var i = 0;
            foreach (var field in LogerConst.WhatsminerSummaryLogFields)
            {
                if (i <= LogerConst.LogSummaryJsonFields.Count - 1)
                {
                    fieldDic.Add(field, LogerConst.LogSummaryJsonFields[i]);
                }
                i++;
            }
            var sb = await ParseLogBody(body, fieldDic);
            return sb.ToString();
        }

        async Task<StringBuilder> ParseLogBody(string body, Dictionary<string, string> fieldDic)
        {       
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();
                var bodyDetails = body.Split(",");
                var i = 0;
                foreach (var det in bodyDetails)
                {
                    var s = GetJsonString(det, fieldDic);
                    if (s != string.Empty)
                    {
                        sb.AppendLine(s);
                        if (i < fieldDic.Count - 1)
                            sb.Append(',');
                        i++;
                    }
                }
                return sb;
            });
        }

        async Task<string> ParseDevs(string log)
        {
            var tmpList = log.Split("|");
            //var header = tmpList[0];            
            var fieldDic = new Dictionary<string, string>();
            var i = 0;
            foreach (var field in LogerConst.WhatsminerDevsLogFields)
            {
                if (i <= LogerConst.LogDevsJsonFields.Count - 1)
                {
                    fieldDic.Add(field, LogerConst.LogDevsJsonFields[i]);
                }
                i++;
            }
            var sb = new StringBuilder();
            var j = 0;
            for ( i = 1; i < tmpList.Length-1; i++)
            {
                sb.AppendLine("{");                
                sb.Append(await ParseLogBody(tmpList[i],  fieldDic));
                sb.AppendLine("}");
                if(j<tmpList.Length-3)
                    sb.Append(',');
                j++;
            }
            return sb.ToString();
        }

        string GetJsonString(string logString,Dictionary<string,string> fields)
        {
            var pos = logString.IndexOf("=");
            if (pos > 0) 
            {
                var name = logString.Substring(0, pos);
                var jsonField = string.Empty;
                if (fields.TryGetValue(name, out jsonField))
                {
                    var val = logString.Substring(pos + 1);
                    return $"\"{jsonField}\":\"{val}\"";
                }
            }
            return string.Empty;
        }
    }
}
