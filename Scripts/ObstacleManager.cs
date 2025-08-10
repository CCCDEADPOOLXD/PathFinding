using UnityEngine;

public class ObstacleManager : MonoBehaviour //Assignment 1 and 2, grid generation and obstacle generation, with node connection for path finding
{
    public ObstacleData obstacleData; //scriptable object that stores bool for each tile
    public GameObject normalTilePrefab;
    public GameObject obstaclePrefab; // This would be a black cube for obstacles
    private Node[,] gridNodes = new Node[10, 10]; //2D array to hold references to the generated nodes

    void Start()
    {
        GenerateGrid();
        ConnectNodes();
    }

    void GenerateGrid() // 10*10 grid generation
    {
        for (int y = 0; y < 10; y++)  //for every element in y
        {
            for (int x = 0; x < 10; x++) //for every element in x
            {
                int index = y * 10 + x; //to convert 2d array to 1d array
                GameObject tile;
                if (obstacleData.isBlocked[index]) //checks the bool from Scriptable Object ObstacleData
                {
                    // Spawn red obstacle tile without node component
                    tile = Instantiate(obstaclePrefab, new Vector3(x, 0, y), Quaternion.identity);
                }
                else
                {
                    // Spawn normal tile with node component
                    tile = Instantiate(normalTilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    gridNodes[x, y] = tile.GetComponent<Node>(); // array of nodes which are not obstacle
                }
            }
        }
    }

    void ConnectNodes() //nodes will be used for path finding
    {
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Node currentNode = gridNodes[x, y];
                if (currentNode == null) continue; // only does this if normal tile not for obstacle tile which doesnt have any node component

                // Check and connect neighbors which are on its 4 directions
                CheckAndConnect(currentNode, x, y, x + 1, y); // Right
                CheckAndConnect(currentNode, x, y, x - 1, y); // Left
                CheckAndConnect(currentNode, x, y, x, y + 1); // Up
                CheckAndConnect(currentNode, x, y, x, y - 1); // Down
            }
        }
    }

    void CheckAndConnect(Node currentNode, int currentX, int currentY, int neighborX, int neighborY)
    {
        if (neighborX >= 0 && neighborX < 10 && neighborY >= 0 && neighborY < 10) // must be under grid limit
        {
            Node neighborNode = gridNodes[neighborX, neighborY];
            if (neighborNode != null)
            {
                currentNode.connections.Add(neighborNode); //adds connection in NodeScript List Connection
            }
        }
    }
}