using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizationGraph : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button searchButton;
    public GraphNode graphNodeReference;
    public float neighborDist = 100;

    private Dictionary<string, GraphNode> m_graphNodes = new Dictionary<string, GraphNode>();

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

        List<string> results = await GoogleSuggest.Search(text);
        for (int i = 0; i < results.Count; i++)
        {
            string resultText = results[i];

            if(!m_graphNodes.ContainsKey(resultText))
            {
                GraphNode graphNode = Instantiate(graphNodeReference, transform);
                graphNode.gameObject.SetActive(true);
                graphNode.SetController(this);
                graphNode.SetText(resultText);

                Vector2 position = Vector2.zero;

                if(i != 0)
                {
                    position = Vector2.up * neighborDist;
                    position = Quaternion.Euler(0, 0, 360f / (results.Count - 1) * i) * position;
                    position += m_graphNodes[results[0]].GetPosition();
                }

                graphNode.SetPosition(position);
                m_graphNodes.Add(resultText, graphNode);
            }
            else if(i == 0)
            {
                m_graphNodes[results[i]].AddSeparationVector();
            }

            if(i != 0)
            {
                GraphNode parentNode = m_graphNodes[results[0]];
                GraphNode childNode = m_graphNodes[results[i]];
                parentNode.AddGoogleSuggestResult(childNode);
                childNode.AddGoogleSuggestResult(parentNode);
            }
        }
    }
}
