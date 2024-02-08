using UnityEngine;
using UnityEngine.UI;

namespace UnitySampleAssets.Utility
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour {
        private float fpsMeasurePeriod = 0.5f;
        private int fpsAccumulator = 0;
        private float fpsNextPeriod = 0;
        private int currentFps;
        private string display = "{0} FPS";
        private Text text;

        private void Start() {
            fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            text = GetComponent<Text>();
        }

        private void Update() {
            // measure average frames per second
            fpsAccumulator++;
            if (Time.realtimeSinceStartup > fpsNextPeriod) {
                currentFps = (int) (fpsAccumulator/fpsMeasurePeriod);
                fpsAccumulator = 0;
                fpsNextPeriod += fpsMeasurePeriod;
                text.text = string.Format(display, currentFps);
            }
        }
    }
}
