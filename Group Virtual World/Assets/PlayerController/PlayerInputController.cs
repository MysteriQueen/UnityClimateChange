using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : PersonController {

    #region Crosshair

    // Editor variables
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;
    
    #endregion

    #region Camera Movement Variables

    // Editor variables
    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 0.01f;
    protected float handSensitivty = 1.0f;
    public float maxLookAngle = 50f;
    public float movementSensitivity = 4.0f;
    public float fastMovementSensitivity = 7.5f;

    // Internal Variables
    protected bool godCamera = true;
    protected Vector2 mouseDelta;
    protected float yaw = 0.0f;
    protected float pitch = 0.0f;
    protected Vector2 direction = new Vector2();
    protected float heightDelta = 0.0f;
    protected bool dualLook = false;
    protected bool fastMove = false;


    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    protected bool isZoomed = false;

    #endregion
    #endregion   

    #region Hand Movement Variables

    // Editor variables
    public GameObject handObject;
    public Transform handPivot;

    // Internal variables
    protected Vector3 mouseWorldPos;
    protected Vector3 defaultHandPosition;
    protected bool grasping = false;

    #endregion

    protected byte hotbarSelection = 0;

    public void Look(InputAction.CallbackContext context) {
        mouseDelta = context.action.ReadValue<Vector2>();
        
    }

    public void Move(InputAction.CallbackContext context) {
        direction = context.action.ReadValue<Vector2>();

    }

    public void Elevate(InputAction.CallbackContext context) {
        heightDelta = context.action.ReadValue<float>();
    }

    public void CameraToggle(InputAction.CallbackContext context) {
        godCamera = !context.action.activeControl.IsPressed();
    }

    public void Interact(InputAction.CallbackContext context) {
        grasping = context.action.activeControl.IsPressed();

    }

    public void DualLookToggle(InputAction.CallbackContext context) {
        dualLook = context.action.activeControl.IsPressed();
    }

    public void DeviceChange(PlayerInput input) {
        Debug.Log(input.actions);
    }

    public void HMDPosition(InputAction.CallbackContext context) {
        Debug.Log(context.action.ReadValue<Vector3>());
    }

    public void FastMove(InputAction.CallbackContext context) {
        fastMove = !fastMove;
    }

    public void Hotbar1(InputAction.CallbackContext context) {
        if (hotbarSelection == 1)
            hotbarSelection = 0;
        else hotbarSelection = 1;
    }

    public void Hotbar2(InputAction.CallbackContext context) {
        if (hotbarSelection == 2)
            hotbarSelection = 0;
        else hotbarSelection = 2;
    }

    public void Hotbar3(InputAction.CallbackContext context) {
        if (hotbarSelection == 4)
            hotbarSelection = 0;
        else hotbarSelection = 4;
    }

    public void Hotbar4(InputAction.CallbackContext context) {
        if (hotbarSelection == 8)
            hotbarSelection = 0;
        else hotbarSelection = 8;
    }

    public void Hotbar5(InputAction.CallbackContext context) {
        if (hotbarSelection == 16)
            hotbarSelection = 0;
        else hotbarSelection = 16;
    }

}
