class Notifier
{
        public static void SendNotification(string title, string message)
    {
        System.Diagnostics.Process.Start("notify-send", $"\"{title}\" \"{message}\"");
    }
}
    