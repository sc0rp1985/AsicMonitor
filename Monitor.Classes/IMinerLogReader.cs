namespace Monitor.Classes
{
    public enum LogMode 
    { 
        Summary,
        Dev
    }    

    public interface IMinerLogReader
    {
        Task<string> ReadLog(string ip, int port, LogMode mode);
    }
}