namespace ringrem.core;

public class Core
{
    public static Models.Person Run()
    {
        Models.Person kuba = new(1, "3", "2", "3", 3);
        string peoplePath = Path.Combine(AppContext.BaseDirectory, "people.json");
        var data = DataIO.LoadData<Models.Person>(peoplePath);
        return kuba;
    }
}
