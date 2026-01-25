using ringrem.models;

namespace ringrem.interfaces;
public record State(List<Person> people, List<Group> groups, string peoplePath, string groupsPath, DateTime now);
public interface ILog
{
    public void Log(string message);
}

public interface ICommand{}

public record RunNotifyCommand(bool Now) : ICommand;
public record AddPersonCommand(string Name, int? groupId, string? Description, DateTime LastSpoke) : ICommand;
public record RemovePersonCommand(int Id) : ICommand;
public record AddGroupCommand(string Name, double IntervalDays, double NotifyHour, string Description) : ICommand;
public record RemoveGroupCommand(int Id, bool Recursive, int NewGroupId) : ICommand;
public record ListCommand(bool group, bool people) : ICommand;