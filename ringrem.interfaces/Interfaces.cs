using ringrem.models;

namespace ringrem.interfaces;
public record State(List<Person> people, List<Group> groups, string peoplePath, string groupsPath, DateTime now);
public interface ILog
{
    public void Log(string message);
    public void Write();
}

public class StandardLog : ILog
{
    public List<string> logs {get; set;} = new List<string>();

    public void Log(string message)
    {
        logs.Add(message);
    }

    public void Write()
    {
        foreach(var s in logs)
            Console.WriteLine(s);
    }
}

public interface ICommand{}

public record RunNotifyCommand(bool Now) : ICommand;
public record AddPersonCommand(string Name, int? GroupId, string? Description, DateTime LastSpoke) : ICommand;
public record RemovePersonCommand(int Id) : ICommand;
public record AddGroupCommand(string Name, double IntervalDays, double? NotifyHour, string? Description) : ICommand;
public record RemoveGroupCommand(int Id, bool Recursive, int NewGroupId) : ICommand;
public record ListCommand(bool Group, bool People) : ICommand;