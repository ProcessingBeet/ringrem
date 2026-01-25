using ringrem.models;
using ringrem.interfaces;
namespace ringrem.core;

public class Core
{
    public static void Run(ICommand command, State state, ILog? log)
    {

        switch (command)
        {
            case RunNotifyCommand comm:
                if(comm.Now)
                    RunCheck(state.now.AddDays(36500.0), state.people, state.groups, log);
                else
                    RunCheck(state.now, state.people, state.groups, log);
                break;

            case AddPersonCommand comm:
                Console.WriteLine("apc");
                break;

            case RemovePersonCommand comm:
                Console.WriteLine($"rpc {comm.Id}");
                break;
            
            case AddGroupCommand comm:
                Console.WriteLine("agc");
                break;

            case RemoveGroupCommand comm:
                Console.WriteLine("rgc");
                break;

            case ListCommand comm:
                Console.WriteLine("list");
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public static void RunCheck(DateTime now, List<Person> people, List<Group> groups, ILog? log)
    {
        var toNotify = WhoNotify(now, people, groups, log);
        Notify(toNotify, log);
    }
    public static List<Person> WhoNotify(   DateTime currentTime, 
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
    public static void AddPerson()
    {
        
    }
}
