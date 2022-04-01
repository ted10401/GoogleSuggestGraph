using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSuggestGraph : MonoBehaviour
{
    public RectTransform graphNodeParent;
    public GraphNode graphNodeReference;
    public Image graphLinkReference;
    public Vector2 segmentLengthRange = new Vector2(200, 400);
    [Range(0f, 2f)] public float constraintElastic = 0.5f;
    public float velocity = 1f;
    [Range(0f, 0.99f)] public float velocityElastic = 0.9f;

    private Dictionary<string, GraphNode> m_graphNodes = new Dictionary<string, GraphNode>();
    private List<GraphNode> m_rootNodes = new List<GraphNode>();

    private void Update()
    {
        ApplyConstraint();
        ApplyVelocity();
    }

    private void ApplyConstraint()
    {
        float length = segmentLengthRange.y * (m_rootNodes.Count - 1);
        length *= Mathf.PI;
        length /= m_rootNodes.Count;

        GraphNodeUtil.LengthConstrait(m_rootNodes, length, constraintElastic, true);
    }

    private void ApplyVelocity()
    {
        GraphNodeUtil.UpdateVelocity(m_rootNodes, velocity, velocityElastic);
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
            rootNode.isRootNode = true;
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
