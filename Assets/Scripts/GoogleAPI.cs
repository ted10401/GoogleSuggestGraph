using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public static class GoogleAPI
{
    private const string FIND_SUGGEST_URL = "http://suggestqueries.google.com/complete/search?output=firefox&q={0}&hl=en";
    private const string SEARCH_URL = "https://www.google.com/search?q={0}";

    public static async Task<List<string>> FindSuggest(string q)
    {
        string url = string.Format(FIND_SUGGEST_URL, UnityWebRequest.EscapeURL(q));
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();

        while (!unityWebRequest.isDone)
        {
            await Task.Yield();
        }

        List<string> results = new List<string>();

        if(string.IsNullOrEmpty( unityWebRequest.error))
        {
            JSONNode jSONNode = JSONNode.Parse(unityWebRequest.downloadHandler.text);

            for (int i = 0; i < jSONNode[1].Count; i++)
            {
                results.Add(jSONNode[1][i].Value);
            }
        }
        else
        {
            Debug.LogError(unityWebRequest.error);
        }

        return results;
    }

    public static void Search(string q)
    {
        string url = string.Format(SEARCH_URL, UnityWebRequest.EscapeURL(q));
        Application.OpenURL(url);
    }
}
