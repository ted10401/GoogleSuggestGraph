using System.Collections.Generic;
using UnityEngine;

public class GoogleSuggestGraph : MonoBehaviour
{
    public Vector2 segmentLengthRange = new Vector2(200, 400);
    [Range(0f, 2f)] public float constraintElastic = 0.5f;
    public float velocity = 5f;
    [Range(0f, 0.99f)] public float velocityElastic = 0.9f;

    private List<GraphNode> m_rootNodes = new List<GraphNode>();

    public void Setup(List<GraphNode> rootNodes)
    {
        m_rootNodes = rootNodes;
    }

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
}
