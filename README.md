# delve-into-data
Ad-hoc frequency analysis with .NET and Catalyst.

`dotnet run` will build the project, analyse the sample data included in the project, and `Console.Write` the results as CSV-formatted text you can paste into Excel to make charts.

### Data files:

One file per event. Results are ordered by filename so use filenames which reflect the chronological order of your events.

Inside each file, one line per talk description.

#### data/2021-creaturecon.txt

```
Scary Cows
Spiders are Friends
Fun with Fish
Lions and Cats
```

#### data/2022-creaturecon.txt

```
Pigeons are Fun
Spiders are Scary
Cows are Cuddly
Cows are Smart
Lions vs Cows
```

and so on.
