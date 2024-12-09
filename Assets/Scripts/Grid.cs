using Unity.VisualScripting;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float _cellSize;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this._cellSize = cellSize;
    }

    public Vector2Int GetGridDimensions()
    {
        return new Vector2Int(width, height);
    }


    public Vector2 WorldToGridCoordinates(Vector3 worldCoordinates)
    {
        return new Vector2(Mathf.Floor(worldCoordinates.x / _cellSize), Mathf.Floor(worldCoordinates.z / _cellSize));
    }
    
    public Vector3 GridToWorldCoordinates(Vector2 gridCoordinates)
    {
        return new Vector3(gridCoordinates.x * _cellSize, 0, gridCoordinates.y * _cellSize);
    }
}


