using System.Collections.Generic;
using UnityEngine;

public static class GraphNodeUtil
{
    public static void LengthConstrait(List<GraphNode> graphNodes, float length, float elastic, bool ignorePinnedNode = false)
    {
        for (int i = 0; i < graphNodes.Count; i++)
        {
            for (int j = 0; j < graphNodes.Count; j++)
            {
                if(i == j)
                {
                    continue;
                }

                LengthConstrait(graphNodes[i], graphNodes[j], length, elastic, ignorePinnedNode);
            }
        }
    }

    public static void LengthConstrait(GraphNode targetNode, GraphNode neighborNode, float length, float elastic, bool ignorePinnedNode = false)
    {
        if (targetNode == neighborNode)
        {
            return;
        }

        float distance = Vector2.Distance(targetNode.rectTransform.anchoredPosition, neighborNode.rectTransform.anchoredPosition);
        float constaintMagnitude = Mathf.Abs(distance - length);
        Vector2 constraintDirection = Vector2.zero;

        if (distance > length)
        {
            constraintDirection = neighborNode.rectTransform.anchoredPosition - targetNode.rectTransform.anchoredPosition;
            constraintDirection.Normalize();
        }
        else if (distance < length)
        {
            constraintDirection = targetNode.rectTransform.anchoredPosition - neighborNode.rectTransform.anchoredPosition;
            constraintDirection.Normalize();
        }

        Vector2 constraint = constraintDirection * constaintMagnitude;

        if (!targetNode.isRootNode || ignorePinnedNode)
        {
            targetNode.velocity += constraint * elastic * 0.5f;
        }

        if (!neighborNode.isRootNode || ignorePinnedNode)
        {
            neighborNode.velocity -= constraint * elastic * 0.5f;
        }
    }

    public static void UpdateVelocity(List<GraphNode> graphNodes, float velocity, float velocityElastic)
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < graphNodes.Count; i++)
        {
            Vector2 noiseVelocity = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 2f;
            graphNodes[i].velocity += noiseVelocity * velocity;
            graphNodes[i].velocity *= velocityElastic;
            graphNodes[i].rectTransform.anchoredPosition += graphNodes[i].velocity * deltaTime;
        }
    }
}
