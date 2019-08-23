using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class ProgramFetcher : MonoBehaviour
{

    public static string SearchInput = "";
    private string _baseURL = "https://external.api.yle.fi/v1/programs/items.json";
    private int _limit = 10;
    private int _offset = 0;
    private string _lastSearch = "";

    public static bool SearchInProgress { get; private set; }

    #region Singleton
    public static ProgramFetcher Instance { get { return _instance; } }
    private static ProgramFetcher _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Debug.LogWarning("Too many ProgramFetchers! Destroying the extra at " + name);
            Destroy(this);
        }

    }
    #endregion

    public void Search(string query)
    {
        Search(query, false);
    }

    public void SearchNext()
    {
        Search(_lastSearch, true);
    }

    private void Search(string query, bool append)
    {
        SearchInProgress = true;
        string searchQuery = (query.Length > 0) ? query : SearchInput;

        if (searchQuery.Length == 0)
        {
            ResultsDisplay.Instance.DisplayError("Please enter something to search for!"); // Let's not search without keywords
            return;
        }

        Debug.Log("Searching " + searchQuery);

        if (!append)
        {
            // First search
            _offset = 0;
            _lastSearch = searchQuery;
        } else 
        {
            // Repeating the previous search and appending to the results
            _offset += _limit;
            searchQuery = _lastSearch;
        }

        string url = GetSearchString(searchQuery);
        StartCoroutine(DoSearch(url, append));        
    }

    IEnumerator DoSearch(string url, bool append = false)
    {
        ResultsDisplay.Instance.DisplayLoadingBar(append);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            List<ProgramEntry> entries = new List<ProgramEntry>();
            request.SendWebRequest();

            while (!request.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            string content = "";

            if (request.isNetworkError || request.responseCode != 200)
            {
                Debug.Log("A network error occurred: " + request.error);
                content = request.error;
                Debug.Log(request.downloadHandler.text);
            } else
            {
                content = request.downloadHandler.text;
                Debug.Log(content);
                entries =  SearchParser.ParseSearch(content);                
            }

            if (entries.Count == 0)
            {
                ResultsDisplay.Instance.DisplayError("No search results for '" + _lastSearch + "' :(");
            }
            else
            {                
                ResultsDisplay.Instance.DisplayResults(entries, append);
            }

        }
        SearchInProgress = false;
    }

    private string GetSearchString(string query)
    {
        string searchURL = "";
        APIKeys keys = GetAPIKeys();

        searchURL = string.Format("{0}?app_id={1}&app_key={2}&limit={3}&offset={4}&q={5}", _baseURL, keys.AppID, keys.AppKey, _limit, _offset, query);
        Debug.Log("URL: " + searchURL);
        return searchURL;
    }
    private APIKeys GetAPIKeys()
    {
        APIKeys keys = new APIKeys();
        string keyFile;
        using (StreamReader sr = new StreamReader("key"))
        {
            keyFile = sr.ReadToEnd();
        }
        string[] keyRows = keyFile.Split('\n');

        /* The keys file needs to be exactly two rows, on the first the App ID 
         * and on the second row the App Key provided by Yle */
        if (keyRows.Length == 2)     
        {
            keys.AppID = keyRows[0].Trim(); // Remember to trim Windows's \r 😬
            keys.AppKey = keyRows[1].Trim();
            Debug.LogFormat("<color=#060>API Keys read OK</color>\nApp ID: [{0}] App Key: [{1}]", keys.AppID, keys.AppKey);
        } else
        {
            Debug.LogErrorFormat("Problem with reading the API keys. Please check or create the keys file in project root.\n(Rows from file: {0})", keyRows.Length);
        }
        return keys;

    }
}

struct APIKeys
{
    public string AppID;
    public string AppKey;
}
