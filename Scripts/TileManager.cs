using TMPro;
using UnityEngine;

public class TileManager : MonoBehaviour //Assignment 1 Doing Raycast to get required properties of each Tile
{
    public TextMeshProUGUI Xposition;
    public TextMeshProUGUI Zposition;
    public TextMeshProUGUI IsObstacle; //text box refrences
    public GameObject UiRoot;
    public LayerMask tileLayer; // Layer mask to only hit the cubes and not other objects
    public float maxRaycastDistance = 100f;
    
    private GameObject lastHoveredObject;

    void Update()
    {
        HandleMouseHover(); // Check if the mouse is hovering over a tile
    }

    private void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //fires a raycast from camera to mouse position
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, tileLayer))
        {
            GameObject hitObject = hit.collider.gameObject; //stores the object (tile) we hit

            Node hitNode = hitObject.GetComponent<Node>();  //gets the node component from that tile            
            
            bool isObstacle = hitNode == null; // If it's an obstacle, it won't have a node component

            ProcessTile(hitObject, isObstacle, hitNode); // Call a custom function to process the tile information
        }
        else
        {
            // The raycast didn't hit a tile
            ProcessTile(null, false, null);
        }
    }

    private void ProcessTile(GameObject tileObject, bool isObstacle, Node tileNode)
    { 
        if (tileObject != lastHoveredObject) // Check if the current tile is different from the last one
        {
            UiRoot.SetActive(true);
            
            if (lastHoveredObject != null) // If there was a previous tile, reset its color
            {
                // Only reset the color if the last hovered object was a walkable tile.
                if (lastHoveredObject.GetComponent<Node>() != null)
                {
                    lastHoveredObject.GetComponent<Renderer>().material.color = Color.white;
                }
            }

            // Update the last hovered object to the current one
            lastHoveredObject = tileObject;
            
            // If a new tile is being hovered over, process its information and highlight it
            if (tileObject != null)
            {
                float displayPosx = tileObject.transform.position.x+1;
                float displayPosz = tileObject.transform.position.z+1;
                Xposition.SetText(displayPosx.ToString()); // sets the text in ui
                Zposition.SetText(displayPosz.ToString());

                if (!isObstacle)
                {
                    IsObstacle.SetText("Walkable");
                    tileObject.GetComponent<Renderer>().material.color = Color.yellow; //highlight selected walkable tile
                }
                else // is obstacle
                {
                    IsObstacle.SetText("Obstacle");
                    //tileObject.GetComponent<Renderer>().material.color = Color.red; 
                }
            }
            else // No tile is being hovered over
            {
                UiRoot.SetActive(false); //turns off the ui and resets the values
                IsObstacle.SetText("");
                Xposition.SetText("");
                Zposition.SetText("");
            }
        }
    }
}