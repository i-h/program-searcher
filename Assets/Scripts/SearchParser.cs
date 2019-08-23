using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SearchParser
{
    /// <summary>
    /// Parse a JSON received from the Yle API v1/programs/items.json into a list of ProgramEntry structs
    /// </summary>
    /// <param name="json">The JSON data received from the API</param>
    /// <returns>List of ProgramEntry structs with finnish titles and other information</returns>
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

            JToken rating = dataItem["contentRating"]["title"]["en"];
            JToken country = dataItem["countryOfOrigin"].First;
            JToken mediaType = dataItem["typeMedia"];
            JToken id = dataItem["id"];
            newEntry.ContentRating = rating != null ? rating.ToString() : "(No rating information)";
            newEntry.CountryOfOrigin = country != null ? country.ToString() : "(No country of origin information)";
            newEntry.MediaType = mediaType != null ? mediaType.ToString() : "(No media type information)";
            newEntry.ID = id != null ? id.ToString() : "";

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
    public string ContentRating;
    public string CountryOfOrigin;
    public string MediaType;
    public string ID;
}
