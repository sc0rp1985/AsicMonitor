using Monitor.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Classes
{
    public interface ILogHelper
    {
        Task<string> GetJsonLog(MinerEnum miner);
    }
}
