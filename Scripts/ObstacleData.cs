using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Custom/Obstacle Data")]
public class ObstacleData : ScriptableObject //Assignment 2, to make a ScriptableObject to generate obstacle in the level
{
    // A 1D array of 100 booleans to represent the 10x10 grid.
    public bool[] isBlocked = new bool[100]; 
}