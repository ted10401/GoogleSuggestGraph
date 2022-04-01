using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public static class GoogleSearch
{
    private const string SUGGEST_URL = "http://suggestqueries.google.com/complete/search?output=firefox&q={0}&hl=en";

    public static async Task<List<string>> Search(string q)
    {
        string url = string.Format(SUGGEST_URL, q);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SendWebRequest();

        while (!unityWebRequest.isDone)
        {
            await Task.Yield();
        }

        List<string> results = new List<string>();

        if(string.IsNullOrEmpty( unityWebRequest.error))
        {
            Debug.Log(unityWebRequest.downloadHandler.text);

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
}
