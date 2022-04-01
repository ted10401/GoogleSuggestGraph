using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSearchView : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button searchButton;
    public RectTransform graphNodeParent;
    public GraphNode graphNodeReference;
    public Image graphLinkReference;
    public GoogleSuggestGraph googleSuggestGraph;

    private Dictionary<string, GraphNode> m_graphNodes = new Dictionary<string, GraphNode>();
    private List<GraphNode> m_rootNodes = new List<GraphNode>();

    private void Awake()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
        googleSuggestGraph.Setup(m_rootNodes);
    }

    private void OnSearchButtonClicked()
    {
        Search(searchText.text);
    }

    public async void Search(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        text = text.ToLower();

        GraphNode rootNode;
        if (m_graphNodes.TryGetValue(text, out rootNode))
        {
            foreach (GraphNode graphNode in m_graphNodes.Values)
            {
                graphNode.AddSegmentLength(rootNode);
            }
        }
        else
        {
            rootNode = Instantiate(graphNodeReference, graphNodeParent);
            rootNode.gameObject.SetActive(true);
            rootNode.Setup(this);
            rootNode.SetText(text);

            Vector2 position = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 10f;
            rootNode.SetPosition(position);
            m_graphNodes.Add(text, rootNode);
        }

        if (!m_rootNodes.Contains(rootNode))
        {
            rootNode.isRootNode = true;
            m_rootNodes.Add(rootNode);
        }

        List<string> results = await GoogleSuggest.Search(text);
        results.Remove(text);

        for (int i = 0; i < results.Count; i++)
        {
            string resultText = results[i];

            if (!m_graphNodes.ContainsKey(resultText))
            {
                GraphNode graphNode = Instantiate(graphNodeReference, graphNodeParent);
                graphNode.gameObject.SetActive(true);
                graphNode.Setup(this);
                graphNode.SetText(resultText);

                Vector2 position = rootNode.GetPosition();

                graphNode.SetPosition(position);
                m_graphNodes.Add(resultText, graphNode);
            }

            GraphNode childNode = m_graphNodes[results[i]];
            rootNode.AddLinkNode(childNode);
        }
    }
}
