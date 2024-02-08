using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Manages entities in the scene
 */
public class SceneManager : MonoBehaviour {

    public static List<GameObject> entities;

    public static void Refresh() {
        entities = new List<GameObject>();

        foreach (GameObject entity in GameObject.FindGameObjectsWithTag("Animal")) {
            AddEntity(entity);

        }

    }

    public static void AddEntity(GameObject entity) {
        entities.Add(entity);
    }

    public static void RemoveEntity(GameObject entity) {
        entities.Remove(entity);
    }

}
