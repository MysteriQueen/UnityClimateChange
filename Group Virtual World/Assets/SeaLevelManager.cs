using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLevelManager : MonoBehaviour {

    public float minLevel = 1.5f;

    private static float baseSeaLevel = 3.5f;
    private static float seaLevelRise { get; set; }

    public delegate void SeaLevelRise();
    public static event SeaLevelRise OnRise;

    private StateManager stateManager;

    private void Awake() {
        stateManager = GameObject.Find("State Manager").GetComponent<StateManager>();
    }

    private void Start() {
        transform.localPosition = new Vector3(transform.localPosition.x, baseSeaLevel, transform.localPosition.z);
        
    }

    private void Update() {
        if (!stateManager.Paused) {
            seaLevelRise = (1 - StateManager.statistics.oxygenOptimalPercentage) * Time.deltaTime;

            transform.Translate(new Vector3(0.0f, seaLevelRise, 0.0f));
            if (transform.position.y < minLevel) {
                transform.position = new Vector3(transform.position.x, minLevel, transform.position.z);
            }

            OnRise?.Invoke();
        }
    }

    public static float GetHeight() {
        return baseSeaLevel + seaLevelRise;
    }

    public static float GetSeaLevelRise() {
        return seaLevelRise;
    }

}
