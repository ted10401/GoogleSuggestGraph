using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphNode : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rectTransform = null;
    public Vector2 velocity = Vector2.zero;
    public bool isRootNode;

    [SerializeField] private Text m_text = null;

    public UnityEvent onLeftClick;
    public UnityEvent onMiddleClick;
    public UnityEvent onRightClick;

    private GoogleSearchView m_googleSearchView;
    private GoogleSuggestGraph m_googleSuggestGraph;
    private List<GraphNode> m_graphNodes = new List<GraphNode>();
    private List<Image> m_graphLinks = new List<Image>();
    private List<float> m_linkSegmentLengths = new List<float>();

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftClick.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                onMiddleClick.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                onRightClick.Invoke();
                break;
        }
    }

    public void SetGoogleSearchView(GoogleSearchView googleSearchView)
    {
        m_googleSearchView = googleSearchView;
        m_googleSuggestGraph = m_googleSearchView.googleSuggestGraph;
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public Vector2 GetPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public void AddLinkNode(GraphNode graphNode)
    {
        if(m_graphNodes.Contains(graphNode))
        {
            return;
        }

        m_graphNodes.Add(graphNode);

        Image graphLink = Instantiate(m_googleSearchView.graphLinkReference);
        graphLink.gameObject.SetActive(true);
        graphLink.rectTransform.SetParent(rectTransform, false);
        m_graphLinks.Add(graphLink);

        m_linkSegmentLengths.Add(Random.Range(m_googleSuggestGraph.segmentLengthRange.x, m_googleSuggestGraph.segmentLengthRange.y));
    }

    public void AddSegmentLength(GraphNode graphNode)
    {
        if(!m_graphNodes.Contains(graphNode))
        {
            return;
        }

        for (int i = m_graphNodes.Count - 1; i >= 0; i--)
        {
            if(m_graphNodes[i] == graphNode)
            {
                m_linkSegmentLengths[i] = m_googleSuggestGraph.segmentLengthRange.y * 3f;
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
        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            GraphNodeUtil.LengthConstrait(this, m_graphNodes[i], m_linkSegmentLengths[i], m_googleSuggestGraph.constraintElastic);
        }
    }

    private void ApplySeparation()
    {
        for (int i = 0; i < m_graphNodes.Count; i++)
        {
            if(m_graphNodes[i].isRootNode)
            {
                continue;
            }

            Vector2 position = Vector2.up * m_linkSegmentLengths[i];
            float angle = 360f / m_graphNodes.Count * i;
            position = Quaternion.Euler(0, 0, angle) * position;
            position += GetPosition();

            m_graphNodes[i].velocity += position - m_graphNodes[i].rectTransform.anchoredPosition;
        }
    }

    private void ApplyVelocity()
    {
        GraphNodeUtil.UpdateVelocity(m_graphNodes, m_googleSuggestGraph.velocity, m_googleSuggestGraph.velocityElastic);
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