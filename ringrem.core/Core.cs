using ringrem.models;
using ringrem.interfaces;
namespace ringrem.core;

public class Core
{
    public static void Run(ICommand command, State state, ILog? log)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        var conf = DataIO.LoadData<Config>(configPath, log)[0];

        switch (command)
        {
            case RunNotifyCommand comm:
                if(comm.Now)
                    RunCheck(state.now.AddDays(36500.0), state.people, state.groups, log);
                else
                    RunCheck(state.now, state.people, state.groups, log);
                break;

            case AddPersonCommand comm:
                AddPerson(state.people, comm, conf, log);
                break;

            case RemovePersonCommand comm:
                RemovePerson(state.people, comm.Id, log);
                break;
            
            case AddGroupCommand comm:
                AddGroup(state.groups, comm, conf, log);
                break;

            case RemoveGroupCommand comm:
                RemoveGroup(state.groups, comm.Id, log);
                break;

            case ListCommand comm:
                // nothing ever happens
                break;

            default:
                throw new NotSupportedException();
        }
        DataIO.SaveData(configPath, new List<Config> { conf }, log);
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
    public static void AddPerson(List<Person> people, AddPersonCommand comm, Config conf, ILog? log)
    {
        conf.PeopleIdIncrement++;
        try
        {
           people.Add(new Person(conf.PeopleIdIncrement, 
                          comm.Name, 
                          comm.LastSpoke,
                          comm.Description ?? "",
                          comm.GroupId ?? 0)); 
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
        }
    }

    public static void RemovePerson(List<Person> people, int id, ILog? log)
    {
        log?.Log("Starting RemovePerson procedure");
        var searching = true;
        var i = 0; var len = people.Count();
        while (searching)
        {
            if(people[i].Id == id)
            {

                log?.Log($"{people[i]} is ...");
                people.RemoveAt(i);
                searching = false;
                log?.Log($"... deleted sucessfully.");
            }
            else if (i == len - 1)
            {
                searching = false;
                log?.Log($"There is no person of index {id}");
            }
            else
                i++;
        }
    }

    public static void AddGroup(List<Group> groups, AddGroupCommand comm, Config conf, ILog? log)
    {
        conf.GroupIdIncrement++;
        try
        {
           groups.Add(new Group(conf.GroupIdIncrement, 
                          comm.Name, 
                          comm.Description ?? "",
                          comm.IntervalDays,
                          comm.NotifyHour ?? 0)); 
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
        }
    }

    public static void RemoveGroup(List<Group> groups, int id, ILog? log)
    {
        log?.Log("Starting RemoveGroup procedure");
        var searching = true;
        var i = 0; var len = groups.Count();
        while (searching)
        {
            if(groups[i].Id == id)
            {

                log?.Log($"{groups[i]} is ...");
                groups.RemoveAt(i);
                searching = false;
                log?.Log($"... deleted sucessfully.");
            }
            else if (i == len - 1)
            {
                searching = false;
                log?.Log($"There is no group of index {id}");
            }
            else
                i++;
        }
    }
}
