# ringrem CLI

Command-line tool for reminding about contacts based on last interaction time.

## Usage

ringrem <command> [options]

## Commands

### notify

Checks which people should be notified, then updates database.

ringrem notify

Options:
  --now           Runs notify but with date today + 100 years
  --dry-run       Does not modify any data
  --log           Print logs

Examples:
  ringrem notify
  ringrem notify --log
  ringrem notify --dry-run --now


### add person

Adds a new person to people.json.

ringrem add person --name <name> [options]

Options:
  --group <groupId> Adds person to group under this id, default is 0
  --desc <text>     Description / last conversation note, default is empty string
  --last <date>     Last spoke date, default is current date (ISO format, optional)

Example:
  ringrem add person --name "Jan Kowalski" --group 1 --desc "Project X"


### remove person

Removes a person from people.json.

ringrem remove person --id <personId>

Example:
  ringrem remove person --id 3


### add group

Adds a new group to groups.json. Interval stands for how many days should the
notification be sent. Notify hour is the hour at which the notification is sent.

ringrem add group --name <name> --interval <days> --notify-hour <hour> [options]

Options:
  --desc <text>     Description / reason of conversation note, default is empty string

Example:
  ringrem add group --name "Work" --interval 3 --notify-hour 9


### remove group

Removes a group from groups.json.

ringrem remove group <groupId> [opitons]

Note:
Removing a group does NOT automatically remove or reassign people
belonging to that group.

Options:
  --r               Delete group with people alinged with them
  --new <groupId>   Assign people to new group under id

Example:
  ringrem remove group 2
  ringrem remove group 1 --new 2
  ringrem remove group 1 --r

## Data files

The CLI reads and writes the following files:

- people.json
- groups.json

Files are expected to be located in the application base directory
(AppContext.BaseDirectory).

## Notes

- `notify` sends notifications and updates data
- `remove` commands are destructive and cannot be undone
- JSON files are expected to match the application data model
