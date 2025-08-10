using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour // Player Spawn, player movement by path tracking
{
    
    public float moveSpeed = 5f;
    public LayerMask tileLayer;
    public Node currentNode; 
    private AStarManager aStarManager;
    private bool isMoving = false;
    private List<Node> path = new List<Node>();

    void Start()
    {
        aStarManager = AStarManager.instance; 
        StartCoroutine(InitializePlayer());
    }

    private IEnumerator InitializePlayer()
    {
        while (aStarManager == null || aStarManager.AllNodes().Length == 0) //waiting for all nodes to be generated
        {
            yield return null; // Wait for the next frame
        }

        currentNode = aStarManager.FindNearestNode(transform.position); //snaps the player to nearst tile
        
        if (currentNode != null)
        {
            Vector3 startPosition = currentNode.transform.position;
            startPosition.y = 0.6f; // Set the player's starting height to 0.6
            transform.position = startPosition;
        }
    }

    void Update()
    {
        // Only check for input if the player is not currently moving
        if (!isMoving)
        {
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
    {
        // Add a null check to ensure the player has been initialized on a node before accepting input
        if (currentNode == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) //input
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //fires a ray from camera to mouse pos
            RaycastHit hit; 

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer)) //raycast
            {
                GameObject hitObject = hit.collider.gameObject; 
                Node targetNode = hitObject.GetComponent<Node>();

                if (targetNode != null) // Check if the target is a valid, walkable node (not an obstacle)
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
                    
                    path = aStarManager.GeneratePath(currentNode, targetNode); // Generate the path from the current node to the target node

                    if (path != null && path.Count > 0)
                    {
                        StartCoroutine(MoveAlongPath()); //start moving when path found
                    }
                }
            }
        }
    }

    private IEnumerator MoveAlongPath() // coroutine to handle the smooth, step by step player movement
    {
        isMoving = true; // Set the flag to disable input

        foreach (Node targetNode in path)
        {
            Vector3 targetPosition = targetNode.transform.position;
            targetPosition.y = 0.6f; 

            targetNode.GetComponent<Renderer>().material.color = Color.green; // change the color of the current tile to green to show found path
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f) // Move the player smoothly towards the target node
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }

            transform.position = targetPosition; // to snap the position
            
            currentNode = targetNode;
        }

        // After the movement is complete, reset all tiles in the path back to their original color
        foreach (Node node in path)
        {
            if (node != null && node.GetComponent<Renderer>() != null)
            {
                node.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        isMoving = false; // re-enable input
    }
}
