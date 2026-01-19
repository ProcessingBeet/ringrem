namespace Models;
public record Person(int Id, string Name, string LastSpoke, string Description, int GroupId);
public record Group(int Id, string Name, string Description, double IntervalDays);

public interface ILog
{
    public void Log(string message);
}