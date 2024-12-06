using UnityEngine;

[ExecuteAlways]
public class TerrainBoundingBox : MonoBehaviour
{
    public Terrain terrain; // Assign the terrain in the inspector
    public Color gizmoColor = Color.green; // Color of the bounding box

    private void OnDrawGizmos()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Terrain is not assigned.");
            return;
        }

        // Get the bounds of the terrain
        Bounds terrainBounds = terrain.terrainData.bounds;

        // Transform the bounds to world space
        Vector3 worldCenter = terrain.transform.position + terrainBounds.center;
        Vector3 worldSize = terrainBounds.size;

        // Draw the bounding box
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }
}
