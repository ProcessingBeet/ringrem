namespace ringrem.models;
public record Person(int Id, string Name, DateTime LastSpoke, string Description, int GroupId)
{
    public int Id {get; init;} = Id;
    public string Name {get; set;} = Name;
    public DateTime LastSpoke {get; set;} = LastSpoke;
    public string Description {get; set;} = Description;
    public int GroupId {get; set;} = GroupId;
}
public record Group(int Id, string Name, string Description, double IntervalDays, double NotifyHour)
{
    public int Id {get; init;} = Id;
    public string Name {get; set;} = Name;
    public string Description {get; set;} = Description;
    public double IntervalDays {get; set;} = IntervalDays;
    public double NotifyHour {get; set;} = NotifyHour;
}