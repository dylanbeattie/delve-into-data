using Catalyst;
using Catalyst.Models;
using Mosaik.Core;

English.Register();

Storage.Current = new DiskStorage("catalyst-models");
var nlp = await Pipeline.ForAsync(Language.English);

var talks = LoadTalks("data").ToList();
Console.WriteLine($"Loaded {talks.Count} talks!");
Console.WriteLine(talks.Sum(t => t.Description.Split(' ').Length) + " total words");

var words = new List<string>();
var counts = new Dictionary<string, Dictionary<string, int>>();
foreach (var talk in talks) {
	var doc = new Document(talk.Description, Language.English);
	nlp.ProcessSingle(doc);
	foreach (var lemma in doc.ToTokenList().Select(t => t.Lemma.ToLowerInvariant()).Distinct()) {
		if (!counts.ContainsKey(lemma)) counts[lemma] = new Dictionary<string, int>();
		if (!counts[lemma].ContainsKey("@total")) {
			counts[lemma]["@total"] = 1;
		} else {
			counts[lemma]["@total"]++;
		}
		if (!counts[lemma].ContainsKey(talk.EventName)) {
			counts[lemma][talk.EventName] = 1;
		} else {
			counts[lemma][talk.EventName]++;
		}
	}
}

Console.WriteLine(counts.Count + " unique words");
const int THRESHOLD = 4; // what percentage makes a talk "interesting" ?
var defaultColor = Console.ForegroundColor;
var totals = talks.GroupBy(t => t.EventName).ToDictionary(group => group.Key, group => group.Count());
totals["@total"] = talks.Count();
var events = talks.Select(t => t.EventName).Distinct().OrderBy(t => t).ToArray();
var headers = "Word,Total," + String.Join(',', events);
Console.WriteLine(headers);
foreach (var word in counts.OrderBy(g => g.Key)) {
	var totalPct = 100.00m * word.Value["@total"] / totals["@total"];
	var thisWordIsInteresting = word.Value
		.Where(e => e.Key != "@total")
		.Select(evt => 100.000m * evt.Value / totals[evt.Key])
		// Find any talk where a word occurs more than 2x the overall percentage,
		// AND occurs more than THRESHOLD percent at this event.
		.Any(pct => pct > 2 * totalPct && pct > THRESHOLD);
	if (thisWordIsInteresting) {
		Console.Write($"{word.Key},{totalPct:0.000}");
		foreach (var key in events) {
			Console.Write(",");
			if (word.Value.ContainsKey(key)) {
				var pct = 100.000m * word.Value[key] / totals[key];
				Console.Write($"{pct:0.000}");
			} else {
				Console.Write("0.000");
			}
		}
		Console.WriteLine();
	}
}

static IEnumerable<Talk> LoadTalks(string path) {
	foreach (var file in Directory.GetFiles(path, "*.txt")) {
		var eventName = Path.GetFileNameWithoutExtension(file);
		foreach (var description in File.ReadAllLines(file)) {
			yield return new Talk {
				EventName = eventName,
				Description = description
			};
		}
	}
}
