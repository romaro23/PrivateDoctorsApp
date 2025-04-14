namespace PrivateDoctorsApp.Model
{
    internal class LogEventBase
    {
        public delegate void LogEventHandler(object sender, string action, string tableName);
        public event LogEventHandler LogEvent;
        public void OnLogEvent(string action, string tableName)
        {
            LogEvent?.Invoke(this, action, tableName);
        }
    }
}
