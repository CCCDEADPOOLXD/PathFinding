using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour //here each node has its own  gScore, hScore and calculated Fscore, its used to find the next shortest path
{
    public Node cameFrom;
    public List<Node> connections;

    public float gScore; //cost
    public float hScore; //heuristic (estimate cost, which can be calculated by vector2.Distance)

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        if(connections.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for(int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position); //to see the connections
            }
        }
    }
}