using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class TerrainGrid : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize;
    public bool fillTerrain;

    private Camera _camera;
    private Grid _grid;
    private Terrain _terrain;
    void Start()
    {
        _camera = Camera.main;
        if (fillTerrain)
        {
            // Get terrain dimensions and divide by cell size to get w and h
            _terrain = GetComponent<Terrain>();
            Bounds terrainBounds= _terrain.terrainData.bounds;
            Vector3 terrainSize = terrainBounds.size; 
            width = (int)Mathf.Floor(terrainSize.x / cellSize);
            height = (int)Mathf.Floor(terrainSize.z / cellSize);
            _grid = new Grid(width, height, cellSize);
        }
        else
        {
            _grid = new Grid(width, height, cellSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldCoords = MouseToWorldCoordinates();
            Debug.Log(_grid.WorldToGridCoordinates(worldCoords));
        }
    }

    private Vector3 MouseToWorldCoordinates()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Object hitObject = hit.transform.gameObject;
            Vector3 hitPoint = hit.point;
            if (hitObject != null && hitObject.name == "Terrain")
            {
                return hitPoint;
            }
        }
        return Vector3.zero;
    }
    
    private void OnDrawGizmos()
    {

        if (fillTerrain)
        {
            _terrain = GetComponent<Terrain>();
            Bounds terrainBounds= _terrain.terrainData.bounds;
            Vector3 terrainSize = terrainBounds.size; 
            width = (int)Mathf.Floor(terrainSize.x / cellSize);
            height = (int)Mathf.Floor(terrainSize.z / cellSize);
            _grid = new Grid(width, height, cellSize);
        }
        else
        {
            _grid = new Grid(width, height, cellSize);
        }
        Vector2Int gridSize = _grid.GetGridDimensions();
        for (int row = 0; row <= gridSize.y ; row+= 50)
        {
            for (int col = 0; col <= gridSize.x; col+= 50)
            {
                Gizmos.DrawSphere(_grid.GridToWorldCoordinates(new Vector2(row,col)), 5);
            }
        }
    }
}
