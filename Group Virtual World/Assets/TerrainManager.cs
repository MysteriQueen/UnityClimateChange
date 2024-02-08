using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager {

    private static Terrain terrain;

    /// <summary>
    /// Gets the terrain object.
    /// Initialises it to the active terrain if not yet initialised.
    /// </summary>
    /// <returns>The Terrain object the forest manager operates on</returns>
    public static Terrain GetTerrain() {
        if (terrain == null)
            Initialise();

        return terrain;
    }

    /// <summary>
    /// Gets the terrain object's position (offset).
    /// Initialises it to the active terrain if not yet initialised.
    /// </summary>
    /// <returns>Vector3 representing the position of the terrain the forest manager operates on</returns>
    public static Vector3 GetTerrainOffset() {
        if (terrain == null)
            Initialise();

        return terrain.transform.position;

    }

    /// <summary>
    /// Gets the terrain object's size.
    /// Initialises it to the active terrain if not yet initialised.
    /// </summary>
    /// <returns>Vector3 representing the size of the terrain the forest manager operates on</returns>
    public static Vector3 GetTerrainSize() {
        if (terrain == null)
            Initialise();

        return terrain.terrainData.size;
    }

    /// <summary>
    /// Sets the terrain reference to the active terrain.
    /// Sets the tree instances to a new array.
    /// </summary>
    private static void Initialise() {
        terrain = Terrain.activeTerrain;
        terrain.terrainData.treeInstances = new TreeInstance[0];
    }

    public static Vector3 WorldToTerrain(float x, float z) {

        Vector3 ret = new Vector3((x - GetTerrainOffset().x) / GetTerrainSize().x, terrain.SampleHeight(new Vector3(x, 0.0f, z)) / GetTerrainSize().y, (z - GetTerrainOffset().z) / GetTerrainSize().z);
        //Vector3 ret = new Vector3((x - TerrainOffset.x) / TerrainSize.x, terrain.terrainData.GetHeight((int) (x - TerrainOffset.x), (int) (z - TerrainOffset.z)) / TerrainSize.y, (z - TerrainOffset.z) / TerrainSize.z);

        return ret;
    }

    public static Vector3 WorldToTerrain(Vector3 pos) {
        return WorldToTerrain(pos.x, pos.z);
    }

    public static float GetTerrainHeight(Vector3 pos) {
        return terrain.SampleHeight(pos);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>Array of 8 positions (around pos)</returns>
    public static Vector3[] GetSurroundingPositions(Vector3 pos, float distance) {
        Vector3[] positions = new Vector3[8];

        int count = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {

                // Skip centre
                if (i == 1 && j == 1)
                    continue;

                Vector3 samplePos = pos;
                samplePos.x += (i - 1) * distance;
                samplePos.y = 0.0f;
                samplePos.z += (j - 1) * distance;

                positions[count++] = TerrainToWorld(WorldToTerrain(samplePos));

            }
        }

        return positions;
    }

    public static Vector3 TerrainToWorld(Vector3 pos) {
        return Vector3.Scale(pos, GetTerrainSize()) + GetTerrainOffset();

    }

}
