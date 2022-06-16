namespace Monitor.Const
{
    public enum MinerEnum
    {
        Whatsminer,
    }

    public static class LogerConst 
    {
        /// <summary>
        /// поля итогового json лога summary
        /// </summary>
        public static  List<string> LogSummaryJsonFields = new()
        {
            "Elapsed","PowerMode","AvgHashRate", "Accepted", "Rejected","HardwareError","Temperature","FreqAvg",
            "FanSpeedIn","FanSpeedOut", "Voltage","Power","PowerFanSpeed", "PowerCurrent",
            "ChipTempMin","ChipTempMax","ChipTempAvg","Uptime",};
        /// <summary>
        /// поля итогового json файла лога devs - детализация по платам
        /// </summary>
        public static readonly List<string> LogDevsJsonFields = new()
        { 
            "ASC", "Name","ID","Slot","Enabled","Status","Temperature","ChipFrequency","FanSpeedIn","FanSpeedOut",
            "MHSav","MHS5s","MHS1m","MHS5m","MHS15m","Accepted","Rejected","Hardware Errors","Utility","LastSharePool",
            "LastShareTime","TotalMH","Diff1Work","DifficultyAccepted","DifficultyRejected","LastShareDifficulty","LastValidWork",
            "DeviceHardware","DeviceRejected","DeviceElapsed","UpfreqComplete","EffectiveChips","PCBSN","ChipTempMin","ChipTempMax",
            "ChipTempAvg" };
        /// <summary>
        /// поля лога summary whatsminer
        /// </summary>
        public static readonly List<string> WhatsminerSummaryLogFields = new()
        {
            "Elapsed","Power Mode","MHS av", "Accepted", "Rejected","Hardware Errors","Temperature","freq_avg",
            "Fan Speed In","Fan Speed Out", "Voltage","Power","Power Fanspeed", "Power Current",
            "Chip Temp Min","Chip Temp Max","Chip Temp Avg","Uptime"};

        /// <summary>
        /// поля лога devs whatsminer
        /// </summary>
        public static readonly List<string> WhatsminerDevsLogFields = new()
        { 
            "ASC","Name","ID","Slot","Enabled","Status","Temperature","Chip Frequency","Fan Speed In","Fan Speed Out",
            "MHS av","MHS 5s","MHS 1m","MHS 5m","MHS 15m","Accepted","Rejected","Hardware Errors","Utility","Last Share Pool",
            "Last Share Time","Total MH","Diff1 Work","Difficulty Accepted","Difficulty Rejected","Last Share Difficulty","Last Valid Work",
            "Device Hardware%","Device Rejected%","Device Elapsed","Upfreq Complete","Effective Chips","PCB SN","Chip Temp Min","Chip Temp Max",
            "Chip Temp Avg" };
    }
}