using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizationGraph : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button searchButton;
    public RectTransform graphNodeParent;
    public GraphNode graphNodeReference;
    public Image graphLinkReference;
    public Vector2 segmentLengthRange = new Vector2(200, 300);
    [Range(0f, 2f)] public float constraintElastic = 1.0f;
    public float separationElastic = 1.0f;
    public Vector2 randomRangeX = new Vector2(-1, 1);
    public Vector2 randomRangeY = new Vector2(-1, 1);
    public float velocity = 1f;
    [Range(0f, 0.99f)] public float velocityElastic = 0.99f;

    private Dictionary<string, GraphNode> m_graphNodes = new Dictionary<string, GraphNode>();
    private List<GraphNode> m_rootNodes = new List<GraphNode>();

    private void Awake()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void Update()
    {
        ApplyAlignment();
        ApplyConstraint();
    }

    private void ApplyAlignment()
    {
        Vector2 alignment = Vector2.zero;
        foreach (GraphNode graphNode in m_graphNodes.Values)
        {
            alignment += graphNode.rectTransform.anchoredPosition;
        }

        alignment /= m_graphNodes.Count;
        graphNodeParent.anchoredPosition = -alignment;
    }

    private void ApplyConstraint()
    {
        for (int i = 0; i < m_rootNodes.Count; i++)
        {
            for (int j = 0; j < m_rootNodes.Count; j++)
            {
                if(i == j)
                {
                    continue;
                }

                GraphNode node1 = m_rootNodes[i];
                GraphNode node2 = m_rootNodes[j];
                float segmentLength = segmentLengthRange.y * 3f;

                float distance = Vector2.Distance(node1.rectTransform.anchoredPosition, node2.rectTransform.anchoredPosition);
                float constaintMagnitude = Mathf.Abs(distance - segmentLength);
                Vector2 constraintDirection = Vector2.zero;

                if (distance > segmentLength)
                {
                    constraintDirection = node2.rectTransform.anchoredPosition - node1.rectTransform.anchoredPosition;
                    constraintDirection.Normalize();
                }
                else if (distance < segmentLength)
                {
                    constraintDirection = node1.rectTransform.anchoredPosition - node2.rectTransform.anchoredPosition;
                    constraintDirection.Normalize();
                }

                Vector2 constraint = constraintDirection * constaintMagnitude;
                node1.velocity += constraint * constraintElastic * 0.5f;
                node2.velocity -= constraint * constraintElastic * 0.5f;
            }
        }

        for (int i = 0; i < m_rootNodes.Count; i++)
        {
            Vector2 randomVelocity = new Vector2(Random.Range(randomRangeX.x, randomRangeX.y), Random.Range(randomRangeY.x, randomRangeY.y));
            m_rootNodes[i].velocity += randomVelocity * velocity;
            m_rootNodes[i].velocity *= velocityElastic;
            m_rootNodes[i].rectTransform.anchoredPosition += m_rootNodes[i].velocity * Time.deltaTime;
        }
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
            rootNode.SetController(this);
            rootNode.SetText(text);

            Vector2 position = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 10f;
            rootNode.SetPosition(position);
            m_graphNodes.Add(text, rootNode);
        }

        if(!m_rootNodes.Contains(rootNode))
        {
            m_rootNodes.Add(rootNode);
        }

        List<string> results = await GoogleSuggest.Search(text);
        results.Remove(text);

        for (int i = 0; i < results.Count; i++)
        {
            string resultText = results[i];

            if(!m_graphNodes.ContainsKey(resultText))
            {
                GraphNode graphNode = Instantiate(graphNodeReference, graphNodeParent);
                graphNode.gameObject.SetActive(true);
                graphNode.SetController(this);
                graphNode.SetText(resultText);

                Vector2 position = Vector2.up * Random.Range(segmentLengthRange.x, segmentLengthRange.y);
                position = Quaternion.Euler(0, 0, 360f / results.Count * i) * position;
                position += rootNode.GetPosition();

                graphNode.SetPosition(position);
                m_graphNodes.Add(resultText, graphNode);
            }

            GraphNode childNode = m_graphNodes[results[i]];
            rootNode.AddLinkNode(childNode);
        }
    }
}
