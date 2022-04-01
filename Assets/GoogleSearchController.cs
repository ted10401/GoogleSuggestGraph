using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSearchController : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button searchButton;
    public GoogleSearchResult referenceGoogleSearchResult;
    public float neighborDist = 100;

    private Dictionary<string, GoogleSearchResult> m_googleSearchResults = new Dictionary<string, GoogleSearchResult>();

    private void Awake()
    {
        searchInputField.onEndEdit.AddListener(OnSearchInputFieldEndEdited);
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchInputFieldEndEdited(string text)
    {
        Search(text);
    }

    private void OnSearchButtonClicked()
    {
        Search(searchText.text);
    }

    public async void Search(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return;
        }

        List<string> results = await GoogleSearch.Search(text);
        for (int i = 0; i < results.Count; i++)
        {
            string resultText = results[i];

            if(!m_googleSearchResults.ContainsKey(resultText))
            {
                GoogleSearchResult googleSearchResult = Instantiate(referenceGoogleSearchResult, transform);
                googleSearchResult.gameObject.SetActive(true);
                googleSearchResult.SetController(this);
                googleSearchResult.SetText(resultText);

                Vector2 position = Vector2.zero;

                if(i != 0)
                {
                    position = Vector2.up * neighborDist;
                    position = Quaternion.Euler(0, 0, 360f / (results.Count - 1) * i) * position;
                    position += m_googleSearchResults[results[0]].GetPosition();
                }

                googleSearchResult.SetPosition(position);
                m_googleSearchResults.Add(resultText, googleSearchResult);
            }
            else if(i == 0)
            {
                m_googleSearchResults[results[i]].AddSeparationVector();
            }

            if(i != 0)
            {
                GoogleSearchResult centerResult = m_googleSearchResults[results[0]];
                GoogleSearchResult currentResult = m_googleSearchResults[results[i]];
                centerResult.AddGoogleSearchResult(currentResult);
                currentResult.AddGoogleSearchResult(centerResult);
                centerResult.alignment = Vector2.zero;
                currentResult.alignment = (currentResult.rectTransform.anchoredPosition - centerResult.rectTransform.anchoredPosition).normalized;
            }
        }
    }
}
