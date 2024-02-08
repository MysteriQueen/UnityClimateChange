using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class GodPersonController : PlayerInputController {

    private Material handMaterial;
    private Animation handAnimation;
    private byte animationState = 1;
    private bool resetHand = false;

    private ForestManager forestManager;
    private TreePicker treePicker;
    private AnimalPicker animalPicker;

    public GameObject hotbarSelectionSquare;

    public GameObject foxPrefab;
    public GameObject deerPrefab;
    public GameObject bearPrefab;

    /* On script instantiation */
    protected override void Awake() {
        SceneManager.Refresh();
        stateManager = GameObject.Find("State Manager").GetComponent<StateManager>();
        treePicker = GetComponent<TreePicker>();
        animalPicker = GetComponent<AnimalPicker>();

    }
    protected override void Start() {

        if(lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        }

        stateManager.Paused = false;

        // Set internal variables
        playerCamera.fieldOfView = fov;
        defaultHandPosition = handObject.transform.localPosition;
        handMaterial = ((SkinnedMeshRenderer) (handObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>())).material;
        handAnimation = handObject.GetComponent<Animation>();

    }

    float camRotation;

    protected override void Update() {

        #region Main Menu
        if(Input.GetKeyUp(KeyCode.Escape)) {
            if(!stateManager.Paused) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            stateManager.Paused = !stateManager.Paused;
            
        }
        #endregion

        if (!stateManager.Paused) {
            #region Mouse

            if (lockCursor) {
                if (Input.GetKey(KeyCode.LeftAlt)) {
                    Cursor.lockState = CursorLockMode.Confined;
                    cameraCanMove = false;
                } else {
                    Cursor.lockState = CursorLockMode.Locked;
                    cameraCanMove = true;
                }
            }

            #region Hand Movement 

            if (!godCamera) {
                Vector3 pos = handObject.transform.position + 
                    (handPivot.transform.right * mouseDelta.x + handPivot.transform.forward * mouseDelta.y) * handSensitivty * stateManager.HandSensitivity * Time.deltaTime;

                if (!dualLook) {
                    
                    float oldX = pos.x;
                    float oldZ = pos.z;

                    /*pos.x = Mathf.Clamp(pos.x, transform.position.x - 2.5f, transform.position.x + 5.25f);
                    //pos.y = transform.position.y + defaultHandPosition.y;
                    pos.z = Mathf.Clamp(pos.z, transform.position.z - 3.5f, transform.position.z + 12.0f);*/

                    if (oldX != pos.x || oldZ != pos.z) { // Value was clamped
                        //dualLook = true;
                    }

                    handObject.transform.position = pos;

                // Within limits again
                } else if (pos.x > transform.position.x - 2.5f && pos.x < transform.position.x + 5.25f && pos.z > transform.position.z - 3.5f && pos.z < transform.position.z + 12.0f) {
                    //dualLook = false;
                    
                }

                resetHand = true;

            } else if(resetHand) {
                handObject.transform.localPosition = defaultHandPosition;
                resetHand = false;
            }

            // Raycast from mouse, down camera angle to terrain
            /*Ray ray = new Ray(transform.position, playerCamera.transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, LayerMask.GetMask("Terrain"))) {




                handObject.transform.position = hit.point;

            }*/

            #endregion

            #endregion

            #region Camera

            if (cameraCanMove & !stateManager.Paused) {
                Vector3 forward = playerCamera.transform.forward;
                Vector3 right = playerCamera.transform.right;

                forward.y = 0.0f; forward.Normalize();
                right.y = 0.0f; right.Normalize();

                float movementMultiplier = movementSensitivity;
                if (fastMove && godCamera && !dualLook) {
                    movementMultiplier = fastMovementSensitivity;
                }

                transform.position += (forward * direction.y + right * direction.x) * movementMultiplier * Time.deltaTime;
                Vector3 pos = transform.position;
                pos.y = Mathf.Clamp(pos.y + heightDelta * movementMultiplier * Time.deltaTime, 10.0f, 35.0f);

                transform.position = pos;

                if (godCamera || dualLook) {

                    yaw = transform.localEulerAngles.y + mouseDelta.x * mouseSensitivity * stateManager.Sensitivity;

                    if (!invertCamera) {
                        pitch -= mouseDelta.y * mouseSensitivity * stateManager.Sensitivity;
                    } else {
                        // Inverted Y
                        pitch += mouseDelta.y * mouseSensitivity * stateManager.Sensitivity;
                    }

                    // Clamp pitch between lookAngle
                    pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

                    transform.localEulerAngles = new Vector3(0, yaw, 0);
                    playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);

                }

            }

            #endregion

            #region Hand Interaction 

            if (grasping) {

                if (hotbarSelection != 0) {
                    if (hotbarSelection == 1) {
                        if (treePicker.TreeRef.Equals(TreePicker.NULLTREE)) {
                            treePicker.SetTree(Terrain.activeTerrain, ForestManager.AddTree());

                        }

                    } else if (hotbarSelection == 2) {
                        if (animalPicker.Animal == null) {
                            animalPicker.SetAnimal(null, Instantiate(foxPrefab));
                        }

                    } else if (hotbarSelection == 4) {
                        if (animalPicker.Animal == null) {
                            animalPicker.SetAnimal(null, Instantiate(deerPrefab));
                        }

                    } else if (hotbarSelection == 8) {
                        if (animalPicker.Animal == null) {
                            animalPicker.SetAnimal(null, Instantiate(bearPrefab));
                        }

                    } else if (hotbarSelection == 16) {
                        treePicker.Destroy();
                        animalPicker.Kill();
                        hotbarSelection = 0;

                    }

                    handAnimation["FistClench"].speed = 1.1f;
                    handAnimation["FistClench"].time = Mathf.Max(handAnimation["FistClench"].time, 0.0f);
                    animationState = 1;

                } else if (animationState == 0) {

                    #region Entity Detection

                    float padding = 1.5f;

                    // Animal picking
                    foreach (GameObject entity in SceneManager.entities) {
                        if (entity) {
                            if (handObject.transform.position.x > entity.transform.position.x - padding && handObject.transform.position.x < entity.transform.position.x + padding &&
                                handObject.transform.position.y > entity.transform.position.y - padding && handObject.transform.position.y < entity.transform.position.y + padding &&
                                handObject.transform.position.z > entity.transform.position.z - padding && handObject.transform.position.z < entity.transform.position.z + padding) {

                                animalPicker.SetAnimal(entity.transform.parent.gameObject, entity);

                                break;

                            }
                        }
                    }

                    padding = 1.0f;

                    // Tree picking
                    for (int i = 0; i < Terrain.activeTerrain.terrainData.treeInstanceCount; i++) {
                        TreeInstance tree = Terrain.activeTerrain.terrainData.treeInstances[i];
                        Vector3 pos = TerrainManager.TerrainToWorld(tree.position);

                        if (handObject.transform.position.x > pos.x - padding && handObject.transform.position.x < pos.x + padding &&
                            handObject.transform.position.y > pos.y - padding && handObject.transform.position.y < pos.y + 5 + padding && // Account for height of tree
                            handObject.transform.position.z > pos.z - padding && handObject.transform.position.z < pos.z + padding) {

                            treePicker.SetTree(Terrain.activeTerrain, i);
                            break;

                        }

                    }

                    #endregion

                    handAnimation["FistClench"].speed = 1.1f;
                    handAnimation["FistClench"].time = Mathf.Max(handAnimation["FistClench"].time, 0.0f);
                    animationState = 1;
                }
                handMaterial.color = Color.green;
                handAnimation.Play("FistClench");
                
            } else {
                if (!treePicker.TreeRef.Equals(TreePicker.NULLTREE))
                    treePicker.DropTree();

                if (animalPicker.Animal != null) {
                    SceneManager.AddEntity(animalPicker.Animal);
                    animalPicker.DropAnimal();
                }

                if (animationState == 1) {
                    handAnimation["FistClench"].speed = -1.7f;
                    handAnimation["FistClench"].time = Mathf.Min(handAnimation["FistClench"].time, 1.0f);

                    animationState = 0;
                }

                handMaterial.color = Color.white;
                handAnimation.Play("FistClench");
  
            }

            #endregion

            #region UI

            hotbarSelectionSquare.SetActive(true);
            switch (hotbarSelection) {
                case 0:
                    hotbarSelectionSquare.SetActive(false);
                    break;
                case 1:
                    hotbarSelectionSquare.transform.localPosition = new Vector3(-300f, hotbarSelectionSquare.transform.localPosition.y, hotbarSelectionSquare.transform.localPosition.z);
                    break;
                case 2:
                    hotbarSelectionSquare.transform.localPosition = new Vector3(-150f, hotbarSelectionSquare.transform.localPosition.y, hotbarSelectionSquare.transform.localPosition.z);
                    break;
                case 4:
                    hotbarSelectionSquare.transform.localPosition = new Vector3(0f, hotbarSelectionSquare.transform.localPosition.y, hotbarSelectionSquare.transform.localPosition.z);
                    break;
                case 8:
                    hotbarSelectionSquare.transform.localPosition = new Vector3(150f, hotbarSelectionSquare.transform.localPosition.y, hotbarSelectionSquare.transform.localPosition.z);
                    break;
                case 16:
                    hotbarSelectionSquare.transform.localPosition = new Vector3(300f, hotbarSelectionSquare.transform.localPosition.y, hotbarSelectionSquare.transform.localPosition.z);
                    break;
            }
            

            #endregion

        }

    }
 }

// Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(GodPersonController)), InitializeOnLoadAttribute]
public class GodPersonControllerEditor : Editor {
    GodPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable() {
        fpc = (GodPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI() {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("God Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.playerCamera, typeof(Camera), true);
        fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), fpc.fov, fpc.zoomFOV, 179f);
        fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.cameraCanMove);

        GUI.enabled = fpc.cameraCanMove;
        fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.invertCamera);
        fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), fpc.mouseSensitivity, .01f, 10f);
        fpc.movementSensitivity = EditorGUILayout.Slider(new GUIContent("Movement Sensitivity", "Determines how sensitive the movement of the camera and hand is."), fpc.movementSensitivity, 1f, 100f);
        fpc.fastMovementSensitivity = EditorGUILayout.Slider(new GUIContent("Fast Movement Sensitivity", "Determines how sensitive the fast movement of the camera is."), fpc.fastMovementSensitivity, 1f, 100f);

        fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.lockCursor);

        fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), fpc.crosshair);

        // Only displays crosshair options if crosshair is enabled
        if(fpc.crosshair) 
        { 
            EditorGUI.indentLevel++; 
            EditorGUILayout.BeginHorizontal(); 
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair.")); 
            fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--; 
        }

        EditorGUILayout.Space();

        #endregion

        #region Hand Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Hand Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.handObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hand Object", "The 'God Hand' game object to be used with this controller"), fpc.handObject, typeof(GameObject), true);
        fpc.handPivot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Hand Pivot", "The parent transform of the hand for global rotations"), fpc.handPivot, typeof(Transform), true);

        #endregion

        #region UI Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("UI Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.hotbarSelectionSquare = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hotbar Select UI", "The hotbar selection square."), fpc.hotbarSelectionSquare, typeof(GameObject), true);
        fpc.foxPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Fox Prefab", "The animal which the player can spawn."), fpc.foxPrefab, typeof(GameObject), true);
        fpc.deerPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Deer Prefab", "The animal which the player can spawn."), fpc.deerPrefab, typeof(GameObject), true);
        fpc.bearPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Bear Prefab", "The animal which the player can spawn."), fpc.bearPrefab, typeof(GameObject), true);
        
        #endregion

        //Sets any changes from the prefab
        if (GUI.changed) {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }

}

#endif
      
      