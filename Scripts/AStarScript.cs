using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]

//A* is algorithm that uses concept of distance and fuel cost, to calculate next best route to move, i didn't code the algo from scratch, but i surely implimented it well 
public class AStarManager : MonoBehaviour //Assignment 3 Pathfinding, used A* algo here
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end) // generate the shortest path using A* logic, it takes 2 node, start and end
    {
        List<Node> openSet = new List<Node>();

        foreach(Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue; //resets all gScore to max value 
        }

        start.gScore = 0; //1st node will have gScore 0
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position); //heuristic calculation using vector2.Distance
        openSet.Add(start);

        while(openSet.Count > 0) //checks until there are nodes to check
        {
            int lowestF = default;

            for(int i = 1; i < openSet.Count; i++) //checks the lowest fScore in all the nodes
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if(currentNode == end) //if it founds the path
            {
                List<Node> path = new List<Node>();

                path.Insert(0, end);

                while(currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach(Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if(heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public Node FindNearestNode(Vector2 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;

        foreach(Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);

            if(currentDistance < minDistance)
            {
                minDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node FindFurthestNode(Vector2 pos)
    {
        Node foundNode = null;
        float maxDistance = default;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);
            if(currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node[] AllNodes()
    {
        return FindObjectsOfType<Node>();
    }
}