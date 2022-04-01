using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphNode : MonoBehaviour
{
    public RectTransform rectTransform = null;
    public Vector2 velocity = Vector2.zero;

    [SerializeField] private Button m_searchButton = null;
    [SerializeField] private Text m_text = null;

    private VisualizationGraph m_controller;
    private List<GraphNode> m_graphNodes = new List<GraphNode>();
    private List<Image> m_graphLinks = new List<Image>();
    private List<float> m_linkSegmentLengths = new List<float>();

    private void Awake()
    {
        m_searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        m_controller.Search(m_text.text);
    }

    public void SetController(VisualizationGraph controller)
    {
        m_controller = controller;
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public Vector2 GetPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public void SetText(string value)
    {
        m_text.text = value;
    }

    public void AddLinkNode(GraphNode graphNode)
    {
        if(m_graphNodes.Contains(graphNode))
        {
            return;
        }

        m_graphNodes.Add(graphNode);

        Image graphLink = Instantiate(m_controller.graphLinkReference);
        graphLink.gameObject.SetActive(true);
        graphLink.rectTransform.SetParent(rectTransform, false);
        m_graphLinks.Add(graphLink);

        m_linkSegmentLengths.Add(Random.Range(m_controller.segmentLengthRange.x, m_controller.segmentLengthRange.y));
    }

    public void AddSegmentLength(GraphNode graphNode)
    {
        if(!m_graphNodes.Contains(graphNode))
        {
            return;
        }

        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            if(m_graphNodes[i] == graphNode)
            {
                m_linkSegmentLengths[i] = m_controller.segmentLengthRange.y * 3f;
            }
        }
    }

    private void Update()
    {
        ApplyConstraint();
        ApplySeparation();
        ApplyVelocity();
        UpdateLink();
    }

    private void ApplyConstraint()
    {
        ApplyConstraint(this, m_graphNodes);
    }

    private void ApplyConstraint(GraphNode targetNode, List<GraphNode> graphNodes)
    {
        for (int i = 0; i < graphNodes.Count; i++)
        {
            GraphNode graphNode = graphNodes[i];
            if(targetNode == graphNode)
            {
                continue;
            }

            float segmentLength = m_linkSegmentLengths[i];

            float distance = Vector2.Distance(targetNode.rectTransform.anchoredPosition, graphNode.rectTransform.anchoredPosition);
            float constaintMagnitude = Mathf.Abs(distance - segmentLength);
            Vector2 constraintDirection = Vector2.zero;

            if (distance > segmentLength)
            {
                constraintDirection = graphNode.rectTransform.anchoredPosition - targetNode.rectTransform.anchoredPosition;
                constraintDirection.Normalize();
            }
            else if (distance < segmentLength)
            {
                constraintDirection = targetNode.rectTransform.anchoredPosition - graphNode.rectTransform.anchoredPosition;
                constraintDirection.Normalize();
            }

            Vector2 constraint = constraintDirection * constaintMagnitude;
            targetNode.velocity += constraint * m_controller.constraintElastic * 0.5f;
            graphNode.velocity -= constraint * m_controller.constraintElastic * 0.5f;
        }
    }

    private void ApplySeparation()
    {
        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            GraphNode graphNode = m_graphNodes[i];
            float segmentLength = m_linkSegmentLengths[i];
            ApplySeparation(graphNode, segmentLength, m_graphNodes);
        }
    }

    public void ApplySeparation(GraphNode graphNode, float segmentLength, List<GraphNode> linkNodes)
    {
        Vector2 separationVector = Vector2.zero;
        int count = 0;

        for (int i = 0; i < linkNodes.Count; i++)
        {
            if(linkNodes[i] == graphNode)
            {
                continue;
            }

            separationVector += GetSeparationVector(graphNode.rectTransform, linkNodes[i].rectTransform, segmentLength);
            count++;
        }

        if(count == 0)
        {
            return;
        }

        separationVector /= count;
        graphNode.velocity += separationVector * m_controller.separationElastic;
    }

    private Vector2 GetSeparationVector(RectTransform target, RectTransform neighbor, float segmentLength)
    {
        var diff = target.anchoredPosition - neighbor.anchoredPosition;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / segmentLength);
        return diff * (scaler / diffLen);
    }

    private void ApplyVelocity()
    {
        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            Vector2 randomVelocity = new Vector2(Random.Range(m_controller.randomRangeX.x, m_controller.randomRangeX.y), Random.Range(m_controller.randomRangeY.x, m_controller.randomRangeY.y));
            m_graphNodes[i].velocity += randomVelocity * m_controller.velocity;
            m_graphNodes[i].velocity *= m_controller.velocityElastic;
            m_graphNodes[i].rectTransform.anchoredPosition += m_graphNodes[i].velocity * Time.deltaTime;
        }
    }

    private void UpdateLink()
    {
        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            GraphNode graphNode = m_graphNodes[i];
            Image image = m_graphLinks[i];
            Vector2 direction = graphNode.rectTransform.anchoredPosition - rectTransform.anchoredPosition;

            image.rectTransform.anchoredPosition = direction.normalized * graphNode.rectTransform.sizeDelta.y * 0.5f;
            image.rectTransform.up = direction;
            image.rectTransform.sizeDelta = new Vector2(5, direction.magnitude - graphNode.rectTransform.sizeDelta.y);
        }
    }
}