using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoogleSearchResult : MonoBehaviour
{
    public RectTransform rectTransform = null;
    public Vector2 alignment;

    [SerializeField] private Button m_searchButton = null;
    [SerializeField] private Text m_text = null;

    private GoogleSearchController m_controller;
    private List<GoogleSearchResult> m_linkedGoogleSearchResults = new List<GoogleSearchResult>();
    private List<Image> m_linkedLines = new List<Image>();

    private void Awake()
    {
        m_searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        m_controller.Search(m_text.text);
    }

    public void SetController(GoogleSearchController controller)
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

    public void AddGoogleSearchResult(GoogleSearchResult googleSearchResult)
    {
        if(m_linkedGoogleSearchResults.Contains(googleSearchResult))
        {
            return;
        }

        m_linkedGoogleSearchResults.Add(googleSearchResult);

        Image image = new GameObject("Linked Line").AddComponent<Image>();
        image.rectTransform.SetParent(rectTransform, false);
        image.rectTransform.pivot = new Vector2(0.5f, 0f);
        Color color = Color.cyan;
        color.a = 0.25f;
        image.color = color;
        m_linkedLines.Add(image);
    }

    public void AddSeparationVector()
    {
        Vector2 separation = Vector2.zero;
        foreach (GoogleSearchResult googleSearchResult in m_linkedGoogleSearchResults)
        {
            separation += rectTransform.anchoredPosition - googleSearchResult.rectTransform.anchoredPosition;
        }

        separation /= m_linkedGoogleSearchResults.Count;

        rectTransform.anchoredPosition += separation;
    }

    private void Update()
    {
        for (int i = 0; i < m_linkedGoogleSearchResults.Count; i++)
        {
            GoogleSearchResult googleSearchResult = m_linkedGoogleSearchResults[i];
            Image image = m_linkedLines[i];
            Vector2 direction = googleSearchResult.rectTransform.anchoredPosition - rectTransform.anchoredPosition;

            image.rectTransform.anchoredPosition = direction.normalized * googleSearchResult.rectTransform.sizeDelta.y;
            image.rectTransform.up = direction;
            image.rectTransform.sizeDelta = new Vector2(5, direction.magnitude - googleSearchResult.rectTransform.sizeDelta.y * 2);
        }
    }
}