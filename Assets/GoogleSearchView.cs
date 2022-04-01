using UnityEngine;
using UnityEngine.UI;

public class GoogleSearchView : MonoBehaviour
{
    public InputField searchInputField;
    public Text searchText;
    public Button searchButton;
    public GoogleSuggestGraph googleSuggestGraph;

    private void Awake()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    private void OnSearchButtonClicked()
    {
        googleSuggestGraph.Search(searchText.text);
    }
}
