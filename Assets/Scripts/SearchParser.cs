using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SearchParser
{
    public static List<ProgramEntry> ParseSearch(string json)
    {
        List<ProgramEntry> results = new List<ProgramEntry>();

        JObject obj = JObject.Parse(json);
        JEnumerable<JToken> data = obj["data"].Children();

        foreach (JToken dataItem in data)
        {
            ProgramEntry newEntry = new ProgramEntry();

            JToken[] titles = { dataItem["title"]["fi"], dataItem["title"]["sv"] };
            JToken[] descs = { dataItem["description"]["fi"], dataItem["description"]["sv"] };

            // If an entry doesn't have a finnish name, assume the swedish name as the finnish one
            newEntry.TitleFinnish = titles[0] != null ? titles[0].ToString() : titles[1] != null ? titles[1].ToString() : "(No title available)";
            newEntry.Description = descs[0] != null ? descs[0].ToString() : descs[0] != null ? descs[0].ToString() : "(No description available)";

            //Debug.LogFormat("Parsed entry: {0}: {1}", newEntry.TitleFinnish, newEntry.Description);
            results.Add(newEntry);
        }

        return results;        
    }
}

public struct ProgramEntry
{
    public string TitleFinnish;
    public string Description;
}
