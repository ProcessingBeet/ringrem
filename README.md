# Call Reminder

## About

We often forget to call friends or family.  
Not calendar-style calls at fixed dates, but simple “call from time to time” check-ins.

This project is an application that **reminds you to call people**.

The idea is simple:
- you define people and groups,
- you define how often you want to stay in touch (daily, every 2 weeks, every 3 weeks, etc.),
- the app periodically checks your call history,
- if you haven’t called someone within the defined interval, it sends a notification.

---

## How it works

- The app stores:
  - people
  - groups
  - call frequency rules
- It (will integrate) integrates with:
  - contacts
  - call history (who you called and when)
- A background process runs every 30–60 minutes and:
  - checks who should be contacted,
  - sends a notification if a call is due.

This is **not a calendar app** — it’s a lightweight reminder system based on real call activity.

---

## Current state

- Core logic is to be fully implemented
- UI is **not ready yet** (currently only CLI interface)
- Available only on **Linux** for now
- Configuration is stored in JSON files:
  - `people.json`
  - `groups.json`

These files must be placed in the appropriate binary/config directory.

---

## Installation (current, manual)

At the moment there is **no installer**.

To run the app:
1. Build the binary
2. Configure a system timer (e.g. a systemd timer) to periodically run it
3. Place `people.json` and `groups.json` next to the compiled binary or in the expected config directory

This will be simplified in the future.

---

## Planned features

- Proper GUI
- Android version (with contacts and call log integration)
- Installer
- More user-friendly configuration

---

## Status

This is an **early-stage project** focused on core functionality.  
Expect rough edges.