using Monitor.Classes;
using Monitor.Common;
using Monitor.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Unity;

namespace AsicMonitor
{
    internal class Worker
    {
        IUnityContainer _cfg;
        [Dependency]
        public ILogHelper LogHelper { get; set; }

        public Worker(IUnityContainer cfg)
        {
            _cfg = cfg;
            _cfg.BuildUp(this);
        }

        public async void DoWork()
        {           
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync())
            {
                OnTimerTick();
            }
        }
        void OnTimerTick()
        {
            try
            {
                var json = LogHelper.GetJsonLog(MinerEnum.Whatsminer).Result;
                Console.WriteLine(FormatOutputString2(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Упало: " + ex.Message);
            }
        }

        string FormatOutputString2(string logJson)
        {
            var jw = new JsonWrapper(logJson);
            var sb = new StringBuilder();
            var elased = jw.GetValue("Elapsed");
            var uptime = jw.GetValue("Uptime");

            sb.Append($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - elapsed = ");
            sb.Append(TimeSpan.FromSeconds(Convert.ToUInt32(elased)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s"));
            sb.Append("| uptime = ");
            sb.Append(TimeSpan.FromSeconds(Convert.ToUInt32(uptime)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s"));
            sb.Append($@"| GHSav = {jw.GetValue("AvgHashRate")}| FanSpeedIn =  {jw.GetValue("FanSpeedIn")}| 
FanSpeedOut =  {jw.GetValue("FanSpeedOut")}| Power = {jw.GetValue("Power")}| PowerFanSpeed = {jw.GetValue("PowerFanSpeed")}| Temperature = {jw.GetValue("Temperature")}");
            sb.Append("(" + string.Join('|', jw.GetValuesFromJArray<decimal>("details", "Temperature")) + ")");
            return sb.ToString();
        }

        string FormatOutputString(string logJson)
        {
            JsonDocument jsDoc = JsonDocument.Parse(logJson);
            var root = jsDoc.RootElement;
            var sb = new StringBuilder();
            var elased = root.GetProperty("Elapsed").GetString();
            var uptime = root.GetProperty("Uptime").GetString();

            sb.Append($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - elapsed = ");
            sb.Append(TimeSpan.FromSeconds(Convert.ToUInt32(elased)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s"));
            sb.Append("| uptime = ");
            sb.Append(TimeSpan.FromSeconds(Convert.ToUInt32(uptime)).ToString(@"dd\d\ hh\h\ mm\m\ ss\s"));
            sb.Append($@"| GHSav = {root.GetProperty("AvgHashRate").GetString()}| FanSpeedIn =  {root.GetProperty("FanSpeedIn").GetString()}| 
FanSpeedOut =  {root.GetProperty("FanSpeedOut").GetString()}| Temperature = {root.GetProperty("Temperature").GetString()}");

            return sb.ToString();
        }
    }
}
