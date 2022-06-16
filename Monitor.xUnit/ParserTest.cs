using Monitor.Classes;
using Monitor.Classes.Impl;
using Monitor.Common;
using Monitor.Const;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Xunit;

namespace Monitor.xUnit
{
    public class ParserTest
    {

        UnityContainer cfg = new UnityContainer();
        public ParserTest()
        {
            cfg
                .RegisterInstance<IUnityContainer>(cfg)
                .Monitor_Classes();

        }

        [Fact]
        public async void ParseDevsTest()
        { 
            var reader = cfg.Resolve<IMinerLogReader>(MinerEnum.Whatsminer.ToString());
            var log = await reader.ReadLog("192.168.1.100", 4028, LogMode.Dev);
            var parser = cfg.Resolve<IMinerLogParser>(MinerEnum.Whatsminer.ToString());
            var json = parser.Parse(log);
            
        }

        [Fact]
        public async void ParserHelperTest()
        {
            var helper = cfg.Resolve<ILogHelper>();
            var fullJson = await helper.GetJsonLog(MinerEnum.Whatsminer);          
            
            var jObj = JObject.Parse(fullJson);
            //var details = jObj.Property("details");
            var i = 0;
            var details = jObj["details"];
            foreach (var det in details) 
            {
                var name = $"{det.SelectToken("Name")}{det.SelectToken("ID")}";
                Assert.True(name == $"SM{i}");
                i++;
            }

            var temp = string.Join(',', details.Select(x => x.SelectToken("Temperature")?.ToString() ?? string.Empty));

            var jw = new JsonWrapper(fullJson);
            var val = jw.GetValue("PowerFanSpeed");
            Assert.NotEmpty(val);
            val = jw.GetValue("kdjf");
            Assert.Empty(val);

            var tempList = jw.GetValuesFromJArray<decimal>("details", "Temperature");
            Assert.NotEmpty(tempList);

            tempList = jw.GetValuesFromJArray<decimal>("klfdlfl", "Temperature");
            Assert.Empty(tempList);
            
            tempList = jw.GetValuesFromJArray<decimal>("details", "kdkfdkdfk");
            Assert.Empty(tempList);
            foreach (var item in jw.GetValuesFromIEnumerable<decimal>("details", "Temperature"))
            {
                var a = item;
                Assert.NotNull(a);
            }
        }        

    }
}
