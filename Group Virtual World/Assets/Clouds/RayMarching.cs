using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayMarching : MonoBehaviour {

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
    private Vector3 distance (Vector3 p, Vector3 c) {
        return c - p;
    }

    /**
     * Length of a vector 
     */
    private float length(Vector3 v) {
        return Mathf.Sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
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
    public GameObject visualiserPrefab;
    public GameObject visualiserPointPrefab;
    public GameObject rootEntity;

    private static int MAX_SAMPLES = 25;

    private Vector3 samplePos;
    private int sampleCount;

    private List<GameObject> visualisers = new List<GameObject>(MAX_SAMPLES);
    private List<GameObject> visualiserPoints = new List<GameObject>(MAX_SAMPLES);

    private float visualerPointSize = 0.25f;

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < MAX_SAMPLES; i++) {
            visualisers.Add(Instantiate(visualiserPrefab));
            visualisers[i].transform.localScale = Vector3.zero;
            visualisers[i].transform.parent = transform;

            visualiserPoints.Add(Instantiate(visualiserPointPrefab));
            visualiserPoints[i].transform.localScale = Vector3.zero;
            visualiserPoints[i].transform.parent = transform;
        }

    }

    // Update is called once per frame
    void Update() {
        /*samplePos = rootEntity.transform.position;
        
        float collideDIstance = 0.01f;
        float maxDistance = 10f;
        float entityDistance;

        for (sampleCount = 0; sampleCount < MAX_SAMPLES; sampleCount++) {
            visualisers[sampleCount].transform.localScale = Vector3.zero;

            GameObject entity = SceneManager.ClosestEntity(samplePos, maxDistance, out entityDistance);

            Vector3 dir = distance(samplePos, entity.transform.position);
           
            entityDistance = signedDstToCircle(samplePos, entity.transform.position, entity.transform.localScale.x / 2);
            Debug.DrawRay(samplePos, dir.normalized * entityDistance, Color.red);

            if (entityDistance < collideDIstance) {
                //Debug.Log("Hit " + Time.deltaTime);
                Debug.Log("Hit");

                for (; sampleCount < MAX_SAMPLES; sampleCount++) {
                    visualisers[sampleCount].transform.localScale = Vector3.zero;
                    visualiserPoints[sampleCount].transform.localScale = Vector3.zero;
                }

            } else {
                visualisers[sampleCount].transform.position = samplePos;
                visualisers[sampleCount].transform.localScale = new Vector3(entityDistance, entityDistance, entityDistance) * 2;

                visualiserPoints[sampleCount].transform.position = samplePos;
                visualiserPoints[sampleCount].transform.localScale = new Vector3(visualerPointSize, visualerPointSize, visualerPointSize) * entityDistance;

                samplePos += rootEntity.transform.forward * entityDistance;

            }
            
        }

        Debug.DrawLine(rootEntity.transform.position, samplePos, Color.blue);
        */
    }

}
