// Lab 10
// Student name: Ella Gladders
// Student number: 78754249

using System;
using System.Collections.Generic;

Console.WriteLine("CPEN223 Lab 10");

//Testing: Write some test cases to test well all methods you are to implement
//         This is to demonstrates what test cases you have considered
//TODO
// print whether each test passes
void Check(string name, bool passed)
{
    Console.WriteLine($"{name}: {(passed ? "PASS" : "FAIL")}");
}

// example data includes diff sensors, unsorted times, repeated timestamps, and samples at both window boundaries
var samples = new List<Sample>
{
    new Sample("A", 4, 8),
    new Sample("B", 2, 4),
    new Sample("A", 0, 6),
    new Sample("A", 2, 1),
    new Sample("A", 2, 3),
    new Sample("A", 5, 9),
    new Sample("a", 2, 7)
};

// save the original contents to check for changes later
var original = new List<Sample>(samples);

// select sensor A from min 0 inclusive to min 5 exclusive
var result = WindowSelector.SelectWindow(samples, "A", 0, 5);

// expect A at minutes 4, 0, 2, 2 in that order 
Check("correct records and order",
    result.Count == 4 &&
    result[0] == samples[0] &&
    result[1] == samples[2] &&
    result[2] == samples[3] &&
    result[3] == samples[4]);

// both samples at minute 2 should remain
var duplicates = WindowSelector.SelectWindow(samples, "A", 2, 3);

Check("duplicate times preserved",
    duplicates.Count == 2 &&
    duplicates[0] == samples[3] &&
    duplicates[1] == samples[4]);

// lowercase sensor name should not match uppercase
var lowercase = WindowSelector.SelectWindow(samples, "a", 0, 5);

Check("case-sensitive names",
    lowercase.Count == 1 && lowercase[0] == samples[6]);

// equal start and end times makes an empty window
Check("equal bounds give empty result",
    WindowSelector.SelectWindow(samples, "A", 2, 2).Count == 0);

// sensor that does not exist returns empty list
Check("unknown sensor gives empty result",
    WindowSelector.SelectWindow(samples, "X", 0, 5).Count == 0);

// a window contianing none of the recorded times should be empty
Check("no matching times gives empty result",
    WindowSelector.SelectWindow(samples, "A", 10, 20).Count == 0);

// empty input returns a new empty list
var empty = new List<Sample>();
var emptyResult = WindowSelector.SelectWindow(empty, "A", 0, 5);

Check("empty input gives a new empty list",
    emptyResult.Count == 0 &&
    !ReferenceEquals(empty, emptyResult));

// returned list should be a different object from the input
Check("result is a new list", !ReferenceEquals(samples, result));

// compare input with saved copy to check for any changes
bool unchanged = samples.Count == original.Count;
for (int i = 0; unchanged && i < samples.Count; i++)
{
    unchanged = samples[i] == original[i];
}

Check("input unchanged", unchanged);
//end Testing code

//Do not change the program skeleton: keep the record structs, the class name,
//method names, parameters, and return types exactly as given.
//You may add private helper methods.

public readonly record struct Sample(
    string Sensor,
    int Minute,
    double Value);

public static class WindowSelector
{
    /// <summary>
    /// Selects records for one sensor in a half-open time window.
    /// </summary>
    /// <param name="samples">
    /// Non-null. Each Sample.Sensor is non-null and each Value is
    /// finite. Minute is measured in integer minutes since the
    /// beginning of the fictional session.
    /// </param>
    /// <param name="sensor">Non-null, nonempty sensor name.</param>
    /// <param name="startInclusive">Inclusive start (minutes).</param>
    /// <param name="endExclusive">
    /// Exclusive end (minutes); at least startInclusive.
    /// </param>
    /// <returns>
    /// A new list of Sample records with matching Sensor (ordinal,
    /// case-sensitive equality) and Minute in [startInclusive,
    /// endExclusive). Preserve input order and duplicate times.
    /// Return an empty list when none match.
    /// </returns>
    /// <remarks>
    /// Requires: all input conditions above must hold.
    /// Effects: neither the input nor its elements are modified.
    /// No outcome is specified for invalid calls.
    /// </remarks>
    public static IReadOnlyList<Sample> SelectWindow(
        IReadOnlyList<Sample> samples,
        string sensor,
        int startInclusive,
        int endExclusive)
    {
        var selected = new List<Sample>(); 

        // check each sample in its original order
        foreach (Sample sample in samples)
        {
            // keep sample if its sensor matches and its time is in the window 
            if (sample.Sensor == sensor && 
            sample.Minute >= startInclusive &&
            sample.Minute < endExclusive)
            {
                // add each match, keeping dupes and the original order
                selected.Add(sample);
            }
        }
        return selected;
    }
}
