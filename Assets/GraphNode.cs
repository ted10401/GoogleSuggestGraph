using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphNode : MonoBehaviour
{
    public RectTransform rectTransform = null;

    [SerializeField] private Button m_searchButton = null;
    [SerializeField] private Text m_text = null;

    private VisualizationGraph m_controller;
    private List<GraphNode> m_linkNodes = new List<GraphNode>();
    private List<Image> m_linkImages = new List<Image>();

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

    public void AddGoogleSuggestResult(GraphNode googleSuggestResult)
    {
        if(m_linkNodes.Contains(googleSuggestResult))
        {
            return;
        }

        m_linkNodes.Add(googleSuggestResult);

        Image image = new GameObject("Link Image").AddComponent<Image>();
        image.rectTransform.SetParent(rectTransform, false);
        image.rectTransform.pivot = new Vector2(0.5f, 0f);
        Color color = Color.cyan;
        color.a = 0.25f;
        image.color = color;
        m_linkImages.Add(image);
    }

    public void AddSeparationVector()
    {
        Vector2 separation = Vector2.zero;
        foreach (GraphNode googleSuggestResult in m_linkNodes)
        {
            separation += rectTransform.anchoredPosition - googleSuggestResult.rectTransform.anchoredPosition;
        }

        separation /= m_linkNodes.Count;

        rectTransform.anchoredPosition += separation;
    }

    private void Update()
    {
        for (int i = 0; i < m_linkNodes.Count; i++)
        {
            GraphNode googleSuggestResult = m_linkNodes[i];
            Image image = m_linkImages[i];
            Vector2 direction = googleSuggestResult.rectTransform.anchoredPosition - rectTransform.anchoredPosition;

            image.rectTransform.anchoredPosition = direction.normalized * googleSuggestResult.rectTransform.sizeDelta.y;
            image.rectTransform.up = direction;
            image.rectTransform.sizeDelta = new Vector2(5, direction.magnitude - googleSuggestResult.rectTransform.sizeDelta.y * 2);
        }
    }
}