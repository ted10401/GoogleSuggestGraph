using UnityEngine;
using UnityEngine.UI;

public class GoogleSuggestGraphController : MonoBehaviour
{
    [SerializeField] private RectTransform m_scrollParent = null;
    [SerializeField] private CanvasScaler m_canvasScaler = null;
    [SerializeField] private float m_zoomStrength = 50;

    private Camera m_camera;
    private Vector2 m_lastMousePosition;
    private Vector2 m_lastResolution;

    private void Awake()
    {
        m_camera = Camera.main;
        m_lastResolution = m_canvasScaler.referenceResolution;
    }

    private void Update()
    {
        ApplyScroll();
        ApplyZoom();
    }

    private void ApplyScroll()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
        {
            m_lastMousePosition = GetMousePosition();
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
        {
            Vector2 mousePosition = GetMousePosition();
            m_scrollParent.anchoredPosition += mousePosition - m_lastMousePosition;
            m_lastMousePosition = mousePosition;
        }
    }

    private Vector2 GetMousePosition()
    {
        Vector2 mousePosition = m_camera.ScreenToViewportPoint(Input.mousePosition);
        mousePosition -= Vector2.one * 0.5f;
        mousePosition.y *= m_canvasScaler.referenceResolution.y;
        mousePosition.x *= (float)Screen.width / Screen.height * m_canvasScaler.referenceResolution.y;
        return mousePosition;
    }

    private void ApplyZoom()
    {
        m_lastResolution.y -= Input.GetAxis("Mouse ScrollWheel") * m_zoomStrength;
        m_canvasScaler.referenceResolution = m_lastResolution;
    }
}
