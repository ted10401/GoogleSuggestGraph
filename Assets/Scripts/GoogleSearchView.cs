using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSearchView : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button findSuggestButton;
    public RectTransform graphNodeParent;
    public GraphNode graphNodeReference;
    public Image graphLinkReference;
    public GoogleSuggestGraph googleSuggestGraph;

    private Dictionary<string, GraphNode> m_graphNodes = new Dictionary<string, GraphNode>();
    private List<GraphNode> m_rootNodes = new List<GraphNode>();

    private void Awake()
    {
        findSuggestButton.onClick.AddListener(OnFindSuggestButtonClicked);
        googleSuggestGraph.Setup(m_rootNodes);
    }

    private void OnFindSuggestButtonClicked()
    {
        FindSuggest(searchText.text);
    }

    public async void FindSuggest(string text)
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
            SetupGraphNode(rootNode, text, new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 10f);
            
            m_graphNodes.Add(text, rootNode);
        }

        if (!m_rootNodes.Contains(rootNode))
        {
            rootNode.isRootNode = true;
            m_rootNodes.Add(rootNode);
        }

        List<string> results = await GoogleAPI.FindSuggest(text);
        results.Remove(text);

        for (int i = 0; i < results.Count; i++)
        {
            string resultText = results[i];

            if (!m_graphNodes.ContainsKey(resultText))
            {
                GraphNode graphNode = Instantiate(graphNodeReference, graphNodeParent);
                Vector2 position = rootNode.GetPosition();
                SetupGraphNode(graphNode, resultText, position);

                m_graphNodes.Add(resultText, graphNode);
            }

            GraphNode childNode = m_graphNodes[results[i]];
            rootNode.AddLinkNode(childNode);
        }
    }

    private void SetupGraphNode(GraphNode graphNode, string text, Vector2 position)
    {
        graphNode.gameObject.SetActive(true);
        graphNode.SetGoogleSearchView(this);
        graphNode.SetText(text);
        graphNode.SetPosition(position);
        graphNode.onLeftClick.AddListener(() => { FindSuggest(text); });
        graphNode.onRightClick.AddListener(() => { Search(text); });
    }

    public void Search(string text)
    {
        GoogleAPI.Search(text);
    }
}
