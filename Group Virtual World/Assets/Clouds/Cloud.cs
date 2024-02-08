using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Cloud : MonoBehaviour {

    private Vector3 abs(Vector3 v) {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        v.z = Mathf.Abs(v.z);

        return v;
    }

    private float maxLength(Vector3 a, Vector3 b) {
        return Mathf.Max(length(a), length(b));
    }

    private float minLength(Vector3 a, Vector3 b) {
        return Mathf.Min(length(a), length(b));

    }

    private float maxLength(Vector3 a, Vector3 b, out Vector3 larger) {
        float lengthA = length(a);
        float ret = Mathf.Max(lengthA, length(b));
        larger = ret == lengthA ? a : b;
        return ret;
    }

    private float minLength(Vector3 a, Vector3 b, out Vector3 smaller) {
        float lengthA = length(a);
        float ret = Mathf.Min(lengthA, length(b));
        smaller = ret == lengthA ? a : b;
        return ret;
    }

    private Vector3 max(Vector3 a, Vector3 b) {
        Vector3 res;
        maxLength(a, b, out res);
        return res;
    }

    private Vector3 min(Vector3 a, Vector3 b) {
        Vector3 res;
        minLength(a, b, out res);
        return res;
    }

    private float maxComponent(Vector3 v) {
        return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
    }

    /**
     * Distance to a point
     */
    private Vector3 distance(Vector3 p, Vector3 c) {
        return c - p;
    }

    /**
     * Length of a vector 
     */
    private float length(Vector3 v) {
        return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }

    /**
     * Signed distance to a circle (point - radius)
     * Note: If point is within circle, the distance is negative
     */
    private float signedDstToCircle(Vector3 p, Vector3 centre, float radius) {
        return length(distance(p, centre)) - radius;
    }

    /**
     * Signed distance to a box (point - radius)
     * Size: The "semi-size" of the box - from centre to edge
     * Note: If point is within box, the distance is negative
     * 
     * https://iquilezles.org/www/articles/distfunctions/distfunctions.htm
     * https://www.youtube.com/watch?v=62-pRVZuS5c&ab_channel=InigoQuilez
     */
    private float signedDstToBox(Vector3 p, Vector3 centre, Vector3 size) {
        // Convert to upper-right quadrant (absolute), and localise p to (0, 0), and 
        Vector3 offset = abs(p - centre) - size;

        float unsignedDst = maxLength(offset, Vector3.zero); // Clamps the distance to +0 == length(max(q, 0.0))
        float dstInsideBox = maxComponent(min(offset, Vector3.zero)); // Clamps the distance to -0 and returns the largest cardinal distance == min(max(q.x, max(q.y, q.z)), 0.0);

        return unsignedDst + dstInsideBox;
    }

    public SceneManager scene;
    public GameObject rootEntity;

    private static int MAX_SAMPLES = 25;

    private Renderer[] renderers;

    private List<Material> materials = new List<Material>();


    public List<GameObject> testEntities = new List<GameObject>();

    private void Awake() {
        renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) {
            materials.Add(renderer.sharedMaterial);
        }

    }

    private void Start() {
        
    }

    private void Update() {
        /*GameObject[] nearbyEntities = scene.GetNearbyEntities(rootEntity.transform.position, 50f);
        Vector4[] shaderEntities = new Vector4[nearbyEntities.Length];
        int count = Mathf.Max(nearbyEntities.Length, MAX_SAMPLES);*/

        Vector4[] shaderEntities = new Vector4[testEntities.Count];
        int count = testEntities.Count;
        Debug.Log(count);

        for (int i = 0; i < count; i++) {
            shaderEntities[i] = new Vector4(
                testEntities[i].transform.position.x,
                testEntities[i].transform.position.y,
                testEntities[i].transform.position.z);

            /*shaderEntities[i] = new Vector4(
                nearbyEntities[i].transform.position.x,
                nearbyEntities[i].transform.position.y,
                nearbyEntities[i].transform.position.z);*/
        }

        int index = 0;
        foreach (Material material in materials) {
            material.SetInt("_EntityCount", count);
            material.SetVectorArray("_Entities", shaderEntities);
            material.SetVector("_Test", Color.red);

        }
    }

}
