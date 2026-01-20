namespace ringrem.interfaces;
public interface ILog
{
    public void Log(string message);
}

public interface ICommand{}

public record RunNotifyCommand(bool Now, bool DryRun) : ICommand;
public record AddPersonCommand(string Name, int groupId, string Description, DateTime LastSpoke) : ICommand;
public record RemovePersonCommand(int Id) : ICommand;
public record AddGroupCommand(string Name, double IntervalDays, double NotifyHour, string Description) : ICommand;
public record RemoveGroupCommand(int Id, bool Recursive, int NewGroupId) : ICommand;

