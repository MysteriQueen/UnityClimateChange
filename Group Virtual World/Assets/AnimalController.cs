using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {

    private StateManager stateManager;

    public AnimalGroupManager manager;
    [SerializeField] private float speed = 1.75f;
    [SerializeField] private float detectionRadius = 10.0f;
    [SerializeField] private float rotateSpeed = 0.025f;
    [SerializeField] private float seaLevelDetection = 2.0f;
    [SerializeField] private float elevationDetectionDistance = 4.0f;
    public AnimalGroupManager.AcceptedAnimals animalType;
    [SerializeField] private float animStartTime = 0.0f;
    [SerializeField] private float animEndTime = 1.0f;
    [SerializeField] private float yOffset = -0.3f;

    private float health = 80.0f; // Health out of 100

    
    private List<Vector3> nearbyEntityDirections = new List<Vector3>();
    private List<AnimalController> packMembers = new List<AnimalController>();

    public bool PickedUp { get; set; } = false;

    // Movement direction vectors
    private Vector3 packVector = new Vector3();
    private Vector3 preyVector = new Vector3();
    private Vector3 elevationVector = new Vector3();
    private Vector3 resourceVector = new Vector3();

    private Animation animation;

    void Awake() {
        if (manager == null) {
            manager = GetComponentInParent<AnimalGroupManager>();

        }

        stateManager = GameObject.Find("State Manager").GetComponent<StateManager>();
        animation = GetComponent<Animation>();

    }

    void Start() {
        if (animation) {
            animation["SingleTimeline"].normalizedTime = animStartTime;
            animation.Play();

        }

    }

    public void GroupUpdate() {
        if (!stateManager.Paused) {
    
            if (!PickedUp) {

                #region AI

                #region Pack Following

                // Try and maintain a constant distance from all other pack members - "pack resistance"
                float packResistance = 0.6f;
                float minimumStray = packResistance + 0.5f;

                // Set the "pack vector" which denotes the direction the pack wants to move in
                // Resource vector should be a pack-related vector, and have a high weight (all members of the pack move towards resource - habitat, food)
                // Avoidance of predators should be a pack-related vector and per-animal vector
                // Sea level avoidance should be a per-animal vector

                // Further an animal is from the group centre, the more it wants to get back to reduce that distance. This is expoential, so that to allow for per-animal exploration.
                // Local position is the offset from group centre
                float strayWeight = Mathf.Clamp(Mathf.Exp(transform.localPosition.magnitude / manager.maxStray) - minimumStray, -packResistance, manager.packingTendency);
                packVector = transform.localPosition * -strayWeight;


                #endregion

                #region Prey Avoidance

                foreach (Vector3 direction in nearbyEntityDirections) {
                    preyVector += direction;
                    Debug.DrawLine(transform.position, transform.position + direction, Color.red);
                }

                preyVector /= nearbyEntityDirections.Count;
                preyVector *= -1;
                preyVector.y = 0;

                #endregion

                #region Sea Level Avoidance

                if (SeaLevelManager.GetHeight() > transform.position.y + transform.localScale.y) {
                    manager.KillAnimal(this);

                    return;

                } else {

                    Vector3[] surroundings = TerrainManager.GetSurroundingPositions(transform.position, elevationDetectionDistance);

                    List<Vector3> sortedSurroundings = new List<Vector3>(surroundings);
                    sortedSurroundings.Sort(new HeightSort());

                    if (transform.position.y - transform.localScale.y < SeaLevelManager.GetHeight() + seaLevelDetection) {
                        elevationVector = sortedSurroundings[7] - transform.position;

                    } else {
                        Vector3 a = sortedSurroundings[3] - transform.position;
                        Vector3 b = sortedSurroundings[4] - transform.position;
                        elevationVector = a - b;

                    }

                    elevationVector.Normalize();

                }

                #endregion

                // Influence vector sum
                // Pack venter has inbuilt multiplier
                // Elevation vector is normalised
                Vector3 influence = packVector + elevationVector * manager.selfPreservation;

                /*Debug.DrawLine(manager.GetGroupCentre() + transform.localPosition, manager.GetGroupCentre() + transform.localPosition + packVector, Color.green);
                Debug.DrawLine(manager.GetGroupCentre() + transform.localPosition, manager.GetGroupCentre() + transform.localPosition + elevationVector, Color.red);
                Debug.DrawLine(manager.GetGroupCentre() + transform.localPosition, manager.GetGroupCentre() + transform.localPosition + influence, Color.blue);*/

                // Calculate the amount of turn
                Vector3 dir = Vector3.RotateTowards(transform.forward, influence, rotateSpeed, 0f);
                dir.y = transform.forward.y;
                transform.rotation = Quaternion.LookRotation(dir);

                // Calculate angle of animal
                RaycastHit hitA;
                if (Physics.Raycast(transform.position + transform.forward, -transform.up, out hitA)) {
                    RaycastHit hitB;
                    if (Physics.Raycast(transform.position - transform.forward, -transform.up, out hitB)) {
                        transform.forward = (hitA.point - hitB.point) * Time.deltaTime;

                    }
                }

                float angle = ((Mathf.Atan2(transform.position.y, transform.position.x) * Mathf.Rad2Deg));
                //transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), angle);

                // Calculate the amount to move
                float turnFactor = Vector3.Dot(transform.forward, influence);
                if (turnFactor != 0)
                    turnFactor = Mathf.Clamp(Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.forward, influence), -1.0f, 1.0f)), 0f, 0.5f);

                float movementFactor = 1f - turnFactor;
                transform.position += transform.forward * speed * movementFactor * Time.deltaTime;
                
                #endregion

                transform.position = new Vector3(
                    transform.position.x,
                    transform.localScale.y/2 + TerrainManager.WorldToTerrain(transform.position).y * TerrainManager.GetTerrainSize().y + yOffset,
                    transform.position.z
                );

                #region Animation

                if (animation) {
                    animation.Play();
   
                    if (animation["SingleTimeline"].normalizedTime >= animEndTime || animation["SingleTimeline"].normalizedTime <= animStartTime) 
                        animation["SingleTimeline"].normalizedTime = animStartTime;
                    
                }

                #endregion

            } else {
                if (animation) {
                    animation.Stop();
                }
            }

        } else {
            if (animation) {
                animation.Stop();
            }

        }
    }

    /**
     * 
     */
    void GoHere(Vector3 loc, float time) {
        Vector3 newPos = transform.position;
        newPos.x += Mathf.Lerp(transform.position.x, loc.x, time); 
        newPos.y += Mathf.Lerp(transform.position.y, loc.y, time); 
        newPos.z += Mathf.Lerp(transform.position.z, loc.z, time);

        transform.position = newPos;
    }

    // Getters
    public Vector3 NearbyDirection {
        set => nearbyEntityDirections.Add(value);
    }
    public List<Vector3> NearbyDirections {
        set => nearbyEntityDirections = value;
    }

    public float DetectionRadius {
        get => detectionRadius;
    }

    public Vector3 Position {
        get => transform.position;
    }

    private class HeightSort : IComparer<Vector3> {

        public int Compare(Vector3 a, Vector3 b) {
            return a.y < b.y ? -1 : 1;
        }
    }

    /// <summary>
    /// For use by the animal control manager.
    /// </summary>
    public void SetPackMembers(List<AnimalController> members) {
        packMembers = members;
    }

}

