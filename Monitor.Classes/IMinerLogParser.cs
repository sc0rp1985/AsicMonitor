using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitor.Const;

namespace Monitor.Classes
{
    public interface IMinerLogParser
    {
        /// <summary>
        /// возвращает лог в виде json определенного формата <see cref="LogSummaryJsonFields"/>
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task<string> Parse(string log);
    }
}
