using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.UI;

[RequireComponent(typeof(Texture))]
public class ForcedReset : MonoBehaviour {

    void Update () {
        
        // if we have forced a reset ...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            
            //... reload the scene
            Application.LoadLevelAsync (Application.loadedLevelName);
        }
    }

}
