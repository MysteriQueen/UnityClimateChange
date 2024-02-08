using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Represents a forest in the world, i.e., a "forest zone" under a forest manager
 * 
 */
public class ForestZone : MonoBehaviour {

    [SerializeField, Range(3, 500)] private int initialPopulation = 10;
    [SerializeField, Range(5, 100)] private int initialArea = 20; // "Diameter"
    [SerializeField] private Material highlughtMaterial;

    private List<int> trees = new List<int>(); // List of lists of trees, for each tree set in each terrain tile
    private GameObject areaHighlight; // Forest highlight material

    /**
     * Instantiations
     */
    private void Awake() {
        SeaLevelManager.OnRise += WaterOnRise;

        //manager.Terrain.terrainData.treeInstances = new TreeInstance[0];

        for (int i = 0; i < initialPopulation; i++) {
            PlantTree();

        }

        TerrainManager.GetTerrain().Flush();

    }

    /**
     * Object setup
     */
    private void Start() {

    }

    private void Update() {
        float probability = 0.00005f; // Tree growth statistics
        if (Random.Range(0.0f, 1f/probability) < trees.Count) {
            FellTree();
            PlantTree();
        }
    }

    private void PlantTree() {
        TreeInstance tree = new TreeInstance();
        tree.prototypeIndex = (int)Random.Range(0.0f, TerrainManager.GetTerrain().terrainData.treePrototypes.Length);
        tree.widthScale = 1.0f;
        tree.heightScale = 1.0f;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;

        tree.position = TerrainManager.WorldToTerrain(
            transform.position.x + Random.Range(0, initialArea) - initialArea / 2,
            transform.position.z + Random.Range(0, initialArea) - initialArea / 2);

        trees.Add(ForestManager.AddTree(tree));
    }

    private void FellTree() {
        int index = Random.Range(0, trees.Count);
        ForestManager.RemoveTree(trees[index]);
        trees.RemoveAt(index);
    }

    private void WaterOnRise() {

        // Make a stack of trees
        Stack<int> treeStack = new Stack<int>(trees);

        // Keep and remove stacks
        Stack<int> keepStack = new Stack<int>();
        Stack<int> removeStack = new Stack<int>();

        while (treeStack.Count != 0) { // Loop over stack until all trees are handled

            // Sample each top of stack, always popping either into the remove or keep stack
            if (treeStack.Peek() < TerrainManager.GetTerrain().terrainData.treeInstanceCount) {

                if (TerrainManager.TerrainToWorld(ForestManager.GetTree(treeStack.Peek()).position).y < SeaLevelManager.GetHeight()) {

                    // Pop and push onto remove stack
                    removeStack.Push(treeStack.Pop());

                } else {
                    // Pop and push onto keep stack
                    keepStack.Push(treeStack.Pop());
                }
            } else {
                keepStack.Push(treeStack.Pop());
            }
        }

        ForestManager.RemoveTrees(removeStack.ToArray());
        trees = new List<int>(keepStack);

        TerrainManager.GetTerrain().Flush();

    }
}
   

public class RadarConvexHullAlgorithm {

    private SortedSet<int> returnIndices = new SortedSet<int>();

    /*
     * Sort vertices into a primitive single-depth quadtree
     * 
     * quadrantB, quadrantA,
     * quadrantD, quadrantC
     */
    List<int>[] quadtree = new List<int>[] { new List<int>(), new List<int>(),
                                             new List<int>(), new List<int>() };

    private Vector3[] points;
    private Vector3 center;

    private int highestPoint;
    private float highestValue = float.MinValue;
    private float lowestValue = float.MaxValue;

    public RadarConvexHullAlgorithm(Vector3[] vertices, Vector3 _center) {
        points = vertices;
        center = _center;
    }

    /**
     * Personal algorithm developed for this assignment to select all vertices out of a set to produce a convex hull
     * Dubbed the "*Radar* Convex Hull Algorithm"
     */
    private void convexVertexSelection() {

        int p0 = 0;

        

        // Sort the vertices into the four quadrants
        // Also find the highest point
        for (int i = 0; i < points.Length; i++) {

            points[i] -= center;

            // Get the quadrant this vertex belongs to
            byte quadrant = getRadarConvexAlgorithmMask(points[i]);

            // Add index of vertex to quadtree
            quadtree[quadrant].Add(i);

            // Set the highest point
            p0 = (points[i].z > points[p0].z) ? i : p0;

        }

        // Loop over vertices again, this time computing the convex hull
        for (int i = 0; i < points.Length; i++) {
            int pointIndex = p0 + i;
            if (pointIndex >= points.Length)
                pointIndex = pointIndex - points.Length;

            byte quadrantCell = getRadarConvexAlgorithmMask(points[pointIndex]);

            // Check closest quadrant first 
            List<int> quadrant = quadtree[quadrantCell];

            // Find next point in this quadrant
            foreach (int index in quadrant) {

                // Skip self and already assigned point
                if (index == pointIndex || returnIndices.Contains(index))
                    continue;

                if (nextPointCardinally(points[index], index, quadrantCell)) {

                }

            }

            // Once point is found
            returnIndices.Add(highestPoint);

        }

    }

    /**
     * Finds next "most cardinal" point, meaning: for a given quadrant's cardinal axis (z, x, -z, -x), 
     * find the point which has the next most highest value - effectively "stepping down" in a digitised circle
     * 
     * However, at regions of this theoretical circle which is sin ~= cos within some threshold, the technique will break down. 
     * Therefore near the 45 angles (boundaries between a cardinal axis), the comparision goes from less than and greater than to
     * distance from the origin until the current point is not within this 45 degree region.
     * 
     */
    private bool nextPointCardinally(Vector3 point, int index, byte cell) {


        // Reset highest and lowest values when a new quadrant is entered


        float valueA = (cell == 0 || cell == 3) ? point.z : point.x;

        // z < l
        // z > h

        // x < l
        // x > h
        if (cell == 0 || cell == 2) {
            if (valueA > highestValue) {
                highestPoint = index;
                highestValue = valueA;

                return true;
            }
        } else {
            if (valueA < lowestValue) {
                highestPoint = index;
                lowestValue = valueA;

                return true;
            }
        }

        return false;
 
    }

    /**
     * Create a positive-negative mask where positive is 0. I.e.:
     *      01 00       B A        1 0         x z         l h
     *      11 10       D C        3 2         z x         l h
     */
    private byte getRadarConvexAlgorithmMask(Vector3 vertex) {
        byte mask = 0;
        if (vertex.x < 0) mask |= (byte)1;
        if (vertex.z < 0) mask |= (byte)2;

        return mask;
    }

}
 