using System.Collections.Generic;
using UnityEngine;

public class AnimalGroupManager : MonoBehaviour {

    /// <summary>
    /// Self static reference to list of managers
    /// </summary>
    public static List<AnimalGroupManager> managers = new List<AnimalGroupManager>();

    private List<AnimalController> toKill = new List<AnimalController>();

    /// <summary>
    /// How far the animals of this group can stray for the centre of the pack
    /// </summary>
    public float maxStray = 10.0f;
    public float selfPreservation = 1.33f;

    /// <summary>
    /// The upper bound of weighting applied to the pack-maintaing vector
    /// </summary>
    public float packingTendency = 50.0f;

    private static List<GameObject> otherEntities;

    private StateManager stateManager;
    private Vector3 groupCenter;

    public enum AcceptedAnimals {
        Fox,
        Deer,
        Bear
    }

    public AcceptedAnimals acceptedAnimal;

    // Storage
    private List<AnimalController> members = new List<AnimalController>();

    // Cache
    private List<GameObject> nearbyEntities = new List<GameObject>();
    public List<GameObject> NearbyEntities {
        get => nearbyEntities;
    }

    // Detection Circle
    private Vector3 detectionOrigin;
    private float detectionRadius;

    private void Awake() {
        managers.Add(this);

        #region Get existing animals and entities

        if (otherEntities == null) {
            otherEntities = new List<GameObject>();
            otherEntities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        }

        for (int i = 0; i < transform.childCount; i++) {
            members.Add(transform.GetChild(i).gameObject.GetComponent<AnimalController>());

        }

        #endregion
    }

    private void Start() {
        stateManager = GameObject.Find("State Manager").GetComponent<StateManager>();

        foreach (AnimalController animal in members) {
            groupCenter += animal.Position;
        }

        groupCenter /= members.Count;

    }

    private void Update() {
        if (!stateManager.Paused) {
            ProcessAnimals();
        }
    }

    /**
     * Calculate center of animal group
     * Identify all nearby entities
     * 
     */
    private void ProcessAnimals() {
        if (toKill.Count > 0) {
            foreach (AnimalController animal in toKill) {
                members.Remove(animal); 
                SceneManager.RemoveEntity(animal.gameObject);
                Destroy(animal.gameObject);
                       
            }

            UpdateMemberLists();
            toKill.Clear();

        }

        nearbyEntities.Clear();

        groupCenter = Vector3.zero;
        foreach (AnimalController animal in members) {
            if (!animal.PickedUp)
                groupCenter += animal.Position;

            //GetNearbyEntities(animal);
        }

        groupCenter /= members.Count;

        // Reposition animals and group origin, for visual consistency
        foreach (AnimalController animal in members) {
            if (!animal.PickedUp)
                animal.transform.localPosition = animal.Position - groupCenter;

        }

        transform.position = groupCenter;

        foreach (AnimalController animal in members) {
            animal.GroupUpdate();
        }

    }

    /// <summary>
    /// Assigns the nearby entities of the animal, to it's nearby directions list
    /// </summary>
    /// <param name="animal"></param>
    private void GetNearbyEntities(AnimalController animal) {
        animal.NearbyDirections = new List<Vector3>();

        foreach (GameObject entity in otherEntities) {
            if ((entity.transform.position - animal.transform.position).magnitude < animal.DetectionRadius) {
                nearbyEntities.Add(entity);
                animal.NearbyDirection = entity.transform.position - animal.transform.position;
            }
        }
    }

    public void KillAnimal(AnimalController animal) {
        toKill.Add(animal);
        
    }

    public void AddAnimal(AnimalController animal) {
        members.Add(animal);
        UpdateMemberLists();
    }

    public int GetAnimalCount() {
        return members.Count;
    }

    private void UpdateMemberLists() {
        foreach (AnimalController member in members) {
            member.SetPackMembers(members);
        }
    }

    public Vector3 GetGroupCentre() {
        return groupCenter;
    }
}
