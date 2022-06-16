using Monitor.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Monitor.Classes.Impl
{
    public static class ContainerConfig_Ext
    {
        public static IUnityContainer Monitor_Classes(this IUnityContainer Cfg)
        {
            return Cfg
                // 
                .RegisterType<IMinerLogReader, WhatsminerLogReader>(MinerEnum.Whatsminer.ToString())
                .RegisterType<IMinerLogParser, WhatsminerLogParser>(MinerEnum.Whatsminer.ToString())
                .RegisterType<ILogHelper, LogHelper>();
        }
    }
}
