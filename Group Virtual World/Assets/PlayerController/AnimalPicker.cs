using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Attaches a tree to the player's hand
 * 
 */
public class AnimalPicker : MonoBehaviour {

    private GameObject group;
    private GameObject animal;
    private AnimalController animalController;
    public Transform hand;

    public GameObject Animal { get => animal; }

    public void SetAnimal(GameObject group_, GameObject animal_) {

        // Set animal and it's group
        group = group_;
        animal = animal_;

        // Move animal to hand and parent it
        animal.transform.localPosition = Vector3.zero;
        animal.transform.position = hand.position;
        animal.transform.parent = hand;

        animalController = animal.GetComponent<AnimalController>();
        animalController.PickedUp = true;

        SceneManager.RemoveEntity(animal);

    }

    public void DropAnimal() {

        if (animal != null) {
            Vector3 animalPos = TerrainManager.TerrainToWorld(TerrainManager.WorldToTerrain(hand.position));

            AnimalGroupManager closestManager = null;
            float distance = float.MaxValue;

            // Find group to put animal into
            if (!group) {
                foreach (AnimalGroupManager manager in AnimalGroupManager.managers) {
                    if (manager.acceptedAnimal == animalController.animalType) {
                        float tempDst = (manager.transform.position - animalPos).magnitude;
                        if (tempDst < distance) {
                            distance = tempDst;
                            closestManager = manager;
                        }
                    }
                }

                if (!closestManager) {
                    Destroy(animal);
                    animal = null;
                    return;
                }

                group = closestManager.gameObject;
                animalController.manager = closestManager;
                closestManager.AddAnimal(animalController);

            }

            // Parent the animal back to it's group
            animal.transform.parent = group.transform;
            animal.transform.position = group.transform.position;

            // Set it's local position to the position offset by the group's position
            animal.transform.localPosition = animalPos - group.transform.position;
            animalController.PickedUp = false;

            SceneManager.AddEntity(animal);

        }

        animal = null;

    }

    public void Kill() {
        if (animal) {
            Destroy(animal);
            animal = null;
        }
    }

}
