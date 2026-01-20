using ringrem.models;
using ringrem.interfaces;
namespace ringrem.core;

public class Core
{
    public static void Run(ICommand? command = null, ILog? log = null)
    {
        if(command is null)
            Console.WriteLine("Do stuff");

    }

    public static void RunCheck(ILog? log)
    {
        var now = DateTime.Now;
        log?.Log("RunCheck start.\nSearching data under paths:");
        string peoplePath = Path.Combine(AppContext.BaseDirectory, "people.json");
        string groupsPath = Path.Combine(AppContext.BaseDirectory, "groups.json");

        var people = DataIO.LoadData<Person>(peoplePath, log);
        var groups = DataIO.LoadData<Group>(groupsPath, log);

        var toNotify = WhoNotify(now, people, groups, log);
        Notify(toNotify, log);

        DataIO.SaveData(peoplePath, people, log);
    }
    public static List<Person> WhoNotify(DateTime currentTime, 
                                               List<Person> people,
                                               List<Group> groups,
                                               ILog? log)
    {
        log?.Log("Merging data...");
        var mergedData = from p in people
                         join g in groups on p.GroupId equals g.Id
                         select new { Person = p, Group = g };
        log?.Log("...done.");
        var toNotify = new List<Person>();
        foreach (var elt in mergedData)
        {
            DateTime lastSpoke = elt.Person.LastSpoke;
            DateTime notifyAt = lastSpoke.Date.AddDays(elt.Group.IntervalDays).AddHours(elt.Group.NotifyHour);
            if (currentTime >= notifyAt)
                toNotify.Add(elt.Person);
        }
        return toNotify;
    }

    public static bool Notify(List<Person> toNotify, ILog? log)
    {
        try
        {
            foreach(var elt in toNotify)
            {
                System.Diagnostics.Process.Start("notify-send", $"\"{elt.Name}\" \"{elt.Description}\"");
                elt.LastSpoke = DateTime.Now;
            }
            return true;
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
            return false;
        }
        
    }
}
