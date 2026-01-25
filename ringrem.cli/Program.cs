﻿using ringrem.core;
using ringrem.models;
using ringrem.interfaces;
using System.CommandLine;
using System.CommandLine.Parsing;

class Program
{
    static void Main(string[] args)
    {
        //Initializing CLI
        BuildRootCommand(args);

    }

    static void CoreMain(ICommand command, bool logFlag, bool dryRun)
    {
        //prepare log
        ILog? log = null;  
        if(logFlag)
            log = null;

        //prepare data
        var now = DateTime.Now;
        log?.Log("Run start.\nSearching data under paths:");
        string peoplePath = Path.Combine(AppContext.BaseDirectory, "people.json");
        string groupsPath = Path.Combine(AppContext.BaseDirectory, "groups.json");

        var people = DataIO.LoadData<Person>(peoplePath, log);
        var groups = DataIO.LoadData<Group>(groupsPath, log);

        //pack data to state
        var state = new State(people, groups, peoplePath, groupsPath, now);


        //Run core
        Core.Run(command, state, log);

        //write & modify data
        foreach(var elt in state.people)
            Console.WriteLine(elt);
        if(!dryRun)
          DataIO.SaveState(state, log);
    }

    static void BuildRootCommand(string[] args)
    {

        RootCommand rootCommand = new("Command-line tool for reminding about contacts based on last interaction time");

        var logFlag = false;
        var dryRunFlag = true;

        //notify subcommand
        Command notifyCommand = new("notify", "Checks which people should be notified and updates database");

        Option<bool> nowOption = new("--now")
        {
            Description = "Runs notify but with date today + 100 years"
        };

        notifyCommand.Options.Add(nowOption);

        notifyCommand.SetAction(parseResult =>
        {
            bool nowFlag = parseResult.GetValue(nowOption);

            var command = new RunNotifyCommand(nowFlag);
            CoreMain(command, logFlag, dryRunFlag);
        });

        rootCommand.Subcommands.Add(notifyCommand);

        //add subcommand
        Command addCommand = new("add", "Add person or group");
        rootCommand.Subcommands.Add(addCommand);

        //add person subcommand
        Argument<string> nameArgument = new("name");
        Argument<string> descriptionArgument = new("description");
        Option<string> lastSpokeOpt = new("--last")
        {
            Description = "Last spoke date (ISO 8601 format, e.g. 2024-01-31T14:30:00)"
        };
        Option<int> groupIdOpt = new("--group")
        {
            Description = "Id of group to which add the person"
        };

        Command addPersonCommand = new("person", "Adds new person");
        addPersonCommand.Arguments.Add(nameArgument); addPersonCommand.Arguments.Add(descriptionArgument);
        addPersonCommand.Options.Add(lastSpokeOpt); addPersonCommand.Options.Add(groupIdOpt);

        addPersonCommand.SetAction(parseResult =>
        {
            string? name = parseResult.GetValue(nameArgument);
            string? descr = parseResult.GetValue(descriptionArgument);
            DateTime date = string.IsNullOrEmpty(parseResult.GetValue(lastSpokeOpt)) 
                            ? DateTime.Now 
                            : DateTime.Parse(parseResult.GetValue(lastSpokeOpt));
            int? groupId = parseResult.GetValue(groupIdOpt);

            var command = new AddPersonCommand(name, groupId, descr, date);
            CoreMain(command, logFlag, dryRunFlag);
        });

        addCommand.Subcommands.Add(addPersonCommand);

        //add group subcommand
        Argument<double> intervalDaysArgument = new("day")
        {
            Description = "How much days interval should be there."
        }; 
        Argument<double> notifyHourArgument = new("hour")
        {
            Description = "At which hour the notification should appear"
        };

        Command addGroupCommand = new("group", "Adds new group");
        addGroupCommand.Arguments.Add(nameArgument); addGroupCommand.Arguments.Add(descriptionArgument);
        addGroupCommand.Arguments.Add(intervalDaysArgument); addGroupCommand.Arguments.Add(notifyHourArgument);

        addGroupCommand.SetAction(parseResult =>
        {
            string? name = parseResult.GetValue(nameArgument);
            string? descr = parseResult.GetValue(descriptionArgument);
            double intervalDays = parseResult.GetValue(intervalDaysArgument);
            double notifyHour = parseResult.GetValue(notifyHourArgument);

            var command = new AddGroupCommand(name, intervalDays, notifyHour, descr);
            CoreMain(command, logFlag, dryRunFlag);
        });

        addCommand.Subcommands.Add(addGroupCommand);

        //list subcommand
        Command listCommand = new("list", "List data in database");
        rootCommand.Subcommands.Add(listCommand);

        Option<bool> groupsOpt = new("groups")
        {
            Description = "Print groups table"
        };
        Option<bool> peopleOpt = new("people")
        {
            Description = "Print people table"
        };

        listCommand.Options.Add(groupsOpt); listCommand.Options.Add(peopleOpt);
        listCommand.SetAction(parseResult =>
        {
            bool groups = parseResult.GetValue(groupsOpt);
            bool people = parseResult.GetValue(peopleOpt);

            var command = new ListCommand(groups, people);
            CoreMain(command, logFlag, dryRunFlag);
        });

        // remove subcommand
        Command removeCommand = new("remove", "Remove person or group");
        rootCommand.Subcommands.Add(removeCommand);

        // remove person subcommand
        Argument<int> personIdArgument = new("id")
        {
            Description = "Id of person to remove"
        };

        Command removePersonCommand = new("person", "Removes person");
        removePersonCommand.Arguments.Add(personIdArgument);

        removePersonCommand.SetAction(parseResult =>
        {
            int personId = parseResult.GetValue(personIdArgument);

            var command = new RemovePersonCommand(personId);
            CoreMain(command, logFlag, dryRunFlag);
        });

        removeCommand.Subcommands.Add(removePersonCommand);

        // remove group subcommand 
        Argument<int> groupIdArgument = new("id")
        {
            Description = "Id of group to remove"
        };

        Option<bool> removeWithPeopleOpt = new("--r")
        {
            Description = "Remove group with people assigned to it"
        };

        Option<int> reassignGroupOpt = new("--new")
        {
            Description = "Assign people to another group id"
        };

        Command removeGroupCommand = new("group", "Removes group");
        removeGroupCommand.Arguments.Add(groupIdArgument);
        removeGroupCommand.Options.Add(removeWithPeopleOpt);
        removeGroupCommand.Options.Add(reassignGroupOpt);

        removeGroupCommand.SetAction(parseResult =>
        {
            int groupId = parseResult.GetValue(groupIdArgument);
            bool removeWithPeople = parseResult.GetValue(removeWithPeopleOpt);
            int newGroupId = parseResult.GetValue(reassignGroupOpt); //if not given, then newGroupId is 0

            //validation of data
            if (removeWithPeople && newGroupId != 0)
                throw new Exception("Error: cannot use --r together with --new.");
            if (removeWithPeople && newGroupId != 0)
                throw new Exception("Cannot use --r together with --new.");
            if (newGroupId == groupId)
                throw new Exception("Cannot reassign people to the same group.");
            
            var command = new RemoveGroupCommand(
                groupId,
                removeWithPeople,
                newGroupId
            );

            CoreMain(command, logFlag, dryRunFlag);
        });

        removeCommand.Subcommands.Add(removeGroupCommand);

        //invoking root command
        rootCommand.Parse(args).Invoke();
    }

}


