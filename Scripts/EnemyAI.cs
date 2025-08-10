using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour //assignment 4, using Astar path finding to track player
{
    
    public float moveSpeed = 4f;
    public PlayerController player; // Reference to the player's script
    public Node currentNode;
    private AStarManager aStarManager;
    private bool isTracking = false;
    private bool isMoving = false;
    private Node lastPlayerNode; // A variable to store the player's last known position node
    private List<Node> path = new List<Node>(); // List to store the current path

    void Start()
    {
        aStarManager = AStarManager.instance;
        StartCoroutine(InitializeEnemy());
    }

    private IEnumerator InitializeEnemy()
    {
        // wait until the AStarManager is available and has nodes
        while (aStarManager == null || aStarManager.AllNodes().Length == 0)
        {
            yield return null; // Wait for the next frame
        }

        Node[] allNodes = aStarManager.AllNodes(); // finding a random starting node
        Node spawnNode = allNodes[Random.Range(0, allNodes.Length)]; // spawns the enemy in a random tile which is not obstacle

        currentNode = spawnNode;
        
        // Snap the enemy's position to the center of the starting tile
        Vector3 startPosition = currentNode.transform.position;
        startPosition.y = 0.6f;
        transform.position = startPosition;

        // Set the initial lastPlayerNode so the enemy doesn't move immediately
        lastPlayerNode = player.currentNode;
    }

    void Update()
    {
        if (isTracking && !isMoving) // Only check for new player movement if tracking is enabled and the enemy is not currently moving
        {
            CheckForPlayerMovement();
        }
    }

    // This method will be called by the UI button 
    public void StartTrackingPlayer()
    {
        isTracking = true;
    }
    
    private void CheckForPlayerMovement()
    {
        if (player.currentNode != lastPlayerNode) // if player changed position
        { 
            // To prevent an error, we reset the color of the last path before a new one is generated
            if (path.Count > 0)
            {
                foreach (Node node in path)
                {
                    if (node != null && node.GetComponent<Renderer>() != null)
                    {
                        node.GetComponent<Renderer>().material.color = Color.white;
                    }
                }
            }

            Node playerTargetNode = player.currentNode; // get player's new position      
            Node enemyTargetNode = FindAdjacentNode(playerTargetNode);  // Find an adjacent node to the player's current position to move to
            
            if (enemyTargetNode != null) // If a valid target node is found, generate the path and start moving
            {
                path = aStarManager.GeneratePath(currentNode, enemyTargetNode);

                if (path != null && path.Count > 0)
                {
                    StartCoroutine(MoveAlongPath(path));
                }
            }
            
            // Update the lastPlayerNode to the player's new position
            lastPlayerNode = playerTargetNode;
        }
    }
    
    private Node FindAdjacentNode(Node playerNode) //finds the first neighbor of player's node 
    {
        
        foreach(Node neighbor in playerNode.connections)
        {
            if (neighbor != null && neighbor.GetComponent<Node>() != null) // the node must be walkable
            {
                // Return the first valid neighbor found
                return neighbor;
            }
        }
        return null;
    }

    private IEnumerator MoveAlongPath(List<Node> path) // coroutine to handle the step by step movement, like did in PlayerController
    {
        isMoving = true; //to disable further pathfinding while moving

        foreach (Node targetNode in path)
        {
            Vector3 targetPosition = targetNode.transform.position;
            targetPosition.y = 0.6f; 

            targetNode.GetComponent<Renderer>().material.color = Color.cyan; // Change the color of the current tile to show the path
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f) //movement
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }

            transform.position = targetPosition; // to snap the position
            
            currentNode = targetNode;
        }

        foreach (Node node in path) //reset all tiles in the path back to their original color
        { 
            if (node != null && node.GetComponent<Renderer>() != null)
            {
                node.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        isMoving = false; // Movement is complete, re-enable pathfinding
        isTracking = false;
    }

    public void ResetScene() // to replay
    {
        SceneManager.LoadScene(0);
    }
}
