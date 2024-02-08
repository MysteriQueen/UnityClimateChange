using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestManager {

    /// <summary>
    /// List of the forests being managed
    /// </summary>
    private static List<ForestZone> forestZones;

    /// <summary>
    /// Count of all trees. Sum total from all forest zones under it's management
    /// </summary>
    public static int TreeCount { get; private set; }

    /// <summary>
    /// Initialises forest zones
    /// </summary>
    void Awake() {
        // Get all forest zones
        GameObject[] zones = GameObject.FindGameObjectsWithTag("ForestZone");

        foreach (GameObject zone in zones) {
            forestZones.Add(zone.GetComponent<ForestZone>());
        } 
   
    }

    /// <summary>
    /// Sets the tree at index, "heightScale" to 0.0f 
    /// </summary>
    /// <param name="index">The index of the tree to remove</param>
    public static void RemoveTree(int index) {
        TreeInstance tree = TerrainManager.GetTerrain().terrainData.GetTreeInstance(index);
        tree.heightScale = 0.0f;
        TerrainManager.GetTerrain().terrainData.SetTreeInstance(index, tree);

        TreeCount--;

    }

    public static void RemoveTrees(int[] index) {
        TreeInstance[] trees = TerrainManager.GetTerrain().terrainData.treeInstances;

        foreach (int i in index) {
            trees[i].heightScale = 0.0f;
        }

        TreeCount -= index.Length;

    }

    public static int AddTree(TreeInstance tree) {
        List<TreeInstance> treeList = new List<TreeInstance>(TerrainManager.GetTerrain().terrainData.treeInstances);
        treeList.Add(tree);
        TerrainManager.GetTerrain().terrainData.treeInstances = treeList.ToArray();

        TreeCount++;

        return treeList.Count - 1;

    }

    public static int AddTree() {
        List<TreeInstance> treeList = new List<TreeInstance>(TerrainManager.GetTerrain().terrainData.treeInstances);
        treeList.Add(GenerateTree());
        TerrainManager.GetTerrain().terrainData.treeInstances = treeList.ToArray();

        TreeCount++;

        return treeList.Count - 1;

    }

    private static TreeInstance GenerateTree() {
        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = (int)Random.Range(0.0f, TerrainManager.GetTerrain().terrainData.treePrototypes.Length);
        tree.widthScale = 1.0f;
        tree.heightScale = 1.0f;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;

        return tree;
    }

    public static TreeInstance GetTree(int index) {
        return TerrainManager.GetTerrain().terrainData.GetTreeInstance(index);

    }

    public static void UpdateAllZones() {
        
    }

}
