using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Attaches a tree to the player's hand
 * 
 */
public class TreePicker : MonoBehaviour {

    public static TreeInstance NULLTREE = new TreeInstance();

    private TreeInstance treeRef = NULLTREE;
    private GameObject dynamicTree;
    public Transform hand;

    public TreeInstance TreeRef {
        get => treeRef;
    }

    public void SetTree(Terrain terrain, int index) {
        treeRef = terrain.terrainData.GetTreeInstance(index);

        List<TreeInstance> treeList = new List<TreeInstance>(terrain.terrainData.treeInstances);
        treeList.RemoveAt(index);
        terrain.terrainData.treeInstances = treeList.ToArray();

        dynamicTree = Instantiate(terrain.terrainData.treePrototypes[treeRef.prototypeIndex].prefab);
        dynamicTree.transform.parent = hand;
        dynamicTree.transform.localPosition = Vector3.zero;
        dynamicTree.transform.position = hand.position;

    }

    public void DropTree() {

        Vector3 handToTerrain = TerrainManager.WorldToTerrain(hand.position);

        //if (hand.position.y - 15 < TerrainManager.GetTerrainHeight(handToTerrain) + TerrainManager.GetTerrainOffset().y) {
            treeRef.position = handToTerrain;

            List<TreeInstance> treeList = new List<TreeInstance>(Terrain.activeTerrain.terrainData.treeInstances);
            treeList.Add(treeRef);
            Terrain.activeTerrain.terrainData.treeInstances = treeList.ToArray();
        //}

        Destroy(dynamicTree);
        treeRef = NULLTREE;

        ForestManager.UpdateAllZones();
    }

    public void Destroy() {
        Destroy(dynamicTree);
        treeRef = NULLTREE;

        ForestManager.UpdateAllZones();
    }

}
